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

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.UserInterface.Controls {

  /// <summary>Unit Test for the control event argument container</summary>
  [TestFixture]
  internal class ControlEventArgsTest {

    /// <summary>
    ///   Tests whether a null control can be carried by the event argument container
    /// </summary>
    [Test]
    public void TestNullControl() {
      ControlEventArgs nullArgs = new ControlEventArgs(null);

      Assert.IsNull(nullArgs.Control);
    }

    /// <summary>
    ///   Tests whether a control can be carried by the event argument container
    /// </summary>
    [Test]
    public void TestNormalControl() {
      Control testControl = new Control();
      ControlEventArgs testArgs = new ControlEventArgs(testControl);

      Assert.AreSame(testControl, testArgs.Control);
    }

  }

} // namespace Nuclex.UserInterface.Controls

#endif // UNITTEST
