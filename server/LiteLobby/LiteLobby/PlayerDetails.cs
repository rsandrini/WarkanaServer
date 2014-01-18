using Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarkanaServer
{
    public enum StatusPlayerInGame
    {
        live,
        dead,
        waitRoundStart,
    }


   public  class PlayerDetails
    {
        // Temp Data - This is replicated in TurnPlayer class
        public float damageCaused;
        public float damageReceived;
        public int kills;
        public float timeDead; // Temp var
        public string deathBy;

        public string name;
        public int level;
        public int xp;
        public float lifeMax;
        public float life;
        public float regLife;
        public float mana;
        public float manaMax;
        public float regMana;
        public int gold;
        public bool isReady;
        public bool outTerrain;

        public Actor actor;
        public StatusPlayerInGame statusPlayerInGame;

        // Add class to control spell's

        public PlayerDetails(Actor actor)
        {
            level = 1;
            xp = 0;

            life = ConfigGame.lifeMax;
            lifeMax = ConfigGame.lifeMax;

            mana = ConfigGame.manaMax;
            manaMax = ConfigGame.manaMax;

            regLife = ConfigGame.regLife;
            regMana = ConfigGame.regMana;

            gold = ConfigGame.initialGold;

            isReady = false;
            resetStatistics();
        }

        public void resetStatistics()
        {
            damageCaused = 0;
            damageReceived = 0;
            kills = 0;
            timeDead = 0;
            deathBy = String.Empty;
            statusPlayerInGame = StatusPlayerInGame.waitRoundStart;
            outTerrain = false;
        }

        void adjustMaxValues()
        {
            if (life > lifeMax)
                life = lifeMax;

            if (mana > manaMax)
                mana = manaMax;
        }

        public void ReceiveDamage(float value)
        {
            damageReceived += value;
            if (life > 0)
                life -= value;

            if (life < 0)
            {
                life = 0;
                statusPlayerInGame = StatusPlayerInGame.dead;
            }
        }

        public void addKill()
        {
            kills++;
            gold += ConfigGame.goldKill;
        }

        public void addFirstBlood()
        {
            gold += ConfigGame.goldFirstBlood;
        }

        public void addExtraKill(string typeKill)
        {
            if (typeKill.Equals("double"))
                gold += ConfigGame.goldDoubleKill;
            if (typeKill.Equals("triple"))
                gold += ConfigGame.goldTripleKill;
        }

        public void addExtraGold(string type)
        {
            if (type.Equals("nokill"))
                gold += ConfigGame.goldNoKill;
            if (type.Equals("round"))
                gold += ConfigGame.goldRound;
        }

        public void setKiller(string _deathBy, float _timeDeath)
        {
            deathBy = _deathBy;
            timeDead = _timeDeath;
        }
    }
}
