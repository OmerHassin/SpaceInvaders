using System;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents.Animators.ConcreteAnimators
{
    public class RotatorAnimator : SpriteAnimator
    {
        private float m_RotationsInSecond;

        public float RotationsInSecond
        {
            get { return m_RotationsInSecond; }
            set { m_RotationsInSecond = value; }
        }

        // CTORs
        public RotatorAnimator(string i_Name, float i_RotatePace, TimeSpan i_AnimationLength)
            : base(i_Name, i_AnimationLength)
        {
            m_RotationsInSecond = MathHelper.TwoPi * i_RotatePace;
        }

        public RotatorAnimator(float i_RotatePace, TimeSpan i_AnimationLength)
                : this("Rotate", i_RotatePace, i_AnimationLength)
        {
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            this.BoundSprite.Rotation += m_RotationsInSecond * (float)i_GameTime.ElapsedGameTime.TotalSeconds;
            this.BoundSprite.RotationOrigin = this.BoundSprite.SourceRectangleCenter;
        }

        protected override void RevertToOriginal()
        {
            this.BoundSprite.RotationOrigin = m_OriginalSpriteInfo.RotationOrigin;
            this.BoundSprite.Rotation = m_OriginalSpriteInfo.Rotation;
        }
    }
}
