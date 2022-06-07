using System;
using Infrastructure.ReusableComponents.Objects;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents.Animators.ConcreteAnimators
{
    public class CellAnimator : SpriteAnimator
    {
        private readonly int r_NumOfCells;
        private TimeSpan m_CellTime;
        private TimeSpan m_TimeLeftForCell;
        private bool m_Loop = true;
        private int m_CurrColCellIdx;
        private int m_CurrRowCellIdx;

        public CellAnimator(Sprite2D i_BoundSprite, TimeSpan i_CellTime, int i_NumOfCells, int i_FrameColIndex, int i_FrameRowIndex, TimeSpan i_AnimationLength)
            : base("CelAnimation", i_AnimationLength)
        {
            this.m_CellTime = i_CellTime;
            this.m_TimeLeftForCell = i_CellTime;
            this.r_NumOfCells = i_NumOfCells;
            this.m_CurrColCellIdx = i_FrameColIndex;
            this.m_CurrRowCellIdx = i_FrameRowIndex;
            this.BoundSprite = i_BoundSprite;

            m_Loop = i_AnimationLength == TimeSpan.Zero;
        }

        public CellAnimator(Sprite2D i_BoundSprite, TimeSpan i_CellTime, int i_NumOfCells, TimeSpan i_AnimationLength)
            : this(i_BoundSprite, i_CellTime, i_NumOfCells, 0, 0 , i_AnimationLength)
        {
        }

        private void goToNextFrame()
        {
            m_CurrColCellIdx++;
            if (m_CurrColCellIdx >= r_NumOfCells)
            {
                if (m_Loop)
                {
                    m_CurrColCellIdx = 0;
                }
                else
                {
                    m_CurrColCellIdx = r_NumOfCells - 1; /// lets stop at the last frame
                    this.IsFinished = true;
                }
            }
        }

        protected override void RevertToOriginal()
        {
            this.BoundSprite.SourceRectangle = m_OriginalSpriteInfo.SourceRectangle;
        }

        protected override void DoFrame(GameTime i_GameTime)
        {
            if (m_CellTime != TimeSpan.Zero)
            {
                m_TimeLeftForCell -= i_GameTime.ElapsedGameTime;
                if (m_TimeLeftForCell.TotalSeconds <= 0)
                {
                    /// we have elapsed, so blink
                    goToNextFrame();
                    m_TimeLeftForCell = m_CellTime;
                }
            }

            this.BoundSprite.SourceRectangle = new Rectangle(
                m_CurrColCellIdx * this.BoundSprite.SourceRectangle.Width,
                m_CurrRowCellIdx * this.BoundSprite.SourceRectangle.Height,
                this.BoundSprite.SourceRectangle.Width,
                this.BoundSprite.SourceRectangle.Height);
        }

        public TimeSpan CellTime
        {
            set { m_CellTime = value; }
        }
    }
}
