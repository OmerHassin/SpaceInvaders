using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infrastructure.ReusableComponents.Objects
{
    public class TextBlockcs : Sprite2D
    {
        private string m_TextToDraw;
        private SpriteFont m_SpriteFont;

        public TextBlockcs(string i_AssetName, Game i_Game, Vector2 i_Delta, string i_TextToDraw, Color i_TintColor)
            : base(i_AssetName, i_Game, i_Delta)
        {
            m_TextToDraw = i_TextToDraw;
            m_TintColor = i_TintColor;
        }

        public TextBlockcs(string i_AssetName, Game i_Game, string i_TextToDraw, Color i_TintColor)
            : base(i_AssetName, i_Game)
        {
            m_TextToDraw = i_TextToDraw;
            m_TintColor = i_TintColor;
        }

        public TextBlockcs(string i_AssetName, Game i_Game, string i_TextToDraw)
           : base(i_AssetName, i_Game)
        {
            m_TextToDraw = i_TextToDraw;
        }

        protected override void LoadContent()
        {
            m_SpriteFont = Game.Content.Load<SpriteFont>(m_AssetName);

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
        }

        protected override void InitPositions()
        {
            m_WidthBeforeScale = Width;
            m_HeightBeforeScale = Height;
            m_Position = m_Delta;
        }

        public override void Draw(GameTime i_GameTime)
        {
            m_SpriteBatch.DrawString(m_SpriteFont, m_TextToDraw, PositionForDraw,
                m_TintColor, Rotation, RotationOrigin, Scales
                , SpriteEffects.None, LayerDepth);
        } 

        public string Text
        {
            get { return m_TextToDraw; }
            set { m_TextToDraw = value; }
        }

        public override float Width
        {
            get { return m_SpriteFont.MeasureString(m_TextToDraw).X; }
        }

        public override float Height
        {
            get { return m_SpriteFont.MeasureString(m_TextToDraw).Y; }
        }
    }
}