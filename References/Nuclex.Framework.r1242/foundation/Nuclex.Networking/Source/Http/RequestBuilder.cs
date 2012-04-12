#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2009 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;

namespace Nuclex.Networking.Http {

  /// <summary>Collects data to build an HTTP request container</summary>
  internal class RequestBuilder {

    /// <summary>Wraps a HTTP header field value</summary>
    private class FieldValue {

      /// <summary>Combined contents of the HTTP header field value</summary>
      public string Contents;

    }

    /// <summary>Initializes a new HTTP request builder</summary>
    public RequestBuilder() {
      this.headers = new Dictionary<string, FieldValue>();
      Reset();
    }

    /// <summary>Resets the request builder to its initial state</summary>
    public void Reset() {
      this.headers.Clear();
      this.Method = null;
      this.Uri = null;
      this.Version = null;
    }

    /// <summary>Adds a header field for inclusion in the built HTTP request</summary>
    /// <param name="fieldName">
    ///   Name of the header field to include in the built HTTP request
    /// </param>
    public void AddHeader(string fieldName) {
      AddHeader(fieldName, null);
    }

    /// <summary>
    ///   Adds a header field and its value for inclusion in the built HTTP request
    /// </summary>
    /// <param name="fieldName">
    ///   Name of the header field to include in the built HTTP request
    /// </param>
    /// <param name="append">
    ///   Field value to set or append to the existing field value for that field
    /// </param>
    public void AddHeader(string fieldName, string append) {
      FieldValue value;

      // Try to obtain the header field. If it isn't on record yet, we set up
      // a new header field transparently.
      if(!this.headers.TryGetValue(fieldName, out value)) {
        value = new FieldValue();
        this.headers.Add(fieldName, value);
      }

      // Header field is known. If this is a header field without a value (either new
      // or had no provided value), it will be provided with a value, otherwise, the
      // value will be appended to the existing field value.
      if(value.Contents == null) {
        value.Contents = append;
      } else if(append != null) {
        value.Contents += append;
      }
    }

    /// <summary>Builds an HTTP request from the current data available</summary>
    /// <returns>The newly built HTTP request</returns>
    public Request BuildRequest() {

      // Condense the headers into simple strings (our own storage format is optimized
      // to be extendable without having to readd the values into the dictionary,
      // however, once the request is complete, this is no longer useful).
      Dictionary<string, string> condensedHeaders = new Dictionary<string, string>();
      foreach(KeyValuePair<string, FieldValue> header in this.headers) {
        condensedHeaders.Add(header.Key, header.Value.Contents);
      }

      // Everything is ready, now build the HTTP request container
      return new Request(this.Method, this.Uri, this.Version, condensedHeaders);

    }

    /// <summary>HTTP request method</summary>
    public string Method;
    /// <summary>URI being requested</summary>
    public string Uri;
    /// <summary>Version of the HTTP protocol being used</summary>
    public string Version;
    /// <summary>
    ///   Records the header fields that will be assigned to the built HTTP request
    /// </summary>
    private Dictionary<string, FieldValue> headers;

  }

} // namespace Nuclex.Networking.Http
