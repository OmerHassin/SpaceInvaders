using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InvandersGame.GameScreens
{
    public class GamePauseScreen : GameScreen
    {
        private const string k_BigFontAssetName = @"Fonts\BigConsolas";
        private const string k_SmallFontAssetName = @"Fonts\Consolas";
        private const string k_SmallFontText = @"Press R To Resume";
        private const string k_BigFontText = @"Game Paused";

        private TextBlockcs m_BigTextBlockcs;
        private TextBlockcs m_SmallTextBlockcs;

        public GamePauseScreen(Game i_Game)
            : base(i_Game)
        {
            this.BlackTintAlpha = 0.40f;
            this.UseGradientBackground = false;
            this.IsOverlayed = true;

            creatTextBlocks();
        }

        private void creatTextBlocks()
        {
            this.Add(m_BigTextBlockcs = new TextBlockcs(k_BigFontAssetName, Game, k_BigFontText, Color.White));
            this.Add(m_SmallTextBlockcs = new TextBlockcs(k_SmallFontAssetName, Game, k_SmallFontText, Color.White));
        }

        public override void Initialize()
        {
            base.Initialize();

            m_BigTextBlockcs.Position = new Vector2(CenterOfViewPort.X - (m_BigTextBlockcs.Width / 2), CenterOfViewPort.Y - (m_BigTextBlockcs.Height / 2));
            m_SmallTextBlockcs.Position = new Vector2(CenterOfViewPort.X - (m_SmallTextBlockcs.Width / 2), m_BigTextBlockcs.Position.Y + m_BigTextBlockcs.Height);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputManager.KeyPressed(Keys.R))
            {
                ExitScreen();
            }
        }
    }
}