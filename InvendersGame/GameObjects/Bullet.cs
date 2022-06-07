using System;
using Microsoft.Xna.Framework;
using InvandersGame.Utils;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents;

namespace InvandersGame.GameObjects
{
    public class Bullet : Sprite2D, ICollideble2D
    {
        private const string k_AssetName = @"Sprites\GameObjects\Bullet";
        private const float k_ChanceEnemyBulletToRemain = 0.8f;
        private const float k_BulletWidthSize = 6;
        private const float k_BulletVelocity = 140;

        private readonly Enums.eShooter r_BulletShooter;
        private readonly Random r_Random;

        private ICapableShooter m_Shooter;

        public Bullet(Game i_InvadersGame, Vector2 i_Delta, Enums.eShooter i_BulletShooter, ICapableShooter i_Shooter)
            : base(k_AssetName, i_InvadersGame, i_Delta)
        { 
            r_BulletShooter = i_BulletShooter;
            r_Random = new Random();
            m_Shooter = i_Shooter;
            m_TintColor = i_Shooter.BulletTintColor;
            m_Velocity = new Vector2(0, i_Shooter.BulletDirection * k_BulletVelocity);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_Pixels = new Color[m_Texture.Width * m_Texture.Height];
            m_Texture.GetData<Color>(m_Pixels);
        }

        protected override void InitPositions()
        {
            float x, y;

            x =  -(k_BulletWidthSize / 2);
            y = 0f;

            m_Position = new Vector2(x + m_Delta.X, y + m_Delta.Y);

            base.InitPositions();
        }

        public override void Update(GameTime i_GameTime)
        {
            if (Visible)
            {
                base.Update(i_GameTime);

                if (m_Position.Y < 0 || m_Position.Y > Game.GraphicsDevice.Viewport.Height)
                {
                    Visible = false;
                }
            }
        }

        public void ReInitialize(Vector2 i_Delta, ICapableShooter i_Shooter)
        {
            Delta = i_Delta;
            m_Shooter = i_Shooter;
            Initialize();
            Visible = true;
        }

        public virtual void Collided(ICollidable i_Collidable)
        {
            if (r_BulletShooter == Enums.eShooter.Enemy && i_Collidable is Ship)
            {
                Visible = false;
            }
            else if (r_BulletShooter == Enums.eShooter.Ship && i_Collidable is Enemy)
            {
                Visible = false;
            }
            else if (i_Collidable is MotherShip)
            {
                Visible = false;
            }
            else if (i_Collidable is Bullet && (i_Collidable as Bullet).ShooterType != r_BulletShooter)
            {
                twoBulletsGotHit();
            }
        }

        private void twoBulletsGotHit()
        {
            if (r_BulletShooter == Enums.eShooter.Enemy)
            {
                Visible = getRandomBool();
            }
            else
            {
                Visible = false;
            }
        }

        public void BulletHitBarrierPixel(bool i_BulletHitBarrierPixel)
        {
            if (i_BulletHitBarrierPixel)
            {
                Visible = false;
            }
        }

        private bool getRandomBool()
        {
            bool randomBool = false;

            if (r_Random.NextDouble() * 2 > k_ChanceEnemyBulletToRemain)
            {
                randomBool = true;
            }

            return randomBool;
        }

        public Enums.eShooter ShooterType
        {
            get { return r_BulletShooter; }
        }

        public ICapableShooter Shooter
        {
            get { return m_Shooter; }
            set { m_Shooter = value; }
        }

        public int Direction
        {
            get
            {
                if (r_BulletShooter == Enums.eShooter.Enemy)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}