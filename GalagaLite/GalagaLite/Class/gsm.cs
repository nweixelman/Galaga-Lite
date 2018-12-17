using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaLite.Class
{
    class GSM
    {
        public static int level = 1;
        public static int totalEnemies = 3, holdEnemies = totalEnemies;

        /// <summary>
        /// Sets up the background of the levels based on gamestates
        /// </summary>
        public static void gameLevel()
        {
            if (MainPage.RoundEnded == true && MainPage.lives == 0)
            {
                MainPage.BG = MainPage.GameOver;
            }
            else if (MainPage.RoundEnded == true && MainPage.lives > 0)
                MainPage.BG = MainPage.Continue;
            else
            {
                if (MainPage.GameState == 0)
                {
                    MainPage.BG = MainPage.StartScreen;
                }
                else if (MainPage.GameState == 1)
                {
                    MainPage.BG = MainPage.Rules;
                }
                else
                {
                    MainPage.BG = MainPage.Level1;
                }
            }
            

        }

        /// <summary>
        /// When the button is clicked to continue increases level, increases enemies,
        /// stops the enemy and round timers, resets gamestate to 2 and roundEnded to false
        /// and calls start game to restart timers
        /// 
        /// </summary>
        public static void nextLevel()
        {
            level++;
            MainPage.GameState = 2;
            MainPage.RoundEnded = false;
            if(level <= 5)
                holdEnemies += 2;
            if(level % 5 == 0)
            {
                Alien.fleetDIRL -= 1;
                Alien.fleetDIRR += 1;
                Alien.fleetPOSD -= 1;
                Alien.fleetPOSU += 1;
            }
            totalEnemies = holdEnemies;

            MainPage.myShip.BulletXPOS.Clear();
            MainPage.myShip.BulletYPOS.Clear();
            MainPage.RoundTimer.Stop();
            MainPage.EnemyTimer.Stop();

            startGame();
        }

        /// <summary>
        /// Starts the timers
        /// </summary>
        public static void startGame()
        {
            MainPage.RoundTimer.Start();
            MainPage.EnemyTimer.Start();
            Ship.bulletTimer.Start();
        }

        /// <summary>
        /// Stops all timers, resets all initial conditions and clears
        /// any enemy ships that were left when the game ended
        /// </summary>
        public static void endGame()
        {
            MainPage.GameState = 0;
            MainPage.RoundEnded = false;
            MainPage.lives = 1;
            level = 1;

            holdEnemies = 3;
            totalEnemies = holdEnemies;

            MainPage.myShip.BulletXPOS.Clear();
            MainPage.myShip.BulletYPOS.Clear();
            MainPage.alienList.Clear();
            MainPage.RoundTimer.Stop();
            MainPage.EnemyTimer.Stop();
            MainPage.MyScore = 0;
        }

    }
}
