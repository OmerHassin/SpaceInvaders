using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using InvandersGame.Utils;
using Infrastructure.ReusableComponents;
using Infrastructure.Interfaces;

namespace InvandersGame.GameObjects
{
    public class EnemyMatrix : CompositeDrawableComponent<DrawableGameComponent>
    {
        private const string k_GunShootAsset = @"Sounds\EnemyGunShot";
        private const string k_LevelWinAsset = @"Sounds\LevelWin";
        private const float k_StartingJumpingDistance = 16;
        private const float k_SpaceBetweenEnemies = 1.6f;
        private const float k_ShipsYPositon = 548.8f;
        private const float k_EnemyWidthSize = 32;
        private const float k_EnemyHeighSize = 32;
        private const int k_MaxBulletsOnScreen = 5;
        private const int k_MaxiTimeToWait = 4;
        private const int k_UpperEnemyScore = 300;
        private const int k_MiddleEnemyScore = 200;
        private const int k_BottomEnemyScore = 70;

        private readonly ShootingMachine r_ShootingMachine;
        private readonly List<Enemy> r_Enemies;
        private readonly Random r_Random;
        private readonly int r_NumOfRows;

        private Enemy m_RightMostEnemy;
        private Enemy m_LeftMostEnemy;
        private Enemy m_DownMostEnemy;
        private TimeSpan m_FrameTime;
        private float m_JumpingDistance;
        private float m_ElapsedTime;
        private float m_TimeToWait;
        private int m_NumOfCols;
        private bool m_JumpingRight;

        public event EventHandler EnemiesDistroid;

        public event EventHandler EnemiesReachBottom;

        public EnemyMatrix(Game i_InvadersGame, int i_NumOfCols, int i_NumOfRows, int i_EnemiesAdditionalScore)
            : base(i_InvadersGame)
        {
            m_NumOfCols = i_NumOfCols;
            r_NumOfRows = i_NumOfRows;
            m_JumpingRight = true;
            m_FrameTime = TimeSpan.FromSeconds(0.5);
            m_JumpingDistance = k_StartingJumpingDistance;
            r_Random = new Random();
            r_Enemies = new List<Enemy>();
            createEnemies(i_EnemiesAdditionalScore);
            r_ShootingMachine = new ShootingMachine(i_InvadersGame, k_MaxBulletsOnScreen, Enums.eShooter.Enemy, k_GunShootAsset);
            this.Add(r_ShootingMachine);
        }

        private void createEnemies(int i_EnemiesAdditionalScore)
        {
            Vector2 delta = new Vector2();
            Enemy currentEnemy;

            for (int i = 0; i < m_NumOfCols * r_NumOfRows; i++)
            {
                delta.X = (k_EnemyWidthSize * k_SpaceBetweenEnemies) * (i % m_NumOfCols);
                delta.Y = (k_EnemyHeighSize * k_SpaceBetweenEnemies) * (i / m_NumOfCols);
                currentEnemy = createEnemy(i, delta, i_EnemiesAdditionalScore);
                currentEnemy.VisibleChanged += removeEnemyFromList;
                this.Add(currentEnemy);
                r_Enemies.Add(currentEnemy);
            }
        }

        private Enemy createEnemy(int i_EnemyNumber, Vector2 i_Delta, int i_EnemiesAdditionalScore)
        {
            int colPairity = (i_EnemyNumber / m_NumOfCols) % 2;

            return new Enemy(Game, i_Delta, getEnemyStyle(i_EnemyNumber), m_JumpingDistance, m_FrameTime, colPairity, i_EnemiesAdditionalScore);
        }

        private Enums.eEnemyStyle getEnemyStyle(int i_Index)
        {
            Enums.eEnemyStyle eEnemyStyle;

            switch (i_Index / m_NumOfCols)
            {
                case 0:
                    eEnemyStyle = Enums.eEnemyStyle.PinkEnemy;
                    break;

                case 1:
                case 2:
                    eEnemyStyle = Enums.eEnemyStyle.TealEnemy;
                    break;

                case 3:
                case 4:
                default:
                    eEnemyStyle = Enums.eEnemyStyle.YellowEnemy;
                    break;
            }

            return eEnemyStyle;
        }

