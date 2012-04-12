using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Nuclex.Networking.Http {

  /// <summary>
  ///   Formats an HTTP server response container into a format the client understands
  /// </summary>
  internal class ResponseFormatter {

    /// <summary>ID of the ISO-8859-1 code page</summary>
    private const int ISO_8859_1 = 28591;

    /// <summary>ASCII code for the carriage return character</summary>
    private const byte CR = 13;
    /// <summary>ASCII code for the line feed character</summary>
    private const byte LF = 10;
    /// <summary>ASCII code for the space character</summary>
    private const byte SP = 32;
    /// <summary>ASCII code for the double colon character</summary>
    private const byte DC = 58;

    /// <summary>
    ///   Convert the provided response into the HTTP response transport format
    /// </summary>
    /// <param name="response">Response that will be converted</param>
    /// <returns>
    ///   An array of bytes containing the response in the HTTP response format
    /// </returns>
    public static byte[] Format(Response response) {

      // Make sure the status code is in a valid numerical range. We will also need
      // this int later to avoid .NETs enum to string conversion helpfulness
      int intStatusCode = (int)response.StatusCode;
      if((intStatusCode < 100) || (intStatusCode > 599)) {
        throw new InvalidOperationException("Invalid status code in response");
      }

      // Build a usable status message string. If the request processor specified
      // null for the status message, we try to replace it with the default message
      // for the given status code. If that also doesn't work, we'll send an empty
      // status message.
      string statusMessage = response.StatusMessage;
      if(statusMessage == null) {
        statusMessage = StatusCodeHelper.GetDefaultDescription(response.StatusCode);
        if(statusMessage == null) {
          statusMessage = string.Empty;
        }
      }

      // Calculate the total size of the return packet
      int combinedLength;
      int versionLength, statusMessageLength;
      int[,] headerLengths;

      // Status line
      {
        // Sum up the length of the constant parts of the reply
        // <Version> SP:1 <StatusCode:3> SP:1 <StatusMessage> CRLF:2 <Headers> CRLF:2
        combinedLength = 1 + 3 + 1 + 2 + 2; // SP + StatusCode + SP + CRLF + CRLF

        // Add the length of the HTTP version and status message
        versionLength = iso88591Encoding.GetByteCount(response.Version);
        combinedLength += versionLength;
        statusMessageLength = iso88591Encoding.GetByteCount(statusMessage);
        combinedLength += statusMessageLength;
      }
      
      // Headers
      {
        // Add the constant per-header overhead of 4 bytes
        // <FieldName> DCOLON:1 SP:1 <FieldValue> CRLF:2
        combinedLength += response.Headers.Count * 4;
        headerLengths = new int[response.Headers.Count, 2];

        // Now sum up the length of the dynamic parts in all header fields
        int headerIndex = 0;
        foreach(KeyValuePair<string, string> header in response.Headers) {
          headerLengths[headerIndex, 0] = iso88591Encoding.GetByteCount(header.Key);
          headerLengths[headerIndex, 1] = iso88591Encoding.GetByteCount(header.Value);

          combinedLength += headerLengths[headerIndex, 0] + headerLengths[headerIndex, 1];
          ++headerIndex;
        }
      }

      // Now that we know the length of the response message, we can set up a buffer and
      // construct the response in it.
      byte[] responseBytes = new byte[combinedLength];
      int responseByteIndex = 0;

      // Write the HTTP protocol version
      iso88591Encoding.GetBytes(
        response.Version, 0, response.Version.Length, responseBytes, 0
      );
      responseByteIndex += versionLength;
      responseBytes[responseByteIndex] = SP;
      ++responseByteIndex;

      //IFormatProvider
      

      // Write the status code
      iso88591Encoding.GetBytes(
        intStatusCode.ToString(), 0, 3, responseBytes, responseByteIndex
      ); // TODO: use explicit locale
      responseByteIndex += 3;
      responseBytes[responseByteIndex] = SP;
      ++responseByteIndex;

      // Write the status message
      iso88591Encoding.GetBytes(
        statusMessage, 0, statusMessage.Length,
        responseBytes, responseByteIndex
      );
      responseByteIndex += statusMessageLength;
      responseBytes[responseByteIndex] = CR;
      ++responseByteIndex;
      responseBytes[responseByteIndex] = LF;
      ++responseByteIndex;

      // Write headers
      {
        int headerIndex = 0;
        foreach(KeyValuePair<string, string> header in response.Headers) {

          // Write header name
          iso88591Encoding.GetBytes(
            header.Key, 0, header.Key.Length, responseBytes, responseByteIndex
          );
          responseByteIndex += headerLengths[headerIndex, 0];
          responseBytes[responseByteIndex] = DC;
          ++responseByteIndex;
          responseBytes[responseByteIndex] = SP; // not in RFC, but common practice
          ++responseByteIndex;

          // Write header value
          iso88591Encoding.GetBytes(
            header.Value, 0, header.Value.Length, responseBytes, responseByteIndex
          );
          responseByteIndex += headerLengths[headerIndex, 1];
          responseBytes[responseByteIndex] = CR;
          ++responseByteIndex;
          responseBytes[responseByteIndex] = LF; // not in RFC, but common practice
          ++responseByteIndex;
          
          ++headerIndex;

        }
      }

      responseBytes[responseByteIndex] = CR;
      ++responseByteIndex;
      responseBytes[responseByteIndex] = LF; // not in RFC, but common practice
      ++responseByteIndex;

      return responseBytes;
    }

    /// <summary>Encoding for the ISO-8859-1 codepage</summary>
    /// <remarks>
    ///   
    /// </remarks>
    private static readonly Encoding iso88591Encoding = Encoding.GetEncoding(ISO_8859_1);

    /// <summary>CultureInfo of the en-us culture</summary>
    /// <remarks>
    ///   Mainly used to convert numbers to text without relying in that the system's
    ///   current culture transforms into a format the client can understand.
    /// </remarks>
    private static readonly CultureInfo usCulture = new CultureInfo("en-us");
    
  }
} // namespace Nuclex.Networking.Http
