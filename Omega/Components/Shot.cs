using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omega.GraphicsEngine;

namespace Omega.Components
{
    public class Shot : DrawableGameComponent
    {
        #region Field Region

        protected Game1 gameRef;
        protected string name;
        protected bool gender;
        protected string mapName;
        protected Point tile;
        protected AnimatedSprite sprite;
        protected Texture2D texture;

        public float Range;
        
        #endregion

        #region Property Region

        public Vector2 Position
        {
            get { return sprite.Position; }
            set { sprite.Position = value; }
        }

        public AnimatedSprite Sprite
        {
            get { return sprite; }
        }

        #endregion

        #region Constructor Region

        private Shot(Game game) : base(game)
        {
        }

        public Shot(Game game, Vector2 position, Vector2 velocity, float range, Texture2D texture, Dictionary<AnimationKey, Animation> animations) : base(game)
        {
            gameRef = (Game1)game;

            this.Range = range;
            this.texture = texture;
         
            this.sprite = new AnimatedSprite(texture, animations);
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.sprite.Position = position;
            this.sprite.Velocity = velocity;
            this.sprite.Drag = 1f; // no drag
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            Range = Range - gameTime.ElapsedGameTime.Milliseconds;
            if (Range < - 0)
            {
                sprite.Velocity = Vector2.Zero;
                this.Dispose();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        public void CheckCollision(List<Enemy> enemies)
        {
            enemies.ForEach(en => CheckCollision(en));
        }

        public bool CheckCollision(Player player)
        {
            if (player.Sprite.CurrentAnimation == AnimationKey.Dieing)
                return false;

            if (player.Status != PlayerStatus.Active)
                return false;

            return CheckCollision(player.Sprite);
        }

        public bool CheckCollision(Enemy enemy)
        {
            if (enemy.Status == EnemyStatus.Exploding || enemy.Status == EnemyStatus.Dead)
                return false;

            bool collision = CheckCollision(enemy.Sprite);
            if (collision)
            { 
                enemy.Status = EnemyStatus.Hit;
            }

            return collision;
        }

        public bool CheckCollision(AnimatedSprite targetSprite)
        {
            bool collision = false;

            float xDist = (targetSprite.Position.X - this.sprite.Position.X) * (targetSprite.Position.X - this.sprite.Position.X);
            float yDist = (targetSprite.Position.Y - this.sprite.Position.Y) * (targetSprite.Position.Y - this.sprite.Position.Y);

            if (Math.Sqrt(xDist + yDist) < 32)
            {
                collision = true;
                Range = 0;
            }
            return collision;
        }

        #endregion
    }
}