        private void removeEnemyFromList(object sender, EventArgs e)
        {
            r_Enemies.Remove(sender as Enemy);
            if (r_Enemies.Count == 0)
            {
                OnEnemiesDistroid();
            }
        }

        private void OnEnemiesDistroid()
        {
            playSound();
            EnemiesDistroid?.Invoke(this, EventArgs.Empty);
        }

        public override void Initialize()
        {
            base.Initialize();

            initializeEdges();
            drawTimeToWait();
        }

        private void initializeEdges()
        {
            m_RightMostEnemy = r_Enemies[m_NumOfCols - 1];
            m_RightMostEnemy.VisibleChanged += updateRightEdgesIfNeeded;

            m_LeftMostEnemy = r_Enemies[0];
            m_LeftMostEnemy.VisibleChanged += updateLeftEdgesIfNeeded;

            m_DownMostEnemy = r_Enemies[(m_NumOfCols * r_NumOfRows) - 1];
            m_DownMostEnemy.VisibleChanged += updateDownEdgesIfNeeded;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            soundManager.AddSoundEffect(k_LevelWinAsset);
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);

            enemiesShootingHandler(i_GameTime);
            checkNextJumpingDistance();
            checkIfEnemeisReachedBorders();
            checkIfEnemiesReachedBottom();
        }

        private void checkNextJumpingDistance()
        {
            float nextJump;

            if (m_JumpingRight)
            {
                nextJump = Math.Min(k_StartingJumpingDistance, Game.GraphicsDevice.Viewport.Width - (m_RightMostEnemy.Position.X + m_RightMostEnemy.Width));
                if (nextJump != m_RightMostEnemy.Jump)
                {
                    updateEnemiesNextJump(nextJump);
                }
            }
            else
            {
                nextJump = Math.Max(-1 * k_StartingJumpingDistance, 0 - m_LeftMostEnemy.Position.X);
                if (nextJump != m_LeftMostEnemy.Jump)
                {
                    updateEnemiesNextJump(nextJump);
                }
            }
        }

        private void checkIfEnemeisReachedBorders()
        {
            if ((m_JumpingRight && m_RightMostEnemy.Position.X + m_RightMostEnemy.Width >= Game.GraphicsDevice.Viewport.Width)
                ||
               (!m_JumpingRight && m_LeftMostEnemy.Position.X <= 0))
            {
                m_JumpingRight = !m_JumpingRight;
                m_FrameTime *= 0.95f;
                m_JumpingDistance *= -1;

                pushDownEnemies(new Vector2(0, k_EnemyHeighSize / 2));
                updateEnemiesFrameTime();
                updateEnemiesNextJump(m_JumpingDistance);
            }
        }

        private void updateRightEdgesIfNeeded(object sender, EventArgs e)
        {
            if (!m_RightMostEnemy.Visible && r_Enemies.Count != 0)
            {
                Enemy currentRightMost = r_Enemies[0];
                float rightMost = 0;

                foreach (Enemy enemy in r_Enemies)
                {
                    if (enemy.Position.X + enemy.Width > rightMost)
                    {
                        rightMost = enemy.Position.X + enemy.Width;
                        currentRightMost = enemy;
                    }
                }

                m_RightMostEnemy = currentRightMost;
                m_RightMostEnemy.VisibleChanged += updateRightEdgesIfNeeded;
            }
        }

        private void updateLeftEdgesIfNeeded(object sender, EventArgs e)
        {
            if (!m_LeftMostEnemy.Visible && r_Enemies.Count != 0)
            {
                Enemy currentLeftMost = r_Enemies[0];
                float leftMost = Game.GraphicsDevice.Viewport.Width;

                foreach (Enemy enemy in r_Enemies)
                {
                    if (enemy.Position.X < leftMost)
                    {
                        leftMost = enemy.Position.X;
                        currentLeftMost = enemy;
                    }
                }

                m_LeftMostEnemy = currentLeftMost;
                m_LeftMostEnemy.VisibleChanged += updateLeftEdgesIfNeeded;
            }
        }

