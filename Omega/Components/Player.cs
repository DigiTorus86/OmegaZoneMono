using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omega.Content;
using Omega.GraphicsEngine;

namespace Omega.Components
{
    public enum PlayerStatus
    {
        Disabled = 0,
        Active = 1,
        Hit = 2,
        Exploding =3,
        Dead = 4,
        Resurrecting = 5,
        GameOver = 6
    }

    public class Player : DrawableGameComponent
    {
        #region Field Region

        public PlayerStatus Status = PlayerStatus.Active;
        public float MaxSpeed = 4;

        public float shotReloadTime = 200f;
        public float shotReloadTimeRemaining = 0f;

        protected float dyingTime = 600f;
        protected float dyingTimeRemaining = 600f;

        protected float resurrectTime = 2000f;
        protected float resurrectTimeRemaining = 2000f;

        protected Game1 gameRef;
        protected bool gender;
        protected string mapName;
        protected Point tile;
        protected AnimatedSprite sprite;
        protected Texture2D texture;
        protected float speed = 180f;
        protected float rotation = 0f;

        protected int playerIndex = 1;
        protected int score;
        protected int lives;
        
        protected int nextLifeScore;
        protected Difficulty difficulty;

        protected Vector2 position;

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

        public int Score
        {
            get { return score; }
            private set { score = value; }
        }

        public int Lives
        {
            get { return lives; }
            private set { lives = value; }
        }

        public Difficulty Difficulty
        {
            get; set;
        }

        #endregion

        #region Constructor Region

        private Player(Game game) : base(game)
        {
            this.score = 0;
            this.lives = 5;
        }

        public Player(Game game, int playerIndex, Difficulty difficulty) : base(game)
        {
            gameRef = (Game1)game;
            this.playerIndex = playerIndex;
            this.difficulty = difficulty;
            
            SetPlayerShipSprite();

            Status = PlayerStatus.Active;
            this.score = 0;
            this.lives = 5;
            this.nextLifeScore = 5000; // TODO: change based on difficulty
        }

        #endregion

        #region Method Region

        public void AddToScore(int points)
        {
            score += points;

            if (score >= nextLifeScore)
            {
                lives += 1;
                nextLifeScore += 5000;
            }
        }

        public void SavePlayer()
        {
        }

        public static Player Load(Game game)
        {
            Player player = new Player(game);

            return player;
        }

        public override void Initialize()
        {
            Status = PlayerStatus.Active;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Status == PlayerStatus.Disabled)
                return;

            sprite.Update(gameTime);

            if (this.Status == PlayerStatus.Hit)
            {
                Explode();
            }

            if (this.Status == PlayerStatus.Exploding)
            {
                this.dyingTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

            }

            if (this.dyingTimeRemaining <= 0)
            {
                this.dyingTimeRemaining = dyingTime;

                if (this.lives > 0)
                {
                    lives -= 1;
                    this.Status = PlayerStatus.Resurrecting;
                    this.resurrectTimeRemaining = this.resurrectTime;

                    SetPlayerShipSprite();
                }
                else
                {
                    this.Status = PlayerStatus.GameOver;
                }
            }

            if (this.Status == PlayerStatus.Resurrecting)
            {
                this.resurrectTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (this.resurrectTimeRemaining <= 0)
            {
                this.Status = PlayerStatus.Active;
                this.resurrectTimeRemaining = this.resurrectTime;
                gameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Warp).Play();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.Status == PlayerStatus.Disabled || this.Status == PlayerStatus.Resurrecting)
                return;

            base.Draw(gameTime);

            sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        private void Explode()
        {
            Status = PlayerStatus.Exploding;
            this.dyingTimeRemaining = dyingTime;

            gameRef.ContentBank.GetSoundEffect(Content.ContentItem.Sounds_Combat_Explode1).Play();

            Dictionary<AnimationKey, Animation> explodeAnimations = gameRef.ContentBank.GetExplosionAnimations();
            Texture2D explodeTexture = gameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Explode);
            Vector2 currentPosition = Position;

            this.sprite = new AnimatedSprite(explodeTexture, explodeAnimations);
            this.sprite.Position = currentPosition;
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.sprite.IsAnimating = true;
        }


        private void SetPlayerShipSprite()
        {
            if ( this.playerIndex == 1)
            {
                this.texture = gameRef.ContentBank.GetTexture(ContentItem.Graphics_Player_Ship);
            }
            else
            {
                this.texture = gameRef.ContentBank.GetTexture(ContentItem.Graphics_Player_Ship2);
            }
            this.sprite = new AnimatedSprite(texture, gameRef.PlayerAnimations);
            this.sprite.CurrentAnimation = AnimationKey.Idle;
            this.sprite.Drag = 0.995f;
        }
        #endregion
    }
}
