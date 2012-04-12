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

#if UNITTEST

using System;
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the progress report event argument container</summary>
  [TestFixture]
  public class ProgressReportEventArgsTest {

    /// <summary>
    ///   Tests whether the progress report event arguments correctly report zero progress
    /// </summary>
    [Test]
    public void TestZeroProgress() {
      ProgressReportEventArgs zeroProgress = new ProgressReportEventArgs(0.0f);

      Assert.AreEqual(0.0f, zeroProgress.Progress);
    }

    /// <summary>
    ///   Tests whether the progress report event arguments correctly report complete progress
    /// </summary>
    [Test]
    public void TestCompleteProgress() {
      ProgressReportEventArgs zeroProgress = new ProgressReportEventArgs(1.0f);

      Assert.AreEqual(1.0f, zeroProgress.Progress);
    }

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
