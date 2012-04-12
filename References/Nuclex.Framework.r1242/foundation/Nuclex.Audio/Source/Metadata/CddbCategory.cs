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
using System.Collections.Generic;

using Nuclex.Support.Tracking;

namespace Nuclex.Audio.Metadata {

  /// <summary>List of the 11 fixed categories supported by official CDDB databases</summary>
  /// <remarks>
  ///   <para>
  ///     Yes, this list is borked. It differentiates between Blues and Jazz, between Folk and
  ///     Country but coalesces rock, metal, pop, rap and more into a single genre. For
  ///     historical reasons and in order to not break existing (crappy) software, official
  ///     CDDB databases offer only these 11 fixed genres. 
  ///   </para>
  ///   <para>
  ///     Because the CDDB disc id calculation is equally flawed and easily leads to duplicate
  ///     ids, the official recommendation is to first try to submit CDDB informations of a new
  ///     CD with the actual genre and then, if it turns out there's already another CD with
  ///     the same disc id in the database, to use one of the other genres to submit it.
  ///   </para>
  ///   <para>
  ///     From version 5 onwards, the CDDB protocol supports any genre name as an additional
  ///     database field. Thus, these 11 genres might better be seen as "disc id slots" that
  ///     allow up to 11 CDs with the same disc id to be stored in CDDB databases.
  ///   </para>
  /// </remarks>
  public enum CddbCategory {

    /// <summary>Self explanatory</summary>
    Blues,
    /// <summary>Self explanatory</summary>
    Classical,
    /// <summary>Self explanatory</summary>
    Country,
    /// <summary>ISO9660 and other data CDs</summary>
    Data,
    /// <summary>Self explanatory</summary>
    Folk,
    /// <summary>Self explanatory</summary>
    Jazz,
    /// <summary>Self explanatory</summary>
    NewAge,
    /// <summary>Self explanatory</summary>
    Reggae,
    /// <summary>Rock - including funk, soul, rap, pop, industrial, metal, etc.</summary>
    Rock,
    /// <summary>Movies, shows</summary>
    Soundtrack,
    /// <summary>Others that do not fit in the above categories</summary>
    Misc

  }

} // namespace Nuclex.Audio.Metadata
