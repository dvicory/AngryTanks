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

using System;

using Microsoft.Xna.Framework;

namespace Nuclex.Geometry.Volumes {

  /// <summary>Base class for volume visitors</summary>
  /// <remarks>
  ///   See the visitor pattern. 
  ///   <para>
  ///     If you need to perform work on a volume whose type you do not know, you could
  ///     of course just create a set of if(volume is ...) queries. However, you would
  ///     have to manually track down all these queries should a future version of this
  ///     library provide a new type of volume.
  ///   </para>
  ///   <para>
  ///     Your other option is to create a special class that does your work which derives
  ///     from this VolumeVisitor class. It will elegantly resolve the volume type by
  ///     calling into the volume (resolving the type via the vtable) which then calls
  ///     the visitor's distinctive method for the exact kind of volume. If a new volume
  ///     is introduced to the library, it will be added to the VolumeVisitor class and
  ///     your compiler will point out to you where you need to extend your code for the
  ///     new kind of volume because the abstract method will not yet be implemented there.
  ///   </para>
  /// </remarks>
  public abstract class VolumeVisitor {

    /// <summary>Visit a volume and do the action the visitor is intended for</summary>
    /// <param name="volume">Volume to visit</param>
    public void Visit(IVolume3 volume) {
      volume.Accept(this);
    }

    /// <summary>Visit an axis aligned box</summary>
    /// <param name="box">Box to visit</param>
    public abstract void Visit(AxisAlignedBox3 box);

    /// <summary>Visit an oriented box</summary>
    /// <param name="box">Box to visit</param>
    public abstract void Visit(Box3 box);

    /// <summary>Visit a sphere</summary>
    /// <param name="sphere">Sphere to visit</param>
    public abstract void Visit(Sphere3 sphere);

    /// <summary>Visit a cylinder</summary>
    /// <param name="cylinder">Cylinder to visit</param>
    public abstract void Visit(Cylinder3 cylinder);

    /// <summary>Visit a triangle mesh</summary>
    /// <param name="mesh">Mesh to visit</param>
    public abstract void Visit(TriangleMesh3 mesh);

  }

} // namespace Nuclex.Geometry.Volumes
