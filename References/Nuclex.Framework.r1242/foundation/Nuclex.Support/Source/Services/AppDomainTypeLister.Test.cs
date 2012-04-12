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

  /// <summary>Unit Test for the cached app domain type lister</summary>
  [TestFixture]
  public class AppDomainTypeListerTest {

    /// <summary>
    ///   Verifies that the assembly type list is generated correctly for
    ///   the default constructor (using the calling app domain)
    /// </summary>
    [Test]
    public void TestDefaultConstructur() {
      AppDomainTypeLister testLister = new AppDomainTypeLister();

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(AppDomainTypeListerTest)).And.Member(typeof(Assembly))
      );
    }

    /// <summary>
    ///   Verifies that the assembly type list is generated correctly for
    ///   the full constructor
    /// </summary>
    [Test]
    public void TestFullConstructur() {
      AppDomain newAppDomain = AppDomain.CreateDomain("AppDomainTypeListerTest.Domain");
      try {
        AppDomainTypeLister testLister = new AppDomainTypeLister(newAppDomain);

        Assert.That(
          testLister.GetTypes(),
          Has.Member(typeof(Assembly)).And.No.Member(typeof(AppDomainTypeListerTest))
        );
      }
      finally {
        AppDomain.Unload(newAppDomain);
      }
    }

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER

#endif // UNITTEST
