using Microsoft.Xna.Framework;
using Omega.Components;
using Omega.GraphicsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public abstract class EnemyController : IEnemyController, IDisposable
    {
        public EnemyAction Action = EnemyAction.NoAction;

        public float ShotRange = 0f;
        public float RotationSpeed = 0f;

        protected Enemy parent;
        protected float Acceleration = 0;
        protected float MaxSpeed = 0;

        protected bool canLayMines = false;
        protected float mineReloadTime = 8000f;
        protected float mineReloadTimeRemaining;

        protected bool canShoot = false;
        protected float shotReloadTime = 1000f;
        protected float shotReloadTimeRemaining;

        public bool CanEvolve = false;
        protected float evolveTime = 12000f;
        protected float evolveTimeRemaining;

        public EnemyController(Enemy parent)
        {
            this.parent = parent;

            Random rnd = new Random();

            // randomize wait times so that all enemy events don't occur at the same time
            this.shotReloadTimeRemaining = this.shotReloadTime + (rnd.Next((int)this.shotReloadTime));
            this.mineReloadTimeRemaining = this.mineReloadTime + (rnd.Next((int)this.mineReloadTime));
            this.evolveTimeRemaining = this.evolveTime + (rnd.Next((int)this.evolveTime));
        }

        public virtual void Update(GameTime gameTime, List<Vector2> targets)
        {
            if (parent.Sprite.RotationSpeed < this.RotationSpeed)
            {
                parent.Sprite.RotationSpeed += this.RotationSpeed / 100;  // soft spin-up
            }

            if (this.evolveTime > 0)
            {
                this.evolveTimeRemaining = this.evolveTimeRemaining - gameTime.ElapsedGameTime.Milliseconds;
                if (this.evolveTimeRemaining <= 0)
                {
                    this.CanEvolve = true;
                }
            }
        }

        public Vector2 GetClosestTarget(List<Vector2> targets)
        {
            Vector2 closestTarget = Vector2.Zero;
            float minDistance = 0;
            foreach (Vector2 target in targets)
            {
                float dist = VectorDistance(target, parent.Position);
                if ((closestTarget == Vector2.Zero) || (dist < minDistance))
                {
                    closestTarget = target;
                    minDistance = dist;
                }
            }
            return closestTarget;
        }

        protected float VectorDistance(Vector2 vector1, Vector2 vector2)
        {
            return (float)Math.Sqrt(((vector1.X - vector2.X) * (vector1.X - vector2.X)) +
                     ((vector1.Y - vector2.Y) * (vector1.Y - vector2.Y)));
        }

        public void Dispose()
        {
            this.parent = null;
        }
    }
}
