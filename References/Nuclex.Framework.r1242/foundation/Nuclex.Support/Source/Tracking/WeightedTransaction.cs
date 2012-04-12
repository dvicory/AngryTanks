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

namespace Nuclex.Support.Tracking {

  /// <summary>Transaction with an associated weight for the total progress</summary>
  public class WeightedTransaction<TransactionType> where TransactionType : Transaction {

    /// <summary>
    ///   Initializes a new weighted transaction with a default weight of 1.0
    /// </summary>
    /// <param name="transaction">Transaction whose progress to monitor</param>
    public WeightedTransaction(TransactionType transaction) : this(transaction, 1.0f) { }

    /// <summary>Initializes a new weighted transaction</summary>
    /// <param name="transaction">transaction whose progress to monitor</param>
    /// <param name="weight">Weighting of the transaction's progress</param>
    public WeightedTransaction(TransactionType transaction, float weight) {
      this.transaction = transaction;
      this.weight = weight;
    }

    /// <summary>Transaction being wrapped by this weighted transaction</summary>
    public TransactionType Transaction {
      get { return this.transaction; }
    }

    /// <summary>The contribution of this transaction to the total progress</summary>
    public float Weight {
      get { return this.weight; }
    }

    /// <summary>Transaction whose progress we're tracking</summary>
    private TransactionType transaction;
    /// <summary>Weighting of this transaction in the total progress</summary>
    private float weight;

  }

} // namespace Nuclex.Support.Tracking
