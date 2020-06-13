using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Omega.GraphicsEngine
{
    public enum AnimationKey
    {
        Idle,
        Thrusting,
        Shooting,
        Dieing,
    }

    public class AnimatedSprite
    {
        #region Field Region

        public float Drag = 1f; // 1=no drag, 0=all drag, 1 >= acceleration
        public Vector2 Position;
        public float Rotation;
        public float RotationSpeed = 0f;

        Dictionary<AnimationKey, Animation> animations;
        AnimationKey currentAnimation;
        bool isAnimating;
        Texture2D texture;
        Vector2 velocity = Vector2.Zero;
        float speed = 200.0f;
        
        #endregion

        #region Property Region

        public bool IsActive { get; set; }

        public AnimationKey CurrentAnimation
        {
            get { return currentAnimation; }
            set { currentAnimation = value; }
        }

        public bool IsAnimating
        {
            get { return isAnimating; }
            set { isAnimating = value; }
        }

        public int Width
        {
            get { return animations[currentAnimation].FrameWidth; }
        }

        public int Height
        {
            get { return animations[currentAnimation].FrameHeight; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = MathHelper.Clamp(speed, 1.0f, 400.0f); }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Center
        {
            get { return Position + Origin; }
        }

        public Vector2 Origin
        {
            get { return new Vector2(Width / 2, Height / 2); }
        }

        #endregion

        #region Constructor Region

        public AnimatedSprite(Texture2D sprite, Dictionary<AnimationKey, Animation> animation)
        {
            texture = sprite;
            animations = new Dictionary<AnimationKey, Animation>();

            foreach (AnimationKey key in animation.Keys)
                animations.Add(key, (Animation)animation[key].Clone());

            Velocity = Vector2.Zero;
        }

        #endregion

        #region Method Region

        public void ResetAnimation()
        {
            animations[currentAnimation].Reset();
        }

        public virtual void Update(GameTime gameTime)
        {
            if (isAnimating)
                animations[currentAnimation].Update(gameTime);

            Velocity = Velocity * Drag;
            Position = Position + Velocity; // * timescale;
            Rotation = Rotation + (RotationSpeed * gameTime.ElapsedGameTime.Milliseconds);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                Position,
                animations[currentAnimation].CurrentFrameRect,
                Color.White,
                Rotation,
                Origin,
                1f,
                SpriteEffects.None,
                0);
        }

        public void LockToMap(Point mapSize)
        {
            Position.X = MathHelper.Clamp(Position.X, 0, mapSize.X - Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, mapSize.Y - Height);
        }

        #endregion
    }
}
