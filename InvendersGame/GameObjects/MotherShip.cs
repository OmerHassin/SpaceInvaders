using System;
using Microsoft.Xna.Framework;
using InvandersGame.Utils;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;

namespace InvandersGame.GameObjects
{
    public class MotherShip : Sprite2D, ICollideble2D
    {
        private const string k_AssetName = @"Sprites\GameObjects\MotherShip_32x120";
        private const string k_KillSoundAsset = @"Sounds\MotherShipKill";
        private const int k_MaximunTimeToWait = 10;
        private const int k_MotherShipScoreValue = 600;

        private readonly TimeSpan r_MotherShipBlinkPace = TimeSpan.FromSeconds(0.2);
        private readonly TimeSpan r_AnimationLenght = TimeSpan.FromSeconds(3);
        private readonly Random r_Random;

        private float m_ElapsedTime;
        private float m_TimeToWait;

        public MotherShip(Game i_InvadersGame)
            : base(k_AssetName, i_InvadersGame)
        {
            m_TintColor = Color.Red;
            m_Velocity = new Vector2(95, 0);
            m_ElapsedTime = 0;
            m_ScoreValue = k_MotherShipScoreValue;
            r_Random = new Random();
            drawTimeToWait();
        }

        public override void Initialize()
        {
            base.Initialize();

            initAnimations();
        }

        private void initAnimations()
        {
            FaderAnimator faderAnimator = new FaderAnimator(r_AnimationLenght);
            ShrinkerAnimator shrinkerAnimator = new ShrinkerAnimator(r_AnimationLenght);
            BlinkAnimator blinkAnimator = new BlinkAnimator(r_MotherShipBlinkPace, r_AnimationLenght);

            m_Animations.AnimationLength = r_AnimationLenght;
            m_Animations.Add(faderAnimator);
            m_Animations.Add(shrinkerAnimator);
            m_Animations.Add(blinkAnimator);
            m_Animations.Finished += animations_Finished;
        }

        private void animations_Finished(object sender, EventArgs e)
        {
            ReInitialize();
        }
        
        protected override void InitPositions()
        {
            float x, y;

            x = (-1) * m_Texture.Width;
            y = m_Texture.Height;
            m_Position = new Vector2(x, y);

            base.InitPositions();
        }

        private void drawTimeToWait()
        {
            m_TimeToWait = r_Random.Next(k_MaximunTimeToWait);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            SoundManager.AddSoundEffect(k_KillSoundAsset);
        }
    
        public override void Update(GameTime i_GameTime)
        {
            if (m_ElapsedTime >= m_TimeToWait)
            {
                Visible = true;
                base.Update(i_GameTime);
                if (m_Position.X > Game.GraphicsDevice.Viewport.Width)
                {
                    ReInitialize();
                }
            }
            else
            {
                m_ElapsedTime += (float)i_GameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void ReInitialize()
        {
            drawTimeToWait();
            InitPositions();
            Visible = false;
            m_ElapsedTime -= m_TimeToWait;
            m_ScoreValue = k_MotherShipScoreValue;
            m_Animations.Restart();
            m_Animations.Stop();
        }

        public virtual void Collided(ICollidable i_Collidable)
        {
            if (i_Collidable is Bullet && (i_Collidable as Bullet).ShooterType == Enums.eShooter.Ship)
            {
                MotherShipGotHit(i_Collidable as Bullet);
            }
        }

        private void MotherShipGotHit(Bullet i_Bullet)
        {
            motherShipScoreHendler(i_Bullet);
            m_Animations.Start();
            SoundManager.PlayInstance(k_KillSoundAsset);
        }

        private void motherShipScoreHendler(Bullet i_Bullet)
        {
            GameManager.CalculateScore(i_Bullet, this);
            m_ScoreValue = 0;
        }
    }
}