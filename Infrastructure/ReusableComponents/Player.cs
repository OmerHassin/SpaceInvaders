using System;
using Infrastructure.Interfaces;
using Microsoft.Xna.Framework;

namespace Infrastructure.ReusableComponents
{
    public class Player
    {
        private const string k_FontText = @"P{0} Score: {1}";

        private readonly PlayerIndex r_PlayerIndex;

        private Color m_PlayerColor;
        private string m_AssetName;
        private int m_Score = 0;
        private int m_Souls;

        public event EventHandler SecondDeath;

        public event EventHandler ScoreChanged;

        public event EventHandler NumSoulesChanged;

        public Player(PlayerIndex i_PlayerIndex, int i_NumStartingSouls, IGameManager i_GameManager)
        {
            r_PlayerIndex = i_PlayerIndex;
            m_Souls = i_NumStartingSouls;
            m_PlayerColor = i_GameManager.GetColorByPlayerIndex((int)i_PlayerIndex);
            m_AssetName = i_GameManager.GetAssetByPlayerIndex((int)i_PlayerIndex);
        }

        public void GotHit(int i_Score)
        {
            Score += i_Score;
            Souls--;
        }

        public bool IsDead()
        {
            return m_Souls == 0;
        }

        public int Score
        {
            get { return this.m_Score; }
            set
            {
                m_Score = MathHelper.Clamp(value, 0, int.MaxValue);
                OnScoreChanged();
            }
        }

        public int Souls
        {
            get { return this.m_Souls; }
            set
            {
                if (m_Souls != value)
                {
                    this.m_Souls = value;
                    OnNumSoulesChanged();
                }

                if (value == 1)
                {
                    OnSecondDeath();
                }
            }
        }

        private void OnSecondDeath()
        {
            SecondDeath?.Invoke(this, EventArgs.Empty);
        }

        private void OnNumSoulesChanged()
        {
            NumSoulesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnScoreChanged()
        {
            ScoreChanged?.Invoke(this, EventArgs.Empty);
        }

        public string AssetName
        {
            get { return this.m_AssetName; }
        }

        public int Index
        {
            get { return (int)r_PlayerIndex; }
        }

        public Color Color
        {
            get { return m_PlayerColor; }
        }

        public string ScoreToString()
        {
            string scoreToString = string.Format(
@"Player {0} Score is {1}
",
            r_PlayerIndex, Score);
            return scoreToString;
        }

        public override string ToString()
        {
            return string.Format(@"Player {0}", r_PlayerIndex);
        }
    }
}