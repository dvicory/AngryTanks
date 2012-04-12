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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;

namespace Nuclex.Graphics.SpecialEffects.Particles {

  /// <summary>Manages a collection of particle affectors</summary>
  /// <typeparam name="ParticleType">Data type of the particles</typeparam>
  public class AffectorCollection<ParticleType> :
    ICollection<IParticleAffector<ParticleType>>, ICollection {

    #region class Enumerator

    /// <summary>Enumerates over all affectors in the collection</summary>
    private class Enumerator :
      IEnumerator<IParticleAffector<ParticleType>>, IEnumerator {

      /// <summary>Initializes a new affector collection enumerator</summary>
      /// <param name="coalescableAffectors">
      ///   Coalescable affectors that will be enumerated
      /// </param>
      /// <param name="noncoalescableAffectors">
      ///   Non-coalescable affectors that will be enumerated
      /// </param>
      public Enumerator(
        List<IParticleAffector<ParticleType>> coalescableAffectors,
        List<IParticleAffector<ParticleType>> noncoalescableAffectors
      ) {
        this.coalescableAffectors = coalescableAffectors;
        this.nonCoalescableAffectors = noncoalescableAffectors;

        Reset();
      }

      /// <summary>Returns the item at the enumerator's current position</summary>
      public IParticleAffector<ParticleType> Current {
        get {
          if(this.index == -1) {
            throw new InvalidOperationException(
              "The enumerator is not on a valid position"
            );
          }

          return this.currentList[this.index];
        }
      }

      /// <summary>Advances the enumerator to the next item</summary>
      /// <returns>
      ///   True if the next item has been selected, false if the end of
      ///   the enumeration has been reached
      /// </returns>
      public bool MoveNext() {
        if(this.index == -1) {
          if(this.currentList == null) {
            this.currentList = this.coalescableAffectors;
          } else {
            return false;
          }
        }
        
        if(index >= this.currentList.Count - 1) {
          if(ReferenceEquals(this.currentList, this.coalescableAffectors)) {
            this.currentList = this.nonCoalescableAffectors;

            if(this.nonCoalescableAffectors.Count == 0) {
              this.index = -1;
              return false;
            } else {
              this.index = 0;
              return true;
            }
          } else {
            this.index = -1;
            return false;
          }
        }
          
        ++this.index;
        return true;
      }

      /// <summary>Resets the enumerator to its initial position</summary>
      public void Reset() {
        this.currentList = null;
        this.index = -1;
      }

      /// <summary>Immediately releases all resources owned by the enumerator</summary>
      public void Dispose() { }

      /// <summary>Returns the item at the enumerator's current position</summary>
      object IEnumerator.Current {
        get { return Current; }
      }

      /// <summary>Coalescable affectors being enumerated</summary>
      private List<IParticleAffector<ParticleType>> coalescableAffectors;
      /// <summary>Non-coalescable affectors being enumerated</summary>
      private List<IParticleAffector<ParticleType>> nonCoalescableAffectors;

      /// <summary>List being currently enumerated</summary>
      private List<IParticleAffector<ParticleType>> currentList;
      /// <summary>Current enumeration index</summary>
      private int index;

    }

    #endregion // class Enumerator

    /// <summary>Initializes a new collection of affectors</summary>
    internal AffectorCollection() {
      this.coalescableAffectors = new List<IParticleAffector<ParticleType>>();
      this.noncoalescableAffectors = new List<IParticleAffector<ParticleType>>();
    }

    /// <summary>Adds an affector to the collection</summary>
    /// <param name="affector">Affector that will be added to the collection</param>
    public void Add(IParticleAffector<ParticleType> affector) {
      if(affector.IsCoalescable) {
        this.coalescableAffectors.Add(affector);
      } else {
        this.noncoalescableAffectors.Add(affector);
      }
    }

    /// <summary>Removes all affectors from the collection</summary>
    public void Clear() {
      this.coalescableAffectors.Clear();
      this.noncoalescableAffectors.Clear();
    }

    /// <summary>
    ///   Determines whether the collection contains the specified affector
    /// </summary>
    /// <param name="affector">Affector that will be looked up in the collection</param>
    /// <returns>True if the collection contains the specified affector</returns>
    public bool Contains(IParticleAffector<ParticleType> affector) {
      if(affector.IsCoalescable) {
        return this.coalescableAffectors.Contains(affector);
      } else {
        return this.noncoalescableAffectors.Contains(affector);
      }
    }

    /// <summary>Copies the contents of the collection into an array</summary>
    /// <param name="array">Array the collection's contents will be copied to</param>
    /// <param name="start">Index at which writing to the array will begin</param>
    public void CopyTo(IParticleAffector<ParticleType>[] array, int start) {
      int end = start + Count;
      if(end > array.Length) {
        throw new IndexOutOfRangeException("Array is too small to contain all items");
      }

      this.coalescableAffectors.CopyTo(array, start);
      this.noncoalescableAffectors.CopyTo(array, start + this.coalescableAffectors.Count);
    }

    /// <summary>Number of affectors currently contained in the collection</summary>
    public int Count {
      get { return this.coalescableAffectors.Count + this.noncoalescableAffectors.Count; }
    }

    /// <summary>Whether the collection is read-only</summary>
    public bool IsReadOnly {
      get { return false; }
    }

    /// <summary>Removes an affector from the collection</summary>
    /// <param name="affector">Affector that will be removed from the collection</param>
    /// <returns>True if the affector was found and removed</returns>
    public bool Remove(IParticleAffector<ParticleType> affector) {
      if(affector.IsCoalescable) {
        return this.coalescableAffectors.Remove(affector);
      } else {
        return this.noncoalescableAffectors.Remove(affector);
      }
    }

    /// <summary>Creates an enumerator for the affectors in the collection</summary>
    /// <returns>
    ///   A new enumerator that will iterate over all affectors in the collection
    /// </returns>
    public IEnumerator<IParticleAffector<ParticleType>> GetEnumerator() {
      return new Enumerator(this.coalescableAffectors, this.noncoalescableAffectors);
    }

    /// <summary>Copies the contents of the collection into an array</summary>
    /// <param name="array">Array the collection's contents will be copied to</param>
    /// <param name="start">Index at which writing to the array will begin</param>
    void ICollection.CopyTo(Array array, int start) {
      int end = start + Count;
      if(end > array.Length) {
        throw new IndexOutOfRangeException("Array is too small to contain all items");
      }

      (this.coalescableAffectors as ICollection).CopyTo(array, start);
      (this.noncoalescableAffectors as ICollection).CopyTo(
        array, start + this.coalescableAffectors.Count
      );
    }

    /// <summary>Whether the collection is synchronized</summary>
    bool ICollection.IsSynchronized {
      get { return false; }
    }

    /// <summary>The synchronization root for this collection</summary>
    object ICollection.SyncRoot { get { return this; } }

    /// <summary>Creates an enumerator for the affectors in the collection</summary>
    /// <returns>
    ///   A new enumerator that will iterate over all affectors in the collection
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this.coalescableAffectors, this.noncoalescableAffectors);
    }

    /// <summary>All coalescable affectors registered in the collection</summary>
    internal List<IParticleAffector<ParticleType>> CoalescableAffectors {
      get { return this.coalescableAffectors; }
    }

    /// <summary>All non-coalescable affectors registered in the collection</summary>
    internal List<IParticleAffector<ParticleType>> NoncoalescableAffectors {
      get { return this.noncoalescableAffectors; }
    }

    /// <summary>Coalescable affectors registered to the particle system</summary>
    private List<IParticleAffector<ParticleType>> coalescableAffectors;
    /// <summary>Non-coalescable affectors registered to the particle system</summary>
    private List<IParticleAffector<ParticleType>> noncoalescableAffectors;

  }

} // namespace Nuclex.Graphics.SpecialEffects.Particles
