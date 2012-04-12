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
using System.IO;

using NUnit.Framework;

namespace Nuclex.Support.Tracking {

  /// <summary>Unit Test for the weighted transaction wrapper</summary>
  [TestFixture]
  public class WeightedTransactionTest {

    #region class TestTransaction

    /// <summary>Transaction used for testing in this unit test</summary>
    private class TestTransaction : Transaction { }

    #endregion // class TestTransaction

    /// <summary>
    ///   Tests whether the weighted transaction wrapper correctly stores the transaction
    ///   it was given in the constructor
    /// </summary>
    [Test]
    public void TestTransactionStorage() {
      TestTransaction transaction = new TestTransaction();
      WeightedTransaction<Transaction> testWrapper = new WeightedTransaction<Transaction>(
        transaction
      );

      Assert.AreSame(transaction, testWrapper.Transaction);
    }

    /// <summary>
    ///   Tests whether the weighted transaction wrapper correctly applies the default
    ///   unit weight to the transaction if no explicit weight was specified
    /// </summary>
    [Test]
    public void TestDefaultWeight() {
      TestTransaction transaction = new TestTransaction();
      WeightedTransaction<Transaction> testWrapper = new WeightedTransaction<Transaction>(
        transaction
      );

      Assert.AreEqual(1.0f, testWrapper.Weight);
    }

    /// <summary>
    ///   Tests whether the weighted transaction wrapper correctly stores the weight
    ///   it was given in the constructor
    /// </summary>
    [Test]
    public void TestWeightStorage() {
      TestTransaction transaction = new TestTransaction();
      WeightedTransaction<Transaction> testWrapper = new WeightedTransaction<Transaction>(
        transaction, 12.0f
      );

      Assert.AreEqual(12.0f, testWrapper.Weight);
    }

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
