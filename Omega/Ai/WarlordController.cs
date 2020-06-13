using Microsoft.Xna.Framework;
using Omega.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class WarlordController : EnemyController
    {
        public WarlordController(Enemy parent) : base(parent)
        {
            Acceleration = 6;
            MaxSpeed = 6;
            ShotRange = 800;
            this.canShoot = true;
            this.shotReloadTime = 621f;
            this.canLayMines = true;
            this.mineReloadTime = 2100f;
            this.evolveTime = 0f; // can't evolve
            this.RotationSpeed = 0.02f;

            this.parent.PointValue = 50;

            Random rnd = new Random();
            this.mineReloadTimeRemaining = (float)rnd.NextDouble() * this.mineReloadTime;
            this.shotReloadTimeRemaining = (float)rnd.NextDouble() * this.shotReloadTime;

            parent.Sprite.Velocity = new Vector2((.5f - (float)rnd.NextDouble()) * MaxSpeed, (.5f - (float)rnd.NextDouble()) * MaxSpeed); // TODO: remove
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
                // enemy can lay mine now
                // TODO: add variability
                Action = EnemyAction.LayMine;
                this.mineReloadTimeRemaining = this.mineReloadTime;
            }

            if (this.shotReloadTimeRemaining > 0)
            {
                this.shotReloadTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (this.shotReloadTimeRemaining <= 0)
            {
                // enemy can shoot now
                // TODO: add variability
                Action = EnemyAction.Shoot;
                this.shotReloadTimeRemaining = this.shotReloadTime;
            }

            base.Update(gameTime, targets);
        }
    }
}
