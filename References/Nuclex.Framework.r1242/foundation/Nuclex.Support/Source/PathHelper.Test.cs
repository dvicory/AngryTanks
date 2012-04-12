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

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support {

  /// <summary>Unit Test for the path helper class</summary>
  [TestFixture]
  public class PathHelperTest {

    /// <summary>
    ///   Tests whether the relative path creator keeps the absolute path if
    ///   the location being passed is not relative to the base path.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathOfNonRelativePath() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2"),
          platformify("D:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("D:/Folder1/Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("D:/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("D:/Folder1/Folder2/"))
      );

    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to the parent folder of the base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToParentFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2"),
          platformify("C:/Folder1")
        ),
        Is.EqualTo(platformify(".."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("C:/Folder1/")
        ),
        Is.EqualTo(platformify("../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative path to
    ///   the parent folder of the base path for windows paths with more than one level.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToParentFolderTwoLevels() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/Folder3"),
          platformify("C:/Folder1")
        ),
        Is.EqualTo(platformify("../.."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/Folder3/"),
          platformify("C:/Folder1/")
        ),
        Is.EqualTo(platformify("../../"))
      );
    }


    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to the parent folder of the base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToParentFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2"),
          platformify("/Folder1")
        ),
        Is.EqualTo(platformify(".."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/"),
          platformify("/Folder1/")
        ),
        Is.EqualTo(platformify("../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative path to
    ///   the parent folder of the base path for unix paths with more than one level.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToParentFolderTwoLevels() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/Folder3"),
          platformify("/Folder1")
        ),
        Is.EqualTo(platformify("../.."))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/Folder3/"),
          platformify("/Folder1/")
        ),
        Is.EqualTo(platformify("../../"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to a nested folder in the base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToNestedFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1"),
          platformify("C:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/"),
          platformify("C:/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("Folder2/"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to a nested folder in the base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToNestedFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1"),
          platformify("/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("Folder2"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/"),
          platformify("/Folder1/Folder2/")
        ),
        Is.EqualTo(platformify("Folder2/"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to another folder on the same level as base path for windows paths.
    /// </summary>
    [Test]
    public void TestRelativeWindowsPathToSiblingFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2/"),
          platformify("C:/Folder1/Folder2345")
        ),
        Is.EqualTo(platformify("../Folder2345"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("C:/Folder1/Folder2345/"),
          platformify("C:/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("../Folder2"))
      );
    }

    /// <summary>
    ///   Tests whether the relative path creator correctly builds the relative
    ///   path to another folder on the same level as base path for unix paths.
    /// </summary>
    [Test]
    public void TestRelativeUnixPathToSiblingFolder() {
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2/"),
          platformify("/Folder1/Folder2345")
        ),
        Is.EqualTo(platformify("../Folder2345"))
      );
      Assert.That(
        PathHelper.MakeRelative(
          platformify("/Folder1/Folder2345/"),
          platformify("/Folder1/Folder2")
        ),
        Is.EqualTo(platformify("../Folder2"))
      );
    }

    /// <summary>
    ///   Converts unix-style directory separators into the format used by the current platform
    /// </summary>
    /// <param name="path">Path to converts into the platform-dependent format</param>
    /// <returns>Platform-specific version of the provided unix-style path</returns>
    private string platformify(string path) {
      return path.Replace('/', Path.DirectorySeparatorChar);
    }

  }

} // namespace Nuclex.Support

#endif // UNITTEST
