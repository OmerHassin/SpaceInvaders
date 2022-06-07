using System.Linq;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using InvendersGame;
using Microsoft.Xna.Framework;

namespace InvandersGame.GameObjects
{
    public class BarrierMatrix : CompositeDrawableComponent<Barrier>
    {
        private const string k_AssetNameFormat = @"Sprites\GameObjects\Barrier_44x32_{0}";
        private const float k_SpaceFromBarriers = 1.3f;
        private const int k_NumOfBarriers = 4;
        private const int k_BarrierWidhtSize = 44;

        public BarrierMatrix(Game i_InvadersGame, float i_BarriesAccelerator)
            : base(i_InvadersGame)
        {
            creatBarriers(i_BarriesAccelerator);
        }

        private void creatBarriers(float i_BarriesAccelerator)
        {
            Vector2 delta = new Vector2((-k_BarrierWidhtSize * 2) - (k_SpaceFromBarriers * 1.5f * k_BarrierWidhtSize), 0);

            for (int i = 0; i < k_NumOfBarriers; i++)
            {
                this.Add(createBarrier(i, delta, i_BarriesAccelerator));
                delta.X += k_BarrierWidhtSize + (k_BarrierWidhtSize * k_SpaceFromBarriers);
            }
        }

        private Barrier createBarrier(int i_BarrierNumber, Vector2 i_Delta, float i_BarriesAccelerator)
        {
            string assetName = string.Format(k_AssetNameFormat, i_BarrierNumber + 1);

            return new Barrier(assetName, Game as InvadersGame, i_Delta, i_BarriesAccelerator);
        }

        internal void InitializeNextLevel(float i_BarriesAccelerator)
        {
            foreach (Sprite2D barrier in m_Sprites)
            {
                (barrier as Barrier).InitializeNextLevel(i_BarriesAccelerator);
            }
        }
    }
}