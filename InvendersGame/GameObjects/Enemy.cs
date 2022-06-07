using System;
using Microsoft.Xna.Framework;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;
using InvandersGame.Utils;
using Infrastructure.Interfaces;

namespace InvandersGame.GameObjects
{
    public class Enemy : Sprite2D, ICollideble2D, ICapableShooter
    {
        private const string k_AssetName = @"Sprites\GameObjects\Enemies_64x96";
        private const string k_KillSoundAsset = @"Sounds\EnemyKill";
        private const int k_NumOfFramesInCol = 3;
        private const int k_NumOfFramesInRow = 2;
        private const int k_BulletDirection = 1;
        private const float k_EnemyRotatePace = 5;

        private readonly Color r_BulletTintColor = Color.Blue;
        private readonly TimeSpan r_AnimationLenght = TimeSpan.FromSeconds(1.7);

        private CellAnimator m_CellAnimator;
        private TimeSpan m_FrameTime;
        private TimeSpan m_TimeLeftForFrame;
        private float m_JumpingDistance;
        private int m_EnemyFrameRowIndex;
        private int m_EnemyFrameColIndex;
        private bool m_HaveBulletOnScreen;

        public Enemy(Game i_InvadersGame, Vector2 i_Delta, Enums.eEnemyStyle i_Style, float i_JumpingDistance, TimeSpan i_FrameTime, int i_EnemyFrameColIndex, int i_EnemiesAdditionalScore)
            : base(k_AssetName, i_InvadersGame, i_Delta)
        {
            m_EnemyFrameColIndex = i_EnemyFrameColIndex;
            m_HaveBulletOnScreen = false;
            m_JumpingSprite = true;
            m_JumpingDistance = i_JumpingDistance;
            m_FrameTime = i_FrameTime;
            m_TimeLeftForFrame = i_FrameTime;
            EnemyMatrix.LoadEnemyStyle(out m_TintColor, out m_ScoreValue, out m_EnemyFrameRowIndex, i_Style);
            Score += i_EnemiesAdditionalScore;
        }

        public override void Initialize()
        {
            base.Initialize();

            initAnimations();
        }

        private void initAnimations()
        {
            m_CellAnimator = new CellAnimator(this, m_FrameTime, k_NumOfFramesInRow, m_EnemyFrameColIndex, m_EnemyFrameRowIndex, TimeSpan.Zero);
            ShrinkerAnimator shrinkerAnimator = new ShrinkerAnimator(r_AnimationLenght);
            RotatorAnimator rotatorAnimator = new RotatorAnimator(k_EnemyRotatePace, r_AnimationLenght);

            m_Animations.AnimationLength = r_AnimationLenght;
            m_Animations.Add(shrinkerAnimator);
            m_Animations.Add(rotatorAnimator);
            m_Animations.Finished += animations_Finished;
            m_Animations.Enabled = false;
        }

        private void animations_Finished(object sender, EventArgs e)
        {
            Visible = false;
            Enabled = false;
        }

        protected override void InitPositions()
        {
            base.InitPositions();

            float x, y;

            m_WidthBeforeScale = m_WidthBeforeScale / k_NumOfFramesInRow;
            m_HeightBeforeScale = m_HeightBeforeScale / k_NumOfFramesInCol;

            x = 0f;
            y = Height * 3;
            m_Position = new Vector2(x + m_Delta.X, y + m_Delta.Y);
        }

        protected override void LoadContent()
        {
            base.LoadContent();  
            
            SoundManager.AddSoundEffect(k_KillSoundAsset);
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);
            m_CellAnimator.Update(i_GameTime);
        }

        public override void SpriteJump(GameTime i_GameTime)
        {
            m_TimeLeftForFrame -= i_GameTime.ElapsedGameTime;
            if (m_TimeLeftForFrame.TotalSeconds <= 0)
            {
                Position += new Vector2(m_JumpingDistance, 0);
                m_TimeLeftForFrame = m_FrameTime;
            }
        }

        public Vector2 GetPositionForShootiong()
        {
            Vector2 shootingPos;
            float x, y;

            x = m_Position.X + (Width / 2);
            y = m_Position.Y + Height;
            shootingPos = new Vector2(x, y);

            return shootingPos;
        }

        public virtual void Collided(ICollidable i_Collidable)
        {
            if (i_Collidable is Bullet && (i_Collidable as Bullet).ShooterType == Enums.eShooter.Ship)
            {
                enemyGotHitByBullet(i_Collidable as Bullet);
            }
        }

        private void enemyGotHitByBullet(Bullet i_Bullet)
        {
            enemyScoreHendler(i_Bullet);
            m_CellAnimator.Pause();
            m_Animations.Start();
            SoundManager.PlayInstance(k_KillSoundAsset);
        }

        private void enemyScoreHendler(Bullet i_Bullet)
        {
            GameManager.CalculateScore(i_Bullet, this);
            m_ScoreValue = 0;
        }

        public float Jump
        {
            get { return this.m_JumpingDistance; }
            set { m_JumpingDistance = value; }
        }

        public TimeSpan FrameTime
        {
            get { return this.m_FrameTime; }
            set
            {
                if (m_FrameTime != value)
                {
                    m_FrameTime = value;
                    m_CellAnimator.CellTime = value;
                }
            }
        }

        public bool BulletOnScreen
        {
            get { return m_HaveBulletOnScreen; }
            set { m_HaveBulletOnScreen = value; }
        }

        public Color BulletTintColor
        {
            get { return r_BulletTintColor; }
        }

        public int BulletDirection
        {
            get { return k_BulletDirection; }
        }
    }
}