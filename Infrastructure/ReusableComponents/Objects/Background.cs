using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infrastructure.ReusableComponents.Objects
{
    public class Background : Sprite2D
    {
        private const string k_AssetName = @"Sprites\GameObjects\BG_Space01_1024x768";

        public Background(Game i_InvadersGame)
            : base(k_AssetName, i_InvadersGame, int.MinValue)
        {
        }

        public Background(Game i_InvadersGame, Color i_TintColor)
            : this(i_InvadersGame)
        {
            TintColor = i_TintColor;
        }

        protected override void InitBounds()
        {
            base.InitBounds();

            this.DrawOrder = int.MinValue;
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

            m_SpriteBatch.Draw(m_Texture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), TintColor);

            if (!m_UseSharedBatch)
            {
                m_SpriteBatch.End();

                if (SaveAndRestoreDeviceState)
                {
                    this.restoreDeviceStates();
                }
            }
        }
    }
}