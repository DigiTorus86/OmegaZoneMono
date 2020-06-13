using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega.Content
{
    public enum ContentItem
    {
        [StringValue("Fonts/TitleFont")]
        Fonts_TitleFont,

        [StringValue("Fonts/ArenaFont")]
        Fonts_ArenaFont,

        [StringValue("Graphics/Arena/arena-center")]
        Graphics_Arena_Center,

        [StringValue("Graphics/Enemy/mine1")]
        Graphics_Enemy_Mine,

        [StringValue("Graphics/Enemy/enemy1")]
        Graphics_Enemy_Drone,

        [StringValue("Graphics/Enemy/enemy2")]
        Graphics_Enemy_MineLayer,

        [StringValue("Graphics/Enemy/enemy3")]
        Graphics_Enemy_Warrior,

        [StringValue("Graphics/Enemy/enemy4")]
        Graphics_Enemy_Warlord,

        [StringValue("Graphics/Enemy/explode92x6")]
        Graphics_Enemy_Explode,


        [StringValue("Graphics/Menu/menu-button2-off")]
        Graphics_Menu_Button_Off,

        [StringValue("Graphics/Menu/menu-button2-on")]
        Graphics_Menu_Button_On,


        [StringValue("Graphics/Player/player-ship64x3")]
        Graphics_Player_Ship,

        [StringValue("Graphics/Player/player2-ship64x3")]
        Graphics_Player_Ship2,

        [StringValue("Graphics/Player/shot1")]
        Graphics_Player_Shot,


        [StringValue("Graphics/Title/OmegaZone-title")]
        Graphics_Title_Splash,


        [StringValue("Sounds/Combat/evolve")]
        Sounds_Combat_Evolve,

        [StringValue("Sounds/Combat/explode1x")]
        Sounds_Combat_Explode1,

        [StringValue("Sounds/Combat/fire1x")]
        Sounds_Combat_Fire1,

        [StringValue("Sounds/Combat/jump1")]
        Sounds_Combat_Jump,

        [StringValue("Sounds/Combat/rebound1")]
        Sounds_Combat_Rebound1,

        [StringValue("Sounds/Combat/thrust1x")]
        Sounds_Combat_Thrust1,

        [StringValue("Sounds/Combat/warp1")]
        Sounds_Combat_Warp,


        [StringValue("Sounds/Menu/bong1")]
        Sounds_Menu_ButtonSelect,

        [StringValue("Sounds/Menu/plip1")]
        Sounds_Menu_ButtonOver,


        [StringValue("Sounds/Songs/OmegaZone-fight")]
        Sounds_Song_Fight,

        [StringValue("Sounds/Songs/OmegaZone-level")]
        Sounds_Song_Level,

            [StringValue("Sounds/Songs/OmegaZone-level")]
        Sounds_Song_Title
    }
}
