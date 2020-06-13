using Microsoft.Xna.Framework;
using System;

namespace Omega.StateManager
{
    public interface IStateManager
    {
        GameState CurrentState { get; }
        event EventHandler StateChanged;
        void PushState(GameState state, PlayerIndex? index);
        void ChangeState(GameState state, PlayerIndex? index);
        void PopState();
        bool ContainsState(GameState state);
    }
}
