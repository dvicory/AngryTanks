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
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.Xna.Framework.Input;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Input {

  /// <summary>Unit tests for the game pad buttons helper</summary>
  [TestFixture]
  internal class GamePadButtonsHelperTest {

    /// <summary>Verifies that the Contains() method is working</summary>
    [Test]
    public void TestContains() {
      Buttons buttons = Buttons.A | Buttons.X;
      
      Assert.IsTrue(GamePadButtonsHelper.Contains(buttons, Buttons.A));
      Assert.IsTrue(GamePadButtonsHelper.Contains(buttons, Buttons.X));
      Assert.IsFalse(GamePadButtonsHelper.Contains(buttons, Buttons.B));
      Assert.IsFalse(GamePadButtonsHelper.Contains(buttons, Buttons.Y));
    }

  }

} // namespace Nuclex.Input

#endif // UNITTEST
