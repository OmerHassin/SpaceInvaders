using System;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Infrastructure.Interfaces
{
    public interface IGameManager
    {
        event EventHandler PlayersDistroid;
        void LoadPlayerControls(PlayerIndex i_PlayerIndex, out Keys o_LeftKey, out Keys o_RightKey, out Keys o_ShootingKey, out bool o_MouseMode);
        void CalculateScore(Sprite2D i_Sprite2D, ICollidable i_CollidedWithBullet);
        void FirsttLevelConstructor(GameScreen i_PlayScreens, out float m_BarriersVelocityPace, out int m_NumberOfAdditionalColumns, out int m_EnemiesAdditionalScore);
        void GetLevelSetting(out float o_BarriersVelocityPace, out int o_NumberOfAdditionalColumns, out int o_EnemiesAdditionalScore);
        Player GetPlayer(PlayerIndex i_PlayerIndex);
        void PlayerGotHit(Sprite2D i_Sprite2D);
        string ScoresToString();
        string Winner();
        int NumOfPlayers { get; set; }
        int Level { get; set; }
        string PlayerOneAsset { get; }
        string PlayerTwoAsset { get; }
        void ResetGameSettings();
        string GetAssetByPlayerIndex(int i_PlayerIndex);
        Color GetColorByPlayerIndex(int i_PlayerIndex);
    }
}
