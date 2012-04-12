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

  /// <summary>Unit Test for the "idle state" event argument container</summary>
  [TestFixture]
  public class IdleStateEventArgsTest {

    /// <summary>
    ///   Tests whether the idle state event arguments correctly report a non-idle state
    /// </summary>
    [Test]
    public void TestIdleStateChangedToFalse() {
      IdleStateEventArgs idleStateFalse = new IdleStateEventArgs(false);

      Assert.IsFalse(idleStateFalse.Idle);
    }

    /// <summary>
    ///   Tests whether the idle state event arguments correctly report an idle state
    /// </summary>
    [Test]
    public void TestIdleStateChangedToTrue() {
      IdleStateEventArgs idleStateFalse = new IdleStateEventArgs(true);

      Assert.IsTrue(idleStateFalse.Idle);
    }

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
