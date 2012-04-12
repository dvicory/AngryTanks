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
using System.Reflection;

using NUnit.Framework;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  /// <summary>Unit Test for the cached assembly repository type lister</summary>
  [TestFixture]
  public class RepositoryTypeListerTest {

    /// <summary>
    ///   Tests whether the repository lister can cope with an empty repository
    /// </summary>
    [Test]
    public void TestEmptyLister() {
      RepositoryTypeLister testLister = new RepositoryTypeLister();

      Assert.That(testLister.GetTypes(), Is.Empty);
    }

    /// <summary>
    ///   Tests whether the repository lister notices an updated repository
    /// </summary>
    [Test]
    public void TestLateAdd() {
      RepositoryTypeLister testLister = new RepositoryTypeLister();
      testLister.Repository.AddAssembly(typeof(RepositoryTypeListerTest).Assembly);

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(RepositoryTypeListerTest))
      );
    }

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER

#endif // UNITTEST
