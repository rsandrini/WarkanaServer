using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lite.Operations;
using ExitGames.Concurrency.Fibers;
using Lite;

namespace WarkanaServer
{
    public class GameLogic
    {
        /// <summary>
        /// The schedule.
        /// </summary>
        private IDisposable schedule;

        public enum status
        {
            awaitingServer,
            loading,
            initialRound,
            pause,
            preRound,
            playing,
            endGame,
        };

        public int currentRound;
        public TimeSpan currentTimer;
        public status statusGame = status.awaitingServer;
        
        public List<PlayerDetails> listPlayers = new List<PlayerDetails>();
        public List<TurnPlayer> listPlayersTurn = new List<TurnPlayer>();
        // Using to send for all players (temp cache)
        public List<TurnPlayer> listPlayersLastTurn = new List<TurnPlayer>();

        /* This is MAP */
        public List<Vector3> cubes = new List<Vector3>();
        public List<int> cubeTimeout = new List<int>();

        /* Looping fo update */
        public PoolFiber ExecutionFiber { get; private set; }

        public GameLogic()
            : this(new PoolFiber())
        {
            this.ExecutionFiber.Start();

        }

        protected GameLogic(PoolFiber executionFiber)
        {
            this.ExecutionFiber = executionFiber;
            currentRound = 1;
            this.ScheduleUpdate();
            
        }

        public void addPlayer(Actor actor)
        {
            listPlayers.Add(new PlayerDetails(actor));
        }

        public void removePlayer(Actor actor)
        {
            PlayerDetails remove = listPlayers.Find(e => e.actor == actor);
            listPlayers.Remove(remove);
        }

        public void loadingServerToStartGame(){
            createMap();
            statusGame = status.loading;
        }

        // The fucking method to update rounds and status game - Is call to event server
        public void update()
        {
            switch (statusGame)
            {
                case status.awaitingServer:
                    break;
                case status.loading:
                    if (allPlayersIsReady())
                    {
                        statusGame = status.initialRound;
                    }
                    break;
                case status.initialRound:
                    // Wait timeout to buying
                    break;
                case status.playing:
                    if (getTotalPlayersLive() <= 1)
                    {
                        if (currentRound > ConfigGame.totalRoundNumber)
                            statusGame = status.endGame;
                        else
                        {
                            statusGame = status.preRound;
                            processEndLevel();
                        }
                    }
                    
                    break;
                case status.preRound:
                    
                    // Timeout ... ConfigGame.timeTotalToPause;
                    
                    break;
                case status.endGame:
                    // Show resume battles
                    break;
                case status.pause:
                    break;
            }
        }

        void createMap()
        {
            cubes.Clear();
            cubeTimeout.Clear();

            float x=0;
            float y=-1f;
            float z=0;

            float startX=x;
            float startZ=z;

            int total = 4;
            int lineBlocksPerCicle = 2;
            int currentCicle = 1;

            int rangeMin = total * 10;
            int rangeMax = rangeMin + 20;

            int blocksPerCicle = 4;
            int contBlock=0;
        
            int contBlockLine = 0;
            Random r = new Random();
            while (currentCicle <= total)
            {
                Vector3 vTest = new Vector3(x, y, z);
                if (!cubes.Exists(e => e == vTest))
                {
                    cubeTimeout.Add(r.Next(rangeMin, rangeMax));
                    cubes.Add(vTest);
                }
                x += 10;
                contBlockLine++;
                contBlock++;
            
                // Limit of line size
                if (lineBlocksPerCicle == contBlockLine)
                {
                    x = startX;
                    z += 10;
                    contBlockLine = 0;
                }

                // Update Cicle
                if (contBlock == blocksPerCicle)
                {
                    currentCicle++;
                    lineBlocksPerCicle += 2;
                    blocksPerCicle = lineBlocksPerCicle * lineBlocksPerCicle;
                    contBlock = 0;

                    /* Update x and z start's */
                    startX -= 10;
                    startZ -= 10;

                    x = startX;
                    z = startZ;

                    rangeMin -= 7;
                    rangeMax -= 7;
                } 
            }
        }

