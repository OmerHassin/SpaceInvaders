using System;
using Microsoft.Xna.Framework;
using Infrastructure.ReusableComponents.Screens;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;
using Infrastructure.ReusableComponents.Objects;

namespace InvandersGame.GameScreens
{
    public class LevelTransitionScreen : GameScreen
    {
        private const string k_FontAssetName = @"Fonts\BigConsolas";
        private const string k_FontText = @"Level {0}";
        private const float k_TargetScale = 1.2f;
        private const float k_PulsePerSecond = 1;
        private const int k_CounterNumber = 3;
        private const int k_AdditonForPosition = 75;

        private readonly TimeSpan r_CounterAnimatinLenght = TimeSpan.FromSeconds(0.8);

        private TextBlockcs m_TextBlockcs;
        private Sprite2D m_Counter;

        public LevelTransitionScreen(Game i_Game, int i_Level)
            : base(i_Game)
        {
            IsOverlayed = true;

            creatTextBlock(i_Level);
            this.Add(new Background(i_Game));
            this.Add(m_Counter = new Sprite2D(@"Sprites\Titles\Countdown", this.Game));
        }

        private void creatTextBlock(int i_Level)
        {
            string msg = string.Format(k_FontText, i_Level);
            this.Add(m_TextBlockcs = new TextBlockcs(k_FontAssetName, Game, Vector2.Zero, msg, Color.Yellow));
        }

        public override void Initialize()
        {
            base.Initialize();

            initializeAnimations();
            m_TextBlockcs.PositionOrigin = m_TextBlockcs.SourceRectangleCenter;
            m_TextBlockcs.RotationOrigin = m_TextBlockcs.SourceRectangleCenter;
            m_TextBlockcs.Position = new Vector2(CenterOfViewPort.X, CenterOfViewPort.Y - k_AdditonForPosition);

            m_Counter.SourceRectangle = new Rectangle(0, 0, (int)m_Counter.Width / k_CounterNumber, (int)m_Counter.Height);
            m_Counter.PositionOrigin = m_Counter.SourceRectangleCenter;
            m_Counter.Position = CenterOfViewPort;
        }

        private void initializeAnimations()
        {     
            m_TextBlockcs.Animations.Add(new PulseAnimator(TimeSpan.Zero, k_TargetScale, k_PulsePerSecond));
            m_TextBlockcs.Animations.Enabled = true;

            m_Counter.Animations.Add(new CellAnimator(m_Counter, r_CounterAnimatinLenght, k_CounterNumber, r_CounterAnimatinLenght * k_CounterNumber));
            m_Counter.Animations.AnimationLength = r_CounterAnimatinLenght * k_CounterNumber;
            m_Counter.Animations.Finished += Animations_Finished;
            m_Counter.Animations.Enabled = true;
        }

        private void Animations_Finished(object sender, EventArgs e)
        {
            ExitScreen();
        }
    }
}