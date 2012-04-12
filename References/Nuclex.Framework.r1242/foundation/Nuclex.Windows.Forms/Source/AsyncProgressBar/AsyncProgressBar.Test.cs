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

#if UNITTEST

using System;
using System.IO;
using System.Windows.Forms;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.Windows.Forms {

  /// <summary>Unit Test for the asynchronously updating progress bar</summary>
  [TestFixture]
  public class AsyncProgressBarTest {

    /// <summary>
    ///   Verifies that asynchronous progress assignment is working
    /// </summary>
    [Test]
    public void TestProgressAssignment() {
      using(AsyncProgressBar progressBar = new AsyncProgressBar()) {

        // Let the control create its window handle
        progressBar.CreateControl();
        progressBar.Minimum = 0;
        progressBar.Maximum = 100;
        
        Assert.AreEqual(0, progressBar.Value);
        
        // Assign the new value. This will be done asynchronously, so we call
        // Application.DoEvents() to execute the message pump once, guaranteeing
        // that the call will have been executed after Application.DoEvents() returns.
        progressBar.AsyncSetValue(0.33f);
        Application.DoEvents();
        
        Assert.AreEqual(33, progressBar.Value);

        progressBar.AsyncSetValue(0.66f);
        Application.DoEvents();

        Assert.AreEqual(66, progressBar.Value);

      }
    }

  }

} // namespace Nuclex.Windows.Forms

#endif // UNITTEST
