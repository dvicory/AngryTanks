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

  /// <summary>Status codes that can be returned by an HTTP server</summary>
  public enum StatusCode {

    /// <summary>The client should continue with its request</summary>
    /// <remarks>
    ///   This interim response is used to inform the client that the initial part
    ///   of the request has been received and has not yet been rejected by the
    ///   server.
    /// </remarks>
    S100_Continue = 100,

    /// <summary>
    ///   The server understands and is willing to comply with the client's request,
    ///   via the Upgrade message header field (section 14.42), for a change in
    ///   the application protocol being used on this connection.
    /// </summary>
    /// <remarks>
    ///   The server will switch protocols to those defined by the response's
    ///   Upgrade header field immediately after the empty line which terminates
    ///   the 101 response.
    /// </remarks>
    S101_SwitchingProtocols = 101,

    /// <summary>The request has succeeded</summary>
    /// <remarks>
    ///   The information returned with the response is dependent on the method used
    ///   in the request.
    /// </remarks>
    S200_OK = 200,

    /// <summary>
    ///   The request has been fulfilled and resulted in a new resource being created
    /// </summary>
    /// <remarks>
    ///    The newly created resource can be referenced by the URI(s) returned in
    ///    the entity of the response, with the most specific URI for the resource
    ///    given by a Location header field
    /// </remarks>
    S201_Created = 201,

    /// <summary>
    ///   The request has been accepted for processing, but the processing has not
    ///   been completed
    /// </summary>
    /// <remarks>
    ///   The request might or might not eventually be acted upon, as it might be
    ///   disallowed when processing actually takes place.
    /// </remarks>
    S202_Accepted = 202,

    /// <summary>
    ///   The returned metainformation in the entity-header is not the definitive set
    ///   as available from the origin server, but is gathered from a local or a
    ///   third-party copy.
    /// </summary>
    S203_Non_Authoritative_Information = 203,

    /// <summary>
    ///   The server has fulfilled the request but does not need to return an entity-body,
    ///   and might want to return updated metainformation
    /// </summary>
    /// <remarks>
    ///   The response may include new or updated metainformation in the form of
    ///   entity-headers, which if present SHOULD be associated with the requested variant.
    /// </remarks>
    S204_No_Content = 204,

    /// <summary>
    ///   The server has fulfilled the request and the user agent should reset the document
    ///   view which caused the request to be sent
    /// </summary>
    /// <remarks>
    ///   This response is primarily intended to allow input for actions to take place via
    ///   user input, followed by a clearing of the form in which the input is given so
    ///   that the user can easily initiate another input action.
    /// </remarks>
    S205_Reset_Content = 205,

    /// <summary>The server has fulfilled the partial GET request for the resource</summary>
    /// <remarks>
    ///   The request must have included a Range header field (section 14.35) indicating
    ///   the desired range, and may have included an If-Range header field (section 14.27)
    ///   to make the request conditional.
    /// </remarks>
    S206_Partial_Content = 206,

    /// <summary>
    ///   The requested resource corresponds to any one of a set of representations, each
    ///   with its own specific location, and agent-driven negotiation information
    ///   (section 12) is being provided so that the user (or user agent) can select
    ///   a preferred representation and redirect its request to that location
    /// </summary>
    S300_Multiple_Choices = 300,

    /// <summary>
    ///   The requested resource has been assigned a new permanent URI and any future
    ///   references to this resource should use one of the returned URIs
    /// </summary>
    S301_Moved_Permanently = 301,

    /// <summary>The requested resource resides temporarily under a different URI</summary>
    /// <remarks>
    ///   Since the redirection might be altered on occasion, the client should
    ///   continue to use the Request-URI for future requests.
    /// </remarks>
    S302_Found = 302,

    /// <summary>
    ///   The response to the request can be found under a different URI and should be
    ///   retrieved using a GET method on that resource
    /// </summary>
    /// <remarks>
    ///   This method exists primarily to allow the output of a POST-activated script
    ///   to redirect the user agent to a selected resource.
    /// </remarks>
    S303_See_Other = 303,

    /// <summary>
    ///   If the client has performed a conditional GET request and access is allowed,
    ///   but the document has not been modified, the server should respond with this
    ///   status code
    /// </summary>
    S304_Not_Modified = 304,

    /// <summary>
    ///   The requested resource MUST be accessed through the proxy given by
    ///   the Location field
    /// </summary>
    S305_Use_Proxy = 305,

    /// <summary>
    ///   The requested resource resides temporarily under a different URI. Since
    ///   the redirection may be altered on occasion, the client should continue to
    ///   use the Request-URI for future requests
    /// </summary>
    S307_Temporary_Redirect = 307,

    /// <summary>
    ///   The request could not be understood by the server due to malformed syntax
    /// </summary>
    S400_Bad_Request = 400,

    /// <summary>The request requires user authentication</summary>
    /// <remarks>
    ///   The response MUST include a WWW-Authenticate header field (section 14.47)
    ///   containing a challenge applicable to the requested resource.
    /// </remarks>
    S401_Unauthorized = 401,

    /// <summary>This code is reserved for future use</summary>
    S402_Payment_Required = 402,

    /// <summary>The server understood the request, but is refusing to fulfill it</summary>
    /// <remarks>
    ///   Authorization will not help and the request SHOULD NOT be repeated.
    /// </remarks>
    S403_Forbidden = 403,

    /// <summary>The server has not found anything matching the Request-URI</summary>
    S404_Not_Found = 404,

    /// <summary>
    ///   The method specified in the Request-Line is not allowed for the resource
    ///   identified by the Request-URI
    /// </summary>
    S405_Method_Not_Allowed = 405,

    /// <summary>
    ///   The resource identified by the request is only capable of generating response
    ///   entities which have content characteristics not acceptable according to
    ///   the accept headers sent in the request
    /// </summary>
    S406_Not_Acceptable = 406,

    /// <summary>
    ///   This code is similar to 401 (Unauthorized), but indicates that the client must
    ///   first authenticate itself with the proxy
    /// </summary>
    S407_Proxy_Authentication_Required = 407,

    /// <summary>
    ///   The client did not produce a request within the time that the server was
    ///   prepared to wait
    /// </summary>
    S408_Request_Timeout = 408,

    /// <summary>
    ///   The request could not be completed due to a conflict with the current state
    ///   of the resource
    /// </summary>
    S409_Conflict = 409,

    /// <summary>
    ///   The requested resource is no longer available at the server and no forwarding
    ///   address is known
    /// </summary>
    S410_Gone = 410,

    /// <summary>
    ///   The server refuses to accept the request without a defined Content-Length
    /// </summary>
    S411_Length_Required = 411,

    /// <summary>
    ///   The precondition given in one or more of the request-header fields evaluated
    ///   to false when it was tested on the server
    /// </summary>
    S412_Precondition_Field = 412,

    /// <summary>
    ///   The server is refusing to process a request because the request entity is
    ///   larger than the server is willing or able to process
    /// </summary>
    S413_Request_Entity_Too_Large = 413,

    /// <summary>
    ///   The server is refusing to service the request because the Request-URI is
    ///   longer than the server is willing to interpret
    /// </summary>
    S414_Request_Uri_Too_Long = 414,

    /// <summary>
    ///   The server is refusing to service the request because the entity of
    ///   the request is in a format not supported by the requested resource for
    ///   the requested method
    /// </summary>
    S415_Unsupported_Media_Type = 415,

    /// <summary>
    ///   A server should return a response with this status code if a request included
    ///   a Range request-header field (section 14.35), and none of the range-specifier
    ///   values in this field overlap the current extent of the selected resource, and
    ///   the request did not include an If-Range request-header field
    /// </summary>
    S416_Request_Range_Not_Satisfiable = 416,

    /// <summary>
    ///   The expectation given in an Expect request-header field (see section 14.20)
    ///   could not be met by this server, or, if the server is a proxy, the server has
    ///   unambiguous evidence that the request could not be met by the next-hop server
    /// </summary>
    S417_Expectation_Failed = 417,

    /// <summary>
    ///   The server encountered an unexpected condition which prevented it from
    ///   fulfilling the request
    /// </summary>
    S500_Internal_Server_Error = 500,

    /// <summary>
    ///   The server does not support the functionality required to fulfill the request
    /// </summary>
    S501_Not_Implemented = 501,

    /// <summary>
    ///   The server, while acting as a gateway or proxy, received an invalid response
    ///   from the upstream server it accessed in attempting to fulfill the request
    /// </summary>
    S502_Bad_Gateway = 502,

    /// <summary>
    ///   The server is currently unable to handle the request due to a temporary
    ///   overloading or maintenance of the server
    /// </summary>
    S503_Service_Unavailable = 503,

    /// <summary>
    ///   The server, while acting as a gateway or proxy, did not receive a timely response
    ///   from the upstream server specified by the URI (e.g. HTTP, FTP, LDAP) or some
    ///   other auxiliary server (e.g. DNS) it needed to access in attempting to complete
    ///   the request
    /// </summary>
    S504_Gateway_Timeout = 504,

    /// <summary>
    ///   The server does not support, or refuses to support, the HTTP protocol version
    ///   that was used in the request message
    /// </summary>
    S505_Http_Version_Not_Supported = 505

  }

} // namespace Nuclex.Networking.Http
