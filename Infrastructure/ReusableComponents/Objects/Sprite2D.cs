using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents.Animators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infrastructure.ReusableComponents.Objects
{
    public class Sprite2D : LoadableDrawableComponent
    {
        protected CompositeAnimator m_Animations;
        protected SpriteBatch m_SpriteBatch; 
        protected Texture2D m_Texture;
        protected Vector2 m_RotationOrigin = Vector2.Zero;
        protected Vector2 m_Delta = Vector2.Zero;
        protected Vector2 m_PositionOrigin;
        protected Vector2 m_Position = Vector2.Zero;
        protected Vector2 m_Velocity;
        protected Vector2 m_Size2D;
        protected Vector2 m_Scales = Vector2.One;
        protected Rectangle m_Bounds;
        protected Rectangle m_SourceRectangle; 
        protected Color m_TintColor = Color.White;
        protected Color[] m_Pixels;   
        protected bool m_JumpingSprite;
        protected float m_WidthBeforeScale;
        protected float m_HeightBeforeScale;
        protected float m_Rotation = 0;
        protected float m_AngularVelocity;
        protected float m_LayerDepth;
        protected int m_ScoreValue;

        public Sprite2D(string i_AssetName, Game i_Game)
            : base(i_AssetName, i_Game, int.MaxValue)
        {
        }

        public Sprite2D(string i_AssetName, Game i_Game, int i_CallsOrder)
            : base(i_AssetName, i_Game, i_CallsOrder)
        { 
        }

        public Sprite2D(string i_AssetName, Game i_Game, Vector2 i_Delta)
            : base(i_AssetName, i_Game, int.MaxValue)
        {
            m_Delta = i_Delta;
        }

        public override void Initialize()
        {
            m_Animations = new CompositeAnimator(this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
           m_Texture = Game.Content.Load<Texture2D>(m_AssetName);

            if (m_SpriteBatch == null)
            {
                m_SpriteBatch =
                    Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

                if (m_SpriteBatch == null)
                {
                    m_SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
                    m_UseSharedBatch = false;
                }
            }

            base.LoadContent();
        }

        protected virtual void InitPositions()
        {
            m_WidthBeforeScale = m_Texture.Width;
            m_HeightBeforeScale = m_Texture.Height;
        }

        protected override void InitBounds()
        {
            InitPositions();
            InitSourceRectangle();
        }
        
        protected virtual void InitSourceRectangle()
        {
            m_SourceRectangle = new Rectangle(0, 0, (int)m_WidthBeforeScale, (int)m_HeightBeforeScale);
        }

        public override void Update(GameTime i_GameTime)
        {
            float totalSeconds = (float)i_GameTime.ElapsedGameTime.TotalSeconds;

            if (m_JumpingSprite)
            {
                SpriteJump(i_GameTime);               
            }
            else
            {
                Position += m_Velocity * totalSeconds;
            }

            Animations.Update(i_GameTime);

            base.Update(i_GameTime);
        }

        public virtual void ReInitialize()
        {
        }

        public virtual void SpriteJump(GameTime i_GameTime)
        {
        }

        class DeviceStates
        {
            public BlendState BlendState;
            public SamplerState SamplerState;
            public DepthStencilState DepthStencilState;
            public RasterizerState RasterizerState;
        }

        DeviceStates m_SavedDeviceStates = new DeviceStates();
        protected void saveDeviceStates()
        {
            m_SavedDeviceStates.BlendState = GraphicsDevice.BlendState;
            m_SavedDeviceStates.SamplerState = GraphicsDevice.SamplerStates[0];
            m_SavedDeviceStates.DepthStencilState = GraphicsDevice.DepthStencilState;
            m_SavedDeviceStates.RasterizerState = GraphicsDevice.RasterizerState;
        }

        protected void restoreDeviceStates()
        {
            GraphicsDevice.BlendState = m_SavedDeviceStates.BlendState;
            GraphicsDevice.SamplerStates[0] = m_SavedDeviceStates.SamplerState;
            GraphicsDevice.DepthStencilState = m_SavedDeviceStates.DepthStencilState;
            GraphicsDevice.RasterizerState = m_SavedDeviceStates.RasterizerState;
        }

        protected bool m_SaveAndRestoreDeviceState = false;
        public bool SaveAndRestoreDeviceState
        {
            get { return m_SaveAndRestoreDeviceState; }
            set { m_SaveAndRestoreDeviceState = value; }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!m_UseSharedBatch)
            {
                if (SaveAndRestoreDeviceState)
                {
                    saveDeviceStates();
                }

                m_SpriteBatch.Begin(
                    SortMode, BlendState, SamplerState,
                    DepthStencilState, RasterizerState, Shader, TransformMatrix);
            }

            m_SpriteBatch.Draw(m_Texture, this.PositionForDraw,
                 this.SourceRectangle, this.TintColor,
                this.Rotation, this.RotationOrigin, this.Scales,
                SpriteEffects.None, this.LayerDepth);

            if (!m_UseSharedBatch)
            {
                m_SpriteBatch.End();

                if (SaveAndRestoreDeviceState)
                {
                    restoreDeviceStates();
                }
            }

            base.Draw(gameTime);
        }

        public virtual bool CheckCollision(ICollidable i_Source)
        {
            bool collided = false;
            ICollideble2D source = i_Source as ICollideble2D;
            if (source != null)
            {
                collided = Bounds.Intersects(source.Bounds) || Bounds.Contains(source.Bounds);
            }

            return collided;
        }

        protected bool m_UseSharedBatch = false;
        public SpriteBatch SpriteBatch
        {
            set
            {
                m_SpriteBatch = value;
                m_UseSharedBatch = true;
            }
        }

        protected SpriteSortMode m_SortMode = SpriteSortMode.Deferred;
        public SpriteSortMode SortMode
        {
            get { return m_SortMode; }
            set { m_SortMode = value; }
        }

        protected BlendState m_BlendState = BlendState.AlphaBlend;
        public BlendState BlendState
        {
            get { return m_BlendState; }
            set { m_BlendState = value; }
        }

        protected SamplerState m_SamplerState = null;
        public SamplerState SamplerState
        {
            get { return m_SamplerState; }
            set { m_SamplerState = value; }
        }

        protected DepthStencilState m_DepthStencilState = null;
        public DepthStencilState DepthStencilState
        {
            get { return m_DepthStencilState; }
            set { m_DepthStencilState = value; }
        }

        protected RasterizerState m_RasterizerState = null;
        public RasterizerState RasterizerState
        {
            get { return m_RasterizerState; }
            set { m_RasterizerState = value; }
        }

        protected Effect m_Shader = null;
        public Effect Shader
        {
            get { return m_Shader; }
            set { m_Shader = value; }
        }

        protected Matrix m_TransformMatrix = Matrix.Identity;
        public Matrix TransformMatrix
        {
            get { return m_TransformMatrix; }
            set { m_TransformMatrix = value; }
        }

        public Color TintColor
        {
            get { return m_TintColor; }
            set { m_TintColor = value; }
        }

        public virtual Vector2 Velocity
        {
            get { return this.m_Velocity; }
            set { this.m_Velocity = value; }
        }

        public virtual Vector2 Position
        {
            get { return this.m_Position; }
            set
            {
                if (m_Position != value)
                {
                    m_Position = value;
                    OnPositionChanged();
                }
            }
        }

        public Vector2 PositionOrigin
        {
            get { return m_PositionOrigin; }
            set { m_PositionOrigin = value; }
        }

        public Vector2 PositionForDraw
        {
            get { return this.Position - this.PositionOrigin + this.RotationOrigin; }
        }

        public virtual Vector2 Size2D
        {
            get { return new Vector2(this.Height, this.Width); }
        }

        public Vector2 Scales
        {
            get { return m_Scales; }
            set
            {
                if (m_Scales != value)
                {
                    m_Scales = value;
                    OnPositionChanged();
                }
            }
        }

        public virtual float Width
        {
            get { return m_WidthBeforeScale * m_Scales.X; }
            set { m_WidthBeforeScale = value / m_Scales.X; }
        } 

        public virtual float Height
        {
            get { return m_HeightBeforeScale * m_Scales.Y; }
            set { m_HeightBeforeScale = value / m_Scales.Y; }
        }

        public Rectangle SourceRectangle
        {
            get { return m_SourceRectangle; }
            set { m_SourceRectangle = value; }
        }

        public Vector2 SourceRectangleCenter
        {
            get { return new Vector2((float)(m_SourceRectangle.Width / 2), (float)(m_SourceRectangle.Height / 2)); }
        }

        public Vector2 Delta
        {
            get { return this.m_Delta; }
            set { this.m_Delta = value; }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)m_Position.X,
                    (int)m_Position.Y,
                    (int)this.Width,
                    (int)this.Height);
            }
        }

        public float Opacity
        {
            get { return (float)m_TintColor.A / (float)byte.MaxValue; }
            set
            {
                if ((value * (float)byte.MaxValue) < 0)
                {
                    m_TintColor.A = 0;
                }
                else
                {
                    m_TintColor.A = (byte)(value * (float)byte.MaxValue);
                }
            }
        }

        public float Rotation
        {
            get { return m_Rotation; }
            set { m_Rotation = value; }
        }

        public Vector2 RotationOrigin
        {
            get { return m_RotationOrigin; }
            set { m_RotationOrigin = value; }
        }

        public float LayerDepth
        {
            get { return m_LayerDepth; }
            set { m_LayerDepth = value; }
        }

        public ISoundManager SoundManager
        {
            get { return Game.Services.GetService(typeof(ISoundManager)) as ISoundManager; }
        }

        public IGameManager GameManager
        {
            get { return Game.Services.GetService(typeof(IGameManager)) as IGameManager; }
        }

        public int Score
        {
            get { return this.m_ScoreValue; }
            set { m_ScoreValue = value; }
        }

        public CompositeAnimator Animations
        {
            get { return m_Animations; }
            set { m_Animations = value; }
        }

        public Color GetPixlAt(int i_Col, int i_Row)
        {
            return m_Pixels[(i_Row * (int)Width) + i_Col];
        }

        public void ErasePixel(Vector2 i_PixelPos)
        {
            m_Pixels[((int)i_PixelPos.Y * (int)Width) + (int)i_PixelPos.X] = Color.FromNonPremultiplied(0, 0, 0, 0);
        }

        public void SetPixels()
        {
            m_Texture.SetData<Color>(m_Pixels);
        }

        public Sprite2D ShallowClone()
        {
            return this.MemberwiseClone() as Sprite2D;
        }
    }
}