using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents;
using InvandersGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace InvandersGame.GameObjects
{
    public class ShootingMachine : CompositeDrawableComponent<Bullet>
    {
        private readonly int r_MaxBulletsOnScreen;
        private readonly Enums.eShooter r_BulletShooter;
        private readonly Stack<Bullet> r_BulletsReadyToShoot; 
        private readonly List<Bullet> r_BulletsOnScreen;

        private SoundEffect m_GunShoot;
        private string m_GunShootAsset;

        public ShootingMachine(Game i_InvadersGame, int i_MaxBulletsOnScreen, Enums.eShooter i_BulletShooter, string? i_GunShootAsset)
            : base(i_InvadersGame)
        {
            r_MaxBulletsOnScreen = i_MaxBulletsOnScreen;
            r_BulletShooter = i_BulletShooter;
            r_BulletsReadyToShoot = new Stack<Bullet>();
            r_BulletsOnScreen = new List<Bullet>();
            m_GunShootAsset = i_GunShootAsset;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            m_GunShoot = Game.Content.Load<SoundEffect>(m_GunShootAsset);
        }

        public bool Shoot(Vector2 i_Delta, ICapableShooter i_Shooter)
        {
            bool shootSucceed = false;
            Bullet bulletToShoot;

            if (r_BulletsOnScreen.Count < r_MaxBulletsOnScreen)
            {
                if (r_BulletsReadyToShoot.Count > 0)
                {
                    bulletToShoot = r_BulletsReadyToShoot.Pop();
                    bulletToShoot.ReInitialize(i_Delta, i_Shooter);
                }
                else
                {
                    bulletToShoot = new Bullet(Game, i_Delta, r_BulletShooter, i_Shooter);
                }

                loadBullet(bulletToShoot);              
                shootSucceed = true;
                playSound();
            }

            return shootSucceed;
        }

        private void loadBullet(Bullet bulletToShoot)
        {
            this.Add(bulletToShoot);
            r_BulletsOnScreen.Add(bulletToShoot);
            bulletToShoot.VisibleChanged += BulletToShoot_VisibleChanged;
        }

        private void playSound()
        {
            ISoundManager soundManager = Game.Services.GetService(typeof(ISoundManager)) as ISoundManager;
            SoundEffectInstance gunShootInstanse = m_GunShoot.CreateInstance();
            if (!soundManager.Mute)
            {
                gunShootInstanse.Volume = (Game.Services.GetService(typeof(ISoundManager)) as ISoundManager).EffectsVolume / 100;
            }
            else
            {
                gunShootInstanse.Volume = 0;
            }

            gunShootInstanse.Play();
        }

        private void BulletToShoot_VisibleChanged(object sender, EventArgs e)
        {
            Bullet bullet = sender as Bullet;

            if (!bullet.Visible)
            {
                this.Remove(bullet);
                r_BulletsOnScreen.Remove(bullet);
                r_BulletsReadyToShoot.Push(bullet);
                bullet.VisibleChanged -= BulletToShoot_VisibleChanged;
                enemyHandler(bullet);
            }
        }

        private void enemyHandler(Bullet i_Bullet)
        {
            if (r_BulletShooter == Enums.eShooter.Enemy)
            {
                (i_Bullet.Shooter as Enemy).BulletOnScreen = false;
            }
        }

        public bool ReadyToShoot()
        {
            bool readyToShoot = false;

            if (r_BulletsOnScreen.Count < r_MaxBulletsOnScreen)
            {
                readyToShoot = true;
            }

            return readyToShoot;
        }

        public void Reset()
        {
            foreach (Bullet bullet in r_BulletsOnScreen.ToList())
            {
                bullet.Visible = false;
            }
        }
    }
}