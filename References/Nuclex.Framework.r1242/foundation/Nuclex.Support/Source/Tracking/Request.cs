#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

namespace Nuclex.Support.Tracking {

  /// <summary>Asynchronous request running in the background</summary>
  /// <remarks>
  ///   <para>
  ///     If the background process fails, the exception that caused it to fail is
  ///     communicated to all parties waiting on the Request through the Join()
  ///     method. Implementers should store any errors occuring in the asynchronous
  ///     parts of their code in a try..catch block (or avoid throwing and just
  ///     store a new exception) and re-throw them when in ReraiseExceptions()
  ///   </para>
  ///   <para>
  ///     Like in the transaction class, the contract requires you to always call
  ///     OnAsyncEnded(), no matter what the outcome of your operation is.
  ///   </para>
  /// </remarks>
  public abstract class Request : Transaction {

    #region class EndedDummyRequest

    /// <summary>Dummy request that is always in the ended state</summary>
    private class EndedDummyRequest : Request {
      /// <summary>Creates a new successfully completed dummy request</summary>
      public EndedDummyRequest() : this(null) { }
      /// <summary>Creates a new failed dummy request</summary>
      /// <param name="exception">Exception that caused the dummy to fail</param>
      public EndedDummyRequest(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }
      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override void ReraiseExceptions() {
        if(this.exception != null)
          throw this.exception;
      }
      /// <summary>Exception that supposedly caused the request to fail</summary>
      private Exception exception;
    }

    #endregion // EndedDummyRequest

    /// <summary>Succeeded dummy request</summary>
    /// <remarks>
    ///   Use to indicate success if the request has already been completed at
    ///   the time you are asked to perform it.
    /// </remarks>
    public static readonly Request SucceededDummy = new EndedDummyRequest();

    /// <summary>Creates a new failed dummy request</summary>
    /// <param name="exception">Exception that supposedly caused the request to fail</param>
    /// <returns>
    ///   A failed request that reports the provided exception as cause for its failure
    /// </returns>
    public static Request CreateFailedDummy(Exception exception) {
      return new EndedDummyRequest(exception);
    }

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call Wait() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public virtual void Join() {

      // If the request itself hasn't ended yet, block the caller until it has.
      // We could just use WaitHandle.WaitOne() here, but since the WaitHandle is created
      // on-the-fly only when it is requested, we can avoid the WaitHandle creation in
      // case the request is already finished!
      if(!Ended)
        Wait();

      // Allow the implementer to throw an exception in case an error has occured
      ReraiseExceptions();

    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected virtual void ReraiseExceptions() { }

  }

  /// <summary>Request providing a result that can be passed to the caller</summary>
  /// <typeparam name="ResultType">
  ///   Type of the result being provided by the request
  /// </typeparam>
  public abstract class Request<ResultType> : Request {

    #region class SucceededDummyRequest

    /// <summary>Succeeded dummy request that is always in the ended state</summary>
    private class SucceededDummyRequest : Request<ResultType> {
      /// <summary>Creates a new failed dummy request</summary>
      /// <param name="result">Result to return to the request's caller</param>
      public SucceededDummyRequest(ResultType result) {
        this.result = result;
        OnAsyncEnded();
      }
      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override ResultType GatherResults() {
        return this.result;
      }
      /// <summary>Results the succeede dummy request will provide to the caller</summary>
      private ResultType result;
    }

    #endregion // SucceededDummyRequest

    #region class FailedDummyRequest

    /// <summary>Failed dummy request that is always in the ended state</summary>
    private class FailedDummyRequest : Request<ResultType> {
      /// <summary>Creates a new failed dummy request</summary>
      /// <param name="exception">Exception that caused the dummy to fail</param>
      public FailedDummyRequest(Exception exception) {
        this.exception = exception;
        OnAsyncEnded();
      }
      /// <summary>
      ///   Allows the specific request implementation to re-throw an exception if
      ///   the background process finished unsuccessfully
      /// </summary>
      protected override ResultType GatherResults() {
        throw this.exception;
      }
      /// <summary>Exception that supposedly caused the request to fail</summary>
      private Exception exception;
    }

    #endregion // FailedDummyRequest

    /// <summary>Creates a new failed dummy request</summary>
    /// <param name="result">Result to provide to the caller</param>
    /// <returns>
    ///   A succeeded request that returns the provided result to the caller
    /// </returns>
    public static Request<ResultType> CreateSucceededDummy(ResultType result) {
      return new SucceededDummyRequest(result);
    }

    /// <summary>Creates a new failed dummy request</summary>
    /// <param name="exception">Exception that supposedly caused the request to fail</param>
    /// <returns>
    ///   A failed request that reports the provided exception as cause for its failure
    /// </returns>
    public static new Request<ResultType> CreateFailedDummy(Exception exception) {
      return new FailedDummyRequest(exception);
    }

    /// <summary>Waits for the background operation to end</summary>
    /// <remarks>
    ///   Any exceptions raised in the background operation will be thrown
    ///   in this method. If you decide to override this method, you should
    ///   call End() first (and let any possible exception through to your
    ///   caller).
    /// </remarks>
    public new ResultType Join() {
      base.Join();

      // Return the results of the request
      return GatherResults();
    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected override void ReraiseExceptions() {
      // Request and discard the result, so the implementor can do all error handling
      // in the GatherResults() method. This is a good default implementation as long
      // as the returned object does not require IDispose. It if does, this method
      // needs to be overridden.
      GatherResults();
    }

    /// <summary>
    ///   Allows the specific request to return the results of the Request to the
    ///   caller of the Join() method
    /// </summary>
    protected abstract ResultType GatherResults();

  }

} // namespace Nuclex.Support.Tracking
