using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lite
{
    class PlayerDetails
    {

        //public PlayerScript player;
        public float damageCaused;
        public float damageReceived;
        public int kills;
        public float timeDead; // Temp var
        public string deathBy;

        public PlayerDetails()
        {
            //this.player = player;
            this.damageCaused = 0;
            this.damageReceived = 0;
            this.kills = 0;
            this.timeDead = 0;
            this.deathBy = string.Empty;
        }
    }
}
