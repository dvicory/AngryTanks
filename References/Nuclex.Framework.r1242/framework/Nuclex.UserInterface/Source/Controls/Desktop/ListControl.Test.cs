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

using Nuclex.Support;

using NUnit.Framework;
using NMock2;

using Nuclex.Input;
using Nuclex.UserInterface.Input;

namespace Nuclex.UserInterface.Controls.Desktop {

  /// <summary>Unit Test for the list control</summary>
  [TestFixture]
  internal class ListControlTest {

    #region class DummyListRowLocator

    /// <summary>Dummy implementation of a list row locator</summary>
    private class DummyListRowLocator : IListRowLocator {

      /// <summary>Calculates the list row the cursor is in</summary>
      /// <param name="bounds">
      ///   Boundaries of the control, should be in absolute coordinates
      /// </param>
      /// <param name="thumbPosition">
      ///   Position of the thumb in the list's slider
      /// </param>
      /// <param name="itemCount">
      ///   Number of items contained in the list
      /// </param>
      /// <param name="y">Vertical position of the cursor</param>
      /// <returns>The row the cursor is over</returns>
      public int GetRow(RectangleF bounds, float thumbPosition, int itemCount, float y) {
        return (int)(y / 10.0f);
      }

      /// <summary>Determines the height of a row displayed in the list</summary>
      /// <param name="bounds">
      ///   Boundaries of the control, should be in absolute coordinates
      /// </param>
      /// <returns>The height of a single row in the list</returns>
      public float GetRowHeight(RectangleF bounds) {
        return 10.0f;
      }

    }

    #endregion // class DummyListRowLocator

    #region interface IListSubscriber

    /// <summary>Interface for a subscriber to the list control's events</summary>
    public interface IListSubscriber {

      /// <summary>Called when selection made in the list has changed</summary>
      /// <param name="sender">List whose selection has changed</param>
      /// <param name="arguments">Not used</param>
      void SelectionChanged(object sender, EventArgs arguments);

    }

    #endregion // interface IListSubscriber

    /// <summary>Initializes a new list control</summary>
    [Test]
    public void TestConstructor() {
      ListControl list = new ListControl();
      Assert.IsNotNull(list); // nonsense; avoids compiler warning
    }

    /// <summary>Verifies that the RowLocator property is working</summary>
    [Test]
    public void TestRowLocatorProperty() {
      ListControl list = new ListControl();
      DummyListRowLocator rowLocator = new DummyListRowLocator();
      list.ListRowLocator = rowLocator;
      Assert.AreSame(rowLocator, list.ListRowLocator);
    }

    /// <summary>
    ///   Tests whether list rows can be selected with the mouse in
    ///   single-selection mode
    /// </summary>
    [Test]
    public void TestDisabledSelectionByMouse() {
      Screen screen = new Screen();
      ListControl list = new ListControl();
      list.Bounds = new UniRectangle(10, 10, 100, 100);
      list.SelectionMode = ListSelectionMode.None;
      list.ListRowLocator = new DummyListRowLocator();
      screen.Desktop.Children.Add(list);

      // Put 20 items in the list
      fillList(list, 20);

      Assert.AreEqual(0, list.SelectedItems.Count);

      list.ProcessMouseMove(100, 100, 50, 25);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(0, list.SelectedItems.Count);
    }

    /// <summary>
    ///   Tests whether list rows can be selected with the mouse in
    ///   single-selection mode
    /// </summary>
    [Test]
    public void TestSingleSelectionByMouse() {
      Screen screen = new Screen();
      ListControl list = new ListControl();
      list.Bounds = new UniRectangle(10, 10, 100, 100);
      list.SelectionMode = ListSelectionMode.Single;
      list.ListRowLocator = new DummyListRowLocator();
      screen.Desktop.Children.Add(list);

      // Put 20 items in the list
      fillList(list, 20);

      // At the beginning, no items should be selected
      Assert.AreEqual(0, list.SelectedItems.Count);

      // Click on item 2 and verify that it is now selected
      list.ProcessMouseMove(100, 100, 50, 35);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);
      Assert.AreEqual(1, list.SelectedItems.Count);
      Assert.AreEqual(2, list.SelectedItems[0]);

      // Now click on item 4, which should unselect item 2 and select item 4
      list.ProcessMouseMove(100, 100, 50, 55);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);
      Assert.AreEqual(1, list.SelectedItems.Count);
      Assert.AreEqual(4, list.SelectedItems[0]);

