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

  /// <summary>Helper methods for working with HTTP status codes</summary>
  public static class StatusCodeHelper {

    /// <summary>Returns the class of the provided status code</summary>
    /// <param name="statusCode">Status code of which to return the class</param>
    /// <returns>Class of the provided status code</returns>
    public static StatusCodeClass GetClass(StatusCode statusCode) {
      int integerStatusCode = (int)statusCode;

      if(integerStatusCode < 100) { // 0-99
        return StatusCodeClass.Unknown;
      } else if(integerStatusCode < 200) { // 100-199
        return StatusCodeClass.C1xx_Informational;
      } else if(integerStatusCode < 300) { // 200-299
        return StatusCodeClass.C2xx_Successful;
      } else if(integerStatusCode < 400) { // 300-399
        return StatusCodeClass.C3xx_Redirection;
      } else if(integerStatusCode < 500) { // 400-499
        return StatusCodeClass.C4xx_Client_Error;
      } else if(integerStatusCode < 600) { // 500-599
        return StatusCodeClass.C5xx_Server_Error;
      } else {
        return StatusCodeClass.Unknown;
      }
    }

    /// <summary>Returns the default description associated with a status code</summary>
    /// <param name="statusCode">
    ///   Status code of which to obtain the default description
    /// </param>
    /// <returns>The default description for the provided status code</returns>
    public static string GetDefaultDescription(StatusCode statusCode) {
      switch(statusCode) {

        // 1xx
        case StatusCode.S100_Continue: { return "Continue"; }
        case StatusCode.S101_SwitchingProtocols: { return "Switching Protocols"; }

        // 2xx
        case StatusCode.S200_OK: { return "OK"; }
        case StatusCode.S201_Created: { return "Created"; }
        case StatusCode.S202_Accepted: { return "Accepted"; }
        case StatusCode.S203_Non_Authoritative_Information: {
          return "Non-Authoritative Information";
        }
        case StatusCode.S204_No_Content: { return "No Content"; }
        case StatusCode.S205_Reset_Content: { return "Reset Content"; }
        case StatusCode.S206_Partial_Content: { return "Partial Content"; }

        // 3xx
        case StatusCode.S300_Multiple_Choices: { return "Multiple Choices"; }
        case StatusCode.S301_Moved_Permanently: { return "Moved Permanently"; }
        case StatusCode.S302_Found: { return "Found"; }
        case StatusCode.S303_See_Other: { return "See Other"; }
        case StatusCode.S304_Not_Modified: { return "Not Modified"; }
        case StatusCode.S305_Use_Proxy: { return "Use Proxy"; }
        case StatusCode.S307_Temporary_Redirect: { return "Temporary Redirect"; }

        // 4xx
        case StatusCode.S400_Bad_Request: { return "Bad Request"; }
        case StatusCode.S401_Unauthorized: { return "Unauthorized"; }
        case StatusCode.S402_Payment_Required: { return "Payment Required"; }
        case StatusCode.S403_Forbidden: { return "Forbidden"; }
        case StatusCode.S404_Not_Found: { return "Not Found"; }
        case StatusCode.S405_Method_Not_Allowed: { return "Method Not Allowed"; }
        case StatusCode.S406_Not_Acceptable: { return "Not Acceptable"; }
        case StatusCode.S407_Proxy_Authentication_Required: {
          return "Proxy Authentication Required";
        }
        case StatusCode.S408_Request_Timeout: { return "Request Timeout"; }
        case StatusCode.S409_Conflict: { return "Conflict"; }
        case StatusCode.S410_Gone: { return "Gone"; }
        case StatusCode.S411_Length_Required: { return "Length Required"; }
        case StatusCode.S412_Precondition_Field: { return "Precondition Failed"; }
        case StatusCode.S413_Request_Entity_Too_Large: {
          return "Request Entity Too Large";
        }
        case StatusCode.S414_Request_Uri_Too_Long: { return "Request-URI Too Long"; }
        case StatusCode.S415_Unsupported_Media_Type: { return "Unsupported Media Type"; }
        case StatusCode.S416_Request_Range_Not_Satisfiable: {
          return "Requested Range Not Satisfiable";
        }
        case StatusCode.S417_Expectation_Failed: { return "Expectation Failed"; }

        // 5xx
        case StatusCode.S500_Internal_Server_Error: { return "Internal Server Error"; }
        case StatusCode.S501_Not_Implemented: { return "Not Implemented"; }
        case StatusCode.S502_Bad_Gateway: { return "Bad Gateway"; }
        case StatusCode.S503_Service_Unavailable: { return "Service Unavailable"; }
        case StatusCode.S504_Gateway_Timeout: { return "Gateway Timeout"; }
        case StatusCode.S505_Http_Version_Not_Supported: {
          return "HTTP Version Not Supported";
        }

        // Unknown
        default: { return null; }

      }
    }

  }

} // namespace Nuclex.Networking.Http
