using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarkanaServer
{
    static class ConfigGame
    {
        /* Players initial Stats*/
        public static float lifeMax = 100;
        public static float regLife = 0.1f;
        public static float manaMax = 999;
        public static float regMana = 1;

        /* GameConfig  */
        public static int totalRoundNumber;
        public static int goldRound;
        public static int goldFirstBlood;
        public static int goldDoubleKill;
        public static int goldTripleKill;
        //public static int goldGameChanger;
        public static int goldNoKill;
        public static int goldKill;
        public static int initialGold;
        public static float timeTotalToPause;
    }
}
