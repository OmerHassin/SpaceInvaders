using System;
using Microsoft.Xna.Framework;

namespace Infrastructure.Interfaces
{
    public interface ISoundManager
    {
        event EventHandler<EventArgs> MuteToggelChanged;

        bool Mute { get; set; }
        float BackgroundVolume { get; set; }
        float EffectsVolume { get; set; }
        void AddSoundEffect(string i_SoundEffectAsset);
        void Initialize();
        void SetBackroundMusic(string i_AssetName);
        void Update(GameTime gameTime);
        void ToggelMute();
        void PlayInstance(string i_SoundEffectAsset);
    }
}