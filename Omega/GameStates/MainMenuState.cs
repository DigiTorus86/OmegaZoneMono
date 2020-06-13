using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omega.Components;
using Omega.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.GameStates
{
    public class MainMenuState : BaseGameState, IMainMenuState
    {
        #region Field Region

        GameOptions gameOptions;
        Texture2D background;
        SpriteFont spriteFont;
        MenuComponent menuComponent;
        TimeSpan elapsed;
        SoundEffect buttonSelect;
        GamePadState player1pad;

        #endregion

        #region Property Region
        #endregion

        #region Constructor Region

        public MainMenuState(Game game) : base(game)
        {
            this.gameOptions = new GameOptions();
            this.player1pad = GamePad.GetState(PlayerIndex.One);
            game.Services.AddService(typeof(IMainMenuState), this);
        }

        #endregion

        #region Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteFont = GameRef.ContentBank.GetSpriteFont(ContentItem.Fonts_TitleFont);
            //background = Game.Content.Load<Texture2D>(@"Graphics\starfield"); 
            background = new Texture2D(Game.GraphicsDevice, 1, 1);

            Texture2D buttonOff = GameRef.ContentBank.GetTexture(ContentItem.Graphics_Menu_Button_Off);
            Texture2D buttonOn = GameRef.ContentBank.GetTexture(ContentItem.Graphics_Menu_Button_On);

            string[] menuItems = { "NEW 1 PLAYER GAME", "NEW 2 PLAYER GAME", "NEW 2 PLAYER DEATHMATCH", "EXIT" };

            SoundEffect buttonOver = GameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Menu_ButtonOver);
            buttonSelect = GameRef.ContentBank.GetSoundEffect(ContentItem.Sounds_Menu_ButtonSelect);

            menuComponent = new MenuComponent(spriteFont, buttonOn, buttonOff, buttonOver, menuItems);
            Vector2 position = new Vector2();
            position.Y = 90;
            position.X = 640 - menuComponent.Width / 2;
            menuComponent.Postion = position;
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerIndex index = PlayerIndex.One; // TODO
            elapsed += gameTime.ElapsedGameTime;

            menuComponent.Update(gameTime, index);

            this.player1pad = GamePad.GetState(PlayerIndex.One);

            if (Xin.CheckKeyReleased(Keys.Space) || Xin.CheckKeyReleased(Keys.Enter) || (menuComponent.MouseOver && Xin.CheckMouseReleased(MouseButtons.Left)) || this.player1pad.IsButtonDown(Buttons.A))
            {
                buttonSelect.Play();

                if (menuComponent.SelectedIndex == 0) // new single player game
                {
                    Xin.FlushInput();

                    this.gameOptions.GameType = GameType.SinglePlayer;
                    GameRef.GamePlayState.SetUpNewGame(this.gameOptions);
                    GameRef.GamePlayState.StartGame();
                    manager.PushState((GamePlayState)GameRef.GamePlayState, PlayerIndexInControl);
                }
                else if (menuComponent.SelectedIndex == 1) // new 2 player cooperative game
                {
                    Xin.FlushInput();

                    this.gameOptions.GameType = GameType.TwoPlayerCooperative;
                    GameRef.GamePlayState.SetUpNewGame(this.gameOptions);
                    GameRef.GamePlayState.StartGame();
                    manager.PushState((GamePlayState)GameRef.GamePlayState, PlayerIndexInControl);
                }
                else if (menuComponent.SelectedIndex == 2) // new 2 player deathmatch game
                {
                    Xin.FlushInput();

                    this.gameOptions.GameType = GameType.TwoPlayerDeathmatch;
                    GameRef.GamePlayState.SetUpNewGame(this.gameOptions);
                    GameRef.GamePlayState.StartGame();
                    manager.PushState((GamePlayState)GameRef.GamePlayState, PlayerIndexInControl);
                }
                else if (menuComponent.SelectedIndex == 3)
                {
                    Game.Exit();
                }
            }
                base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin();
            GameRef.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            GameRef.SpriteBatch.End();

            base.Draw(gameTime);

            GameRef.SpriteBatch.Begin();
            menuComponent.Draw(gameTime, GameRef.SpriteBatch);
            GameRef.SpriteBatch.End();
        }

        #endregion

    }
}