      // Repeat the click on item 4, nothing should happen
      list.ProcessMouseMove(100, 100, 50, 55);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);
      Assert.AreEqual(1, list.SelectedItems.Count);
      Assert.AreEqual(4, list.SelectedItems[0]);
    }

    /// <summary>
    ///   Tests whether list rows can be selected with the mouse in
    ///   single-selection mode
    /// </summary>
    [Test]
    public void TestMultiSelectionByMouse() {
      Screen screen = new Screen();
      ListControl list = new ListControl();
      list.Bounds = new UniRectangle(10, 10, 100, 100);
      list.SelectionMode = ListSelectionMode.Multi;
      list.ListRowLocator = new DummyListRowLocator();
      screen.Desktop.Children.Add(list);

      // Put 20 items in the list
      fillList(list, 20);

      Assert.AreEqual(0, list.SelectedItems.Count);

      list.ProcessMouseMove(100, 100, 50, 35);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(1, list.SelectedItems.Count);
      Assert.AreEqual(2, list.SelectedItems[0]);

      list.ProcessMouseMove(100, 100, 50, 55);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(2, list.SelectedItems.Count);
      Assert.AreEqual(2, list.SelectedItems[0]);
      Assert.AreEqual(4, list.SelectedItems[1]);

      list.ProcessMouseMove(100, 100, 50, 35);
      list.ProcessMousePress(MouseButtons.Left);
      list.ProcessMouseRelease(MouseButtons.Left);

      Assert.AreEqual(1, list.SelectedItems.Count);
      Assert.AreEqual(4, list.SelectedItems[0]);
    }

    /// <summary>Tests whether the selection mode of the list can be toggled</summary>
    [Test]
    public void TestSelectionModeProperty() {
      ListControl list = new ListControl();

      list.SelectionMode = ListSelectionMode.None;
      Assert.AreEqual(ListSelectionMode.None, list.SelectionMode);

      list.SelectionMode = ListSelectionMode.Single;
      Assert.AreEqual(ListSelectionMode.Single, list.SelectionMode);

      list.SelectionMode = ListSelectionMode.Multi;
      Assert.AreEqual(ListSelectionMode.Multi, list.SelectionMode);
    }

    /// <summary>Tests whether the list control provides access to its slider</summary>
    [Test]
    public void TestSliderProperty() {
      ListControl list = new ListControl();
      Assert.IsNotNull(list.Slider);
    }

    /// <summary>
    ///   Tests whether the SelectionChanged event is fired when the selection changes
    /// </summary>
    [Test]
    public void TestSelectionChangedEvent() {
      using(Mockery mockery = new Mockery()) {
        ListControl list = new ListControl();
        fillList(list, 20);

        IListSubscriber mockedSubscriber = mockSubscriber(mockery, list);

        Expect.Once.On(mockedSubscriber).Method("SelectionChanged").WithAnyArguments();
        list.SelectedItems.Add(1);

        Expect.Once.On(mockedSubscriber).Method("SelectionChanged").WithAnyArguments();
        list.SelectedItems.Remove(1);

        Expect.Once.On(mockedSubscriber).Method("SelectionChanged").WithAnyArguments();
        list.SelectedItems.Add(2);
        Expect.Once.On(mockedSubscriber).Method("SelectionChanged").WithAnyArguments();
        list.SelectedItems.Clear();

        mockery.VerifyAllExpectationsHaveBeenMet();
      }
    }

    /// <summary>Verifies that the list can be scrolled with the mouse wheel</summary>
    [Test]
    public void TestMouseWheel() {
      Screen screen = new Screen();
      ListControl list = new ListControl();
      list.Bounds = new UniRectangle(10, 10, 100, 100);
      list.ListRowLocator = new DummyListRowLocator();
      screen.Desktop.Children.Add(list);

      // Put 20 items in the list
      fillList(list, 20);

      Assert.AreEqual(0.0f, list.Slider.ThumbPosition);
      list.ProcessMouseWheel(-1.0f);

      RectangleF listBounds = list.GetAbsoluteBounds();
      float totalitems = list.Items.Count;
      float itemsInView = listBounds.Height;
      itemsInView /= list.ListRowLocator.GetRowHeight(listBounds);
      float scrollableItems = totalitems - itemsInView;
      float newThumbPosition = 1.0f / scrollableItems * 1.0f;
      
      Assert.AreEqual(newThumbPosition, list.Slider.ThumbPosition);
    }

    /// <summary>
    ///   Tests whether the slider's thumb resizes according to the list
    /// </summary>
    [Test]
    public void TestSliderThumbResizing() {
      Screen screen = new Screen();
      ListControl list = new ListControl();
      list.Bounds = new UniRectangle(10, 10, 100, 100);
      list.ListRowLocator = new DummyListRowLocator();
      screen.Desktop.Children.Add(list);

      RectangleF listBounds = list.GetAbsoluteBounds();
      float itemsInView = listBounds.Height;
      itemsInView /= list.ListRowLocator.GetRowHeight(listBounds);

      // Put 5 items in the list
      fillList(list, 5);
      Assert.AreEqual(1.0f, list.Slider.ThumbSize);

      // Put another 15 items in the list
      fillList(list, 15);
      Assert.AreEqual(itemsInView / 20.0f, list.Slider.ThumbSize);

      // Put another 15 items in the list
      list.Items.RemoveAt(19);
      list.Items.RemoveAt(18);
      list.Items.RemoveAt(17);
      list.Items.RemoveAt(16);
      list.Items.RemoveAt(15);
      Assert.AreEqual(itemsInView / 15.0f, list.Slider.ThumbSize);

      list.Items.Clear();
      Assert.AreEqual(1.0f, list.Slider.ThumbSize);
    }

    /// <summary>Mocks a subscriber for the events of a list</summary>
    /// <param name="mockery">Mockery through which the mock will be created</param>
    /// <param name="list">List to mock an event subscriber for</param>
    /// <returns>The mocked event subscriber</returns>
    private static IListSubscriber mockSubscriber(Mockery mockery, ListControl list) {
      IListSubscriber mockedSubscriber = mockery.NewMock<IListSubscriber>();
      list.SelectionChanged += new EventHandler(mockedSubscriber.SelectionChanged);
      return mockedSubscriber;
    }

    /// <summary>Fills a list control with dummy items</summary>
    /// <param name="list">List control that will be filled</param>
    /// <param name="itemCount">Number of dummy items to generate</param>
    private void fillList(ListControl list, int itemCount) {
      for(int index = 0; index < itemCount; ++index) {
        list.Items.Add("Item " + index.ToString());
      }
    }

  }

} // namespace Nuclex.UserInterface.Controls.Desktop

#endif // UNITTEST
