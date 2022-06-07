using System;
using System.Collections.Generic;
using Infrastructure.Interfaces;
using Infrastructure.ReusableComponents;
using Infrastructure.ReusableComponents.Objects;
using Infrastructure.ReusableComponents.Screens;
using InvandersGame.GameObjects;
using InvandersGame.GameScreens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InvandersGame
{
    public class GameManager : GameService, IGameManager
    {
        private const string k_PlayerOneAssetName = @"Sprites\GameObjects\Ship01_32x32";
        private const string k_PlayerTwoAssetName = @"Sprites\GameObjects\Ship02_32x32";
        private const float k_SpaceBetweenSouls = 1.5f;
        private const double k_BarriersVelocityPace = 1.06;
        private const int k_ShipHeightSize = 32;
        private const int k_EnemiesAdditionalScore = 100;
        private const int k_NumStartingSouls = 3;

        private const Keys k_FirstPlayerLeftKey = Keys.Left;
        private const Keys k_FirstPlayerRightKey = Keys.Right;
        private const Keys k_FirstPlayerShootingKey = Keys.Space;
        private const Keys k_SecondPlayerLeftgKey = Keys.A;
        private const Keys k_SecondPlayerRightKey = Keys.D;
        private const Keys k_SecondPlayerShootingKey = Keys.D1;

        private readonly List<Player> r_Players;

        private ScoreBoard m_ScoreBoard;
        private int m_NumOfPlayers = 1;
        private int m_Level = 1;

        public event EventHandler PlayersDistroid;

        public GameManager(Game i_Game)
            : base(i_Game)
        {
            r_Players = new List<Player>();
        }

        protected override void RegisterAsService()
        {
            this.Game.Services.AddService(typeof(IGameManager), this);
        }

        public void FirsttLevelConstructor(GameScreen i_PlayScreens, out float o_BarriersVelocityPace, out int o_NumberOfAdditionalColumns, out int o_EnemiesAdditionalScore)
        {
            createPlayers();
            GetLevelSetting(out o_BarriersVelocityPace, out o_NumberOfAdditionalColumns, out o_EnemiesAdditionalScore);
            addScoreBoardToScreen(i_PlayScreens as PlayScreen);
        }

        private void createPlayers()
        {
            for (int i = 0; i < NumOfPlayers; i++)
            {
                r_Players.Add(new Player((PlayerIndex)i, k_NumStartingSouls, this));
            }
        }

        public void GetLevelSetting(out float o_BarriersVelocityPace, out int o_NumberOfAdditionalColumns, out int o_EnemiesAdditionalScore)
        {
            int currentLevelUpgrade = (m_Level - 1) % 4;

            o_NumberOfAdditionalColumns = currentLevelUpgrade;
            o_EnemiesAdditionalScore = k_EnemiesAdditionalScore * currentLevelUpgrade;

            if (currentLevelUpgrade - 1 < 0)
            {
                o_BarriersVelocityPace = 0;
            }
            else
            {
                o_BarriersVelocityPace = (float)Math.Pow(k_BarriersVelocityPace, currentLevelUpgrade - 1);
            }
        }

        private void addScoreBoardToScreen(PlayScreen i_PlayScreens)
        {
            i_PlayScreens.Add(m_ScoreBoard = new ScoreBoard(Game, r_Players, k_NumStartingSouls));
        }

        public void CalculateScore(Sprite2D i_Sprite2D, ICollidable i_CollidedWithBullet)
        {
            if ((i_Sprite2D as Bullet).Shooter is Ship)
            {
                GetPlayer(((i_Sprite2D as Bullet).Shooter as Ship).PlayerIndex).Score += i_CollidedWithBullet.Score;
            }
        }

        public Player GetPlayer(PlayerIndex i_PlayerIndex)
        {
            return r_Players[(int)i_PlayerIndex];
        }

        public void PlayerGotHit(Sprite2D i_Sprite2D)
        {
            GetPlayer((i_Sprite2D as Ship).PlayerIndex).GotHit((i_Sprite2D as Ship).Score);
            checkPlayersDistroid();
        }

        private void checkPlayersDistroid()
        {
            bool playersDistroid = true;

            foreach (Player player in r_Players)
            {
                if (!player.IsDead())
                {
                    playersDistroid = false;
                }
            }

            if (playersDistroid)
            {
                OnPlayersDistroid();
            }
        }

        private void OnPlayersDistroid()
        {
            PlayersDistroid?.Invoke(this, EventArgs.Empty);
        }

        public string ScoresToString()
        {
            string msg = string.Empty;
            foreach (Player player in r_Players)
            {
                msg += player.ScoreToString();
            }

            return msg;
        }

        public string Winner()
        {
            string msg = "Nobody";
            int highestScore = 0;

            foreach (Player player in r_Players)
            {
                if (player.Score > highestScore)
                {
                    highestScore = player.Score;
                    msg = player.ToString();
                }
            }

            if (checkTie(highestScore))
            {
                msg = "Nobody it's a TIE!";
            }

            return msg;
        }

        private bool checkTie(int i_HighestScore)
        {
            bool tie = false;
            int SameToScore = 0;

            foreach (Player player in r_Players)
            {
                if (player.Score == i_HighestScore)
                {
                    SameToScore++;
                }
            }

            if (SameToScore > 1)
            {
                tie = true;
            }

            return tie;
        }

        public void ResetGameSettings()
        {
            m_Level = 1;
            r_Players.Clear();
            m_ScoreBoard.Clear();
        }

        public int NumOfPlayers
        {
            get { return m_NumOfPlayers; }
            set { m_NumOfPlayers = value; }
        }

        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public string PlayerOneAsset
        {
            get { return k_PlayerOneAssetName; }
        }

        public string PlayerTwoAsset
        {
            get { return k_PlayerTwoAssetName; }
        }

        public void LoadPlayerControls(PlayerIndex i_PlayerIndex, out Keys o_LeftKey, out Keys o_RightKey, out Keys o_ShootingKey, out bool o_MouseMode)
        {
            if (i_PlayerIndex == PlayerIndex.One)
            {
                o_LeftKey = k_FirstPlayerLeftKey;
                o_RightKey = k_FirstPlayerRightKey;
                o_ShootingKey = k_FirstPlayerShootingKey;
                o_MouseMode = true;
            }
            else
            {
                o_LeftKey = k_SecondPlayerLeftgKey;
                o_RightKey = k_SecondPlayerRightKey;
                o_ShootingKey = k_SecondPlayerShootingKey;
                o_MouseMode = false;
            }
        }

        public string GetAssetByPlayerIndex(int i_PlayerIndex)
        {
            string assetName = k_PlayerOneAssetName;

            if (i_PlayerIndex > 0)
            {
                assetName = k_PlayerTwoAssetName;
            }

            return assetName;
        }

        public Color GetColorByPlayerIndex(int i_PlayerIndex)
        {
            Color playercolor = Color.Blue;

            if (i_PlayerIndex > 0)
            {
                playercolor = Color.Green;
            }

            return playercolor;
        }
    }
}