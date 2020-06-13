using Microsoft.Xna.Framework;
using Omega.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    class MineLayerController : EnemyController
    {
        
        public MineLayerController(Enemy parent) : base(parent)
        {
            Acceleration = 3;
            MaxSpeed = 4;
            this.canLayMines = true;
            this.mineReloadTime = 2500f;
            this.canShoot = false;
            this.RotationSpeed = 0.002f;

            this.parent.PointValue = 30;

            Random rnd = new Random();
            this.mineReloadTimeRemaining += (float)rnd.NextDouble() * this.mineReloadTime;
            parent.Sprite.Velocity = new Vector2((.5f - (float)rnd.NextDouble()) * MaxSpeed, (.5f - (float)rnd.NextDouble()) * MaxSpeed);
        }

        public override void Update(GameTime gameTime, List<Vector2> targets)
        {
            if (parent.Status != EnemyStatus.Active)
                return;

            if (this.mineReloadTimeRemaining > 0)
            {
                this.mineReloadTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (this.mineReloadTimeRemaining <= 0)
            {
                // minelayer can lay mine now
                // TODO: add variability
                Action = EnemyAction.LayMine;
                this.mineReloadTimeRemaining = mineReloadTime;
            }

            base.Update(gameTime, targets);
        }

    }
}
