using Microsoft.Xna.Framework;
using Omega.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class DroneController : EnemyController
    {
        public DroneController(Enemy parent) : base(parent)
        {
            Acceleration = 2;
            MaxSpeed = 3;

            this.canLayMines = false;
            this.canShoot = false;
            this.RotationSpeed = 0.001f;

            this.parent.PointValue = 20;
            
            Random rnd = new Random();
            parent.Sprite.Velocity = new Vector2(((float)rnd.NextDouble()) * MaxSpeed, ((float)rnd.NextDouble()) * MaxSpeed);
        }

        public override void Update(GameTime gameTime, List<Vector2> targets)
        {

            base.Update(gameTime, targets);
        }
    }
}
