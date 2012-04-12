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

using System;
using System.IO;
using System.Reflection;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Plugins {

  /// <summary>Unit Test for the plugin host class</summary>
  [TestFixture, NoPlugin] // NoPlugin is used in one of the unit tests
  public class PluginHostTest {

    #region class FailingEmployer

    /// <summary>Employer that unexpectedly fails to employ a given type</summary>
    private class FailingEmployer : Employer {

      /// <summary>Employs the specified plugin type</summary>
      /// <param name="type">Type to be employed</param>
      public override void Employ(Type type) {
        if(type.Equals(typeof(PluginRepository))) {
          throw new InvalidOperationException();
        }
      }

    }

    #endregion // class FailingEmployer

    /// <summary>Tests whether the simple constructor is working</summary>
    [Test]
    public void TestSimpleConstructor() {
      new PluginHost(new FactoryEmployer<PluginHostTest>());
    }

    /// <summary>Tests whether the full constructor is working</summary>
    [Test]
    public void TestFullConstructor() {
      new PluginHost(new FactoryEmployer<PluginHostTest>(), new PluginRepository());
    }

    /// <summary>
    ///   Tests whether the AddAssembly() method works by adding the test assembly
    ///   itself to the repository
    /// </summary>
    [Test]
    public void TestFullConstructorWithPreloadedAssembly() {
      PluginRepository testRepository = new PluginRepository();
      FactoryEmployer<PluginRepository> testEmployer = new FactoryEmployer<PluginRepository>();

      // Might also use Assembly.GetCallingAssembly() here, but this leads to the exe of
      // the unit testing tool
      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      PluginHost testHost = new PluginHost(testEmployer, testRepository);

      Assert.AreSame(testEmployer, testHost.Employer);
      Assert.AreEqual(1, testEmployer.Factories.Count);
    }

    /// <summary>
    ///   Verifies that the plugin host correctly stores the provided repository
    /// </summary>
    [Test]
    public void TestRepositoryStorage() {
      PluginRepository testRepository = new PluginRepository();
      FactoryEmployer<PluginRepository> testEmployer = new FactoryEmployer<PluginRepository>();
      PluginHost testHost = new PluginHost(testEmployer, testRepository);

      Assert.AreSame(testRepository, testHost.Repository);
    }

    /// <summary>
    ///   Verifies that the plugin host correctly stores the provided employer
    /// </summary>
    [Test]
    public void TestEmployerStorage() {
      PluginRepository testRepository = new PluginRepository();
      FactoryEmployer<PluginRepository> testEmployer = new FactoryEmployer<PluginRepository>();
      PluginHost testHost = new PluginHost(testEmployer, testRepository);

      Assert.AreSame(testEmployer, testHost.Employer);
    }

    /// <summary>
    ///   Tests whether the plugin host noticed when new assemblies are loaded into
    ///   the repository
    /// </summary>
    [Test]
    public void TestAssemblyLoading() {
      PluginRepository testRepository = new PluginRepository();
      FactoryEmployer<PluginRepository> testEmployer = new FactoryEmployer<PluginRepository>();

      PluginHost testHost = new PluginHost(testEmployer, testRepository);

      // Might also use Assembly.GetCallingAssembly() here, but this leads to the exe of
      // the unit testing tool
      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      Assert.AreSame(testEmployer, testHost.Employer);
      Assert.AreEqual(1, testEmployer.Factories.Count);
    }

    /// <summary>
    ///   Tests whether the plugin host isolates the caller from an exception when the
    ///   employer fails to employ a type in the assembly
    /// </summary>
    [Test]
    public void TestAssemblyLoadingWithEmployFailure() {
      PluginRepository testRepository = new PluginRepository();
      PluginHost testHost = new PluginHost(new FailingEmployer(), testRepository);

      // Might also use Assembly.GetCallingAssembly() here, but this leads to the exe of
      // the unit testing tool
      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      Assert.AreSame(testRepository, testHost.Repository);
    }

    /// <summary>
    ///   Verifies that the plugin host ignores types which have the NoPluginAttribute
    ///   assigned to them
    /// </summary>
    [Test]
    public void TestAssemblyLoadingWithNoPluginAttribute() {
      PluginRepository testRepository = new PluginRepository();
      FactoryEmployer<PluginHostTest> testEmployer = new FactoryEmployer<PluginHostTest>();
      PluginHost testHost = new PluginHost(testEmployer, testRepository);

      // Might also use Assembly.GetCallingAssembly() here, but this leads to the exe of
      // the unit testing tool
      Assembly self = Assembly.GetAssembly(GetType());
      testRepository.AddAssembly(self);

      Assert.AreSame(testRepository, testHost.Repository);
      Assert.AreEqual(0, testEmployer.Factories.Count);
    }

  }

} // namespace Nuclex.Support.Plugins

#endif // UNITTEST
