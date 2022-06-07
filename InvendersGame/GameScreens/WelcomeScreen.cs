using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Infrastructure.ReusableComponents.Screens;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;
using Infrastructure.ReusableComponents.Objects;

namespace InvandersGame.GameScreens
{
    public class WelcomeScreen : GameScreen
    {
        private const float k_TargetScaleAnimation = 1.05f;
        private const float k_PulsePerSecondAnimation = 0.7f;
        private const string k_PreesButtomsText =
@"Press Enter To Start a New Game
      Press M For Main Menu
       Press Esc For Exit";

        private const string k_FontAssetName = @"Fonts\Consolas";

        private readonly TimeSpan r_AnimationLength = TimeSpan.Zero;

        private Sprite2D m_WelcomeMessage;
        private TextBlockcs m_TextBlockcsButtoms;

        public WelcomeScreen(Game i_Game)
            : base(i_Game)
        {
            this.Add(m_WelcomeMessage = new Sprite2D(@"Sprites\Titles\WelcomeMenu", i_Game));
            this.Add(m_TextBlockcsButtoms = new TextBlockcs(k_FontAssetName, i_Game, k_PreesButtomsText, Color.Yellow));
        }

        public override void Initialize()
        {
            base.Initialize();

            m_WelcomeMessage.Animations.Add(new PulseAnimator(r_AnimationLength, k_TargetScaleAnimation, k_PulsePerSecondAnimation));
            m_WelcomeMessage.Animations.Enabled = true;
            m_WelcomeMessage.PositionOrigin = m_WelcomeMessage.SourceRectangleCenter;
            m_WelcomeMessage.RotationOrigin = m_WelcomeMessage.SourceRectangleCenter;
            m_WelcomeMessage.Position = CenterOfViewPort;
            m_TextBlockcsButtoms.Position = new Vector2(CenterOfViewPort.X - (m_TextBlockcsButtoms.Width / 2), m_WelcomeMessage.Position.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.Enter))
            {
                ExitScreen();
                ScreensManager.SetCurrentScreen(new PlayScreen(Game));
            }
            else if (InputManager.KeyPressed(Keys.M))
            {
                ExitScreen();
                ScreensManager.SetCurrentScreen(new MainMenuScreen(Game));
            }
            else if (InputManager.KeyPressed(Keys.Escape))
            {
                Game.Exit();
            }
        }
    }
}