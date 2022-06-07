using System;
using System.Collections.Generic;
using System.Text;

namespace InvandersGame.Utils
{
    public class Enums
    {
        public enum eEnemyStyle
        {
            PinkEnemy,
            TealEnemy,
            YellowEnemy,
        }

        public enum eShooter
        {
            Enemy,
            Ship,
        }

        public enum eScreenState
        {
            Activating,
            Active,
            Deactivating,
            Inactive,
            Closing,
            Closed
        }
    }
}
