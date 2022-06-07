using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using InvandersGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InvandersGame.GameScreens
{
    public class GameOverScreen : GameScreen
    {
        private const string k_SoundAsset = @"Sounds\GameOver";
        private const string k_SummryText =
@"{0}
The Winner is {1}";

        private const string k_KeysText =
@"Press Home To Start a New Game
     Press M For Main Menu
      Press Esc For Exit";

        private const string k_SmallFontAssetName = @"Fonts\Consolas";
        private const int k_AdditonForPosition = 100;

        private bool m_SoundPlayed = false;
        private TextBlockcs m_TextBlockcsOne;
        private TextBlockcs m_TextBlockcsTwo;
        private Background m_Background;
        private Sprite2D m_GameOverMessage;
        private IGameManager m_GameManager;

        public GameOverScreen(Game i_Game)
            : base(i_Game)
        {
            m_Background = new Background(i_Game, Color.Red);
            m_GameOverMessage = new Sprite2D(@"Sprites\Titles\gameover", i_Game);
            creatTextBlocks();
            this.Add(m_Background);
            this.Add(m_GameOverMessage);
            this.Add(m_TextBlockcsOne);
            this.Add(m_TextBlockcsTwo);
            m_GameManager = Game.Services.GetService(typeof(IGameManager)) as IGameManager;
        }

        private void creatTextBlocks()
        {
            m_TextBlockcsOne = new TextBlockcs(k_SmallFontAssetName, Game, k_SummryText, Color.White);
            m_TextBlockcsTwo = new TextBlockcs(k_SmallFontAssetName, Game, k_KeysText, Color.Yellow);
        }

        public override void Initialize()
        {
            base.Initialize();
            m_GameOverMessage.Position = new Vector2(CenterOfViewPort.X - (m_GameOverMessage.Width / 2), k_AdditonForPosition);
            m_TextBlockcsOne.Position = new Vector2(CenterOfViewPort.X - (m_TextBlockcsOne.Width / 2), m_GameOverMessage.Position.Y + m_GameOverMessage.Height);
            m_TextBlockcsTwo.Position = new Vector2(CenterOfViewPort.X - (m_TextBlockcsTwo.Width / 2), m_TextBlockcsOne.Position.Y + k_AdditonForPosition);
            m_SoundPlayed = false;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            soundManager.AddSoundEffect(k_SoundAsset);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            playSound();
            updateSummryBlocks();
            checkInput();
        }

        private void playSound()
        {
            if (!m_SoundPlayed)
            {
                ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
                soundManager.PlayInstance(k_SoundAsset);
                m_SoundPlayed = true;
            }
        }

        private void updateSummryBlocks()
        {
            string msg = string.Format(
                k_SummryText,
                m_GameManager.ScoresToString(),
                m_GameManager.Winner());

            m_TextBlockcsOne.Text = msg;
            m_TextBlockcsOne.Position = new Vector2(CenterOfViewPort.X - (m_TextBlockcsOne.Width / 2), m_GameOverMessage.Position.Y + m_GameOverMessage.Height);
            m_TextBlockcsOne.RotationOrigin = m_TextBlockcsOne.SourceRectangleCenter;
        }

        private void checkInput()
        {
            if (InputManager.KeyPressed(Keys.Home))
            {
                m_GameManager.ResetGameSettings();
                ScreensManager.SetCurrentScreen(new PlayScreen(Game));
            }
            else if (InputManager.KeyPressed(Keys.M))
            {
                m_GameManager.ResetGameSettings();
                ScreensManager.SetCurrentScreen(new MainMenuScreen(Game));
            }
            else if (InputManager.KeyPressed(Keys.Escape))
            {
                ExitScreen();
            }
        }
    }
}