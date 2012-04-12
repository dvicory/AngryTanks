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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NUnit.Framework;

using Nuclex.Graphics.Batching;
using Nuclex.Testing.Xna;

namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel {

  /// <summary>Unit tests for the particle system manager</summary>
  [TestFixture]
  internal class ParticleSystemManagerTest {

    #region class DummyDrawContext

    /// <summary>Dummy drawing context that does nothing</summary>
    private class DummyDrawContext : DrawContext {

      /// <summary>Number of passes this draw context requires for rendering</summary>
      public override int Passes { get { return 0; } }

#if !XNA_4

      /// <summary>Begins the drawing cycle</summary>
      public override void Begin() { }

      /// <summary>Ends the drawing cycle</summary>
      public override void End() { }

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      public override void BeginPass(int pass) { }

      /// <summary>Restores the graphics device after drawing has finished</summary>
      public override void EndPass() { }

#else

      /// <summary>Prepares the graphics device for drawing</summary>
      /// <param name="pass">Index of the pass to begin rendering</param>
      public override void Apply(int pass) { }

#endif

      /// <summary>Tests whether another draw context is identical to this one</summary>
      /// <param name="otherContext">Other context to check for equality</param>
      /// <returns>True if the other context is identical to this one</returns>
      public override bool Equals(DrawContext otherContext) {
        return false;
      }

    }

    #endregion // class DummyDrawContext

    #region class CallbackReceiver

    /// <summary>Helper class used to test callbacks</summary>
    private class CallbackReceiver {

      /// <summary>Callback method that records the state</summary>
      /// <param name="asyncResult">Asynchronous result handle of the operation</param>
      public void Callback(IAsyncResult asyncResult) {
        this.State = asyncResult.AsyncState;
      }

      /// <summary>State that has been passed to the callback method</summary>
      public object State;

    }

    #endregion // class CallbackReceiver

    #region class ExceptionThrowingAffector

    /// <summary>Particle affector which throws an exception</summary>
    private class ExceptionThrowingAffector : IParticleAffector<SimpleParticle> {

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable {
        get { return false; }
      }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(SimpleParticle[] particles, int start, int count, int updates) {
        throw new ArithmeticException(); // some unlikely exception easy to recognize
      }

    }

    #endregion // class ExceptionThrowingAffector

    #region class ExceptionThrowingAffector

    /// <summary>Particle affector which takes a long time</summary>
    private class SlowAffector : IParticleAffector<SimpleParticle> {

      /// <summary>
      ///   Whether the affector can do multiple updates in a single step without
      ///   changing the outcome of the simulation
      /// </summary>
      public bool IsCoalescable {
        get { return false; }
      }

      /// <summary>Applies the affector's effect to a series of particles</summary>
      /// <param name="particles">Particles the affector will be applied to</param>
      /// <param name="start">Index of the first particle that will be affected</param>
      /// <param name="count">Number of particles that will be affected</param>
      /// <param name="updates">Number of updates to perform in the affector</param>
      public void Affect(SimpleParticle[] particles, int start, int count, int updates) {
        Thread.Sleep(0);
      }

    }

    #endregion // class ExceptionThrowingAffector

    #region class DummyRenderer

    /// <summary>Dummy particle renderer for the unit test</summary>
    private class DummyRenderer<ParticleType> : IParticleRenderer<ParticleType>
      where ParticleType : struct
#if XNA_4
, IVertexType
#endif
 {

      /// <summary>Renders a series of particles</summary>
      /// <param name="particles">Particles that will be rendered</param>
      /// <param name="primitiveBatch">
      ///   Primitive batch that will receive the vertices generated by the particles
      /// </param>
      public void Render(
        ArraySegment<ParticleType> particles,
        PrimitiveBatch<ParticleType> primitiveBatch
      ) { }

    }

    #endregion // class DummyRenderer

    /// <summary>Called before each test is run</summary>
    [SetUp]
    public void Setup() {
      this.mockedGraphicsDeviceService = new MockedGraphicsDeviceService(DeviceType.Reference);
      this.mockedGraphicsDeviceService.CreateDevice();

      this.contentManager = new ResourceContentManager(
        GraphicsDeviceServiceHelper.MakePrivateServiceProvider(
          this.mockedGraphicsDeviceService
        ),
        Resources.ScreenMaskResources.ResourceManager
      );
      this.effect = this.contentManager.Load<Effect>("ScreenMaskEffect");
    }

    /// <summary>Called after each test has run</summary>
    [TearDown]
    public void Teardown() {
      if(this.contentManager != null) {
        this.contentManager.Dispose();
        this.effect = null;
        this.contentManager = null;
      }
      if(this.mockedGraphicsDeviceService != null) {
        this.mockedGraphicsDeviceService.DestroyDevice();
        this.mockedGraphicsDeviceService = null;
      }
    }

    /// <summary>
    ///   Verifies that the particle system manager's constructor is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        Assert.IsNotNull(manager); // nonsense; prevents compiler warning
      }
    }

