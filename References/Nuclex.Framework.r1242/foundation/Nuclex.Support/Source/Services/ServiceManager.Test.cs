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

using Nuclex.Support.Plugins;

using NUnit.Framework;

#if ENABLE_SERVICEMANAGER

namespace Nuclex.Support.Services {

  /// <summary>Unit Test for the service manager class</summary>
  [TestFixture]
  public class ServiceManagerTest {

    #region interface IHelloContract

    /// <summary>A simple contract interface used for testing</summary>
    public interface IHelloContract { }

    #endregion // interface IHelloContract

    #region interface IWorldContract

    /// <summary>Another simple contract interface used for testing</summary>
    public interface IWorldContract { }

    #endregion // interface IWorldContract

    #region interface IHaveNoImplementation

    /// <summary>A contract interface that is not implementated anywhere</summary>
    public interface IHaveNoImplementation { }

    #endregion // interface IHaveNoImplementation

    #region class HelloComponent

    /// <summary>Test component that implements the hello contract</summary>
    public class HelloComponent : IHelloContract { }

    #endregion // class HelloComponent

    #region class WorldComponent

    /// <summary>
    ///   Test component that implements the world contract and requires
    ///   an implementation of the hello contract
    /// </summary>
    public class WorldComponent : IWorldContract, IHelloContract {
      /// <summary>Initializes a new world component</summary>
      /// <param name="helloContracts">
      ///   Array of hello contract implementations that will be used
      /// </param>
      public WorldComponent(IHelloContract[] helloContracts) { }
    }

    #endregion // class WorldComponent

    #region class IncompleteComponent

    /// <summary>
    ///   Test component that requires an implementation of a contract that has
    ///   no implementation available
    /// </summary>
    public class IncompleteComponent : IWorldContract {
      /// <summary>Initializes the component</summary>
      /// <param name="noImplementation">
      ///   Implementation of the unimplemented interface (:P) to use
      /// </param>
      public IncompleteComponent(IHaveNoImplementation noImplementation) { }
    }

    #endregion // class IncompleteComponent

    #region class NeedHello

    /// <summary>Component that needs an implementation of the hello contract</summary>
    public class NeedHello : IWorldContract {
      /// <summary>Initializes the component</summary>
      /// <param name="helloContract">
      ///   Implementation of the hello contract that will be used
      /// </param>
      public NeedHello(IHelloContract helloContract) { }
    }

    #endregion // class NeedHello

    #region class NeedWorld

    /// <summary>Component that needs an implementation of the world contract</summary>
    public class NeedWorld : IHelloContract {
      /// <summary>Initializes the component</summary>
      /// <param name="worldContract">
      ///   Implementation of the world contract that will be used
      /// </param>
      public NeedWorld(IWorldContract worldContract) { }
    }

    #endregion // class NeedWorld

    /// <summary>
    ///   Tests whether the GetComponents() method behaves correctly if it is used
    ///   without any assemblies loaded
    /// </summary>
    [Test]
    public void TestGetComponentsWithoutAssembly() {
      ServiceManager serviceManager = new ServiceManager(new PredefinedTypeLister());
      Assert.That(serviceManager.GetComponents<IDisposable>(), Is.Empty);
    }

    /// <summary>
    ///   Tests whether the GetComponents() method can locate a simple component
    /// </summary>
    [Test]
    public void TestGetComponents() {
      RepositoryTypeLister typeLister = new RepositoryTypeLister();
      ServiceManager serviceManager = new ServiceManager(typeLister);
      typeLister.Repository.AddAssembly(typeof(ServiceManagerTest).Assembly);

      Assert.That(
        serviceManager.GetComponents<IHelloContract>(),
        Has.Member(typeof(HelloComponent)).And.Member(typeof(WorldComponent))
      );
    }

    /// <summary>
    ///   Tests whether the GetComponents() method correctly determines which
    ///   components can have their dependencies completely provided.
    /// </summary>
    [Test]
    public void TestFilteredGetComponents() {
      RepositoryTypeLister typeLister = new RepositoryTypeLister();
      ServiceManager serviceManager = new ServiceManager(typeLister);
      typeLister.Repository.AddAssembly(typeof(ServiceManagerTest).Assembly);

      Assert.That(
        serviceManager.GetComponents<IWorldContract>(false),
        Has.Member(typeof(WorldComponent)).And.Member(typeof(IncompleteComponent))
      );
      Assert.That(
        serviceManager.GetComponents<IWorldContract>(true),
        Has.Member(typeof(WorldComponent)).And.No.Member(typeof(IncompleteComponent))
      );
    }

    /// <summary>
    ///   Tests whether the GetComponents() method can cope with two components
    ///   that have a circular dependency through their services.
    /// </summary>
    [Test]
    public void TestCircularDependency() {

    }

    /// <summary>
    ///   Verifies that the right exception is thrown if the non-generic GetService()
    ///   is used on a value type
    /// </summary>
    [Test]
    public void TestGetComponentOnValueType() {
      RepositoryTypeLister typeLister = new RepositoryTypeLister();
      ServiceManager serviceManager = new ServiceManager(typeLister);
      typeLister.Repository.AddAssembly(typeof(int).Assembly);

      Assert.Throws<ArgumentException>(
        delegate() { serviceManager.GetService(typeof(int)); }
      );
    }

  }

} // namespace Nuclex.Support.Services

#endif // ENABLE_SERVICEMANAGER

#endif // UNITTEST
