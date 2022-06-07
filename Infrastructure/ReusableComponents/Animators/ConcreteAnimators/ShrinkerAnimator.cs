using System;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents.Animators.ConcreteAnimators
{
    public class ShrinkerAnimator : SpriteAnimator
    {
        private Vector2 m_AnimationLengthVector;   

        // CTORs
        public ShrinkerAnimator(string i_Name, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_AnimationLengthVector = new Vector2((float)i_AnimationLength.TotalSeconds, (float)i_AnimationLength.TotalSeconds);
        }

        public ShrinkerAnimator(TimeSpan i_AnimationLength)
                : this("Shrink", i_AnimationLength)
        {
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            this.BoundSprite.Scales -= (m_OriginalSpriteInfo.Scales / m_AnimationLengthVector) * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
        }

        protected override void RevertToOriginal()
        {
            this.BoundSprite.Scales = m_OriginalSpriteInfo.Scales;
        }
    }
}
