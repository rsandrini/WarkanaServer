using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lite
{
    public enum statusGame
    {
        awaitingServer,
        initialRound,
        pause,
        preRound,
        playing,
        resume
    };

    class GameController
    {
        public int totalRoundNumber;
        public int currentRound;
        public int goldRound;
        public int goldFirstBlood;
        public int goldDoubleKill;
        public int goldTripleKill;
        public int goldGameChanger;
        public int goldNoKill;
        public int goldKill;
        public int initialGold;
        public float timeTotalToPause;
        public statusGame statusGame = statusGame.awaitingServer;
        public float currentTimer;

        public GameController()
        {
            totalRoundNumber = 10;
            currentRound = 0;
            goldRound = 10;
            goldFirstBlood = 5;
            goldDoubleKill = 5;
            goldTripleKill = 8;
            goldGameChanger = 3;
            goldNoKill = 2;
            goldKill = 3;
            initialGold = 10;
            timeTotalToPause = 25;
            currentTimer = 0;
        }




    }
}
