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
using System.IO;
using System.Windows.Forms;

using NUnit.Framework;

using Nuclex.Support;

namespace Nuclex.Windows.Forms {

  /// <summary>Unit Test for the control container list view</summary>
  [TestFixture]
  public class ContainerListViewTest {

    /// <summary>
    ///   Verifies that the asynchronous progress bar's constructor is working
    /// </summary>
    [Test]
    public void TestConstructor() {
      using(ContainerListView listView = new ContainerListView()) {

        // Let the control create its window handle
        listView.CreateControl();
        listView.Columns.Add("Numeric");
        listView.Columns.Add("Spelled");
        listView.Columns.Add("Nonsense");

        addRow(listView, "1", "One");
        addRow(listView, "2", "Two");
        addRow(listView, "3", "Three");

        using(CheckBox checkBox = new CheckBox()) {
          listView.EmbeddedControls.Add(new ListViewEmbeddedControl(checkBox, 2, 0));
          listView.EmbeddedControls.Clear();

          listView.Refresh();

          ListViewEmbeddedControl embeddedControl = new ListViewEmbeddedControl(
            checkBox, 2, 0
          );
          listView.EmbeddedControls.Add(embeddedControl);
          listView.EmbeddedControls.Remove(embeddedControl);

          listView.Refresh();
        }

      }
    }

    /// <summary>Adds a row to a control container list view</summary>
    /// <param name="listView">List view control the row will be added to</param>
    /// <param name="columns">Values that will appear in the individual columns</param>
    private void addRow(ContainerListView listView, params string[] columns) {
      listView.Items.Add(new ListViewItem(columns));
    }

  }

} // namespace Nuclex.Windows.Forms

#endif // UNITTEST
