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

namespace Nuclex.Game.States {

  /// <summary>
  ///   Implemented by game states to make them loadable via the loading screen state
  /// </summary>
  public interface ILoadableGameState {

    /// <summary>Can be fired when the loading progress has changed</summary>
    event EventHandler<LoadProgressEventArgs> ProgressChanged;

    /// <summary>Begins loading the game state</summary>
    /// <param name="callback">
    ///   Callback to be called when the game state has been loaded
    /// </param>
    /// <param name="state">User defined object to pass on to the callback</param>
    /// <returns>A result handle that can be used to wait for the loading process</returns>
    IAsyncResult BeginLoad(AsyncCallback callback, object state);

    /// <summary>Waits for the loading operation to finish</summary>
    /// <param name="asyncResult">Pending operation to wait for</param>
    void EndLoad(IAsyncResult asyncResult);
  
  }

} // namespace Nuclex.Game.States
