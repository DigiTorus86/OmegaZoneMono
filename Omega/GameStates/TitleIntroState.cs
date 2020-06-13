using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Omega.Components;
using Omega.Content;
using System;

namespace Omega.GameStates
{
    public class TitleIntroState : BaseGameState, ITitleIntroState
    {
        #region Field Region

        Texture2D background;
        Rectangle backgroundDestination;
        SpriteFont font;
        TimeSpan elapsed;
        Vector2 position;
        string message;
        bool doneOnce = false;

        #endregion

        #region Constructor Region

        public TitleIntroState(Game game) : base(game)
        {
            game.Services.AddService(typeof(ITitleIntroState), this);
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            backgroundDestination = Game1.ScreenRectangle;
            elapsed = TimeSpan.Zero;
            message = "PRESS SPACE TO CONTINUE";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // don't use ContentBank here because it hasn't been loaded yet...
            background = Game.Content.Load<Texture2D>("Graphics/Title/OmegaZone-title");
            font = Game.Content.Load<SpriteFont>("Fonts/TitleFont");

            Vector2 size = font.MeasureString(message);
            position = new Vector2((Game1.ScreenRectangle.Width - size.X) / 2,
            Game1.ScreenRectangle.Bottom - 50 - font.LineSpacing);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.doneOnce)
            {
                Song song = GameRef.ContentBank.GetSong(Content.ContentItem.Sounds_Song_Level);
                MediaPlayer.Volume = 0.8f;
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Play(song);
                this.doneOnce = true;
            }

            PlayerIndex? index = null;
            elapsed += gameTime.ElapsedGameTime;

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter) || Xin.CheckMouseReleased(MouseButtons.Left))
            {
                manager.ChangeState((MainMenuState)GameRef.StartMenuState, index);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();

            GameRef.SpriteBatch.Draw(background, backgroundDestination, Color.White);
            Color color = new Color(1f, 1f, 1f) * (float)Math.Abs(Math.Sin(elapsed.TotalSeconds * 2));
            GameRef.SpriteBatch.DrawString(font, message, position, color);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
        
        #endregion
    }



}
