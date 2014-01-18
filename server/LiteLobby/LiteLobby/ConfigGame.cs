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
        public static int totalRoundNumber = 10;
        public static int goldRound = 5;
        public static int goldFirstBlood = 3;
        public static int goldDoubleKill = 3;
        public static int goldTripleKill = 5;
        //public static int goldGameChanger;
        public static int goldNoKill = 1;
        public static int goldKill = 3;
        public static int initialGold = 15;
        public static float timeTotalToPause = 15;
    }
}
