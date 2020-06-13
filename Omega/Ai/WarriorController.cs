using Microsoft.Xna.Framework;
using Omega.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class WarriorController : EnemyController
    {
        public WarriorController(Enemy parent) : base(parent)
        {
            Acceleration = 5;
            MaxSpeed = 5;
            ShotRange = 600f;
            this.shotReloadTime = 2000f;
            this.canShoot = true;
            this.RotationSpeed = 0.01f;

            this.parent.PointValue = 40;

            Random rnd = new Random();
            this.shotReloadTimeRemaining += (float)rnd.NextDouble() * this.shotReloadTime;
            parent.Sprite.Velocity = new Vector2((.5f - (float)rnd.NextDouble()) * MaxSpeed, (.5f - (float)rnd.NextDouble()) * MaxSpeed);
        }

        public override void Update(GameTime gameTime, List<Vector2> targets)
        {
            if (parent.Status != EnemyStatus.Active)
                return;

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
