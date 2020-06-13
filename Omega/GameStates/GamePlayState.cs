using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Omega.Ai;
using Omega.Components;
using Omega.Content;
using Omega.GraphicsEngine;
using System;
using System.Collections.Generic;

namespace Omega.GameStates
{
    public class GamePlayState : BaseGameState, IGamePlayState
    {
        GameOptions gameOptions = new GameOptions();
        Arena arena;

        Player player1;
        List<Shot> player1shots;
        GamePadState player1pad;

        Player player2;
        List<Shot> player2shots;
        GamePadState player2pad;

        List<Vector2> targets;
        Texture2D shotTexture;

        SoundEffect thrustSound;
        SoundEffect shotFireSound;
        Song song;


        EnemyListController enemyListController;
        List<Enemy> enemies;
        Texture2D enemyExplodeTexture;
        SoundEffect enemyExplodeSound;
        bool contentLoaded = false;

        Dictionary<AnimationKey, Animation> shotAnimations;

        int level = 1;
 
        public GamePlayState(Game game) : base(game)
        {
            player1shots = new List<Shot>();
            player2shots = new List<Shot>();
            enemies = new List<Enemy>();
           
            shotAnimations = new Dictionary<AnimationKey, Animation>();
            Animation animation = new Animation(1, 8, 8, 0, 0);
            shotAnimations.Add(AnimationKey.Idle, animation);

            targets = new List<Vector2>();
            enemyListController = new EnemyListController(game, null, enemies); 

            game.Services.AddService(typeof(IGamePlayState), this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //shots = new List<Shot>();
            //shotAnimations = new Dictionary<AnimationKey, Animation>();
            //Animation animation = new Animation(1, 8, 8, 0, 0);
            //shotAnimations.Add(AnimationKey.Idle, animation);
            
            contentLoaded = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!contentLoaded)
                LoadContent();

            if (player1.Status == PlayerStatus.GameOver && (player2.Status == PlayerStatus.GameOver || player2.Status == PlayerStatus.Disabled))
            {
                SetUpNewGame(this.gameOptions);
                StartGame();
                return;
            }
            
            if (enemies.Count == 0)
            {
                level++;
                StartLevel();
                return;
            }

            this.player1pad = GamePad.GetState(PlayerIndex.One);
            this.player2pad = GamePad.GetState(PlayerIndex.Two);
            targets.Clear();

            if (Xin.CheckKeyReleased(Keys.M)) // MUTE/MUSIC
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
                else
                {
                    MediaPlayer.Resume();
                }
            }

            if (Xin.CheckKeyReleased(Keys.R)) // RESTART
            {
                SetUpNewGame(this.gameOptions);
            }


            if (player1.Status == PlayerStatus.Resurrecting)
            {
                player1.Sprite.Position = arena.GetSafeSpawnPosition(player2, enemies, enemyListController.shots);
            }

            if (player2.Status == PlayerStatus.Resurrecting)
            {
                player2.Sprite.Position = arena.GetSafeSpawnPosition(player1, enemies, enemyListController.shots);
            }

            //--- Player 1 ---//

