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
using System.Collections.Generic;
using System.Text;

#if UNITTEST

using NUnit.Framework;

namespace Nuclex.Support.Parsing {

  /// <summary>Ensures that the command line parser is working properly</summary>
  [TestFixture]
  public class CommandLineTest {

    #region class ArgumentTest

    /// <summary>Unit test for the command line option class</summary>
    [TestFixture]
    public class ArgumentTest {

      /// <summary>
      ///   Verifies that the name of a command line argument without a value can
      ///   be extracted
      /// </summary>
      [Test]
      public void TestNameExtraction() {
        CommandLine.Argument argument = CommandLine.Argument.OptionOnly(
          new StringSegment("--test"), 2, 4
        );

        Assert.AreEqual("--test", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.IsNull(argument.Associator);
        Assert.IsNull(argument.Value);
      }

      /// <summary>
      ///   Verifies that the name of a command line argument without a value can be
      ///   extracted when the argument is contained in a substring of a larger string
      /// </summary>
      [Test]
      public void TestNameExtractionFromSubstring() {
        CommandLine.Argument argument = CommandLine.Argument.OptionOnly(
          new StringSegment("||--test||", 2, 6), 4, 4
        );

        Assert.AreEqual("--test", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.IsNull(argument.Associator);
        Assert.IsNull(argument.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line argument can be extracted
      /// </summary>
      [Test]
      public void TestValueExtraction() {
        CommandLine.Argument argument = new CommandLine.Argument(
          new StringSegment("--test=123"), 2, 4, 7, 3
        );

        Assert.AreEqual("--test=123", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.AreEqual("=", argument.Associator);
        Assert.AreEqual("123", argument.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line argument can be extracted
      ///   when the argument is contained in a substring of a larger string
      /// </summary>
      [Test]
      public void TestValueExtractionFromSubstring() {
        CommandLine.Argument argument = new CommandLine.Argument(
          new StringSegment("||--test=123||", 2, 10), 4, 4, 9, 3
        );

        Assert.AreEqual("--test=123", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.AreEqual("=", argument.Associator);
        Assert.AreEqual("123", argument.Value);
      }

      /// <summary>
      ///   Varifies that the name and value of a command line argument can be extracted
      ///   when the option is assigned a quoted value
      /// </summary>
      [Test]
      public void TestQuotedValueExtraction() {
        CommandLine.Argument argument = new CommandLine.Argument(
          new StringSegment("--test=\"123\"", 0, 12), 2, 4, 8, 3
        );

        Assert.AreEqual("--test=\"123\"", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.AreEqual("=", argument.Associator);
        Assert.AreEqual("123", argument.Value);
      }

      /// <summary>
      ///   Varifies that the associator of a command line argument with an open ended
      ///   value assignment can be retrieved
      /// </summary>
      [Test]
      public void TestValuelessAssociatorRetrieval() {
        CommandLine.Argument argument = CommandLine.Argument.OptionOnly(
          new StringSegment("--test="), 2, 4
        );

        Assert.AreEqual("--test=", argument.Raw);
        Assert.AreEqual("--", argument.Initiator);
        Assert.AreEqual("test", argument.Name);
        Assert.AreEqual("=", argument.Associator);
        Assert.IsNull(argument.Value);
      }

      /// <summary>
      ///   Varifies that the associator of a command line option with an open ended value
      ///   assignment can be retrieved when the option is contained in a substring of
      ///   a larger string
      /// </summary>
      [Test]
      public void TestValuelessAssociatorRetrievalFromSubstring() {
        CommandLine.Argument option = CommandLine.Argument.OptionOnly(
          new StringSegment("||--test=||", 2, 7), 4, 4
        );

        Assert.AreEqual("--test=", option.Raw);
        Assert.AreEqual("--", option.Initiator);
        Assert.AreEqual("test", option.Name);
        Assert.AreEqual("=", option.Associator);
        Assert.IsNull(option.Value);
      }

      /// <summary>
      ///   Varifies that a command line argument without an option name can be retrieved
      /// </summary>
      [Test]
      public void TestNamelessValueRetrieval() {
        CommandLine.Argument argument = CommandLine.Argument.ValueOnly(
          new StringSegment("\"hello world\""), 1, 11
        );

        Assert.AreEqual("\"hello world\"", argument.Raw);
        Assert.IsNull(argument.Initiator);
        Assert.IsNull(argument.Name);
        Assert.IsNull(argument.Associator);
        Assert.AreEqual("hello world", argument.Value);
      }

      /// <summary>
      ///   Varifies that a command line argument without an option name can be retrieved
      ///   that is contained in a substring of larger string
      /// </summary>
      [Test]
      public void TestNamelessValueRetrievalFromSubstring() {
        CommandLine.Argument argument = CommandLine.Argument.ValueOnly(
          new StringSegment("||\"hello world\"||", 2, 13), 3, 11
        );

        Assert.AreEqual("\"hello world\"", argument.Raw);
        Assert.IsNull(argument.Initiator);
        Assert.IsNull(argument.Name);
        Assert.IsNull(argument.Associator);
        Assert.AreEqual("hello world", argument.Value);
      }

    }

    #endregion // class ArgumentTest

    /// <summary>Verifies that the default constructor is working</summary>
    [Test]
    public void TestDefaultConstructor() {
      CommandLine commandLine = new CommandLine();

      Assert.AreEqual(0, commandLine.Arguments.Count);
    }

    /// <summary>
    ///   Validates that the parser can handle an argument initiator with an
    ///   assignment that is missing a name
    /// </summary>
    [Test]
    public void TestParseAmbiguousNameResolution() {
      CommandLine commandLine = CommandLine.Parse("--:test");

      // Without a name, this is not a valid command line option, so it will
      // be parsed as a loose value instead.
      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("--:test", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("--:test", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Verifies that a lone short argument initiator without anything behind
    ///   can be parsed
    /// </summary>
    [Test]
    public void TestParseShortArgumentInitiatorOnly() {
      CommandLine commandLine = CommandLine.Parse("-");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("-", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("-", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Verifies that a lone long argument initiator without anything behind
    ///   can be parsed
    /// </summary>
    [Test]
    public void TestParseLongArgumentInitiatorOnly() {
      CommandLine commandLine = CommandLine.Parse("--");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("--", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("--", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseArgumentInitiatorAtEnd() {
      CommandLine commandLine = CommandLine.Parse("-hello:-world -");

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("-hello:-world", commandLine.Arguments[0].Raw);
      Assert.AreEqual("-", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("hello", commandLine.Arguments[0].Name);
      Assert.AreEqual(":", commandLine.Arguments[0].Associator);
      Assert.AreEqual("-world", commandLine.Arguments[0].Value);

      Assert.AreEqual("-", commandLine.Arguments[1].Raw);
      Assert.IsNull(commandLine.Arguments[1].Initiator);
      Assert.IsNull(commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.AreEqual("-", commandLine.Arguments[1].Value);
    }

    /// <summary>Validates that quoted arguments can be parsed</summary>
    [Test]
    public void TestParseQuotedValue() {
      CommandLine commandLine = CommandLine.Parse("hello -world --this -is=\"a test\"");

      Assert.AreEqual(4, commandLine.Arguments.Count);

      Assert.AreEqual("hello", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("hello", commandLine.Arguments[0].Value);

      Assert.AreEqual("-world", commandLine.Arguments[1].Raw);
      Assert.AreEqual("-", commandLine.Arguments[1].Initiator);
      Assert.AreEqual("world", commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.IsNull(commandLine.Arguments[1].Value);

      Assert.AreEqual("--this", commandLine.Arguments[2].Raw);
      Assert.AreEqual("--", commandLine.Arguments[2].Initiator);
      Assert.AreEqual("this", commandLine.Arguments[2].Name);
      Assert.IsNull(commandLine.Arguments[2].Associator);
      Assert.IsNull(commandLine.Arguments[2].Value);

      Assert.AreEqual("-is=\"a test\"", commandLine.Arguments[3].Raw);
      Assert.AreEqual("-", commandLine.Arguments[3].Initiator);
      Assert.AreEqual("is", commandLine.Arguments[3].Name);
      Assert.AreEqual("=", commandLine.Arguments[3].Associator);
      Assert.AreEqual("a test", commandLine.Arguments[3].Value);
    }

    /// <summary>Validates that null can be parsed</summary>
    [Test]
    public void TestParseNull() {
      CommandLine commandLine = CommandLine.Parse((string)null);

      Assert.AreEqual(0, commandLine.Arguments.Count);
    }

    /// <summary>Validates that a single argument without quotes can be parsed</summary>
    [Test]
    public void TestParseSingleNakedValue() {
      CommandLine commandLine = CommandLine.Parse("hello");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("hello", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("hello", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Validates that the parser can handle a quoted argument that's missing
    ///   the closing quote
    /// </summary>
    [Test]
    public void TestParseQuotedArgumentWithoutClosingQuote() {
      CommandLine commandLine = CommandLine.Parse("\"Quoted argument");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("\"Quoted argument", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("Quoted argument", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Validates that the parser correctly handles a quoted value assignment that's
    ///   missing the closing quote
    /// </summary>
    [Test]
    public void TestParseQuotedValueWithoutClosingQuote() {
      CommandLine commandLine = CommandLine.Parse("--test=\"Quoted argument");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("--test=\"Quoted argument", commandLine.Arguments[0].Raw);
      Assert.AreEqual("--", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("test", commandLine.Arguments[0].Name);
      Assert.AreEqual("=", commandLine.Arguments[0].Associator);
      Assert.AreEqual("Quoted argument", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Validates that the parser can handle an command line consisting of only spaces
    /// </summary>
    [Test]
    public void TestParseSpacesOnly() {
      CommandLine commandLine = CommandLine.Parse(" \t ");

      Assert.AreEqual(0, commandLine.Arguments.Count);
    }

    /// <summary>
    ///   Validates that the parser can handle a quoted option
    /// </summary>
    [Test]
    public void TestParseQuotedOption() {
      CommandLine commandLine = CommandLine.Parse("--\"hello\"");

      // Quoted options are not supported, so this becomes a loose value
      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("--\"hello\"", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("--\"hello\"", commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Validates that the parser can handle multiple lone argument initators without
    ///   a following argument
    /// </summary>
    [Test]
    public void TestParseMultipleLoneArgumentInitiators() {
      CommandLine commandLine = CommandLine.Parse("--- --");

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("---", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("---", commandLine.Arguments[0].Value);

      Assert.AreEqual("--", commandLine.Arguments[1].Raw);
      Assert.IsNull(commandLine.Arguments[1].Initiator);
      Assert.IsNull(commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.AreEqual("--", commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Verifies that the parser correctly handles options with embedded option initiators
    /// </summary>
    [Test]
    public void TestParseOptionWithEmbeddedInitiator() {
      CommandLine commandLine = CommandLine.Parse("-hello/world=123 -test-case");

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("-hello/world=123", commandLine.Arguments[0].Raw);
      Assert.AreEqual("-", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("hello/world", commandLine.Arguments[0].Name);
      Assert.AreEqual("=", commandLine.Arguments[0].Associator);
      Assert.AreEqual("123", commandLine.Arguments[0].Value);

      Assert.AreEqual("-test-case", commandLine.Arguments[1].Raw);
      Assert.AreEqual("-", commandLine.Arguments[1].Initiator);
      Assert.AreEqual("test-case", commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.IsNull(commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Validates that arguments and values without spaces inbetween can be parsed
    /// </summary>
    [Test]
    public void TestParseOptionAndValueWithoutSpaces() {
      CommandLine commandLine = CommandLine.Parse("\"value\"-option\"value\"");

      Assert.AreEqual(3, commandLine.Arguments.Count);

      Assert.AreEqual("\"value\"", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("value", commandLine.Arguments[0].Value);

      Assert.AreEqual("-option", commandLine.Arguments[1].Raw);
      Assert.AreEqual("-", commandLine.Arguments[1].Initiator);
      Assert.AreEqual("option", commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.IsNull(commandLine.Arguments[1].Value);

      Assert.AreEqual("\"value\"", commandLine.Arguments[2].Raw);
      Assert.IsNull(commandLine.Arguments[2].Initiator);
      Assert.IsNull(commandLine.Arguments[2].Name);
      Assert.IsNull(commandLine.Arguments[2].Associator);
      Assert.AreEqual("value", commandLine.Arguments[2].Value);
    }

    /// <summary>
    ///   Validates that options with modifiers at the end of the command line
    ///   are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionWithModifierAtEnd() {
      CommandLine commandLine = CommandLine.Parse("--test-value- -test+");

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("--test-value-", commandLine.Arguments[0].Raw);
      Assert.AreEqual("--", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("test-value", commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("-", commandLine.Arguments[0].Value);

      Assert.AreEqual("-test+", commandLine.Arguments[1].Raw);
      Assert.AreEqual("-", commandLine.Arguments[1].Initiator);
      Assert.AreEqual("test", commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.AreEqual("+", commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Validates that options with values assigned to them are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionWithAssignment() {
      CommandLine commandLine = CommandLine.Parse("-hello: -world=321");

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("-hello:", commandLine.Arguments[0].Raw);
      Assert.AreEqual("-", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("hello", commandLine.Arguments[0].Name);
      Assert.AreEqual(":", commandLine.Arguments[0].Associator);
      Assert.IsNull(commandLine.Arguments[0].Value);

      Assert.AreEqual("-world=321", commandLine.Arguments[1].Raw);
      Assert.AreEqual("-", commandLine.Arguments[1].Initiator);
      Assert.AreEqual("world", commandLine.Arguments[1].Name);
      Assert.AreEqual("=", commandLine.Arguments[1].Associator);
      Assert.AreEqual("321", commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Validates that options with an empty value at the end of the command line
    ///   string are parsed successfully
    /// </summary>
    [Test]
    public void TestParseOptionAtEndOfString() {
      CommandLine commandLine = CommandLine.Parse("--test:");

      Assert.AreEqual(1, commandLine.Arguments.Count);
      Assert.AreEqual("--test:", commandLine.Arguments[0].Raw);
      Assert.AreEqual("--", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("test", commandLine.Arguments[0].Name);
      Assert.AreEqual(":", commandLine.Arguments[0].Associator);
      Assert.IsNull(commandLine.Arguments[0].Value);
    }

    /// <summary>
    ///   Verifies that the parser can recognize windows command line options if
    ///   configured to windows mode
    /// </summary>
    [Test]
    public void TestWindowsOptionInitiator() {
      CommandLine commandLine = CommandLine.Parse("/hello //world", true);

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("/hello", commandLine.Arguments[0].Raw);
      Assert.AreEqual("/", commandLine.Arguments[0].Initiator);
      Assert.AreEqual("hello", commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.IsNull(commandLine.Arguments[0].Value);

      Assert.AreEqual("//world", commandLine.Arguments[1].Raw);
      Assert.IsNull(commandLine.Arguments[1].Initiator);
      Assert.IsNull(commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.AreEqual("//world", commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Verifies that the parser ignores windows command line options if
    ///   configured to non-windows mode
    /// </summary>
    [Test]
    public void TestNonWindowsOptionValues() {
      CommandLine commandLine = CommandLine.Parse("/hello //world", false);

      Assert.AreEqual(2, commandLine.Arguments.Count);

      Assert.AreEqual("/hello", commandLine.Arguments[0].Raw);
      Assert.IsNull(commandLine.Arguments[0].Initiator);
      Assert.IsNull(commandLine.Arguments[0].Name);
      Assert.IsNull(commandLine.Arguments[0].Associator);
      Assert.AreEqual("/hello", commandLine.Arguments[0].Value);

      Assert.AreEqual("//world", commandLine.Arguments[1].Raw);
      Assert.IsNull(commandLine.Arguments[1].Initiator);
      Assert.IsNull(commandLine.Arguments[1].Name);
      Assert.IsNull(commandLine.Arguments[1].Associator);
      Assert.AreEqual("//world", commandLine.Arguments[1].Value);
    }

    /// <summary>
    ///   Tests whether the existence of named arguments can be checked
    /// </summary>
    [Test]
    public void TestHasArgument() {
      CommandLine test = CommandLine.Parse("/first:x /second:y /second:z third");

      Assert.IsTrue(test.HasArgument("first"));
      Assert.IsTrue(test.HasArgument("second"));
      Assert.IsFalse(test.HasArgument("third"));
      Assert.IsFalse(test.HasArgument("fourth"));
    }

    /// <summary>
    ///   Tests whether a command line can be built with the command line class
    /// </summary>
    [Test]
    public void TestCommandLineFormatting() {
      CommandLine commandLine = new CommandLine(true);

      commandLine.AddValue("single");
      commandLine.AddValue("with space");
      commandLine.AddOption("option");
      commandLine.AddOption("@@", "extravagant-option");
      commandLine.AddAssignment("name", "value");
      commandLine.AddAssignment("name", "value with spaces");
      commandLine.AddAssignment("@@", "name", "value");
      commandLine.AddAssignment("@@", "name", "value with spaces");

      Assert.AreEqual(8, commandLine.Arguments.Count);
      Assert.AreEqual("single", commandLine.Arguments[0].Value);
      Assert.AreEqual("with space", commandLine.Arguments[1].Value);
      Assert.AreEqual("option", commandLine.Arguments[2].Name);
      Assert.AreEqual("@@", commandLine.Arguments[3].Initiator);
      Assert.AreEqual("extravagant-option", commandLine.Arguments[3].Name);
      Assert.AreEqual("name", commandLine.Arguments[4].Name);
      Assert.AreEqual("value", commandLine.Arguments[4].Value);
      Assert.AreEqual("name", commandLine.Arguments[5].Name);
      Assert.AreEqual("value with spaces", commandLine.Arguments[5].Value);
      Assert.AreEqual("@@", commandLine.Arguments[6].Initiator);
      Assert.AreEqual("name", commandLine.Arguments[6].Name);
      Assert.AreEqual("value", commandLine.Arguments[6].Value);
      Assert.AreEqual("name", commandLine.Arguments[7].Name);
      Assert.AreEqual("@@", commandLine.Arguments[7].Initiator);
      Assert.AreEqual("value with spaces", commandLine.Arguments[7].Value);

      string commandLineString = commandLine.ToString();
      Assert.AreEqual(
        "single \"with space\" " +
        "-option @@extravagant-option " +
        "-name=value -name=\"value with spaces\" " +
        "@@name=value @@name=\"value with spaces\"",
        commandLineString
      );

    }

    /// <summary>
    ///   Tests whether a command line can be built that contains empty arguments
    /// </summary>
    [Test]
    public void TestNullArgumentFormatting() {
      CommandLine commandLine = new CommandLine(false);

      commandLine.AddValue(string.Empty);
      commandLine.AddValue("hello");
      commandLine.AddValue(null);
      commandLine.AddValue("-test");

      Assert.AreEqual(4, commandLine.Arguments.Count);
      string commandLineString = commandLine.ToString();
      Assert.AreEqual("\"\" hello \"\" \"-test\"", commandLineString);
    }

  }

} // namespace Nuclex.Support.Parsing

#endif // UNITTEST
