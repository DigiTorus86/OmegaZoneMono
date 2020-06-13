using Microsoft.Xna.Framework;
using Omega.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class MineController : EnemyController
    {
        public MineController(Enemy parent) : base(parent)
        {
            Acceleration = 0;
            MaxSpeed = 0;

            this.canLayMines = false;
            this.canShoot = false;
            this.evolveTime = 0; // can't evolve
        }

        public override void Update(GameTime gameTime, List<Vector2> targets)
        {
            // mines just sit there and explode if you hit them
        }
    }
}
