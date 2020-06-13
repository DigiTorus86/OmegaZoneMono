using Microsoft.Xna.Framework;
using Omega.Components;
using Omega.Content;
using Omega.GraphicsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Ai
{
    public class EnemyListController : EnemyController
    {
        protected Game1 gameRef;
        public List<Enemy> enemies;
        public List<Shot> shots;

        protected FlowMap flowMap;
        protected Random rnd;

        public EnemyListController(Game game, Enemy parent, List<Enemy> enemies): base(parent)
        {
            this.gameRef = (Game1)game;

            this.enemies = enemies;
            this.shots = new List<Shot>();
            this.flowMap = new FlowMap();
            this.rnd = new Random();
        }

        public override void Update(GameTime gameTime, List<Vector2> targets)
        {
            List<Enemy> newEnemies = new List<Enemy>();

            foreach (Enemy enemy in enemies)
            {
                if (enemy.EnemyType != EnemyType.Mine)
                    AdjustFlowMovement(enemy);

                enemy.Controller.Update(gameTime, targets);

                switch (enemy.Controller.Action)
                {
                    case EnemyAction.LayMine:
                        newEnemies.Add(LayMine(enemy));
                        break;
                    case EnemyAction.Shoot:
                        shots.Add(Shoot(enemy, targets));
                        break;
                    default:
                        // no action
                        break;
                }

                if (enemy.Controller.CanEvolve)
                {
                    enemy.Evolve();
                }

                enemy.Controller.Action = EnemyAction.NoAction; // reset
            }

            foreach (Enemy enemy in newEnemies)
            {
                enemies.Add(enemy);
            }
        }

        private void AdjustFlowMovement(Enemy enemy)
        {
            int row = (int)enemy.Position.Y / 64;
            int col = (int)enemy.Position.X / 64;

            enemy.Sprite.Velocity += flowMap.Vectors[row, col];
        }

        private Enemy LayMine(Enemy enemy)
        {
            return new Enemy(gameRef, EnemyType.Mine, enemy.Position,
                                    gameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Mine),
                                    gameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Explode),
                                    gameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Explode1));
        }

        private Shot Shoot(Enemy enemy, List<Vector2> targets)
        {
            Dictionary<AnimationKey, Animation> shotAnimations = new Dictionary<AnimationKey, Animation>();
            Animation animation = new Animation(1, 8, 8, 0, 0);
            shotAnimations.Add(AnimationKey.Idle, animation);

            Vector2 position = enemy.Position;

            Vector2 target = Vector2.Zero;
            if (targets.Count > 0)
            {
                target = enemy.Controller.GetClosestTarget(targets);
            }
            else
            {
                new Vector2((float)this.rnd.Next(1200) + 40, (float)this.rnd.Next(640) + 40);  // some random spot
            }

            Vector2 velocity = CalculateVelocity(position, target, 10);

            return new Shot(gameRef, position, velocity, enemy.Controller.ShotRange,
                                    gameRef.ContentBank.GetTexture(ContentItem.Graphics_Player_Shot),
                                    shotAnimations);
        }

        private void Evolve(Enemy enemy)
        {
            enemy.EnemyType += 1;


        }

        private Vector2 CalculateVelocity(Vector2 source, Vector2 target, float maxSpeed)
        {
            double angle = Math.Atan2(target.Y - source.Y, target.X - source.X) - MathHelper.PiOver2;
            return new Vector2((float)Math.Sin(angle) * maxSpeed * -1, (float)Math.Cos(angle) * maxSpeed);
        }
    }
}