#if !XNA_4

    /// <summary>
    ///   Tests whether vertex types can be registered to the particle system manager
    /// </summary>
    [Test]
    public void TestVertexRegistration() {
      using(
        VertexDeclaration vertexDeclaration = new VertexDeclaration(
          this.mockedGraphicsDeviceService.GraphicsDevice, SimpleParticle.VertexElements
        )
      ) {
        using(
          ParticleSystemManager manager = new ParticleSystemManager(
            this.mockedGraphicsDeviceService
          )
        ) {
          manager.RegisterVertex<SimpleParticle>(
            vertexDeclaration, SimpleParticle.SizeInBytes
          );
          manager.UnregisterVertex<SimpleParticle>();
        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if a vertex type is registered twice
    /// </summary>
    [Test]
    public void TestThrowOnRegisterVertexTwice() {
      using(
        VertexDeclaration vertexDeclaration = new VertexDeclaration(
          this.mockedGraphicsDeviceService.GraphicsDevice, SimpleParticle.VertexElements
        )
      ) {
        using(
          ParticleSystemManager manager = new ParticleSystemManager(
            this.mockedGraphicsDeviceService
          )
        ) {

          // Normal registration
          manager.RegisterVertex<SimpleParticle>(
            vertexDeclaration, SimpleParticle.SizeInBytes
          );

          // Registering the vertex on top of another registration
          Assert.Throws<ArgumentException>(
            delegate() {
              manager.RegisterVertex<SimpleParticle>(
                vertexDeclaration, SimpleParticle.SizeInBytes
              );
            }
          );

        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if a vertex type is unregistered that
    ///   has not been registered.
    /// </summary>
    [Test]
    public void TestThrowOnVertexRegistrationErrors() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        Assert.Throws<ArgumentException>(
          delegate() { manager.UnregisterVertex<SimpleParticle>(); }
        );
      }
    }

    /// <summary>
    ///   Tests whether vertex types can be registered to the particle system manager
    ///   when it has already automatically determined a vertex declaration for
    ///   a particle system.
    /// </summary>
    [Test]
    public void TestLateVertexRegistration() {
      using(
        VertexDeclaration vertexDeclaration = new VertexDeclaration(
          this.mockedGraphicsDeviceService.GraphicsDevice, SimpleParticle.VertexElements
        )
      ) {
        using(
          ParticleSystemManager manager = new ParticleSystemManager(
            this.mockedGraphicsDeviceService
          )
        ) {
          manager.AddParticleSystem(
            new ParticleSystem<SimpleParticle>(100), dontPrune, new DummyDrawContext()
          );

          // This should replace the auto-generated vertex declaration
          manager.RegisterVertex<SimpleParticle>(
            vertexDeclaration, SimpleParticle.SizeInBytes, true
          );

          // This should restore the auto-generated vertex declaration
          // (since there's still a particle system that requires this vertex)
          manager.UnregisterVertex<SimpleParticle>();
        }
      }
    }

#endif

    /// <summary>
    ///   Tests whether particle systems can be added and removed from the manager
    /// </summary>
    [Test]
    public void TestAddParticleSystem() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(100);
#if !XNA_4
        // Add and remove the particle system using the default point sprite renderer
        manager.AddParticleSystem(test, dontPrune, this.effect);
        manager.RemoveParticleSystem(test);
#endif
        // Add and remove the particle system using a user-defined renderer
        manager.AddParticleSystem(test, dontPrune, new DummyRenderer<SimpleParticle>());
        manager.RemoveParticleSystem(test);
      }
    }

    /// <summary>
    ///   Verifies that an exception during primitive batch creation is handled
    /// </summary>
    [Test]
    public void TestThrowDuringPrimitiveBatchCreation() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        manager.InducePrimitiveBatchErrorDelegate = delegate() {
          throw new ArithmeticException("Simulated error");
        };
        Assert.Throws<ArithmeticException>(
          delegate() {
            manager.AddParticleSystem(
              new ParticleSystem<SimpleParticle>(100),
              dontPrune, new DummyRenderer<SimpleParticle>()
            );
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown that a particle system is removed from
    ///   the manager that has not been added
    /// </summary>
    [Test]
    public void TestThrowOnRemoveNotAddedParticleSystem() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        Assert.Throws<ArgumentException>(
          delegate() {
            manager.RemoveParticleSystem(new ParticleSystem<SimpleParticle>(100));
          }
        );
      }
    }

#if !XNA_4

    /// <summary>
    ///   Verifies that an exception is thrown if a particle system is added to
    ///   the manager with null specified for the drawing context
    /// </summary>
    [Test]
    public void TestThrowOnAddParticleSystemWithNullContext() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        DrawContext nullContext = null;
        Assert.Throws<ArgumentException>(
          delegate() {
            manager.AddParticleSystem(
              new ParticleSystem<SimpleParticle>(100), dontPrune, nullContext
            );
          }
        );
      }
    }

