using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Omega.Ai;
using Omega.Content;
using Omega.GameStates;
using Omega.GraphicsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Components
{
    public class Arena
    {
        int level;
        GameOptions gameOptions;
        Texture2D centerTexture;
        Vector2 centerPosition;
        Rectangle centerRect;

        SpriteFont font;
        Color fontColor;
        List<Rectangle> barriers;
        SoundEffect reboundSound;
        Random rnd;


        public int Player1Score;
        public int Player1Lives;

        public int Player2Score;
        public int Player2Lives;

        public Arena(Game game,  GameOptions options, int level)
        {
            this.level = level;
            this.gameOptions = options;

            rnd = new Random();

            Game1 gameRef = (Game1)game;
            this.reboundSound = gameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Rebound1);
            this.font = gameRef.ContentBank.GetSpriteFont(ContentItem.Fonts_ArenaFont);
            this.fontColor = Color.White;

            // TODO: load arena based on level?
            this.centerTexture = gameRef.ContentBank.GetTexture(ContentItem.Graphics_Arena_Center);
            this.centerPosition = new Vector2(405f, 275f);
            SetUpLevel();
        }

        public void Update(GameTime gameTime)
        {
            

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(centerTexture, centerPosition, Color.White);

            spriteBatch.DrawString(font, "Level: " + this.level.ToString("00"), new Vector2(600, 320), fontColor);

            spriteBatch.DrawString(font, "PLAYER 1", new Vector2(460, 320), fontColor);
            spriteBatch.DrawString(font, "Score: " + Player1Score.ToString("000000"), new Vector2(460, 350), fontColor);
            spriteBatch.DrawString(font, "Lives: " + this.Player1Lives.ToString("00"), new Vector2(460, 380), fontColor);

            if (this.gameOptions.GameType != GameType.SinglePlayer)
            {
                spriteBatch.DrawString(font, "PLAYER 2", new Vector2(720, 320), fontColor);
                spriteBatch.DrawString(font, "Score: " + Player2Score.ToString("000000"), new Vector2(720, 350), fontColor);
                spriteBatch.DrawString(font, "Lives: " + this.Player2Lives.ToString("00"), new Vector2(720, 380), fontColor);

            }

        }

        public Vector2 GetPlayerSpawnPosition(int player)
        {
            if (player == 2)
            {
                return new Vector2(600, 520);
            }
            return new Vector2(700, 200);
        }

        public Vector2 GetEnemySpawnPosition(List<Vector2> targets)
        {
            Vector2 pos = Vector2.Zero;

            
            bool invalid = true;

            do
            {
                pos = new Vector2(50 + rnd.Next(1100), 50 + rnd.Next(600));
                invalid = IsInsideBarriers(pos) || IsInsidePlayerSpawnAreas(pos);

                if (!invalid)
                { 
                    foreach (Vector2 targetPos in targets)
                    {
                        float dist = (float)Math.Sqrt(((targetPos.X - pos.X) * (targetPos.X - pos.X)) + ((targetPos.Y - pos.Y) * (targetPos.Y - pos.Y)));
                        if (dist < 200)
                        {
                            invalid = true;
                            break;
                        }
                    }
                }


            } while (invalid);
            
            return pos;
        }

        public bool CheckCollision(AnimatedSprite sprite, bool reboundable)
        {
            // rebound checks
            float x = sprite.Velocity.X;
            float y = sprite.Velocity.Y;
            int radius = sprite.Height / 2;
            bool collision = false;

            // check screen edge collisions
            if (sprite.Position.X < 20 + radius || sprite.Position.X > 1260 - radius)
            {
                x = -x;
                collision = true;
            }

            if (sprite.Position.Y < 20 + radius || sprite.Position.Y > 700 - radius)
            {
                y = -y;
                collision = true;
            }

            // check center box collisions
            if (HitsTopSide(sprite.Position, radius, centerRect) || HitsBottomSide(sprite.Position, radius, centerRect))
            {
                y = -y;
                collision = true;
            }

            if (HitsLeftSide(sprite.Position, radius, centerRect) || HitsRightSide(sprite.Position, radius, centerRect))
            {
                x = -x;
                collision = true;
            }


            if (collision && reboundable)
            {
                sprite.Velocity = new Vector2(x, y);
                reboundSound.Play();
            }

            if (collision && !reboundable)
            {
                sprite.Velocity = Vector2.Zero;
            }

            return collision;
        }

        public Vector2 GetSafeSpawnPosition(Player otherPlayer, List<Enemy> enemies, List<Shot> shots)
        {
            Vector2 pos = Vector2.Zero;
            bool valid = false;

            while (!valid)
            {
                pos = new Vector2(70 + rnd.Next(1100), 70 + rnd.Next(580));
                valid = !IsInsideBarriers(pos);

                if (valid)
                {
                    float dist = (float)Math.Sqrt(((otherPlayer.Position.X - pos.X) * (otherPlayer.Position.X - pos.X)) + ((otherPlayer.Position.Y - pos.Y) * (otherPlayer.Position.Y - pos.Y)));
                    if (dist < 128)
                        valid = false;
                }

                if (valid)
                {
                    foreach(Enemy enemy in enemies)
                    {
                        float dist = (float)Math.Sqrt(((enemy.Position.X - pos.X) * (enemy.Position.X - pos.X) ) + ((enemy.Position.Y - pos.Y) * (enemy.Position.Y - pos.Y)));
                        if (dist < 128)
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (valid)
                {
                    foreach (Shot shot in shots)
                    {
                        float dist = (float)Math.Sqrt(((shot.Position.X - pos.X) * (shot.Position.X - pos.X)) + ((shot.Position.Y - pos.Y) * (shot.Position.Y - pos.Y)));
                        if (dist < 200)
                        {
                            valid = false;
                            break;
                        }
                    }
                }
            }
            
            return pos;
        }

        private void SetUpLevel()
        {
            this.barriers = new List<Rectangle>();

            this.barriers.Add(new Rectangle(0, 0, 1280, 20)); // top
            this.barriers.Add(new Rectangle(0, 700, 1280, 720)); // bottom
            this.barriers.Add(new Rectangle(0, 0, 20, 720)); // left
            this.barriers.Add(new Rectangle(1260, 0, 1280, 720)); // right

            centerRect = new Rectangle((int)this.centerPosition.X, (int)this.centerPosition.Y, 480, 180);
            this.barriers.Add(centerRect); // center
        }

        private bool HitsTopSide(Vector2 position, int radius, Rectangle rect)
        {
            bool hit = false;

            if ((position.Y > rect.Y) && (position.Y < rect.Y + radius) && (position.X > rect.X) && (position.X < rect.X + rect.Width))
            {
                hit = true;
            }

            return hit;
        }

        private bool HitsBottomSide(Vector2 position, int radius, Rectangle rect)
        {
            bool hit = false;

            if ((position.Y < rect.Y + rect.Height) && (position.Y > rect.Y + rect.Height - radius) && (position.X > rect.X) && (position.X < rect.X + rect.Width))
            {
                hit = true;
            }

            return hit;
        }

        private bool HitsLeftSide(Vector2 position, int radius, Rectangle rect)
        {
            bool hit = false;

            if ((position.X > rect.X) && (position.X < rect.X + radius) && (position.Y > rect.Y) && (position.Y < rect.Y + rect.Height))
            {
                hit = true;
            }

            return hit;
        }

        private bool HitsRightSide(Vector2 position, int radius, Rectangle rect)
        {
            bool hit = false;

            if ((position.X < rect.X + rect.Width) && (position.X > rect.X + rect.Width - radius) && (position.Y > rect.Y) && (position.Y < rect.Y + rect.Height))
            {
                hit = true;
            }

            return hit;
        }


        private bool IsInsideBarriers(Vector2 position)
        {
            foreach(Rectangle rect in barriers)
            {
                if ((position.X > rect.X) && (position.Y > rect.Y) && (position.X < rect.X + rect.Width) && (position.Y < rect.Y + rect.Height))
                    return true;
            }
            return false;
        }

        private bool IsInsidePlayerSpawnAreas(Vector2 position)
        {
            for (int i = 1; i < 3; i++)
            {
                Vector2 spawnPos = GetPlayerSpawnPosition(i);

                if ((position.X > spawnPos.X) && (position.Y > spawnPos.Y) && (position.X < spawnPos.X + 80) && (position.Y < spawnPos.Y + 80))
                    return true;
            }
            return false;
        }
    }
}