            if (player1.Status == PlayerStatus.Active)
            {
                player1.Sprite.CurrentAnimation = AnimationKey.Idle;

                if (player1.shotReloadTimeRemaining > 0)
                    player1.shotReloadTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                if (Xin.KeyboardState.IsKeyDown(Keys.Left) || this.player1pad.ThumbSticks.Left.X < -0.2f) // rotate counterclockwise
                {
                    player1.Sprite.Rotation -= 0.1f;
                }
                else if (Xin.KeyboardState.IsKeyDown(Keys.Right) || this.player1pad.ThumbSticks.Left.X > 0.2f) // rotate clockwise
                {
                    player1.Sprite.Rotation += 0.1f;
                }

                if (Xin.KeyboardState.IsKeyDown(Keys.Up) || this.player1pad.Triggers.Left > 0.2f) // thrust
                {
                    float xSpeed = player1.Sprite.Velocity.X + (float)Math.Sin(player1.Sprite.Rotation) / 8;
                    float ySpeed = player1.Sprite.Velocity.Y - (float)Math.Cos(player1.Sprite.Rotation) / 8;

                    if (Math.Abs(xSpeed) > player1.MaxSpeed)
                        xSpeed = player1.MaxSpeed * Math.Sign(xSpeed);

                    if (Math.Abs(ySpeed) > player1.MaxSpeed)
                        ySpeed = player1.MaxSpeed * Math.Sign(ySpeed);

                    player1.Sprite.Velocity = new Vector2(xSpeed, ySpeed);
                    player1.Sprite.CurrentAnimation = AnimationKey.Thrusting;
                    thrustSound.Play();
                }

                if (Xin.KeyboardState.IsKeyDown(Keys.RightShift) || this.player1pad.Triggers.Right > 0.2f) // fire!
                {
                    if (player1.shotReloadTimeRemaining <= 0)
                    {
                        // determine shot location at the nose of the ship
                        Vector2 position = new Vector2(player1.Sprite.Position.X + (float)Math.Cos(player1.Sprite.Rotation - MathHelper.PiOver2) * 30,
                                                       player1.Sprite.Position.Y + (float)Math.Sin(player1.Sprite.Rotation - MathHelper.PiOver2) * 30);
                        Vector2 velocity = new Vector2((float)Math.Sin(player1.Sprite.Rotation) * 10, (float)Math.Cos(player1.Sprite.Rotation) * -10);

                        Shot shot = new Shot(Game, position, velocity, 800, shotTexture, shotAnimations);
                        player1shots.Add(shot);
                        shotFireSound.Play();
                        player1.shotReloadTimeRemaining = player1.shotReloadTime;
                    }
                }

                targets.Add(player1.Position);
            }

            //--- Player 2 ---//

            if (player2.Status == PlayerStatus.Active)
            {
                player2.Sprite.CurrentAnimation = AnimationKey.Idle;

                if (player2.shotReloadTimeRemaining > 0)
                    player2.shotReloadTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                if (Xin.KeyboardState.IsKeyDown(Keys.A) || this.player2pad.ThumbSticks.Left.X < -0.2f) // rotate counterclockwise
                {
                    player2.Sprite.Rotation -= 0.1f;
                }
                else if (Xin.KeyboardState.IsKeyDown(Keys.D) || this.player2pad.ThumbSticks.Left.X > 0.2f) // rotate clockwise
                {
                    player2.Sprite.Rotation += 0.1f;
                }

                if (Xin.KeyboardState.IsKeyDown(Keys.W) || this.player2pad.Triggers.Left > 0.2f) // thrust
                {
                    float xSpeed = player2.Sprite.Velocity.X + (float)Math.Sin(player2.Sprite.Rotation) / 8;
                    float ySpeed = player2.Sprite.Velocity.Y - (float)Math.Cos(player2.Sprite.Rotation) / 8;

                    if (Math.Abs(xSpeed) > player2.MaxSpeed)
                        xSpeed = player2.MaxSpeed * Math.Sign(xSpeed);

                    if (Math.Abs(ySpeed) > player2.MaxSpeed)
                        ySpeed = player2.MaxSpeed * Math.Sign(ySpeed);

                    player2.Sprite.Velocity = new Vector2(xSpeed, ySpeed);
                    player2.Sprite.CurrentAnimation = AnimationKey.Thrusting;
                    thrustSound.Play();
                }

                if (Xin.KeyboardState.IsKeyDown(Keys.LeftShift) || this.player2pad.Triggers.Right > 0.2f) // fire!
                {
                    if (player2.shotReloadTimeRemaining <= 0)
                    {
                        // determine shot location at the nose of the ship
                        Vector2 position = new Vector2(player2.Sprite.Position.X + (float)Math.Cos(player2.Sprite.Rotation - MathHelper.PiOver2) * 30,
                                                       player2.Sprite.Position.Y + (float)Math.Sin(player2.Sprite.Rotation - MathHelper.PiOver2) * 30);
                        Vector2 velocity = new Vector2((float)Math.Sin(player2.Sprite.Rotation) * 10, (float)Math.Cos(player2.Sprite.Rotation) * -10);

                        Shot shot = new Shot(Game, position, velocity, 800, shotTexture, shotAnimations);
                        player2shots.Add(shot);
                        shotFireSound.Play();
                        player2.shotReloadTimeRemaining = player2.shotReloadTime;
                    }
                }

                targets.Add(player2.Position);
            }


