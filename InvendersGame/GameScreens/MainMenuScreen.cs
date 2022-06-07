using System;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using InvandersGame.GameObjects;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameScreens
{
    public class MainMenuScreen : MenuItemsScreen
    {
        private const string k_MainMenuHeadLine = @"Sprites\Titles\MainMenuTitle";

        private readonly ScreenSettings r_ScreenSettings;
        private readonly SoundSettingsScreen r_SoundSettingsScreen;

        private SpriteMenuItem m_ScreenSettingsSprite;
        private SpriteMenuItem m_PlayersSprite;
        private SpriteMenuItem m_SoundSettingsSprite;
        private SpriteMenuItem m_PlaySprite;
        private SpriteMenuItem m_QuitSprite;

        public MainMenuScreen(Game i_Game)
            : base(k_MainMenuHeadLine, i_Game)
        {
            r_ScreenSettings = new ScreenSettings(Game);
            r_SoundSettingsScreen = new SoundSettingsScreen(Game);

            m_ActiveItem = m_ScreenSettingsSprite = new SpriteMenuItem(@"Screen Settings", i_Game, true, 1, null, this);
            m_PlayersSprite = new SpriteMenuItem(@"Players:", i_Game, m_ScreenSettingsSprite, 2, getPlayersCurrentSetting(), this);
            m_SoundSettingsSprite = new SpriteMenuItem(@"Sound Settings", i_Game, m_PlayersSprite, 3, null, this);
            m_PlaySprite = new SpriteMenuItem(@"Play", i_Game, m_SoundSettingsSprite, 4, null, this);
            m_QuitSprite = new SpriteMenuItem(@"Quit", i_Game, m_PlaySprite, 5, null, this);

            m_QuitSprite.NextItem = m_ScreenSettingsSprite;
            m_ScreenSettingsSprite.PreviouseItem = m_QuitSprite;
        }

        private string getPlayersCurrentSetting()
        {
            string playersCurrentSetting = k_OneMsg;
            if (m_GameManager.NumOfPlayers == 2)
            {
                playersCurrentSetting = k_TwoMsg;
            }

            return playersCurrentSetting;
        }

        public override void setDelegates()
        {
            base.setDelegates();

            m_ScreenSettingsSprite.EnterPressedOnItem += ScreenSettingsSprite_EnterPressedOnItem;
            m_PlayersSprite.PgUpPressedOnItem += PlayersSprite_PgUpPressedOnItem;
            m_PlayersSprite.PgDownPressedOnItem += PlayersSprite_PgDownPressedOnItem;
            m_SoundSettingsSprite.EnterPressedOnItem += SoundSettingsSprite_EnterPressedOnItem;
            m_PlaySprite.EnterPressedOnItem += PlaySprite_EnterPressedOnItem;
            m_QuitSprite.EnterPressedOnItem += QuitSprite_EnterPressedOnItem;
        }

        private void ScreenSettingsSprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            ScreensManager.SetCurrentScreen(r_ScreenSettings);
        }

        private void PlayersSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelPlayersSpritePgPress();
        }

        private void PlayersSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelPlayersSpritePgPress();
        }

        private void handelPlayersSpritePgPress()
        {
            m_GameManager.NumOfPlayers = (m_GameManager.NumOfPlayers % 2) + 1;
            m_PlayersSprite.ReplaceableText = getPlayersCurrentSetting();
        }

        private void SoundSettingsSprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            ScreensManager.SetCurrentScreen(r_SoundSettingsScreen);
        }

        private void PlaySprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            ExitScreen();
            ScreensManager.SetCurrentScreen(new PlayScreen(Game));
        }

        private void QuitSprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            Game.Exit();
        }
    }
}