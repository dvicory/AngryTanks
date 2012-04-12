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

using Nuclex.Input;
using Nuclex.Support;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the horizontal slider</summary>
  [TestFixture]
  internal class HorizontalSliderControlTest {

    #region class DummyThumbLocator

    /// <summary>Dummy thumb locator returning a fixed location</summary>
    private class DummyThumbLocator : IThumbLocator {

      /// <summary>Initializes a new dummy thumb locator</summary>
      /// <param name="thumbArea">
      ///   Area the locator will report the thumb to be in
      /// </param>
      public DummyThumbLocator(RectangleF thumbArea) {
        this.ThumbArea = thumbArea;
      }

      /// <summary>
      ///   Calculates the position of the thumb on a slider
      /// </summary>
      /// <param name="bounds">
      ///   Boundaries of the control, should be in absolute coordinates
      /// </param>
      /// <param name="thumbPosition">Relative position of the thumb (0.0 .. 1.0)</param>
      /// <param name="thumbSize">Relative size of the thumb (0.0 .. 1.0)</param>
      /// <returns>The region covered by the slider's thumb</returns>
      public RectangleF GetThumbPosition(
        RectangleF bounds, float thumbPosition, float thumbSize
      ) {
        return this.ThumbArea;
      }

      /// <summary>Area the thumb is reported to be in</summary>
      public RectangleF ThumbArea;

    }

    #endregion // class DummyThumbLocator

    /// <summary>
    ///   Verifies that the slider can detect when the mouse cursor is over its thumb
    /// </summary>
    [Test]
    public void TestMouseOverThumb() {
      Screen screen = new Screen(100, 100);
      HorizontalSliderControl slider = new HorizontalSliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ThumbSize = 0.33f;
      slider.ThumbPosition = 0.25f;
      screen.Desktop.Children.Add(slider);

      float movableRange = 1.0f - slider.ThumbSize;
      float left = movableRange * slider.ThumbPosition;
      float right = left + slider.ThumbSize;

      left *= slider.Bounds.Size.X.Offset;
      right *= slider.Bounds.Size.X.Offset;
      float thumbWidth = right - left;

      // Position above the thumb should be reported as outside of the thumb.
      slider.ProcessMouseMove(100, 100, left - thumbWidth / 2.0f, 50);
      Assert.IsFalse(slider.MouseOverThumb);

      // Move the mouse over the thumb. The property should now return true.
      slider.ProcessMouseMove(100, 100, left + thumbWidth / 2.0f, 50);
      Assert.IsTrue(slider.MouseOverThumb);

      // Move the mouse away from the thumb, but stay over the control.
      // The property should now return false again.      
      slider.ProcessMouseMove(100, 100, right + thumbWidth / 2.0f, 50);
      Assert.IsFalse(slider.MouseOverThumb);
    }

    /// <summary>
    ///   Verifies that the slider can detect when the mouse cursor is over its thumb
    ///   using a thumb locator
    /// </summary>
    [Test]
    public void TestMouseOverWithThumbLocator() {
      Screen screen = new Screen(100, 100);
      HorizontalSliderControl slider = new HorizontalSliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ThumbSize = 0.33f;
      slider.ThumbPosition = 0.0f;
      screen.Desktop.Children.Add(slider);

      // The thumb would normally be at the top of the control, but this
      // the thumb locator will tell the control that its thumb is at the bottom.
      slider.ThumbLocator = new DummyThumbLocator(
        new RectangleF(66.6f, 0.0f, 33.3f, 100.0f)
      );

      // Position above the thumb should be reported as outside of the thumb.
      slider.ProcessMouseMove(100, 100, 15, 50);
      Assert.IsFalse(slider.MouseOverThumb);

      // Move the mouse over the thumb. The property should now return true.
      slider.ProcessMouseMove(100, 100, 85, 50);
      Assert.IsTrue(slider.MouseOverThumb);

      // Move the mouse away from the thumb, but stay over the control.
      // The property should now return false again.      
      slider.ProcessMouseMove(100, 100, 50, 50);
      Assert.IsFalse(slider.MouseOverThumb);
    }

    /// <summary>
    ///   Verifies that the mouse can be used to drag the slider's thumb
    /// </summary>
    [Test]
    public void TestThumbDragging() {
      Screen screen = new Screen(100, 100);
      HorizontalSliderControl slider = new HorizontalSliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ThumbSize = 0.33f;
      slider.ThumbPosition = 0.25f;
      screen.Desktop.Children.Add(slider);

      float movableRange = 1.0f - slider.ThumbSize;
      float left = movableRange * slider.ThumbPosition;
      float right = left + slider.ThumbSize;

      left *= slider.Bounds.Size.X.Offset;
      right *= slider.Bounds.Size.X.Offset;
      float thumbWidth = right - left;

      float target = movableRange * (1.0f - slider.ThumbPosition);
      target *= slider.Bounds.Size.X.Offset;

      // Move the mouse over the thumb, press the left mouse button and drag
      // it to a new location
      slider.ProcessMouseMove(100, 100, left + thumbWidth / 2.0f, 50);
      slider.ProcessMousePress(MouseButtons.Left);
      slider.ProcessMouseMove(100, 100, target + thumbWidth / 2.0f, 50);
      Assert.That(slider.ThumbPosition, Is.EqualTo(0.75f).Within(16).Ulps);

      slider.ProcessMouseMove(100, 100, slider.Bounds.Location.X.Offset, 50);
      Assert.That(slider.ThumbPosition, Is.EqualTo(0.0f).Within(16).Ulps);

      slider.ProcessMouseMove(
        100, 100, slider.Bounds.Location.X.Offset + slider.Bounds.Size.X.Offset, 50
      );
      Assert.That(slider.ThumbPosition, Is.EqualTo(1.0f).Within(16).Ulps);
    }

    /// <summary>
    ///   Verifies that no errors occur when the user attempts to drag a thumb that
    ///   fills out the whole slider.
    /// </summary>
    [Test]
    public void TestFullSizeThumbDragging() {
      Screen screen = new Screen(100, 100);
      HorizontalSliderControl slider = new HorizontalSliderControl();
      slider.Bounds = new UniRectangle(10, 10, 100, 100);
      slider.ThumbSize = 1.0f;
      screen.Desktop.Children.Add(slider);

      // Move the mouse over the thumb, press the left mouse button and drag
      // it to a new location
      slider.ProcessMouseMove(100, 100, 50, 50);
      slider.ProcessMousePress(MouseButtons.Left);
      slider.ProcessMouseMove(100, 100, 60, 50);
      Assert.That(slider.ThumbPosition, Is.EqualTo(0.0f).Within(16).Ulps);
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
