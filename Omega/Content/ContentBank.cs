using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Omega.GraphicsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Content
{


    public class ContentBank
    {
        public delegate void ProgressUpdateHandler(object sender, ProgressEventArgs e);
        public event ProgressUpdateHandler OnUpdateProgress;

        protected ContentManager Content;

        public ContentBank(ContentManager contentManager)
        {
            this.Content = contentManager;
        }

        public SpriteFont GetSpriteFont(ContentItem contentItem)
        {
            string key = StringEnum.GetStringValue(contentItem);
            return this.Content.Load<SpriteFont>(key);
        }

        public Song GetSong(ContentItem contentItem)
        {
            string key = StringEnum.GetStringValue(contentItem);
            return this.Content.Load<Song>(key);
        }

        public SoundEffect GetSoundEffect(ContentItem contentItem)
        {
            string key = StringEnum.GetStringValue(contentItem);
            return this.Content.Load<SoundEffect>(key);
        }

        public Texture2D GetTexture(ContentItem contentItem)
        {
            string key = StringEnum.GetStringValue(contentItem);
            return this.Content.Load<Texture2D>(key);
        }

        public Dictionary<AnimationKey, Animation> GetEnemyAnimations()
        {
            Animation animation = new Animation(1, 92, 92, 0, 0);
            animation.FramesPerSecond = 12;
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();
            animations.Add(AnimationKey.Idle, animation);

            return animations;
        }

        public Dictionary<AnimationKey, Animation> GetExplosionAnimations()
        {
            Animation animation = new Animation(6, 92, 92, 0, 0);
            animation.FramesPerSecond = 12;
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();
            animations.Add(AnimationKey.Idle, animation);

            return animations;
        }


        public void LoadMenuContent()
        {
            int total = 4;
            int progress = 1;

            GetTexture(ContentItem.Graphics_Menu_Button_Off);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Menu_Button_On);
            UpdateProgress(progress++, total);

            GetSoundEffect(ContentItem.Sounds_Menu_ButtonOver);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Menu_ButtonSelect);
            UpdateProgress(progress++, total);
        }
    

        public void LoadCombatContent()
        {
            int total = 16;
            int progress = 1;

            GetSpriteFont(ContentItem.Fonts_ArenaFont);
            UpdateProgress(progress++, total);

            GetTexture(ContentItem.Graphics_Enemy_Mine);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Enemy_Drone);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Enemy_MineLayer);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Enemy_Warrior);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Enemy_Warlord);
            UpdateProgress(progress++, total);

            GetTexture(ContentItem.Graphics_Player_Ship);
            UpdateProgress(progress++, total);
            GetTexture(ContentItem.Graphics_Player_Ship2);
            UpdateProgress(progress++, total);

            GetSoundEffect(ContentItem.Sounds_Combat_Evolve);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Combat_Explode1);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Combat_Fire1);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Combat_Rebound1);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Combat_Thrust1);
            UpdateProgress(progress++, total);
            GetSoundEffect(ContentItem.Sounds_Combat_Warp);
            UpdateProgress(progress++, total);

            GetSong(ContentItem.Sounds_Song_Fight);
            UpdateProgress(progress++, total);
            GetSong(ContentItem.Sounds_Song_Level);
            UpdateProgress(progress++, total);
        }

        private void UpdateProgress(int progress, int total)
        {
            if (OnUpdateProgress == null) return;

            ProgressEventArgs args = new ProgressEventArgs(progress, total);
            OnUpdateProgress(this, args);
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; private set; }

        public int Total { get; private set; }

        public ProgressEventArgs(int progress, int total)
        {
            Progress = progress;
            Total = total;
        }
    }
}
