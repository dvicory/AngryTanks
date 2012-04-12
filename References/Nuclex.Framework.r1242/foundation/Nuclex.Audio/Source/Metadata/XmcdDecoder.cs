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
using System.Text;

using Nuclex.Support;

namespace Nuclex.Audio.Metadata {

  /// <summary>Decodes an XMCD database file into the matching structure</summary>
  internal class XmcdDecoder {

    /// <summary>Whitespace characters</summary>
    private static readonly char[] Whitespaces = { ' ', '\t' };

    /// <summary>Numeric characters</summary>
    private static readonly char[] Numbers = {
      '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    #region enum State

    /// <summary>States the XMCD decoder can be in</summary>
    private enum State {

      /// <summary>Expecting the XMCD file id</summary>
      ExpectingFileId,
      /// <summary>
      ///   Decoder is in the comments section and expects optional properties
      /// </summary>
      ExpectingCommentProperties,
      /// <summary>
      ///   Decoder has encountered the "Track frame offsets" comment and is
      ///   expecting track offsets or their termination by another property
      /// </summary>
      ExpectingCommentTrackOffsets,
      /// <summary>
      ///   Decoder has left the comments section and expects keywords
      /// </summary>
      ExpectingKeywords

    }

    #endregion

    /// <summary>Initializes a new XMCD database file decoder</summary>
    public XmcdDecoder() {
      this.state = State.ExpectingFileId;
      this.trackOffsets = new List<int>();
      this.trackTitles = new List<Cddb.DTitle>();
    }

    /// <summary>Processes a line from the XMCD database file</summary>
    /// <param name="line">Line from the database file to process</param>
    public void ProcessLine(string line) {
      switch(this.state) {
        case State.ExpectingFileId: {
          parseFileId(line);
          break;
        }
        case State.ExpectingCommentProperties: {
          parseCommentProperty(line);
          break;
        }
        case State.ExpectingCommentTrackOffsets: {
          parseCommentTrackOffset(line);
          break;
        }
        case State.ExpectingKeywords: {
          parseKeyword(line);
          break;
        }
      }
    }

    /// <summary>Returns the parsed data as a XMCD database entry structure</summary>
    /// <returns>The parsed XMCD database entry structure</returns>
    public Cddb.DatabaseEntry ToDatabaseEntry() {

      // The XMCD file specification says that all these values have to be provided
      // in a valid XMCD file.
      bool isValid =
        this.haveDiscId &&
        this.haveTitle &&
        this.haveYear &&
        this.haveGenre &&
        this.havePlayOrder &&
        this.haveExtendedData;

      if(!isValid) {
        throw new FormatException("XMCD file is missing required fields");
      }

      // Take over the track and track offset lists
      this.entry.Tracks = this.trackTitles.ToArray();
      this.entry.TrackFrameOffsets = this.trackOffsets.ToArray();

      // When we extracted the tracks, we had to leave the artist field empty for
      // any tracks without an explicit artist specification. Now that we have
      // the disc title, we can fill in this field for those tracks.
      for(int index = 0; index < this.entry.Tracks.Length; ++index) {
        if(this.entry.Tracks[index].Artist == null) {
          this.entry.Tracks[index].Artist = this.entry.Artist;
        }
      }

      // Okay, the entry is filled, time to hand it over to our user
      return this.entry;

    }

    /// <summary>Parses the file id of an XMCD database file</summary>
    /// <param name="line">Line containing the XMCD database file id</param>
    private void parseFileId(string line) {
      bool hasFileId = line.StartsWith("# xmcd");

      if(!hasFileId) {
        throw new FormatException("Not an XMCD database file");
      }

      this.state = State.ExpectingCommentProperties;
    }

    /// <summary>Parses a comment line possibly containing a property</summary>
    /// <param name="line">Line containing a comment and possible a property</param>
    private void parseCommentProperty(string line) {

      // If this line doesn't start with a comment sign, we must have reached the end
      // of the comment section and should switch to normal keyword processing.
      if(!line.StartsWith("#")) {
        this.state = State.ExpectingKeywords;
        parseKeyword(line);
        return;
      }

      // Locate any characters in this line
      int propertyStartIndex = -1;
      bool hasPropertyStart =
        (line.Length > 1) &&
        ((propertyStartIndex = StringHelper.IndexNotOfAny(line, Whitespaces, 1)) != -1);

      if(!hasPropertyStart) {
        return; // blank line or other comment
      }

      // If there were characters on the line, we assume them to contain a property
      // that is terminated by a ':'. Look for a ':' character.
      int propertyEndIndex = -1;
      bool hasPropertyEnd =
        (line.Length > propertyStartIndex + 1) &&
        ((propertyEndIndex = line.IndexOf(':', propertyStartIndex + 1)) != -1);

      if(!hasPropertyEnd) {
        return; // other comment
      }

      // We found a one or more characters in the comment line, terminated by
      // a ':' character, so this seems to be a property. Extract the individual
      // elements and process the property assignment
      string name = line.Substring(
        propertyStartIndex, propertyEndIndex - propertyStartIndex
      );
      int valueStart = StringHelper.IndexNotOfAny(
        line, Whitespaces, propertyEndIndex + 1
      );
      if(valueStart == -1) {
        interpretCommentProperty(name, line.Substring(propertyEndIndex + 1));
      } else {
        interpretCommentProperty(name, line.Substring(valueStart));
      }

    }

    /// <summary>
    ///   Analyzes a property that appeared in the comments section and adds its
    ///   information to the database entry
    /// </summary>
    /// <param name="name">Name of the encountered property</param>
    /// <param name="value">Value that has been assigned to the property</param>
    private void interpretCommentProperty(string name, string value) {
      if(name == "Track frame offsets") {
        this.state = State.ExpectingCommentTrackOffsets;
      } else if(name == "Disc length") {
        int endIndex = StringHelper.IndexNotOfAny(value, Numbers);
        if(endIndex == -1) {
          this.entry.DiscLengthSeconds = Convert.ToInt32(value);
        } else {
          this.entry.DiscLengthSeconds = Convert.ToInt32(value.Substring(0, endIndex));
        }
      } else if(name == "Revision") {
        this.entry.Revision = Convert.ToInt32(value);
      } else if(name == "Submitted via") {
        this.entry.Submitter = value;
      }
    }

    /// <summary>Parses a comment line possibly containing track offset</summary>
    /// <param name="line">Line containing a comment and possible a track offset</param>
    private void parseCommentTrackOffset(string line) {

      // If this line doesn't begin with a comment sign, we must have reached
      // the end of the comment section and should continue parsing for keywords.
      if(!line.StartsWith("#")) {
        this.state = State.ExpectingKeywords;
        parseKeyword(line);
        return;
      }

      // Locate the start of any characters within this comment. If the first
      // character encountered is not a number, we must have reached the end of
      // the track offset list and should continue normal comment processing.
      int offsetStartIndex = -1;
      bool hasTrackOffset =
        (line.Length > 1) &&
        ((offsetStartIndex = StringHelper.IndexNotOfAny(line, Whitespaces, 1)) != -1) &&
        char.IsNumber(line[offsetStartIndex]);

      if(!hasTrackOffset) {
        this.state = State.ExpectingCommentProperties;
        parseCommentProperty(line);
        return;
      }

      // Find out where the offset specification ends
      int offsetEndIndex = StringHelper.IndexNotOfAny(
        line, Numbers, offsetStartIndex + 1
      );

      // All positions have been determined, now extract the track offset and add
      // it to our list
      if(offsetEndIndex == -1) {
        this.trackOffsets.Add(Convert.ToInt32(line.Substring(offsetStartIndex)));
      } else {
        this.trackOffsets.Add(
          Convert.ToInt32(
            line.Substring(offsetStartIndex, offsetEndIndex - offsetStartIndex)
          )
        );
      }

    }

    /// <summary>Parses a comment line possibly containing a property</summary>
    /// <param name="line">Line containing a comment and possible a property</param>
    private void parseKeyword(string line) {

      // Look for the start of the keyword
      int keywordStartIndex = -1;
      bool hasKeyword =
        (line.Length > 0) &&
        ((keywordStartIndex = StringHelper.IndexNotOfAny(line, Whitespaces)) != -1);

      if(!hasKeyword) {
        return; // blank line
      }

      // Look for the equals sign
      int equalsIndex = -1;
      bool hasAssignment =
        (line.Length > keywordStartIndex + 1) &&
        ((equalsIndex = line.IndexOf('=', keywordStartIndex + 1)) != -1);

      if(!hasAssignment) {
        return; // strange line!
      }

      // Find out where the value starts
      int valueIndex = -1;
      bool hasValue =
        (line.Length > equalsIndex + 1) &&
        ((valueIndex = StringHelper.IndexNotOfAny(line, Whitespaces, equalsIndex + 1)) != -1);

      // All found, now extract the informations we found an process them
      string name = line.Substring(keywordStartIndex, equalsIndex - keywordStartIndex);
      if(hasValue) {
        interpretKeyword(name, line.Substring(valueIndex));
      } else {
        interpretKeyword(name, string.Empty);
      }

    }

    /// <summary>Interprets a keyword assignment appearing in an XMCD file</summary>
    /// <param name="name">Name of the keyword</param>
    /// <param name="value">Value assigned to the keyword</param>
    private void interpretKeyword(string name, string value) {
      if(name == "DISCID") {
        this.haveDiscId = true;
        this.entry.DiscIds = parseDiscIds(value);
      } else if(name == "DTITLE") {
        this.haveTitle = true;
        Cddb.DTitle dTitle = Cddb.SplitDiscTitle(value);
        this.entry.Artist = dTitle.Artist;
        this.entry.Album = dTitle.Title;
      } else if(name == "DYEAR") {
        this.haveYear = true;
        this.entry.Year = Convert.ToInt32(value);
      } else if(name == "DGENRE") {
        this.haveGenre = true;
        this.entry.Genre = value;
      } else if(name == "EXTD") {
        this.haveExtendedData = true;
      } else if(name == "PLAYORDER") {
        this.havePlayOrder = true;
      } else if(name.StartsWith("TTITLE")) {
        int trackIndex = Convert.ToInt32(name.Substring(6));

        // Make sure there are enough entries in the list (minus one because it allows
        // us to use the Add() method then, instead of adding an empty entry and replacing
        // it further down
        while(this.trackTitles.Count < trackIndex) {
          this.trackTitles.Add(new Cddb.DTitle());
        }

        // Get the track title. If a " / " sequence is contained in the title, the track
        // contains an artist specification. Otherwise, leave the artist field set to
        // null because we can't be sure that the disc title field has been filled already.
        Cddb.DTitle track;
        if(value.Contains(" / ")) {
          track = Cddb.SplitDiscTitle(value);
        } else {
          track = new Cddb.DTitle(null, value);
        }

        // Finally, add the track to the track list. If it is an out-of-order track, we
        // will replace the existing, empty entry. Otherwise, the list will be just one
        // element short of the track and we can normally add it.
        if(this.trackTitles.Count == trackIndex) {
          this.trackTitles.Add(track);
        } else {
          this.trackTitles[trackIndex] = track;
        }
      } else if(name.StartsWith("EXTT")) {
        // Ignore for now
      }
    }

    /// <summary>Parses the CDDB disc ids from a disc id list</summary>
    /// <param name="discIds">List of disc ids to parse</param>
    /// <returns>An array containing any parsed disc ids</returns>
    private int[] parseDiscIds(string discIds) {
      List<int> discIdList = new List<int>();

      int lastCommaIndex = -1;
      int commaIndex = discIds.IndexOf(',');

      while(commaIndex != -1) {
        string discId = discIds.Substring(
          lastCommaIndex + 1, commaIndex - lastCommaIndex
        );
        discIdList.Add(Convert.ToInt32(discId.Trim(Whitespaces), 16));

        lastCommaIndex = commaIndex;
        commaIndex = discIds.IndexOf(',', commaIndex + 1);
      }

      string finalDiscId = discIds.Substring(lastCommaIndex + 1);
      discIdList.Add(Convert.ToInt32(finalDiscId.Trim(Whitespaces), 16));

      return discIdList.ToArray();
    }

    /// <summary>Current state the decoder is in</summary>
    private State state;

    /// <summary>Database entry structure that is being filled by the decoder</summary>
    private Cddb.DatabaseEntry entry;

    /// <summary>Frame offsets that were specified for the tracks in the XMCD file</summary>
    private List<int> trackOffsets;
    /// <summary>Track titles for all tracks listed in the XMCD file</summary>
    private List<Cddb.DTitle> trackTitles;

    /// <summary>Whether the decoder has encountered a disc id field</summary>
    private bool haveDiscId;
    /// <summary>Whether the decoder has encountered a disc title field</summary>
    private bool haveTitle;
    /// <summary>Whether the decoder has encountered a production year field</summary>
    private bool haveYear;
    /// <summary>Whether the decoder has encountered a genre field</summary>
    private bool haveGenre;
    /// <summary>Whether the decoder has encountered a play order field</summary>
    private bool havePlayOrder;
    /// <summary>Whether the decoder has encountered an extended data field</summary>
    private bool haveExtendedData;

  }

} // namespace Nuclex.Audio.Metadata
