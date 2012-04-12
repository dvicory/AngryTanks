using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex.Game.States {

  /// <summary>Allows the management of game states</summary>
  public interface IGameStateService {

    /// <summary>The currently active game state. Can be null.</summary>
    GameState ActiveState { get; }

    /// <summary>Pushes the specified state onto the state stack</summary>
    /// <param name="state">State that will be pushed onto the stack</param>
    void Push(GameState state);

    /// <summary>Takes the currently active game state from the stack</summary>
    void Pop();

    /// <summary>Switches the game to the specified state</summary>
    /// <param name="state">State the game will be switched to</param>
    /// <remarks>
    ///   This replaces the running game state in the stack with the specified state.
    /// </remarks>
    void Switch(GameState state);

  }

} // namespace Nuclex.Game.States
