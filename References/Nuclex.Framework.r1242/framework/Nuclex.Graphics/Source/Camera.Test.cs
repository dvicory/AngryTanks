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
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

namespace Nuclex.Graphics {

  /// <summary>Unit tests for the camera class</summary>
  [TestFixture]
  internal class CameraTest {

    /// <summary>Verifies that the constructor of the camera class is working</summary>
    [Test]
    public void TestConstructor() {
      new Camera(Matrix.Identity, Matrix.Identity);
    }

    /// <summary>
    ///   Tests the LookAt() method of the camera class from the world's center
    /// </summary>
    [Test]
    public void TestLookAtFromWorldCenter() {
      Camera testCamera = new Camera(Matrix.Identity, Matrix.Identity);

      testCamera.LookAt(Vector3.UnitX); // Look to the right

      Vector3 leftPoint = -Vector3.UnitX;

      Vector3 transformed = Vector3.Transform(leftPoint, testCamera.View);

      // The point should be behind the camera
      Assert.Greater(transformed.Z, 0.0f);
    }

    /// <summary>
    ///   Tests the LookAt() method of the camera class from an arbitrary position
    /// </summary>
    [Test]
    public void TestLookAtFromArbitraryPosition() {
      Camera testCamera = new Camera(Matrix.Identity, Matrix.Identity);

      testCamera.MoveTo(new Vector3(100.0f, 200.0f, 300.0f));
      testCamera.LookAt(Vector3.Zero); // Look to the right

      Vector3 leftPoint = new Vector3(99.0f, 200.0f, 300.0f);
      Vector3 behindPoint = new Vector3(100.0f, 200.0f, 299.0f);

      Vector3 transformedLeft = Vector3.Transform(leftPoint, testCamera.View);
      Vector3 transformedBehind = Vector3.Transform(behindPoint, testCamera.View);

      // The left point should be in front of the camera
      Assert.Less(transformedLeft.Z, 0.0f);
      // The behind point should be right of the camera
      Assert.Greater(transformedBehind.X, 0.0f);
    }

#if false // GeoAssertHelper
    /// <summary>
    ///   Tests whether the position of a camera can be retrieved again
    /// </summary>
    [Test]
    public void TestPositionRetrieval() {
      Camera testCamera = new Camera(
        Matrix.CreateLookAt(
          new Vector3(1.2f, 3.4f, 5.6f),
          Vector3.One,
          Vector3.Up
        ),
        Matrix.Identity
      );

      GeoAssertHelper.AreAlmostEqual(
        new Vector3(1.2f, 3.4f, 5.6f), testCamera.Position, 10
      );
    }

    /// <summary>
    ///   Tests whether the position of a camera can be retrieved again
    /// </summary>
    [Test]
    public void TestPositionChanging() {
      Camera testCamera = new Camera(Matrix.Identity, Matrix.Identity);

      testCamera.Position = new Vector3(6.5f, 4.3f, 2.1f);

      GeoAssertHelper.AreAlmostEqual(
        new Vector3(6.5f, 4.3f, 2.1f), testCamera.Position, 10
      );
    }
#endif

    /// <summary>
    ///   Verifies that the default orthographic camera is constructed as expected
    /// </summary>
    [Test]
    public void TestDefaultOrthographic() {
      Camera defaultCamera = Camera.DefaultOrthographic;

      Assert.AreEqual(Vector3.Zero, defaultCamera.Position);

      // The point should end up in front of the camera
      Vector3 leftPoint = -Vector3.UnitZ;
      Vector3 transformed = Vector3.Transform(leftPoint, defaultCamera.View);
      Assert.Less(transformed.Z, 0.0f);

    }

    /// <summary>
    ///   Verifies that the HandleControls() method of the camera is working
    /// </summary>
    [Test]
    public void TestHandleControls() {
      Camera camera = Camera.DefaultOrthographic;
      camera.HandleControls(OneSecondGameTime);

      // We can't make assumptions about which keys the user held down while
      // the unit test ran, so we just verify it's not blowing up.
    }

    /// <summary>
    ///   Tests whether the camera can be moved forward using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardForwardMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.W);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.Greater(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be moved backward using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardBackwardMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.S);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.Less(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be moved to the left using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardLeftMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.A);
      Assert.Greater(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be moved to the right using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardRightMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.D);
      Assert.Less(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be moved up using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardUpMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.R);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.Less(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be moved up using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardDownMovement() {
      Camera camera = handleControlsOnDefaultCamera(Keys.F);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.X, camera.Position.X);
      Assert.Greater(Camera.DefaultOrthographic.Position.Y, camera.Position.Y);
      Assert.AreEqual(Camera.DefaultOrthographic.Position.Z, camera.Position.Z);
    }

