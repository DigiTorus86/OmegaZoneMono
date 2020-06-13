using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Omega.Components
{
    public class MenuComponent
    {
        #region Fields

        private static int BUTTON_MARGIN = 30;

        GamePadState player1pad;
        SpriteFont spriteFont;
        readonly List<string> menuItems = new List<string>();
        int selectedIndex = -1;
        bool mouseOver;
        int width;
        int height;
        Color normalColor = Color.Gray;
        Color hiliteColor = Color.White;

        Texture2D buttonOffTexture;
        Texture2D buttonOnTexture;

        Vector2 position;

        SoundEffect buttonChange;
        bool playbuttonChange = false;

        #endregion Fields

        #region Properties

        public Vector2 Postion
        {
            get { return position; }
            set { position = value; }
        }

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = (int)MathHelper.Clamp(value, 0, menuItems.Count - 1);
            }
        }

        public Color NormalColor
        {
            get { return normalColor; }
            set { normalColor = value; }
        }

        public Color HiliteColor
        {
            get { return hiliteColor; }
            set { hiliteColor = value; }
        }

        public bool MouseOver
        {
            get { return mouseOver; }
        }

        #endregion Properties

        #region Constructors

        public MenuComponent(SpriteFont spriteFont, Texture2D buttonOn, Texture2D buttonOff,  SoundEffect buttonChange)
        {
            this.mouseOver = false;
            this.spriteFont = spriteFont;
            this.buttonOnTexture = buttonOn;
            this.buttonOffTexture = buttonOff;
            this.buttonChange = buttonChange;
        }

        public MenuComponent(SpriteFont spriteFont, Texture2D buttonOn, Texture2D buttonOff, SoundEffect buttonChange, string[] menuItems) 
                : this(spriteFont, buttonOn, buttonOff, buttonChange)
        {
            selectedIndex = 0;
            foreach (string s in menuItems)
            {
                this.menuItems.Add(s);
            }
            MeassureMenu();
        }

        #endregion Constructors

        #region Methods

        public void SetMenuItems(string[] items)
        {
            menuItems.Clear();
            menuItems.AddRange(items);
            MeassureMenu();
            selectedIndex = 0;
        }

        private void MeassureMenu()
        {
            width = buttonOffTexture.Width;
            height = 0;
            foreach (string s in menuItems)
            {
                Vector2 size = spriteFont.MeasureString(s);
                if (size.X > width)
                    width = (int)size.X;
                height += buttonOffTexture.Height + BUTTON_MARGIN;
            }
            height -= BUTTON_MARGIN;
        }

        public void Update(GameTime gameTime, PlayerIndex index)
        {
            Vector2 menuPosition = position;
            Point p = Xin.MouseState.Position;
            Rectangle buttonRect;
            mouseOver = false;
            for (int i = 0; i < menuItems.Count; i++)
            {
                buttonRect = new Rectangle((int)menuPosition.X, (int)menuPosition.Y,
               buttonOffTexture.Width, buttonOffTexture.Height);

                if (buttonRect.Contains(p))
                {
                    selectedIndex = i;
                    mouseOver = true;
                }
                menuPosition.Y += buttonOffTexture.Height + BUTTON_MARGIN;
            }

            this.player1pad = GamePad.GetState(PlayerIndex.One);

            if (!mouseOver && (Xin.CheckKeyReleased(Keys.Up)))
            {
                playbuttonChange = true;
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = menuItems.Count - 1;
                    
            }
            else if (!mouseOver && (Xin.CheckKeyReleased(Keys.Down)))
            {
                playbuttonChange = true;
                selectedIndex++;
                if (selectedIndex > menuItems.Count - 1)
                    selectedIndex = 0;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (playbuttonChange)
            {
                buttonChange.Play();
                playbuttonChange = false;
            }

            Vector2 menuPosition = position;
            Color myColor;
            for (int i = 0; i < menuItems.Count; i++)
            {
                Vector2 textSize = spriteFont.MeasureString(menuItems[i]);
                Vector2 textPosition = menuPosition + new Vector2((int)(buttonOffTexture.Width - textSize.X) / 2, (int)(buttonOffTexture.Height - textSize.Y) / 2);

                if (i == SelectedIndex)
                {
                    myColor = HiliteColor;
                    spriteBatch.Draw(buttonOnTexture, menuPosition, Color.White);
                }
                else
                {
                    myColor = NormalColor;
                    spriteBatch.Draw(buttonOffTexture, menuPosition, Color.White);
                }
               
                spriteBatch.DrawString(spriteFont,
                menuItems[i],
                textPosition,
                myColor);
                menuPosition.Y += buttonOffTexture.Height + BUTTON_MARGIN;
            }
        }

        #endregion Methods

        #region Virtual Methods
        #endregion Virtual Methods
    }
 }

