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

using Nuclex.Support;

using NUnit.Framework;
using NMock2;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the close window button</summary>
  [TestFixture]
  internal class ClosewindowButtonControlTest {

    /// <summary>
    ///   Verifies that instances of the close window button can be created
    /// </summary>
    [Test]
    public void TestConstructor() {
      CloseWindowButtonControl button = new CloseWindowButtonControl();
      Assert.IsNotNull(button); // nonsense; prevents compiler warning
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
