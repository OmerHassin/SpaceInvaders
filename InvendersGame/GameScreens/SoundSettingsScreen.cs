using System;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameScreens
{
    public class SoundSettingsScreen : MenuItemsScreen
    {
        private const string k_ScreenSettingsHeadLine = @"Sprites\Titles\SoundSettingTitle";
        private const float k_VoluemJump = 10;

        private SpriteMenuItem m_ToggleSoundSprite;
        private SpriteMenuItem m_BackgroundMusicVolumeSprite;
        private SpriteMenuItem m_SoundsEffectsVolumeSprite;
        private SpriteMenuItem m_DoneSprite;
        private ISoundManager m_SoundManager;

        public SoundSettingsScreen(Game i_Game)
            : base(k_ScreenSettingsHeadLine, i_Game)
        {
            m_SoundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            m_SoundManager.MuteToggelChanged += M_SoundManager_MuteToggelChanged;

            m_ActiveItem = m_ToggleSoundSprite = new SpriteMenuItem(@"Toggle Sound: ", i_Game, true, 1, getSoundCurrentSetting(), this);
            m_BackgroundMusicVolumeSprite = new SpriteMenuItem(@"Background Music Volume: ", i_Game, m_ToggleSoundSprite, 2, getBackgroundMusicVolume(), this);
            m_SoundsEffectsVolumeSprite = new SpriteMenuItem(@"Sounds Effects Volume: ", i_Game, m_BackgroundMusicVolumeSprite, 3, getEffectsVolume(), this);
            m_DoneSprite = new SpriteMenuItem(@"Done", i_Game, m_SoundsEffectsVolumeSprite, 4, null, this);
            m_DoneSprite.NextItem = m_ToggleSoundSprite;
            m_ToggleSoundSprite.PreviouseItem = m_DoneSprite;
        }

        private string getSoundCurrentSetting()
        {
            string SoundCurrentSetting = k_OnMsg;
            if (m_SoundManager.Mute)
            {
                SoundCurrentSetting = k_OffMsg;
            }

            return SoundCurrentSetting;
        }

        private string getBackgroundMusicVolume()
        {
            return m_SoundManager.BackgroundVolume.ToString();
        }

        private string getEffectsVolume()
        {
            return m_SoundManager.EffectsVolume.ToString();
        }

        public override void setDelegates()
        {
            base.setDelegates();

            m_ToggleSoundSprite.PgUpPressedOnItem += ToggleSoundSprite_PgUpPressedOnItem;
            m_ToggleSoundSprite.PgDownPressedOnItem += ToggleSoundSprite_PgDownPressedOnItem;

            m_BackgroundMusicVolumeSprite.PgUpPressedOnItem += BackgroundMusicVolumeSprite_PgUpPressedOnItem;
            m_BackgroundMusicVolumeSprite.PgDownPressedOnItem += BackgroundMusicVolumeSprite_PgDownPressedOnItem;

            m_SoundsEffectsVolumeSprite.PgUpPressedOnItem += SoundsEffectsVolumeSprite_PgUpPressedOnItem;
            m_SoundsEffectsVolumeSprite.PgDownPressedOnItem += SoundsEffectsVolumeSprite_PgDownPressedOnItem;

            m_DoneSprite.EnterPressedOnItem += DoneSprite_EnterPressedOnItem;
        }

        private void M_SoundManager_MuteToggelChanged(object sender, EventArgs e)
        {
            m_ToggleSoundSprite.ReplaceableText = getSoundCurrentSetting();
        }

        private void ToggleSoundSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelToggleSoundPgPress();
        }

        private void ToggleSoundSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelToggleSoundPgPress();
        }

        private void handelToggleSoundPgPress()
        {
            m_SoundManager.ToggelMute();
            m_ToggleSoundSprite.ReplaceableText = getSoundCurrentSetting();
        }

        private void BackgroundMusicVolumeSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelBackgroundMusicPgPress(k_VoluemJump);
        }

        private void BackgroundMusicVolumeSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelBackgroundMusicPgPress(-k_VoluemJump);
        }

        private void handelBackgroundMusicPgPress(float i_VoluemJump)
        {
            m_SoundManager.BackgroundVolume += i_VoluemJump;
            m_BackgroundMusicVolumeSprite.ReplaceableText = getBackgroundMusicVolume();
        }

        private void SoundsEffectsVolumeSprite_PgUpPressedOnItem(object sender, EventArgs e)
        {
            handelSoundsEffectsPgPress(k_VoluemJump);
        }

        private void SoundsEffectsVolumeSprite_PgDownPressedOnItem(object sender, EventArgs e)
        {
            handelSoundsEffectsPgPress(-k_VoluemJump);
        }

        private void handelSoundsEffectsPgPress(float i_VoluemJump)
        {
            m_SoundManager.EffectsVolume += i_VoluemJump;
            m_SoundsEffectsVolumeSprite.ReplaceableText = getEffectsVolume();
        }

        private void DoneSprite_EnterPressedOnItem(object sender, EventArgs e)
        {
            ExitScreen();
        }
    }
}