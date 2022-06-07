using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameObjects
{
    public class Barrier : Sprite2D, ICollideble2D
    {
        private const string k_BarrierHitSoundAsset = @"Sounds\BarrierHit";
        private const float k_ShipTextureHeight = 32f;
        private const float k_ShipSpaceFromBorders = 1.6f;
        private const float k_BiteSizeFromBarrier = 0.35f;
        private bool m_BulletHitBarrierPixel;
        private Vector2 m_OriginalPosition;
        private Color[] m_OriginalPixels;

        public Barrier(string i_AssetName, Game i_InvadersGame, Vector2 i_Delta, float i_BarriesAccelerator)
            : base(i_AssetName, i_InvadersGame, i_Delta)
        {
            m_Velocity = new Vector2(35, 0) * i_BarriesAccelerator;
            m_TintColor = Color.White;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_Pixels = new Color[m_Texture.Width * m_Texture.Height];
            m_OriginalPixels = new Color[m_Texture.Width * m_Texture.Height];
            m_Texture.GetData<Color>(m_Pixels);
            m_Texture.GetData<Color>(m_OriginalPixels);

            SoundManager.AddSoundEffect(k_BarrierHitSoundAsset);
        }

        protected override void InitPositions()
        {
            base.InitPositions();

            float x, y;
            x = Game.GraphicsDevice.Viewport.Width / 2;
            y = ((float)Game.GraphicsDevice.Viewport.Height) - (k_ShipTextureHeight * k_ShipSpaceFromBorders) - (2 * Height);

            m_Position = m_OriginalPosition = new Vector2(x + m_Delta.X, y + m_Delta.Y);
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);
            if (m_Position.X >= m_OriginalPosition.X + (this.Width / 2) || m_Position.X <= m_OriginalPosition.X - (this.Width / 2))
            {
                m_Velocity *= -1;
            }
        }

        public void InitializeNextLevel(float i_BarriesAccelerator)
        {
            Position = m_OriginalPosition;
            reInitializeTexture();
            m_Velocity = new Vector2(35, 0) * i_BarriesAccelerator;
        }

        private void reInitializeTexture()
        {
            m_Texture.SetData<Color>(m_OriginalPixels);
            m_Texture.GetData<Color>(m_Pixels);
        }

        public virtual void Collided(ICollidable i_Collidable)
        {
            if (i_Collidable is Bullet)
            {
                gotHitByBullet(i_Collidable as Bullet);
                (i_Collidable as Bullet).BulletHitBarrierPixel(m_BulletHitBarrierPixel);
            }
            else if (i_Collidable is Enemy)
            {
                gotHitByEnemy(i_Collidable as Enemy);
            }
        }

        private void gotHitByBullet(Bullet i_Bullet)
        {
            checkPixelColition(i_Bullet);
            if (m_BulletHitBarrierPixel)
            {
                SetPixels();
                SoundManager.PlayInstance(k_BarrierHitSoundAsset);
            }
        }

        private void checkPixelColition(Bullet i_Bullet)
        {
            Vector2 visibleBarrierPixelPos;
            m_BulletHitBarrierPixel = false;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (this.GetPixlAt(j, i).A != 0)
                    {
                        visibleBarrierPixelPos = new Vector2((int)(Position.X + j), (int)(Position.Y + i));
                        checkPixelIntersectionWithBullet(i_Bullet, visibleBarrierPixelPos);
                    }
                }
            }
        }

        private void checkPixelIntersectionWithBullet(Bullet i_Bullet, Vector2 i_VisibleBarrierPixelPos)
        {
            Vector2 visibleBulletPixelPos;

            for (int i = 0; i < i_Bullet.Height; i++)
            {
                for (int j = 0; j < i_Bullet.Width; j++)
                {
                    visibleBulletPixelPos = new Vector2((int)(i_Bullet.Position.X + j), (int)(i_Bullet.Position.Y + i));
                    if (i_Bullet.GetPixlAt(j, i).A != 0
                        &&
                        visibleBulletPixelPos == i_VisibleBarrierPixelPos)
                    {
                        m_BulletHitBarrierPixel = true;
                        erasePartOfBarrier(i_Bullet, i_VisibleBarrierPixelPos);
                    }
                }
            }
        }

        private void erasePartOfBarrier(Bullet i_Bullet, Vector2 i_BarrierPixelPos)
        {
            Vector2 screenPositonToErase;
            for (int i = 0; i < (int)(i_Bullet.Height * k_BiteSizeFromBarrier); i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    screenPositonToErase = new Vector2(i_BarrierPixelPos.X + j, i_BarrierPixelPos.Y + (i_Bullet.Direction * i));
                    ErasePixel(getBarrierPosAtTexture(screenPositonToErase));
                }
            }
        }

        private Vector2 getBarrierPosAtTexture(Vector2 i_PositonOnScreen)
        {
            Vector2 PixelPositon = new Vector2();

            PixelPositon.X = (int)i_PositonOnScreen.X - (int)m_Position.X;
            PixelPositon.Y = (int)i_PositonOnScreen.Y - (int)m_Position.Y;

            checkAndRepairLegalPos(ref PixelPositon);

            return PixelPositon;
        }

        private void gotHitByEnemy(Enemy i_Enemy)
        {
            for (int i = 0; i < i_Enemy.Width; i++)
            {
                for (int j = 0; j < i_Enemy.Height; j++)
                {
                    ErasePixel(getBarrierPosAtTexture(new Vector2(i_Enemy.Position.X + i, i_Enemy.Position.Y + j)));
                }
            }

            SetPixels();
        }

        private void checkAndRepairLegalPos(ref Vector2 io_PixelPos)
        {
            if (io_PixelPos.X < 0)
            {
                io_PixelPos.X = 0;
            }
            else if (io_PixelPos.X >= m_Texture.Width)
            {
                io_PixelPos.X = m_Texture.Width - 1;
            }

            if (io_PixelPos.Y < 0)
            {
                io_PixelPos.Y = 0;
            }
            else if (io_PixelPos.Y >= m_Texture.Height)
            {
                io_PixelPos.Y = m_Texture.Height - 1;
            }
        }

        protected override void Dispose(bool disposing)
        {
            reInitializeTexture();
            base.Dispose(disposing);
        }
    }
}