            // check for collisions with arena walls and obstacles
            arena.CheckCollision(player1.Sprite, true);
            if (player2.Status == PlayerStatus.Active)
                arena.CheckCollision(player2.Sprite, true);

            enemies.ForEach(en => arena.CheckCollision(en.Sprite, true));

            // check to see if the enemy got hit
            foreach (Shot shot in player1shots)
            {
                foreach(Enemy enemy in enemies)
                {
                    if (shot.CheckCollision(enemy))
                    {
                        player1.AddToScore(enemy.PointValue);
                    }
                }
            }

            foreach (Shot shot in player2shots)
            {
                foreach (Enemy enemy in enemies)
                {
                    if (shot.CheckCollision(enemy))
                    {
                        player2.AddToScore(enemy.PointValue);
                    }
                }
            }

            // check to see if the player got hit
            foreach (Shot shot in enemyListController.shots)
            {
                if (shot.CheckCollision(player1))
                {
                    player1.Status = PlayerStatus.Hit;
                }
                if ((player2.Status == PlayerStatus.Active) && (shot.CheckCollision(player2)))
                {
                    player2.Status = PlayerStatus.Hit;
                }
            }

            if (this.gameOptions.GameType == GameType.TwoPlayerDeathmatch)
            {
                // see if the players hit each other
                foreach (Shot shot in player1shots)
                {
                    if (shot.CheckCollision(player2))
                    {
                        player2.Status = PlayerStatus.Hit;
                    }
                }
                foreach (Shot shot in player2shots)
                {
                    if (shot.CheckCollision(player1))
                    {
                        player1.Status = PlayerStatus.Hit;
                        
                    }
                }
            }

            foreach (Enemy enemy in enemies)
            {
                if (player1.Status == PlayerStatus.Active && enemy.CheckCollision(player1.Sprite))
                {
                    player1.Status = PlayerStatus.Hit;
                    enemy.Status = EnemyStatus.Hit;
                }
                if ((player2.Status == PlayerStatus.Active) && (enemy.CheckCollision(player2.Sprite)))
                {
                    player2.Status = PlayerStatus.Hit;
                    enemy.Status = EnemyStatus.Hit;
                }
            }

            enemyListController.shots.RemoveAll(shot => arena.CheckCollision(shot.Sprite, false));
            enemyListController.shots.RemoveAll(shot => shot.Range <= 0);

            player1shots.RemoveAll(sh => arena.CheckCollision(sh.Sprite, false));
            player1shots.RemoveAll(sh => sh.Range <= 0);
            player2shots.RemoveAll(sh => arena.CheckCollision(sh.Sprite, false));
            player2shots.RemoveAll(sh => sh.Range <= 0);

            enemies.RemoveAll(en => en.Status == EnemyStatus.Dead);

            // update the game objects
            player1shots.ForEach(sh => sh.Update(gameTime));
            player2shots.ForEach(sh => sh.Update(gameTime));
            enemyListController.shots.ForEach(shot => shot.Update(gameTime));

            enemyListController.Update(gameTime, targets);
            enemies.ForEach(en => en.Update(gameTime));

            if ((player1.Status != PlayerStatus.Disabled) && (player1.Status != PlayerStatus.Dead))
                player1.Update(gameTime);

            if ((player2.Status != PlayerStatus.Disabled) && (player2.Status != PlayerStatus.Dead))
                player2.Update(gameTime);

            arena.Player1Score = player1.Score;
            arena.Player1Lives = player1.Lives;

            arena.Player2Lives = player2.Score;
            arena.Player2Lives = player2.Lives;

