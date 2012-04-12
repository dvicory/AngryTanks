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
using System.Threading;

#if UNITTEST

using NUnit.Framework;
using NMock2;

namespace Nuclex.Support.Tracking {

  /// <summary>
  ///   Unit Test for the observation wrapper collection of weighted transactions
  /// </summary>
  [TestFixture]
  public class WeightedTransactionWrapperCollectionTest {

    /// <summary>
    ///   Tests whether the wrapper collection is handing out the unwrapped transactions
    /// </summary>
    [Test]
    public void TestWrapperCollection() {
      WeightedTransaction<Transaction> transaction = new WeightedTransaction<Transaction>(
        Transaction.EndedDummy
      );

      ObservedWeightedTransaction<Transaction> observed =
        new ObservedWeightedTransaction<Transaction>(
          transaction,
          endedCallback,
          progressUpdatedCallback
        );

      WeightedTransactionWrapperCollection<Transaction> wrapper =
        new WeightedTransactionWrapperCollection<Transaction>(
          new ObservedWeightedTransaction<Transaction>[] { observed }
        );

      Assert.AreSame(transaction, wrapper[0]);
    }

    /// <summary>Dummy callback used as event subscriber in the tests</summary>
    private void endedCallback() { }

    /// <summary>Dummy callback used as event subscriber in the tests</summary>
    private void progressUpdatedCallback() { }

  }

} // namespace Nuclex.Support.Tracking

#endif // UNITTEST
