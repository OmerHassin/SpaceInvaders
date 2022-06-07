using Infrastructure.ReusableComponents.Objects;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameObjects
{
    public class ShipSoul : Sprite2D
    {
        private const int k_SpaceBetweenSouls = 8;
        private const float k_SoulOpacity = 0.5f;

        public ShipSoul(string i_AssetName, Game i_InvadersGame, Vector2 i_Delta, Vector2 i_Scales)
            : base(i_AssetName, i_InvadersGame, i_Delta)
        {
            m_Scales = i_Scales;
            m_TintColor = Color.White;
            Opacity = k_SoulOpacity;
        }      

        protected override void InitPositions()
        {
            base.InitPositions();

            float x, y;

            x = Game.GraphicsDevice.Viewport.Width - (k_SpaceBetweenSouls + (Width / 2));
            y = k_SpaceBetweenSouls;
            m_Position = new Vector2(x + m_Delta.X, y + m_Delta.Y);
        }
    }
}