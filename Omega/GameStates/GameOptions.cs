using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.GameStates
{
    public enum GameType
    {
        SinglePlayer = 1,
        TwoPlayerCooperative = 2,
        TwoPlayerDeathmatch = 3
    }

    public class GameOptions
    {
        public GameType GameType;
    }
}
