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
using System.Diagnostics;
using System.Threading;

namespace Nuclex.Support.Scheduling {

  /// <summary>Operation that executes a method in a background thread</summary>
  public abstract class ThreadOperation : Operation {

    /// <summary>
    ///   Initializes a new threaded operation.
    /// </summary>
    /// <remarks>
    ///   Uses a ThreadPool thread to execute the method in a background thread.
    /// </remarks>
    public ThreadOperation() : this(true) { }

    /// <summary>
    ///   Initializes a new threaded operation which optionally uses the ThreadPool.
    /// </summary>
    /// <param name="useThreadPool">Whether to use a ThreadPool thread.</param>
    /// <remarks>
    ///   If useThreadPool is false, a new thread will be created. This guarantees
    ///   that the method will be executed immediately but has an impact on
    ///   performance since the creation of new threads is not a cheap operation.
    /// </remarks>
    public ThreadOperation(bool useThreadPool) {
      this.useThreadPool = useThreadPool;
    }

    /// <summary>Launches the background operation</summary>
    public override void Start() {
      Debug.Assert(
        !Ended,
        "Tried to Start an Operation again that has already ended",
        "Operations cannot be re-run"
      );
      if(useThreadPool) {
        ThreadPool.QueueUserWorkItem(new WaitCallback(callMethod));
      } else {
        Thread thread = new Thread(new ThreadStart(callMethod));
        thread.Name = "Nuclex.Support.Scheduling.ThreadOperation";
        thread.IsBackground = true;
        thread.Start();
      }
    }

    /// <summary>Contains the payload to be executed in the background thread</summary>
    protected abstract void Execute();

    /// <summary>Invokes the delegate passed as an argument</summary>
    /// <param name="state">Not used</param>
    private void callMethod(object state) {
      callMethod();
    }

    /// <summary>Invokes the delegate passed as an argument</summary>
    private void callMethod() {
      try {
        Execute();
        Debug.Assert(
          !Ended,
          "Operation unexpectedly ended during Execute()",
          "Do not call OnAsyncEnded() yourself when deriving from ThreadOperation"
        );
      }
      catch(Exception exception) {
        this.exception = exception;
      }
      finally {
        OnAsyncEnded();
      }
    }

    /// <summary>
    ///   Allows the specific request implementation to re-throw an exception if
    ///   the background process finished unsuccessfully
    /// </summary>
    protected override void ReraiseExceptions() {
      if(this.exception != null)
        throw this.exception;
    }

    /// <summary>Whether to use the ThreadPool for obtaining a background thread</summary>
    private bool useThreadPool;
    /// <summary>Exception that has occured in the background process</summary>
    private volatile Exception exception;

  }

} // namespace Nuclex.Support.Scheduling
