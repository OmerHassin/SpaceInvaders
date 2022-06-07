using System;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using InvandersGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InvandersGame.GameScreens
{
    public class PlayScreen : GameScreen
    {
        private const int k_EnemiesRow = 5;
        private const int k_EnemiesColumn = 9;

        private readonly BarrierMatrix r_BarrierMatrix;
        private readonly EnemyMatrix r_EnemyMatrix;
        private readonly IGameManager r_GameManager;

        private GamePauseScreen m_GamePauseScreen;
        private bool m_TransitionScreenDisplayed = false;
        private float m_BarriesAccelerator;
        private int m_NumberOfAdditionalColumns;
        private int m_EnemiesAdditionalScore;

        public PlayScreen(Game i_Game)
            : base(i_Game)
        {
            m_GamePauseScreen = new GamePauseScreen(i_Game);
            r_GameManager = i_Game.Services.GetService(typeof(IGameManager)) as IGameManager;
            r_GameManager.FirsttLevelConstructor(this, out m_BarriesAccelerator, out m_NumberOfAdditionalColumns, out m_EnemiesAdditionalScore);

            this.Add(new MotherShip(i_Game));
            this.Add(r_BarrierMatrix = new BarrierMatrix(i_Game, m_BarriesAccelerator));
            this.Add(r_EnemyMatrix = new EnemyMatrix(i_Game, k_EnemiesColumn + m_NumberOfAdditionalColumns, k_EnemiesRow, m_EnemiesAdditionalScore));
            creatShips();
        }

        private void creatShips()
        {
            for (int i = 0; i < r_GameManager.NumOfPlayers; i++)
            {
                this.Add(new Ship(Game, Vector2.Zero, (PlayerIndex)i, this));
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            r_GameManager.PlayersDistroid += GameManager_PlayersDistroid;
            r_EnemyMatrix.EnemiesDistroid += EnemyMatrix_EnemiesDistroid;
            EnemyMatrix.EnemiesReachBottom += EnemyMatrix_EnemiesReachBottom;
        }

        private void GameManager_PlayersDistroid(object sender, EventArgs e)
        {
            onGameOver();
        }

        private void EnemyMatrix_EnemiesReachBottom(object sender, EventArgs e)
        {
            onGameOver();
        }

        private void onGameOver()
        {
            Dispose();
            ExitScreen();
            ScreensManager.SetCurrentScreen(ScreensManager.ActiveScreen);
        }

        private void EnemyMatrix_EnemiesDistroid(object sender, EventArgs e)
        {
            r_GameManager.Level++;
            initializeNextLevel();
        }

        private void initializeNextLevel()
        {
            m_TransitionScreenDisplayed = false;
            r_GameManager.GetLevelSetting(out m_BarriesAccelerator, out m_NumberOfAdditionalColumns, out m_EnemiesAdditionalScore);

            r_BarrierMatrix.InitializeNextLevel(m_BarriesAccelerator);
            r_EnemyMatrix.InitializeNextLevel(k_EnemiesColumn + m_NumberOfAdditionalColumns, m_EnemiesAdditionalScore);

            foreach (Sprite2D sprite in m_Sprites)
            {
                sprite.ReInitialize();
            }
        }

        public override void Update(GameTime i_GameTime)
        {
            checkInputs();

            if (!m_TransitionScreenDisplayed)
            {
                ScreensManager.SetCurrentScreen(new LevelTransitionScreen(Game, r_GameManager.Level));
                m_TransitionScreenDisplayed = true;
            }

            base.Update(i_GameTime);
        }

        private void checkInputs()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Game.Exit();
            }
            else if (InputManager.KeyPressed(Keys.P))
            {
                ScreensManager.SetCurrentScreen(m_GamePauseScreen);
            }
            else if (InputManager.KeyPressed(Keys.M))
            {
                (Game.Services.GetService(typeof(ISoundManager)) as ISoundManager).ToggelMute();
            }
        }

        public override void Draw(GameTime i_GameTime)
        {
            if (m_TransitionScreenDisplayed)
            {
                base.Draw(i_GameTime);
            }
        }

        public EnemyMatrix EnemyMatrix
        {
            get { return r_EnemyMatrix; }
        }
    }
}