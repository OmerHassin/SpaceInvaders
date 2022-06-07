using Microsoft.Xna.Framework;
using InvandersGame.GameScreens;
using Infrastructure.Managers;
using InvandersGame;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;

namespace InvendersGame
{
    public class InvadersGame : Game
    {
        private const int k_WidthSize = 800;
        private const int k_HeightSize = 600;
        private const string k_BackgroundMusicAsset = @"Sounds\BGMusic";

        private readonly GraphicsDeviceManager r_GraphicsManager;
        private readonly ScreensMananger r_ScreensMananger;
        private readonly InputManager m_InputManager;

        public InvadersGame()
        {
            r_GraphicsManager = new GraphicsDeviceManager(this);
            Services.AddService(typeof(GraphicsDeviceManager), r_GraphicsManager);
            r_ScreensMananger = new ScreensMananger(this);
            m_InputManager = new InputManager(this);
            new SoundManager(this, k_BackgroundMusicAsset);
            new GameManager(this);
            new CollisionsManager(this);

            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            r_GraphicsManager.PreferredBackBufferWidth = k_WidthSize;
            r_GraphicsManager.PreferredBackBufferHeight = k_HeightSize;

            Components.Add(new Background(this));
            r_ScreensMananger.Push(new GameOverScreen(this));
            r_ScreensMananger.SetCurrentScreen(new WelcomeScreen(this));
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.Window.Title = "InvandersGame";
        }
    }
}