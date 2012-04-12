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

  /// <summary>Unit Test for the cached assembly type lister</summary>
  [TestFixture]
  public class MultiAssemblyTypeListerTest {

    #region class TestAssemblyTypeLister

    /// <summary>Test implementation of a cached assembly type lister</summary>
    private class TestAssemblyTypeLister : MultiAssemblyTypeLister {

      /// <summary>Initializes a new test assembly type lister</summary>
      /// <param name="assemblies">Assemblies whose types will be listed</param>
      public TestAssemblyTypeLister(params Assembly[] assemblies) {
        ReplaceAssemblyList(assemblies);
      }

      /// <summary>Replaces the list of assemblies whose types to list</summary>
      /// <param name="assemblies">Assemblies whose types will be listed</param>
      public void ReplaceAssemblyList(params Assembly[] assemblies) {
        this.assemblies = assemblies;
      }

      /// <summary>Obtains a list of any assemblies whose types should be listed</summary>
      /// <returns>A list of any assemblies whose types to list</returns>
      protected override IEnumerable<Assembly> GetAssemblies() {
        return this.assemblies;
      }

      /// <summary>Assemblies whose types the test assembly type lister lists</summary>
      private Assembly[] assemblies;

    }

    #endregion // class TestAssemblyTypeLister

    /// <summary>
    ///   Verifies that the assembly type list is generated correctly
    /// </summary>
    [Test]
    public void TestAssemblyListGeneration() {
      TestAssemblyTypeLister testLister = new TestAssemblyTypeLister(
        typeof(MultiAssemblyTypeListerTest).Assembly
      );

      Assert.That(
        testLister.GetTypes(), Has.Member(typeof(MultiAssemblyTypeListerTest))
      );
    }

    /// <summary>
    ///   Verifies that the assembly type list is updated when list of assemblies
    ///   changes inbetween calls
    /// </summary>
    [Test]
    public void TestAssemblyListReplacement() {
      TestAssemblyTypeLister testLister = new TestAssemblyTypeLister(
        typeof(Assembly).Assembly,
        typeof(TestAttribute).Assembly
      );

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(TestAttribute)).And.Not.Member(typeof(MultiAssemblyTypeListerTest))
      );

      testLister.ReplaceAssemblyList(
        typeof(Assembly).Assembly,
        typeof(MultiAssemblyTypeListerTest).Assembly
      );

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(MultiAssemblyTypeListerTest)).And.Not.Member(typeof(TestAttribute))
      );
    }

    /// <summary>
    ///   Verifies that the assembly type list is updated when an assembly is removed
    ///   from the list inbetween calls
    /// </summary>
    [Test]
    public void TestAssemblyListRemoval() {
      TestAssemblyTypeLister testLister = new TestAssemblyTypeLister(
        typeof(Assembly).Assembly,
        typeof(TestAttribute).Assembly,
        typeof(MultiAssemblyTypeListerTest).Assembly
      );

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(TestAttribute)).And.Member(typeof(MultiAssemblyTypeListerTest))
      );

      testLister.ReplaceAssemblyList(
        typeof(Assembly).Assembly,
        typeof(MultiAssemblyTypeListerTest).Assembly
      );

      Assert.That(
        testLister.GetTypes(),
        Has.Member(typeof(MultiAssemblyTypeListerTest)).And.Not.Member(typeof(TestAttribute))
      );
    }

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER

#endif // UNITTEST

