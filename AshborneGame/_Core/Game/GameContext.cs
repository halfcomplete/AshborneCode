using AshborneGame._Core._Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game
{
    public static class GameContext
    {
        public static Player Player { get; private set; }
        public static GameStateManager GameState { get; private set; }

        public static void Initialise(Player player, GameStateManager gameState)
        {
            Player = player;
            GameState = gameState;
        }
    }
}