        void processEndLevel()
        {
            //sendToDeadPlace();

            listPlayersLastTurn.Clear();
            foreach (PlayerDetails _player in listPlayers)
            {
                if (getTotalPlayerKill(_player.name) == 0)
                    _player.addExtraGold("nokill");

                _player.addExtraGold("round");
                /* Add statistics for turn */
                TurnPlayer tp = new TurnPlayer();
                tp.playerName = _player.name;
                tp.kills = _player.kills;
                tp.timeDead = _player.timeDead;
                tp.damageCaused = _player.damageCaused;
                tp.damageReceived = _player.damageReceived;
                tp.turn = currentRound;
                // add statistics 
                listPlayersTurn.Add(tp);

                // temp to send for small resume (last round)
                listPlayersLastTurn.Add(tp);

                /* Reset data in local controler*/
                _player.resetStatistics();
            }
            createMap();

        }

        int getTotalPlayersLive()
        {
            int dead = listPlayers.FindAll(e => e.statusPlayerInGame.ToString().Equals("dead")).Count;
            return listPlayers.Count - dead;
        }

        int getTotalPlayerKill(string player)
        {
            return listPlayers.FindAll(e => e.statusPlayerInGame.ToString().Equals("dead") && e.deathBy.Equals(player)).Count;
        }

        bool allPlayersIsReady(){
            if (listPlayers.FindAll(e => e.statusPlayerInGame.ToString().Equals("waitRoundStart") 
                && e.isReady).Count == listPlayers.Count){
                return true;
            }
            else
                return false;
        }

        bool isFirstBlood()
        {
            List<PlayerDetails> tlistFiltered = listPlayers.FindAll(item => item.statusPlayerInGame.ToString() == "dead");
            if (tlistFiltered.Count == 0)
                return true;
            else
                return false;
        }

        int getTotalKillIn2Secs(string playerName, float timePass)
        {
            int total=1;
            List<PlayerDetails> tlistFiltered = listPlayers.FindAll(item => item.deathBy == playerName);
            // Test if total of deaths is bigger of "doubleKill" 
            if (tlistFiltered.Count > 1)
            {
                float lastDeath=0;
                foreach (PlayerDetails pg in tlistFiltered)
                {
                    // ensures that the range is correct
                    if (timePass - pg.timeDead <= 3)
                    {
                        if (lastDeath.Equals(0))
                            lastDeath = pg.timeDead;
                        else
                            if ((pg.timeDead - lastDeath) <= 2 && (pg.timeDead - lastDeath) >= 0)
                                total++;
                            else
                                lastDeath = pg.timeDead;
                    }
                }   
            }
            return total;
        }

        public void setOutTerrain(string player, bool outTerrain)
        {
            PlayerDetails playerThis = listPlayers.Find(item => item.name == player);
            playerThis.outTerrain = outTerrain;
        }

        public void ApplyDamage(string to, string from, float value, float timePass)
        {
            PlayerDetails player = listPlayers.Find(item => item.name == to);
            PlayerDetails playerAtt = listPlayers.Find(item => item.name == to);
            if (player.statusPlayerInGame.ToString() == "live")
            {
                // Add damage in playerObject
                player.ReceiveDamage(value);

                if (!from.Equals("terrain"))
                {
                    playerAtt.damageCaused += value;

                    if (player.statusPlayerInGame.ToString().Equals("dead"))
                    {
                        playerAtt.addKill();
                        // Add Death By in
                        player.setKiller(playerAtt.name, timePass);

                        // if First Blood add aditional gold 
                        bool _firstBlood = isFirstBlood();
                        if (_firstBlood)
                        {
                            playerAtt.addFirstBlood();
                        }
                        // Set the player 
                        int killsInTime = getTotalKillIn2Secs(playerAtt.name, timePass);
                        if (killsInTime == 2)
                        {
                            playerAtt.addExtraKill("double");
                        }
                        else if (killsInTime == 3)
                        {
                            playerAtt.addExtraKill("triple");
                        }
                    }
                }
            }
        }

        /// <summary>Schedules a broadcast of all changes.</summary>
        private void SchedulePublishChanges()
        {
            this.schedule = this.ScheduleMethod((long)1000);
        }

        public IDisposable ScheduleMethod(long timeMs)
        {
            return this.ExecutionFiber.Schedule(() => this.update(), timeMs);
        }
 
        /// <summary>Sends the change list to all users in the lobby and then clears it.</summary>
        private void ScheduleUpdate()
        {
            //schedule the next call!
            this.SchedulePublishChanges();
        }




    }
}
