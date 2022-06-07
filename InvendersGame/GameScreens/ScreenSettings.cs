using System;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameScreens
{
    public class ScreenSettings : MenuItemsScreen
    {
        private const string k_ScreenSettingsHeadLine = @"Sprites\Titles\ScreenSettingsTitle";

        private SpriteMenuItem m_AllowWindowResizingSprite;
        private SpriteMenuItem m_FullScreenModeSprite;
        private SpriteMenuItem m_MouseVisabilitySprite;
        private SpriteMenuItem m_DoneSprite;

        public ScreenSettings(Game i_Game)
            : base(k_ScreenSettingsHeadLine, i_Game)
        {
            m_ActiveItem = m_AllowWindowResizingSprite = new SpriteMenuItem(@"Allow Window Resizing: ", i_Game, true, 1, getWindowCurrentSetting(), this);
            m_FullScreenModeSprite = new SpriteMenuItem(@"Full Screen Mode: ", i_Game, m_AllowWindowResizingSprite, 2, getScreenModeCurrentSetting(), this);
            m_MouseVisabilitySprite = new SpriteMenuItem(@"Mouse Visibility: ", i_Game, m_FullScreenModeSprite, 3, getMouseVisabilityCurrentSetting(), this);
            m_DoneSprite = new SpriteMenuItem(@"Done", i_Game, m_MouseVisabilitySprite, 4, null, this);

            m_DoneSprite.NextItem = m_AllowWindowResizingSprite;
            m_AllowWindowResizingSprite.PreviouseItem = m_DoneSprite;
        }

        private string getWindowCurrentSetting()
        {
            string windowCurrentSetting = k_OffMsg;
            if (Game.Window.AllowUserResizing)
            {
                windowCurrentSetting = k_OnMsg;
            }

            return windowCurrentSetting;
        }

        private string getScreenModeCurrentSetting()
        {
            string screenModeCurrentSetting = k_OffMsg;
            if ((Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager).IsFullScreen)
            {
                screenModeCurrentSetting = k_OnMsg;
            }

            return screenModeCurrentSetting;
        }

        private string getMouseVisabilityCurrentSetting()
        {
            string mouseCurrentSetting = k_VissibleMsg;
            if (!Game.IsMouseVisible)
            {
                mouseCurrentSetting = k_InvisibleMsg;
            }

            return mouseCurrentSetting;
        }

        public override void setDelegates()
        {
            base.setDelegates();

            m_AllowWindowResizingSprite.PgUpPressedOnItem += AllowWindowResizingSprite_PgUpPressedOnItem;
            m_AllowWindowResizingSprite.PgDownPressedOnItem += AllowWindowResizingSprite_PgDownPressedOnItem;

            m_FullScreenModeSprite.PgUpPressedOnItem += FullScreenModeSprite_PgUpPressedOnItem;
            m_FullScreenModeSprite.PgDownPressedOnItem += FullScreenModeSprite_PgDownPressedOnItem;

            m_MouseVisabilitySprite.PgUpPressedOnItem += MouseVisabilitySprite_PgUpPressedOnItem;
            m_MouseVisabilitySprite.PgDownPressedOnItem += MouseVisabilitySprite_PgDownPressedOnItem;

            m_DoneSprite.EnterPressedOnItem += DoneSprite_EnterPressedOnItem;
        }

        private void AllowWindowResizingSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelWindowResizingPgPress();
        }

        private void AllowWindowResizingSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelWindowResizingPgPress();
        }

        private void handelWindowResizingPgPress()
        {
            Game.Window.AllowUserResizing = !Game.Window.AllowUserResizing;
            m_AllowWindowResizingSprite.ReplaceableText = getWindowCurrentSetting();
        }

        private void FullScreenModeSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelScreenModePgPress();
        }

        private void FullScreenModeSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelScreenModePgPress();
        }

        private void handelScreenModePgPress()
        {
            (Game.Services.GetService(typeof(GraphicsDeviceManager)) as GraphicsDeviceManager).ToggleFullScreen();
            m_FullScreenModeSprite.ReplaceableText = getScreenModeCurrentSetting();
        }

        private void MouseVisabilitySprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelMouseVisabilitPgPress();
        }

        private void MouseVisabilitySprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelMouseVisabilitPgPress();
        }

        private void handelMouseVisabilitPgPress()
        {
            Game.IsMouseVisible = !Game.IsMouseVisible;
            m_MouseVisabilitySprite.ReplaceableText = getMouseVisabilityCurrentSetting();
        }

        private void DoneSprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            ExitScreen();
        }
    }
}