#endif

    /// <summary>
    ///   Verifies that an exception is thrown if a particle system is added to
    ///   the manager with null specified for the renderer
    /// </summary>
    [Test]
    public void TestThrowOnAddParticleSystemWithNullRenderer() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        IParticleRenderer<SimpleParticle> nullRenderer = null;
        Assert.Throws<ArgumentException>(
          delegate() {
            manager.AddParticleSystem(
              new ParticleSystem<SimpleParticle>(100), dontPrune, nullRenderer
            );
          }
        );
      }
    }

    /// <summary>
    ///   Verifies that the particle system manager can update its particle systems
    /// </summary>
    [Test]
    public void TestUpdate() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        for(int index = 0; index < 2; ++index) {
          manager.AddParticleSystem(
            new ParticleSystem<SimpleParticle>(100),
            dontPrune, new DummyRenderer<SimpleParticle>()
          );
        }

        manager.Update(2);
      }
    }

    /// <summary>
    ///   Verifies that the particle system manager can update its particle systems
    ///   asynchronously
    /// </summary>
    [Test]
    public void TestAsynchronousUpdate() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        // Add one particle system where there's stuff to do
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(16);
        for(int index = 0; index < test.Capacity; ++index) {
          test.AddParticle(new SimpleParticle());
        }
        test.Affectors.Add(new SlowAffector());
        manager.AddParticleSystem(test, dontPrune, new DummyRenderer<SimpleParticle>());

        // Add 16 other particle systems
        for(int index = 0; index < 16; ++index) {
          manager.AddParticleSystem(
            new ParticleSystem<SimpleParticle>(100),
            dontPrune, new DummyRenderer<SimpleParticle>()
          );
        }

        // Now update everything
        for(int repetition = 0; repetition < 2; ++repetition) {
          IAsyncResult asyncResult = manager.BeginUpdate(2, 4, null, null);
          manager.EndUpdate(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if a wrong async result is specified
    ///   when calling the EndUpdate() method
    /// </summary>
    [Test]
    public void TestThrowOnWrongAsyncResultInEndUpdate() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        IAsyncResult asyncResult = manager.BeginUpdate(1, 1, null, null);
        try {
          Assert.Throws<ArgumentException>(
            delegate() { manager.EndUpdate(null); }
          );
        }
        finally {
          manager.EndUpdate(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that the user-defined callback is invoked when an asynchronous
    ///   update completes
    /// </summary>
    [Test]
    public void TestAsynchronousUpdateCallback() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        CallbackReceiver receiver = new CallbackReceiver();
        object state = new object();

        IAsyncResult asyncResult = manager.BeginUpdate(
          1, 1, receiver.Callback, state
        );
        manager.EndUpdate(asyncResult);

        Assert.AreSame(state, receiver.State);
        Assert.IsFalse(asyncResult.CompletedSynchronously);
      }
    }

    /// <summary>
    ///   Tests whether exceptions during asynchronous updating are handles
    /// </summary>
    [Test]
    public void TestThrowDuringAsynchronousUpdate() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(4096);
        for(int index = 0; index < test.Capacity; ++index) {
          test.AddParticle(new SimpleParticle());
        }
        test.Affectors.Add(new ExceptionThrowingAffector());

        manager.AddParticleSystem(test, dontPrune, new DummyRenderer<SimpleParticle>());

        IAsyncResult asyncResult = manager.BeginUpdate(1, 1, null, null);
        Assert.Throws<ArithmeticException>(
          delegate() { manager.EndUpdate(asyncResult); }
        );
      }
    }

    /// <summary>
    ///   Verifies that the particle system manager can prune its particle systems
    /// </summary>
    [Test]
    public void TestPrune() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        for(int index = 0; index < 2; ++index) {
          manager.AddParticleSystem(
            new ParticleSystem<SimpleParticle>(100),
            dontPrune, new DummyRenderer<SimpleParticle>()
          );
        }

        manager.Prune();
      }
    }

    /// <summary>
    ///   Verifies that the particle system manager can prune its particle systems
    ///   asynchronously
    /// </summary>
    [Test]
    public void TestAsynchronousPrune() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        // Add one particle system where there's stuff to do
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(16);
        for(int index = 0; index < test.Capacity; ++index) {
          test.AddParticle(new SimpleParticle());
        }
        manager.AddParticleSystem(test, slowPrune, new DummyRenderer<SimpleParticle>());

        // Add 16 other particle systems
        for(int index = 0; index < 16; ++index) {
          manager.AddParticleSystem(
            new ParticleSystem<SimpleParticle>(100),
            dontPrune, new DummyRenderer<SimpleParticle>()
          );
        }

        // Now update everything
        for(int repetition = 0; repetition < 2; ++repetition) {
          IAsyncResult asyncResult = manager.BeginPrune(null, null);
          manager.EndPrune(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that an exception is thrown if a wrong async result is specified
    ///   when calling the EndPrune() method
    /// </summary>
    [Test]
    public void TestThrowOnWrongAsyncResultInEndPrune() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        IAsyncResult asyncResult = manager.BeginPrune(null, null);
        try {
          Assert.Throws<ArgumentException>(
            delegate() { manager.EndPrune(null); }
          );
        }
        finally {
          manager.EndPrune(asyncResult);
        }
      }
    }

    /// <summary>
    ///   Verifies that the user-defined callback is invoked when an asynchronous
    ///   pruning process completes
    /// </summary>
    [Test]
    public void TestAsynchronousPruneCallback() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        CallbackReceiver receiver = new CallbackReceiver();
        object state = new object();

        IAsyncResult asyncResult = manager.BeginPrune(receiver.Callback, state);
        manager.EndPrune(asyncResult);

        Assert.AreSame(state, receiver.State);
        Assert.IsFalse(asyncResult.CompletedSynchronously);
      }
    }

    /// <summary>
    ///   Tests whether exceptions during asynchronous updating are handles
    /// </summary>
    [Test]
    public void TestThrowDuringAsynchronousPrune() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(4096);
        for(int index = 0; index < test.Capacity; ++index) {
          test.AddParticle(new SimpleParticle());
        }

        manager.AddParticleSystem(
          test,
          delegate(ref SimpleParticle particle) { throw new ArithmeticException(); },
          new DummyRenderer<SimpleParticle>()
        );

        IAsyncResult asyncResult = manager.BeginPrune(null, null);
        Assert.Throws<ArithmeticException>(
          delegate() { manager.EndPrune(asyncResult); }
        );
      }
    }

    /// <summary>
    ///   Tests whether the particle manager can draw its particle systems
    /// </summary>
    [Test]
    public void TestDraw() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        // Add one particle system where there's stuff to do
        ParticleSystem<SimpleParticle> test = new ParticleSystem<SimpleParticle>(4096);
        for(int index = 0; index < test.Capacity; ++index) {
          test.AddParticle(new SimpleParticle());
        }
        manager.AddParticleSystem(test, dontPrune, new DummyRenderer<SimpleParticle>());

        manager.Draw(new GameTime());
      }
    }

    /// <summary>
    ///   Tests whether the particle manager can handle a large number of vertex types
    /// </summary>
    [Test]
    public void TestManyVertexTypes() {
      using(
        ParticleSystemManager manager = new ParticleSystemManager(
          this.mockedGraphicsDeviceService
        )
      ) {
        manager.AddParticleSystem(
          new ParticleSystem<SimpleParticle>(100),
          dontPrune, new DummyRenderer<SimpleParticle>()
        );
        manager.AddParticleSystem(
          new ParticleSystem<Sky.SkyboxVertex>(100),
          dontPrune, new DummyRenderer<Sky.SkyboxVertex>()
        );
        manager.AddParticleSystem(
          new ParticleSystem<Water.WaterVertex>(100),
          dontPrune, new DummyRenderer<Water.WaterVertex>()
        );
        manager.AddParticleSystem(
          new ParticleSystem<Masks.PositionVertex>(100),
          dontPrune, new DummyRenderer<Masks.PositionVertex>()
        );
        manager.AddParticleSystem(
          new ParticleSystem<Trails.TrailVertex>(100),
          dontPrune, new DummyRenderer<Trails.TrailVertex>()
        );

        manager.Draw(new GameTime());
      }
    }

    /// <summary>Prune method that always returns false</summary>
    /// <typeparam name="ParticleType">Type of particles to process</typeparam>
    /// <param name="particle">Not used</param>
    /// <returns>False. Always.</returns>
    private static bool dontPrune<ParticleType>(ref ParticleType particle) { return false; }

    /// <summary>Prune method that is very slow</summary>
    /// <typeparam name="ParticleType">Type of particles to process</typeparam>
    /// <param name="particle">Not used</param>
    /// <returns>False. Always.</returns>
    private static bool slowPrune<ParticleType>(ref ParticleType particle) {
      Thread.Sleep(0);
      return false;
    }

    /// <summary>Mocked graphics device service used to run the unit tests</summary>
    private MockedGraphicsDeviceService mockedGraphicsDeviceService;
    /// <summary>
    ///   Content manager used to load the effect by which the particles are drawn
    /// </summary>
    private ResourceContentManager contentManager;
    /// <summary>Effect used to draw the particles</summary>
    private Effect effect;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles.HighLevel

#endif // UNITTEST
