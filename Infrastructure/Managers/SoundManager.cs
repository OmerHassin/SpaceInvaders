using System;
using System.Collections.Generic;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Infrastructure.Managers
{
    public class SoundManager : GameService, ISoundManager
    {
        private readonly Dictionary<string, SoundEffectInstance> r_Sounds;

        private SoundEffectInstance m_BackgroundMusic;
        private string m_BackgroundMusicAsset;
        private float m_BackgroundMusicVolume;
        private float m_SoundsEffectsVolume;
        private bool m_MuteSound;

        public event EventHandler<EventArgs> MuteToggelChanged;

        public SoundManager(Game i_Game, string k_BackgroundMusicAsset)
            : this(i_Game)
        {
            m_BackgroundMusicAsset = k_BackgroundMusicAsset;
        }

        public SoundManager(Game i_Game)
            : base(i_Game)
        {
            r_Sounds = new Dictionary<string, SoundEffectInstance>();
            m_BackgroundMusicVolume = 50;
            m_SoundsEffectsVolume = 50;
            m_MuteSound = false;
        }

        protected override void RegisterAsService()
        {
            this.Game.Services.AddService(typeof(ISoundManager), this);
        }

        public override void Initialize()
        {
            if (m_BackgroundMusicAsset != null)
            {
                SetBackroundMusic(m_BackgroundMusicAsset);
            }
        }

        public void SetBackroundMusic(string i_AssetName)
        {
            m_BackgroundMusic = Game.Content.Load<SoundEffect>(i_AssetName).CreateInstance();
            m_BackgroundMusic.Volume = m_BackgroundMusicVolume / 100;
            m_BackgroundMusic.IsLooped = true;
            m_BackgroundMusic.Play();
        }

        public void AddSoundEffect(string i_SoundEffectAsset)
        {
            SoundEffectInstance soundEffectInstance;
            if (!r_Sounds.TryGetValue(i_SoundEffectAsset, out soundEffectInstance))
            {
                soundEffectInstance = Game.Content.Load<SoundEffect>(i_SoundEffectAsset).CreateInstance();
                soundEffectInstance.Volume = m_SoundsEffectsVolume / 100;
                r_Sounds.Add(i_SoundEffectAsset, soundEffectInstance);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void ToggelMute()
        {
            Mute = !Mute;
        }

        private void OnBackgroundMusicVolumeChanged()
        {
            if (m_BackgroundMusic != null)
            {
                m_BackgroundMusic.Volume = BackgroundVolume / 100;
            }
        }

        private void OnSoundsEffectsVolumChanged()
        {
            foreach (KeyValuePair<string, SoundEffectInstance> pair in r_Sounds)
            {
                pair.Value.Volume = EffectsVolume / 100;
            }
        }

        public bool Mute
        {
            get { return m_MuteSound; }
            set
            {
                if (m_MuteSound != value)
                {
                    m_MuteSound = value;
                    OnMuteToggelChanged();
                }
            }
        }

        public float BackgroundVolume
        {
            get { return m_BackgroundMusicVolume; }
            set
            {
                if (value <= 100 && value >= 0)
                {
                    if (m_BackgroundMusicVolume != value)
                    {
                        m_BackgroundMusicVolume = value;
                        OnBackgroundMusicVolumeChanged();
                    }
                }
            }
        }

        public float EffectsVolume
        {
            get { return m_SoundsEffectsVolume; }
            set
            {
                if (value <= 100 && value >= 0)
                {
                    if (m_SoundsEffectsVolume != value)
                    {
                        m_SoundsEffectsVolume = value;
                        OnSoundsEffectsVolumChanged();
                    }
                }
            }
        }

        private void OnMuteToggelChanged()
        {
            if (Mute)
            {
                SoundEffect.MasterVolume = 0.0f;
            }
            else
            {
                SoundEffect.MasterVolume = 1.0f;
            }

            MuteToggelChanged?.Invoke(this, EventArgs.Empty);
        }

        public void PlayInstance(string i_SoundEffectAsset)
        {
            r_Sounds[i_SoundEffectAsset].Play();
        }
    }
}