        private void updateDownEdgesIfNeeded(object sender, EventArgs e)
        {
            if (!m_DownMostEnemy.Visible && r_Enemies.Count != 0)
            {
                Enemy currentDownMost = r_Enemies[0];
                float downMost = 0;

                foreach (Enemy enemy in r_Enemies)
                {
                    if (enemy.Position.Y + enemy.Height > downMost)
                    {
                        downMost = enemy.Position.Y + enemy.Height;
                        currentDownMost = enemy;
                    }
                }

                m_DownMostEnemy = currentDownMost;
                m_DownMostEnemy.VisibleChanged += updateDownEdgesIfNeeded;
            }
        }

        private void checkIfEnemiesReachedBottom()
        {
            if (m_DownMostEnemy.Position.Y + m_DownMostEnemy.Height >= k_ShipsYPositon)
            {
                OnEnemiesReachBottom();
            }
        }

        private void OnEnemiesReachBottom()
        {
            EnemiesReachBottom?.Invoke(this, EventArgs.Empty);
        }

        internal void InitializeNextLevel(int i_NumOfCols, int i_EnemiesAdditionalScore)
        {
            m_JumpingRight = true;
            m_FrameTime = TimeSpan.FromSeconds(0.5);
            m_JumpingDistance = k_StartingJumpingDistance;
            m_NumOfCols = i_NumOfCols;
            createEnemies(i_EnemiesAdditionalScore);
            r_ShootingMachine.Reset();
        }

        private void updateEnemiesNextJump(float i_JumpingDistance)
        {
            foreach (Enemy enemy in r_Enemies)
            {
                enemy.Jump = i_JumpingDistance;
            }
        }

        private void updateEnemiesFrameTime()
        {
            foreach (Enemy enemy in r_Enemies)
            {
                enemy.FrameTime = m_FrameTime;
            }
        }

        private void pushDownEnemies(Vector2 i_delta)
        {
            foreach (Enemy enemy in r_Enemies)
            {
                enemy.Position += i_delta;
            }
        }

        private void enemiesShootingHandler(GameTime i_GameTime)
        {
            if (m_ElapsedTime >= m_TimeToWait && r_Enemies.Count != 0)
            {
                if (r_ShootingMachine.ReadyToShoot() && ShootBullet())
                {
                    drawTimeToWait();
                    m_ElapsedTime = 0;
                }
            }
            else
            {
                m_ElapsedTime += (float)i_GameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public bool ShootBullet()
        {
            bool shootSucceed = false;
            int enemyindex = getRandomListIndex();
            Enemy enemyShooting = r_Enemies[enemyindex];

            if (!enemyShooting.BulletOnScreen && r_ShootingMachine.Shoot(enemyShooting.GetPositionForShootiong(), enemyShooting))
            {
                enemyShooting.BulletOnScreen = true;
                shootSucceed = true;
            }

            return shootSucceed;
        }

        private void playSound()
        {
            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            soundManager.PlayInstance(k_LevelWinAsset);
        }

        private void drawTimeToWait()
        {
            m_TimeToWait = r_Random.Next(k_MaxiTimeToWait);
        }

        private int getRandomListIndex()
        {
            return r_Random.Next(r_Enemies.Count);
        }

        public static void LoadEnemyStyle(out Color o_TintColor, out int o_ScoreValue, out int o_EnemyFrameRowIndex, Enums.eEnemyStyle i_Style)
        {
            switch (i_Style)
            {
                case Enums.eEnemyStyle.PinkEnemy:
                    o_TintColor = Color.Pink;
                    o_ScoreValue = k_UpperEnemyScore;
                    o_EnemyFrameRowIndex = 0;
                    break;

                case Enums.eEnemyStyle.TealEnemy:
                    o_TintColor = Color.Teal;
                    o_ScoreValue = k_MiddleEnemyScore;
                    o_EnemyFrameRowIndex = 1;
                    break;

                case Enums.eEnemyStyle.YellowEnemy:
                default:
                    o_TintColor = Color.Yellow;
                    o_ScoreValue = k_BottomEnemyScore;
                    o_EnemyFrameRowIndex = 2;
                    break;
            }
        }
    }
}