    /// <summary>
    ///   Tests whether the camera can be rotated upwards using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardUpRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad2);
      Assert.That(camera.Forward.X, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Y, Is.GreaterThan(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated downwards using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardDownRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad8);
      Assert.That(camera.Forward.X, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Y, Is.LessThan(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated to the left using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardLeftRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad4);
      Assert.That(camera.Forward.X, Is.LessThan(0.0f));
      Assert.That(camera.Forward.Y, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated to the right using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardRightRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad6);
      Assert.That(camera.Forward.X, Is.GreaterThan(0.0f));
      Assert.That(camera.Forward.Y, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated clockwise using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardClockwiseRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad9);
      Assert.That(camera.Up.X, Is.GreaterThan(0.0f));
      Assert.That(camera.Up.Y, Is.LessThan(1.0f));
      Assert.That(camera.Up.Z, Is.EqualTo(0.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated counter-clockwise using the keyboard
    /// </summary>
    [Test]
    public void TestKeyboardCounterClockwiseRotation() {
      Camera camera = handleControlsOnDefaultCamera(Keys.NumPad7);
      Assert.That(camera.Right.X, Is.LessThan(1.0f));
      Assert.That(camera.Right.Y, Is.GreaterThan(0.0f));
      Assert.That(camera.Right.Z, Is.EqualTo(0.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated upwards using the game pad
    /// </summary>
    [Test]
    public void TestGamePadUpRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadDPad(
          ButtonState.Released, // up
          ButtonState.Pressed, // down
          ButtonState.Released, // left
          ButtonState.Released // right
        )
      );
      Assert.That(camera.Forward.X, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Y, Is.GreaterThan(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated downwards using the game pad
    /// </summary>
    [Test]
    public void TestGamePadDownRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadDPad(
          ButtonState.Pressed, // up
          ButtonState.Released, // down
          ButtonState.Released, // left
          ButtonState.Released // right
        )
      );
      Assert.That(camera.Forward.X, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Y, Is.LessThan(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated to the left using the game pad
    /// </summary>
    [Test]
    public void TestGamePadLeftRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadDPad(
          ButtonState.Released, // up
          ButtonState.Released, // down
          ButtonState.Pressed, // left
          ButtonState.Released // right
        )
      );
      Assert.That(camera.Forward.X, Is.LessThan(0.0f));
      Assert.That(camera.Forward.Y, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated to the left using the game pad
    /// </summary>
    [Test]
    public void TestGamePadRightRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadDPad(
          ButtonState.Released, // up
          ButtonState.Released, // down
          ButtonState.Released, // left
          ButtonState.Pressed // right
        )
      );
      Assert.That(camera.Forward.X, Is.GreaterThan(0.0f));
      Assert.That(camera.Forward.Y, Is.EqualTo(0.0f));
      Assert.That(camera.Forward.Z, Is.GreaterThan(-1.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated clockwise using the game pad
    /// </summary>
    [Test]
    public void TestGamePadClockwiseRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadButtons(Buttons.RightShoulder)
      );
      Assert.That(camera.Up.X, Is.GreaterThan(0.0f));
      Assert.That(camera.Up.Y, Is.LessThan(1.0f));
      Assert.That(camera.Up.Z, Is.EqualTo(0.0f));
    }

    /// <summary>
    ///   Tests whether the camera can be rotated counter-clockwise using the game pad
    /// </summary>
    [Test]
    public void TestGamePadCounterClockwiseRotation() {
      Camera camera = handleControlsOnDefaultCamera(
        new GamePadButtons(Buttons.LeftShoulder)
      );
      Assert.That(camera.Right.X, Is.LessThan(1.0f));
      Assert.That(camera.Right.Y, Is.GreaterThan(0.0f));
      Assert.That(camera.Right.Z, Is.EqualTo(0.0f));
    }

    /// <summary>
    ///   Creates a default orthographic camera and lets it respond as if
    ///   the specified keys were pressed
    /// </summary>
    /// <param name="keys">Keys that will be reported to the camera as pressed</param>
    /// <returns>The camera after it has responded to the provided controls</returns>
    private Camera handleControlsOnDefaultCamera(params Keys[] keys) {
      return handleControlsOnDefaultCamera(
        new KeyboardState(keys), new GamePadState()
      );
    }

    /// <summary>
    ///   Creates a default orthographic camera and lets it respond as if
    ///   the game pad's directional pad was in the specified state
    /// </summary>
    /// <param name="directionalPad">State of the game pad's directional pad</param>
    /// <returns>The camera after it has responded to the provided controls</returns>
    private Camera handleControlsOnDefaultCamera(GamePadDPad directionalPad) {
      return handleControlsOnDefaultCamera(
        new KeyboardState(),
        new GamePadState(
          new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(),
          directionalPad
        )
      );
    }

    /// <summary>
    ///   Creates a default orthographic camera and lets it respond as if
    ///   the game pad's buttons were in the specified state
    /// </summary>
    /// <param name="buttons">State of the game pad's buttons</param>
    /// <returns>The camera after it has responded to the provided controls</returns>
    private Camera handleControlsOnDefaultCamera(GamePadButtons buttons) {
      return handleControlsOnDefaultCamera(
        new KeyboardState(),
        new GamePadState(
          new GamePadThumbSticks(), new GamePadTriggers(), buttons, new GamePadDPad()
        )
      );
    }

    /// <summary>
    ///   Creates a default orthographic camera and lets it respond to the provided
    ///   controls once with a 1 second update.
    /// </summary>
    /// <param name="keyboardState">
    ///   Keyboard state that will be reported to the camera
    /// </param>
    /// <param name="gamePadState">
    ///   GamePad state that will be reported to the camera
    /// </param>
    /// <returns>The camera after it has responded to the provided controls</returns>
    private Camera handleControlsOnDefaultCamera(
      KeyboardState keyboardState, GamePadState gamePadState
    ) {
      Camera camera = Camera.DefaultOrthographic;
      camera.HandleControls(OneSecondGameTime, keyboardState, gamePadState);
      return camera;
    }

    /// <summary>A GameTime instance in which one second has passed</summary>
    private static GameTime OneSecondGameTime {
      get {
        return new GameTime(
#if !XNA_4
          TimeSpan.FromSeconds(1000), TimeSpan.FromSeconds(1),
#endif
          TimeSpan.FromSeconds(500), TimeSpan.FromSeconds(1)
        );
      }
    }

  }

} // namespace Nuclex.Graphics

#endif // UNITTEST
