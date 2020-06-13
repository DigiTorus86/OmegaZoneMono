using System;
using Microsoft.Xna.Framework;

namespace Omega.StateManager
{
    public interface IGameState
    {
        GameState Tag { get; }
        PlayerIndex? PlayerIndexInControl { get; set; }
    }
}
