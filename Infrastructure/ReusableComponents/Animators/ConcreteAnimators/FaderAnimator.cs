using System;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents.Animators.ConcreteAnimators
{
    public class FaderAnimator : SpriteAnimator
    {
        private float m_FadeInSecond;

        public FaderAnimator(string i_Name, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_FadeInSecond = 1 / (float)i_AnimationLength.TotalSeconds;
        }

        public FaderAnimator(TimeSpan i_AnimationLength)
                : this("Fader", i_AnimationLength)
        {
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            this.BoundSprite.Opacity -= (float)(m_OriginalSpriteInfo.Opacity / AnimationLength.TotalSeconds) * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void RevertToOriginal()
        {
            this.BoundSprite.Opacity = m_OriginalSpriteInfo.Opacity;
        }
    }
}
