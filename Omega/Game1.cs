/* Omega Zone classic vector arcade shooter game
 * 
 * Requires Monogame  https://www.monogame.net/
 *
 * Copyright (c) 2018 Paul Pagel
 * https://twobittinker.com 
 * 
 * This is free software; see the license.txt file for more information.
 * There is no warranty; not even for merchantability or fitness for a particular purpose.
 */

﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Omega.Components;
using Omega.Content;
using Omega.GameStates;
using Omega.GraphicsEngine;
using Omega.StateManager;
using System.Collections.Generic;

namespace Omega
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public ContentBank ContentBank;
        public GameOptions Options;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dictionary<AnimationKey, Animation> playerAnimations = new Dictionary<AnimationKey, Animation>();

        GameStateManager gameStateManager;

        ITitleIntroState titleIntroState;
        IMainMenuState startMenuState;
        IGamePlayState gamePlayState;

        static Rectangle screenRectangle;

        Texture2D title;
        SpriteFont font;
        
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public static Rectangle ScreenRectangle
        {
            get { return screenRectangle; }
        }

        public GameStateManager GameStateManager
        {
            get { return gameStateManager; }
        }
        public ITitleIntroState TitleIntroState
        {
            get { return titleIntroState; }
        }

        public IMainMenuState StartMenuState
        {
            get { return startMenuState; }
        }

        public IGamePlayState GamePlayState
        {
            get { return gamePlayState; }
        }

        public Dictionary<AnimationKey, Animation> PlayerAnimations
        {
            get { return playerAnimations; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            screenRectangle = new Rectangle(0, 0, 1280, 720);
            graphics.PreferredBackBufferWidth = ScreenRectangle.Width;
            graphics.PreferredBackBufferHeight = ScreenRectangle.Height;

            gameStateManager = new GameStateManager(this);
            Components.Add(gameStateManager);

            Options = new GameOptions();
            Options.GameType = GameType.TwoPlayerCooperative; // TODO: set this on the menu screen
           
            titleIntroState = new TitleIntroState(this);
            startMenuState = new MainMenuState(this);
            

            gameStateManager.ChangeState((TitleIntroState)titleIntroState, PlayerIndex.One);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Components.Add(new Xin(this));

            Animation animation = new Animation(1, 64, 64, 0, 0);
            playerAnimations.Add(AnimationKey.Idle, animation);

            animation = new Animation(2, 64, 64, 64, 0);
            playerAnimations.Add(AnimationKey.Thrusting, animation);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            title = this.Content.Load<Texture2D>("Graphics/OmegaZone-title");
            font = this.Content.Load<SpriteFont>("Fonts/TitleFont");

            this.ContentBank = new ContentBank(this.Content);
            this.ContentBank.LoadCombatContent();
            gamePlayState = new GamePlayState(this);

            // for testing only!!!
            //GameOptions options = new GameOptions();
            //options.GameType = GameType.SinglePlayer;
            //gamePlayState.SetUpNewGame(options);
            //gamePlayState.StartGame();
            //gameStateManager.ChangeState((GamePlayState)gamePlayState, PlayerIndex.One);  // TODO: remove later
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
