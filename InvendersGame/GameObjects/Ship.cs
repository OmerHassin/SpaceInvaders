using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using InvandersGame.GameScreens;
using Infrastructure.ReusableComponents.Animators.ConcreteAnimators;
using Infrastructure.ReusableComponents;
using Infrastructure.Interfaces;
using InvandersGame.Utils;
using Infrastructure.ReusableComponents.Objects;

namespace InvandersGame.GameObjects
{
    public class Ship : Sprite2D, ICollideble2D, ICapableShooter
    {
        private const eInputButtons k_MouseShootingButton = eInputButtons.Left;
        private const string k_GunShootAsset = @"Sounds\SSGunShot";
        private const string k_LifeDiessetAsset = @"Sounds\LifeDie";
        private const float k_ShipVelocity = 140;
        private const float k_InitSpaceFromBorders = 1.6f;
        private const float k_ShipRotatePace = 6;
        private const int k_ShipScoreValue = -600;
        private const int k_BulletDirection = -1;
        private const byte k_MaxBulletsOnScreen = 20;

        private readonly TimeSpan r_LastAnimationLenght = TimeSpan.FromSeconds(2.6);
        private readonly TimeSpan r_AnimationLenght = TimeSpan.FromSeconds(2);
        private readonly TimeSpan r_BlinkPace = TimeSpan.FromSeconds(0.125);
        private readonly ShootingMachine r_ShootingMachine;
        private readonly PlayerIndex r_PlayerIndex;
        private readonly Color r_BulletTintColor = Color.Red;

        private IInputManager m_InputManager;
        private bool m_ShootAvailble;
        private bool m_MouseMode;
        private Keys m_Leftkey;
        private Keys m_Rightkey;
        private Keys m_Shootingkey;

        public Ship(string i_AssetName, Game i_InvadersGame, Vector2 i_Delta, PlayerIndex i_PlayerIndex, PlayScreen i_PlayScreens)
            : base(i_AssetName, i_InvadersGame, i_Delta)
        {
            m_TintColor = Color.White;
            m_ScoreValue = k_ShipScoreValue;
            r_PlayerIndex = i_PlayerIndex;
            m_ShootAvailble = true;
            i_PlayScreens.Add(r_ShootingMachine = new ShootingMachine(i_InvadersGame, k_MaxBulletsOnScreen, Enums.eShooter.Ship, k_GunShootAsset));
            GameManager.LoadPlayerControls(i_PlayerIndex, out m_Leftkey, out m_Rightkey, out m_Shootingkey, out m_MouseMode);
        }

        public Ship(Game i_InvadersGame, Vector2 i_Delta, PlayerIndex i_PlayerIndex, PlayScreen i_PlayScreens)
            : this(((IGameManager)i_InvadersGame.Services.GetService(typeof(IGameManager))).GetAssetByPlayerIndex((int)i_PlayerIndex), i_InvadersGame, i_Delta, i_PlayerIndex, i_PlayScreens)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            m_InputManager = Game.Services.GetService(typeof(IInputManager)) as IInputManager;
            initAnimations();
            GameManager.GetPlayer(r_PlayerIndex).SecondDeath += Ship_SecondDeath;
        }

        private void initAnimations()
        {
            BlinkAnimator blinkAnimator = new BlinkAnimator(r_BlinkPace, r_AnimationLenght);

            m_Animations.AnimationLength = r_AnimationLenght;
            m_Animations.Add(blinkAnimator);
            m_Animations.Enabled = false;
            m_Animations.Finished += Animations_Finished;
        }

        private void Ship_SecondDeath(object sender, EventArgs e)
        {
            RotatorAnimator rotatorAnimator = new RotatorAnimator(k_ShipRotatePace, r_LastAnimationLenght);
            FaderAnimator faderAnimator = new FaderAnimator(r_LastAnimationLenght);

            m_Animations.AnimationLength = r_LastAnimationLenght;
            m_Animations.Remove("Blink");
            m_Animations.Add(faderAnimator);
            m_Animations.Add(rotatorAnimator);
            m_Animations.Enabled = false;
        }

        private void Animations_Finished(object sender, EventArgs e)
        {
            GameManager.PlayerGotHit(this);
            SoundManager.PlayInstance(k_LifeDiessetAsset);
            if (GameManager.GetPlayer(r_PlayerIndex).IsDead())
            {
                Visible = false;
            }
            else
            {
                resetShip();
            }
        }

        private void resetShip()
        {
            InitPositions();
            m_ShootAvailble = true;
            m_Animations.Reset();
            m_Animations.Stop();
        }

        protected override void InitPositions()
        {
            base.InitPositions();

            float x, y;

            x = (m_Texture.Width * k_InitSpaceFromBorders) + m_Delta.X;
            y = ((float)Game.GraphicsDevice.Viewport.Height) - (Height * k_InitSpaceFromBorders);
            m_Position = new Vector2(x, y);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            SoundManager.AddSoundEffect(k_LifeDiessetAsset);
        }

        public override void Update(GameTime i_GameTime)
        {
            base.Update(i_GameTime);

            updateVelocityByKeyBoard();
            updateVelocityByMaouse();
            m_Position.X = MathHelper.Clamp(m_Position.X, 0, Game.GraphicsDevice.Viewport.Width - m_Texture.Width);
            checkIfShootNeeded();
        }

        private void updateVelocityByKeyBoard()
        {      
            if (m_InputManager.KeyHeld(m_Rightkey))
            {
                m_Velocity.X = k_ShipVelocity;
            }
            else if (m_InputManager.KeyHeld(m_Leftkey))
            {
                m_Velocity.X = -k_ShipVelocity;
            }
            else
            {
                m_Velocity.X = 0;
            }
        }

        private void updateVelocityByMaouse()
        {
            if (m_MouseMode && m_InputManager.PrevMouseState.X != 0)
            {              
                m_Position.X += m_InputManager.MousePositionDelta.X;
            }
        }

        private void checkIfShootNeeded()
        {
            if (m_ShootAvailble
                &&
                (m_InputManager.KeyPressed(m_Shootingkey)
                ||
                (m_MouseMode && m_InputManager.ButtonPressed(k_MouseShootingButton))))
            {
                r_ShootingMachine.Shoot(GetPositionForShootiong(), this);
            }
        }

        public Vector2 GetPositionForShootiong()
        {
            return new Vector2(m_Position.X + (m_Texture.Width / 2), m_Position.Y);
        }

        public override void ReInitialize()
        {
            resetShip();
            r_ShootingMachine.Reset();
        }

        public virtual void Collided(ICollidable i_Collidable)
        {
            if (i_Collidable is Bullet)
            {
                if ((i_Collidable as Bullet).ShooterType == Enums.eShooter.Enemy)
                {
                    ShipGotHit();
                }
            }
        }

        public void ShipGotHit()
        {
            m_ShootAvailble = false;
            m_Animations.Start();
        }

        public PlayerIndex PlayerIndex
        {
            get { return this.r_PlayerIndex; }
        }

        public Color BulletTintColor
        {
            get { return r_BulletTintColor; }
        }

        public int BulletDirection
        {
            get { return k_BulletDirection; }
        }

        protected override void Dispose(bool disposing)
        {
            r_ShootingMachine.Reset();
            base.Dispose(disposing);
        }
    }
}