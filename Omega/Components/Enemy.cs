using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omega.Ai;
using Omega.GraphicsEngine;

namespace Omega.Components
{
    public enum EnemyStatus
    {
        Dormant,
        Active,
        Hit,
        Exploding,
        Dead
    }

    public class Enemy : DrawableGameComponent
    {
        #region Field Region

        public EnemyStatus Status;
        public EnemyController Controller;
        public int PointValue = 10;
        public float timeToDie = 600;

        protected Game1 gameRef;
        protected string name;
        protected bool gender;
        protected string mapName;
        protected Point tile;
        protected AnimatedSprite sprite;
        protected Texture2D texture;
        protected Texture2D explodeTexture;
      
        protected float speed = 180f;
        protected float rotation = 0f;

        protected EnemyType enemyType;
        protected Vector2 position;
        protected SoundEffect explodeSound;

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

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public EnemyType EnemyType
        {
            get; set;
        }

        #endregion

        #region Constructor Region

        private Enemy(Game game) : base(game)
        {
        }

        public Enemy(Game game, EnemyType enemyType, Vector2 startPosition, Texture2D texture, Texture2D explodeTexture, SoundEffect explodeSound) : base(game)
        {
            gameRef = (Game1)game;
            
            this.texture = texture;
            this.explodeTexture = explodeTexture;
            this.explodeSound = explodeSound;
            this.sprite = new AnimatedSprite(texture, gameRef.PlayerAnimations);
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.Position = startPosition;
            this.enemyType = enemyType;

            this.Controller = CreateController(); // always do this last
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

            if (this.Status == EnemyStatus.Hit)
            {
                Explode();
            }

            if (this.Status == EnemyStatus.Exploding)
            {
                this.timeToDie -= gameTime.ElapsedGameTime.Milliseconds;
                
            }

            if (this.timeToDie <= 0)
            {
                this.Status = EnemyStatus.Dead;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        public bool CheckCollision(AnimatedSprite targetSprite)
        {
            if ((Status == EnemyStatus.Exploding) || (Status == EnemyStatus.Dead))
                return false;

            bool collision = false;

            float xDist = (targetSprite.Position.X - this.sprite.Position.X) * (targetSprite.Position.X - this.sprite.Position.X);
            float yDist = (targetSprite.Position.Y - this.sprite.Position.Y) * (targetSprite.Position.Y - this.sprite.Position.Y);

            if (Math.Sqrt(xDist + yDist) < 60)
            {
                collision = true;
            }
            return collision;
        }

        public void Evolve()
        {
            if (this.enemyType == EnemyType.Warlord)
                return;

            this.enemyType += 1;
            this.Controller = CreateController();
            LoadTexture();

            gameRef.ContentBank.GetSoundEffect(Content.ContentItem.Sounds_Combat_Evolve).Play();

            Vector2 currentPosition = Position; // do this before you replace the sprite
            Vector2 velocity = this.sprite.Velocity;

            Dictionary<AnimationKey, Animation> animations = gameRef.ContentBank.GetEnemyAnimations();

            this.sprite = new AnimatedSprite(this.texture, animations);
            this.sprite.Position = currentPosition;
            this.sprite.Velocity = velocity;
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.sprite.IsAnimating = true;
        }

        private EnemyController CreateController()
        {
            EnemyController controller;

            switch (this.enemyType)
            {
                case EnemyType.Mine:
                    controller = new MineController(this);
                    break;
                case EnemyType.Drone:
                    controller = new DroneController(this);
                    break;
                case EnemyType.MineLayer:
                    controller = new MineLayerController(this);
                    break;
                case EnemyType.Warrior:
                    controller = new WarriorController(this);
                    break;
                case EnemyType.Warlord:
                    controller = new WarriorController(this);
                    break;
                default:
                    controller = new MineController(this);
                    break;
            }
            return controller;
        }

        private void LoadTexture()
        {
            switch (this.enemyType)
            {
                case EnemyType.Mine:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_Mine);
                    break;
                case EnemyType.Drone:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_Drone);
                    break;
                case EnemyType.MineLayer:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_MineLayer);
                    break;
                case EnemyType.Warrior:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_Warrior);
                    break;
                case EnemyType.Warlord:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_Warlord);
                    break;
                default:
                    this.texture = gameRef.ContentBank.GetTexture(Content.ContentItem.Graphics_Enemy_Mine);
                    break;
            }
        }

        private void Explode()
        {
            // explosion
            this.Status = EnemyStatus.Exploding;
            explodeSound.Play();

            Animation animation = new Animation(6, 92, 92, 0, 0);
            animation.FramesPerSecond = 12;
            Dictionary<AnimationKey, Animation> explodeAnimations = new Dictionary<AnimationKey, Animation>();
            Vector2 currentPosition = Position;

            explodeAnimations.Add(AnimationKey.Idle, animation);
            this.sprite = new AnimatedSprite(explodeTexture, explodeAnimations);
            this.sprite.Position = currentPosition;
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.sprite.IsAnimating = true;
        }
        #endregion
    }
}