            //base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            arena.Draw(gameTime, GameRef.SpriteBatch);

            player1shots.ForEach(sh => sh.Draw(gameTime));
            player2shots.ForEach(sh => sh.Draw(gameTime));

            enemies.ForEach(en => en.Draw(gameTime));
            enemyListController.shots.ForEach(shot => shot.Draw(gameTime));

            if ((player1.Status != PlayerStatus.Disabled) && (player1.Status != PlayerStatus.Dead))
                player1.Draw(gameTime);

            if ((player2.Status != PlayerStatus.Disabled) && (player2.Status != PlayerStatus.Dead))
                player2.Draw(gameTime);

            GameRef.SpriteBatch.End();

            //base.Draw(gameTime);
        }

        public void SetUpNewGame(GameOptions options)
        {
            this.gameOptions = options;
            this.level = 1;

            this.player1 = new Player(Game, 1, Difficulty.Normal);
            this.player1.Position = new Vector2(700, 200);
            targets.Add(player1.Position);
            player1.Status = PlayerStatus.Active;

            this.player2 = new Player(Game, 1, Difficulty.Normal);
            if (options.GameType == GameType.SinglePlayer)
            {
                this.player2.Status = PlayerStatus.Disabled;
            }
            else
            {
                player2.Status = PlayerStatus.Active;
                this.player2.Position = new Vector2(600, 520);
                targets.Add(player2.Position);
            }

        }

        public void LoadExistingGame()
        {
            // TODO: some day...
        }

        public void StartGame()
        {
            this.player1pad = GamePad.GetState(PlayerIndex.One);
            this.player2pad = GamePad.GetState(PlayerIndex.Two);

            shotTexture = GameRef.ContentBank.GetTexture(ContentItem.Graphics_Player_Shot);
            enemyExplodeTexture = GameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Explode);
            shotFireSound = GameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Fire1);
            thrustSound = GameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Thrust1);
            enemyExplodeSound = GameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Combat_Explode1);

            song = GameRef.ContentBank.GetSong(ContentItem.Sounds_Song_Fight);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(song);
            MediaPlayer.Pause();

            StartLevel();
        }

        public void StartLevel()
        {
            this.arena = new Arena(Game, this.gameOptions, this.level);
            this.enemies = new List<Enemy>();

            // randomized enemy data for testing
            // TODO: create defined enemy types, quantities, and locations
            Vector2 startPos = arena.GetEnemySpawnPosition(targets);
            for (int i = 0; i < 3 + level; i++)
            {
                startPos = arena.GetEnemySpawnPosition(targets);
                Enemy drone = new Enemy(Game, EnemyType.Drone, startPos, GameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Drone), enemyExplodeTexture, enemyExplodeSound);
                drone.Status = EnemyStatus.Active;
                enemies.Add(drone);
            }

            for (int i = 0; i < 1 + level; i++)
            {
                startPos = arena.GetEnemySpawnPosition(targets);
                Enemy minelayer = new Enemy(Game, EnemyType.MineLayer, startPos, GameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_MineLayer), enemyExplodeTexture, enemyExplodeSound);
                minelayer.Status = EnemyStatus.Active;
                enemies.Add(minelayer);
            }

            for (int i = 0; i < level; i++)
            {
                startPos = arena.GetEnemySpawnPosition(targets);
                Enemy warrior = new Enemy(Game, EnemyType.Warrior, startPos, GameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Warrior), enemyExplodeTexture, enemyExplodeSound);
                warrior.Status = EnemyStatus.Active;
                enemies.Add(warrior);
            }

            
            for (int i = 0; i < level - 2; i++)
            {
                startPos = arena.GetEnemySpawnPosition(targets);
                Enemy warlord = new Enemy(Game, EnemyType.Warlord, startPos, GameRef.ContentBank.GetTexture(ContentItem.Graphics_Enemy_Warlord), enemyExplodeTexture, enemyExplodeSound);
                warlord.Status = EnemyStatus.Active;
                enemies.Add(warlord);
            }
            enemyListController.enemies = enemies;

        }


    }
}