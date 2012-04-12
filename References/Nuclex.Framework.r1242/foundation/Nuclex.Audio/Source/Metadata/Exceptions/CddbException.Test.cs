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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using NUnit.Framework;

namespace Nuclex.Audio.Metadata.Exceptions {

  /// <summary>Unit Test for the CddbException class</summary>
  [TestFixture]
  public class CddbExceptionTest {

    #region class TestCddbException

    /// <summary>Test implementation of a CDDB exception for unit testing</summary>
    [Serializable]
    private class TestCddbException : CddbException {

      /// <summary>Initializes the exception</summary>
      /// <param name="statusCode">CDDB status code to provide with the exception</param>
      public TestCddbException(int statusCode) : base(statusCode) { }

      /// <summary>Initializes the exception with an error message</summary>
      /// <param name="statusCode">CDDB status code to provide with the exception</param>
      /// <param name="message">Error message describing the cause of the exception</param>
      public TestCddbException(int statusCode, string message) :
        base(statusCode, message) { }

      /// <summary>Initializes the exception as a followup exception</summary>
      /// <param name="statusCode">CDDB status code to provide with the exception</param>
      /// <param name="message">Error message describing the cause of the exception</param>
      /// <param name="inner">Preceding exception that has caused this exception</param>
      public TestCddbException(int statusCode, string message, Exception inner) :
        base(statusCode, message, inner) { }

      /// <summary>Initializes the exception from its serialized state</summary>
      /// <param name="info">Contains the serialized fields of the exception</param>
      /// <param name="context">Additional environmental informations</param>
      protected TestCddbException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context
      ) :
        base(info, context) { }

    }

    #endregion // class TestCddbException

    /// <summary>
    ///   Verifies that the exception's default constructor is working
    /// </summary>
    [Test]
    public void TestSimpleConstructor() {
      CddbException testException = new TestCddbException(123);

      Assert.AreEqual(123, testException.StatusCode);

      string testExceptionString = testException.ToString();
      Assert.IsNotNull(testExceptionString);
    }

    /// <summary>
    ///   Checks whether the exception correctly stores its inner exception
    /// </summary>
    [Test]
    public void TestInnerException() {
      Exception inner = new Exception("This is a test");
      CddbException testException = new TestCddbException(
        456, "Hello World", inner
      );

      Assert.AreSame(inner, testException.InnerException);
    }

    /// <summary>
    ///   Test whether the exception can be serialized
    /// </summary>
    [Test]
    public void TestSerialization() {
      BinaryFormatter formatter = new BinaryFormatter();

      using(MemoryStream memory = new MemoryStream()) {
        TestCddbException exception1 = new TestCddbException(
          789, "Hello World"
        );

        formatter.Serialize(memory, exception1);
        memory.Position = 0;
        object exception2 = formatter.Deserialize(memory);

        Assert.IsInstanceOf<TestCddbException>(exception2);
        Assert.AreEqual(
          exception1.StatusCode, ((TestCddbException)exception2).StatusCode
        );
        Assert.AreEqual(
          exception1.Message, ((TestCddbException)exception2).Message
        );
      }
    }

  }

} // namespace Nuclex.Audio.Metadata.Exceptions

#endif // UNITTEST
