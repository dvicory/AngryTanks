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
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;
using NMock2;

namespace Nuclex.Game.States {

  /// <summary>Unit test for the load progress event argument container</summary>
  [TestFixture]
  internal class LoadProgressEventArgsTest {

    /// <summary>
    ///   Tests whether an integer argument can be stored in the argument container
    /// </summary>
    [Test]
    public void TestIntegerArgument() {
      LoadProgressEventArgs test = new LoadProgressEventArgs(0.12345f);
      Assert.AreEqual(0.12345f, test.Progress);
    }

  }

} // namespace Nuclex.Game.States

#endif // UNITTEST
