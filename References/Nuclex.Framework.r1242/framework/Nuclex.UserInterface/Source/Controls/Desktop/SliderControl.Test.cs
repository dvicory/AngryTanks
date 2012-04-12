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

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the slider control</summary>
  [TestFixture]
  internal class SliderControlTest {

    #region interface ISliderSubscriber

    /// <summary>Interface for a subscriber to the slider's events</summary>
    public interface ISliderSubscriber {

      /// <summary>Called when the slider's thumb has been moved</summary>
      /// <param name="sender">Slider whose thumb has been moved</param>
      /// <param name="arguments">Not used</param>
      void Moved(object sender, EventArgs arguments);

    }

    #endregion // interface ISliderSubscriber

    #region class DummySliderControl

    /// <summary>Dummy implementation of a slider for unit testing</summary>
    private class DummySliderControl : SliderControl {

      /// <summary>Moves the thumb to the specified location</summary>
      /// <returns>Location the thumb will be moved to</returns>
      protected override void MoveThumb(float x, float y) {
        this.ThumbX = x;
        this.ThumbY = y;

        this.ReportedThumbRegion.X = x;
        this.ReportedThumbRegion.Y = y;
      }

      /// <summary>Obtains the region covered by the slider's thumb</summary>
      /// <returns>The region covered by the slider's thumb</returns>
      protected override RectangleF GetThumbRegion() {
        return this.ReportedThumbRegion;
      }

      /// <summary>Manually fires the slider's Moved event</summary>
      public void FireMoveEvent() {
        OnMoved();
      }

      /// <summary>Movement of the thumb on the X axis</summary>
      public float ThumbX;
      /// <summary>Movement of the thumb on the Y axis</summary>
      public float ThumbY;
      /// <summary>Region the thumb is reported to be in</summary>
      public RectangleF ReportedThumbRegion;

    }

    #endregion // class DummySliderControl

    /// <summary>Verifies that the slider's constructor is working</summary>
    [Test]
    public void TestConstructor() {
      DummySliderControl slider = new DummySliderControl();
      Assert.IsNotNull(slider); // nonsense; avoids compiler warning
    }

    /// <summary>
    ///   Verifies that the slider can detect when the mouse cursor is over its thumb
    /// </summary>
    [Test]
    public void TestMouseOverThumb() {
      DummySliderControl slider = new DummySliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ReportedThumbRegion = new RectangleF(10, 20, 100, 20);

      // Unknown mouse position should not be over thumb
      Assert.IsFalse(slider.MouseOverThumb);

      // Move the mouse over the thumb. The property should now return true.
      slider.ProcessMouseMove(100, 100, 50, 30);
      Assert.IsTrue(slider.MouseOverThumb);

      // Move the mouse away from the thumb, but stay over the control.
      // The property should now return false again.      
      slider.ProcessMouseMove(100, 100, 50, 50);
      Assert.IsFalse(slider.MouseOverThumb);

      // Move the mouse over the thumb and then away fro mthe control.
      // The property should be false again after both movements.
      slider.ProcessMouseMove(100, 100, 50, 30);
      slider.ProcessMouseMove(100, 100, 5, 5);
      Assert.IsFalse(slider.MouseOverThumb);
    }

    /// <summary>
    ///   Verifies that the mouse can press down the slider's thumb
    /// </summary>
    [Test]
    public void TestThumbPressing() {
      DummySliderControl slider = new DummySliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ReportedThumbRegion = new RectangleF(10, 20, 100, 20);

      // Move the mouse over the thumb
      slider.ProcessMouseMove(100, 100, 50, 30);

      // Press the left mouse button. The thumb should now be depressed.
      slider.ProcessMousePress(MouseButtons.Left);
      Assert.IsTrue(slider.ThumbDepressed);

      // Release the left mouse button. The thumb should have risen again.
      slider.ProcessMouseRelease(MouseButtons.Left);
      Assert.IsFalse(slider.ThumbDepressed);
    }

    /// <summary>
    ///   Verifies that the mouse can be used to drag the slider's thumb
    /// </summary>
    [Test]
    public void TestThumbDragging() {
      DummySliderControl slider = new DummySliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ReportedThumbRegion = new RectangleF(10, 20, 100, 20);

      // Move the mouse over the thumb, press the left mouse button and drag
      // it to a new location
      slider.ProcessMouseMove(100, 100, 50, 30);
      slider.ProcessMousePress(MouseButtons.Left);
      slider.ProcessMouseMove(100, 100, 60, 50);

      // The thumb should now be moved to the new location (these are
      // absolute coordinates: the slider was at 10, 20 and we moved
      // the mouse by 10, 20, so now it's at 20, 40)
      Assert.AreEqual(20.0f, slider.ThumbX);
      Assert.AreEqual(40.0f, slider.ThumbY);
    }

    /// <summary>Tests whether the slider ignores right-clicks</summary>
    [Test]
    public void TestRightClickProducesNoAction() {
      DummySliderControl slider = new DummySliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ReportedThumbRegion = new RectangleF(10, 20, 100, 20);

      // Move the mouse over the thumb and do a right-click
      slider.ProcessMouseMove(100, 100, 50, 30);
      slider.ProcessMousePress(MouseButtons.Right);

      Assert.IsFalse(slider.ThumbDepressed);
    }

    /// <summary>Verifies that the slider can fire its 'Moved' event</summary>
    [Test]
    public void TestMoveEvent() {
      using(Mockery mockery = new Mockery()) {
        DummySliderControl slider = new DummySliderControl();
        ISliderSubscriber subscriber = mockSubscriber(mockery, slider);

        Expect.Once.On(subscriber).Method("Moved").WithAnyArguments();
        slider.FireMoveEvent();

        mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>Mocks a subscriber for the events of a slider</summary>
    /// <param name="mockery">Mockery through which the mock will be created</param>
    /// <param name="slider">Slider to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private static ISliderSubscriber mockSubscriber(
      Mockery mockery, SliderControl slider
    ) {
      ISliderSubscriber mockedSubscriber = mockery.NewMock<ISliderSubscriber>();
      slider.Moved += new EventHandler(mockedSubscriber.Moved);
      return mockedSubscriber;
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
