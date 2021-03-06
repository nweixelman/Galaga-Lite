/*
 * Because we had no idea where to start with this project we did a little bit of research and came upon a youtube video
 * to help us get started. https://www.youtube.com/watch?v=XrVvoay7afg&list=PL9MEkPNM4g8VypAr9Z9wvCdvd8A9lsDtO is a very
 * primitive space shooter game and although there may be parts that are similar such as the scaling it was not good enough
 * for us to use in general. However, it did give us a good starting point and helped us to better understand the concepts
 * behind each of the methods in the xaml page and we were able to adjust those methods to better suit or needs. 
 */

//For the Galaga Logo, https://www.deviantart.com/dreamcopter/art/Galaga-vector-logo-701705816
//For the startscreen, rules, continue, and game over background https://wallpapercave.com/earth-from-space-wallpapers
//For the level background,  http://www.zs-byczyna.info/super-hd-wallpapers-space.html
//For the continue icon,  https://www.shutterstock.com/es/video/clip-33270040-videogame-ending-screen-text-on-tv-game
//For the game over icon, https://www.giantbomb.com/forums/general-discussion-30/geek-mind-video-game-screenshot-quiz-ultimate-jeff-1475902/
//For the laser beam, https://opengameart.org/content/bullet-collection-1-m484
//For the spaceship, and heart image, https://www.kisspng.com/png-galaga-galaxian-golden-age-of-arcade-video-games-a-1052746/
//For alien1 image, http://pixelartmaker.com/art/1c7f60a112b7ada
//For alien 2 image, http://pixelartmaker.com/art/65a3ee4d8ce8eb6
//For explosion image, https://www.vectorstock.com/royalty-free-vector/pixel-art-explosions-game-icons-set-comic-boom-vector-21211964

using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using GalagaLite.Class;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;

namespace GalagaLite
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static CanvasBitmap AlienLaser, Continue, GameOver, BG, Rules, StartScreen, Level1, Photon, Enemy1, Enemy2, ALIEN_IMG, MyShip, Boom, Heart;
        public static Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
        public static float DesignWidth = 1920;
        public static float DesignHeight = 1080;
        public static float scaleWidth, scaleHeight;
        public static float MyScore, boomX, boomY;
        public static int boomCount = 60;
        public static bool RoundEnded = false;
        public static int lives = 3;
        public static int liveScore = 0;    //keeps track of points needed to gain an extra life
        public static Boolean firstBonus = true;

        public static int GameState = 0;

        //Timers
        public static DispatcherTimer EnemyTimer = new DispatcherTimer();

        public static Ship myShip;

        //Lists (Enemies)
        public static List<Alien> alienList = new List<Alien>();
        public Random AlienAttackRand = new Random();

        /// <summary>
        /// Constructor to intialize all timers and to create files
        /// </summary>
        public MainPage()
        {

            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;

            Scaling.SetScale();

            EnemyTimer.Tick += EnemyTimer_Tick;
            EnemyTimer.Interval = new TimeSpan(0, 0, 0, 0, AlienAttackRand.Next(2000, 3000));

            Storage.CreateFile();
            Storage.ReadFile();

            myShip = new Ship((float)bounds.Width / 2 - (64 * scaleWidth), (float)bounds.Height - (200 * scaleHeight));
        }

        /// <summary>
        /// Creates the enemies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnemyTimer_Tick(object sender, object e)
        {
            if (alienList.Count > 0)
            {
                int AlienAttack = AlienAttackRand.Next(0, alienList.Count);
                alienList[AlienAttack].AlienYPOS += 10;
            }
        }

        /// <summary>
        /// Creates the height and width of the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            Scaling.SetScale();
        }
        /// <summary>
        /// create resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameCanvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        /// <summary>
        /// Creates the images of backgrounds, ship, aliens, bullets, explosions and lives left
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        async Task CreateResourcesAsync(CanvasControl sender)
        {
            // images are stored in the assets folder of the solution
            StartScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/startedit.png"));
            Rules = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/rulesedit.png"));
            Level1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/background2edit.png"));
            Continue = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/continueedit.png"));
            GameOver = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/gameoveredit.png"));
            Photon = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/beam.png"));
            AlienLaser = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alienbeam.png"));
            MyShip = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/spaceship.png"));
            Heart = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/lifecount.png"));
            Enemy1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien.png"));
            Enemy2 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien2.png"));
            Boom = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/boom.png"));
        }

        /// <summary>
        /// This is where all the drawings actually take place. By going through for loops and constantly changing the values
        /// of x and y of the images  we can make it appear that the ships, bullets and aliens are moving. We also use this method
        /// to show the highscore, current score, lives and levels. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GameCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            GSM.gameLevel();                                        //intializes the backgrounds
            args.DrawingSession.DrawImage(Scaling.img(BG));         //draws the backgrounds
            Alien.createAliens();

            //additional things to draw if the game is over
            if (RoundEnded == true)
            {
                if (lives == 0)
                    Storage.UpdateScore();
                //When a new Highscore is reached
                if (Storage.update == true)
                {
                    Storage.ReadFile();

                    CanvasTextLayout textLayout1 = new CanvasTextLayout(args.DrawingSession, "Score\n" + MyScore.ToString(), new CanvasTextFormat() { FontSize = (35 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                    //Positions the current high score after game
                    CanvasTextLayout textLayout2 = new CanvasTextLayout(args.DrawingSession, "High Score\n" + Storage.highScore.ToString(), new CanvasTextFormat() { FontSize = (35 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                    //Displays if a new high score is reached
                    CanvasTextLayout textLayout3 = new CanvasTextLayout(args.DrawingSession, "NEW HIGHSCORE!!!!!", new CanvasTextFormat() { FontSize = (50 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                    args.DrawingSession.DrawTextLayout(textLayout1, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2), 400 * scaleHeight, Colors.White);
                    args.DrawingSession.DrawTextLayout(textLayout2, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2), 560 * scaleHeight, Colors.White);
                    args.DrawingSession.DrawTextLayout(textLayout3, ((float)bounds.Width / 2) - 160, 50 * scaleHeight, Colors.Red);
                }
                //Every other time
                else
                {
                    Storage.ReadFile();

                    CanvasTextLayout textLayout1 = new CanvasTextLayout(args.DrawingSession, "Score\n" + MyScore.ToString(), new CanvasTextFormat() { FontSize = (35 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                    //Positions the current high score after game
                    CanvasTextLayout textLayout2 = new CanvasTextLayout(args.DrawingSession, "High Score\n" + Storage.highScore.ToString(), new CanvasTextFormat() { FontSize = (35 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                    args.DrawingSession.DrawTextLayout(textLayout1, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2), 400 * scaleHeight, Colors.White);
                    args.DrawingSession.DrawTextLayout(textLayout2, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2), 560 * scaleHeight, Colors.White);
                }

            }
            else
            {
                if (GameState > 1)
                {
                    //Positions the level number during game
                    args.DrawingSession.DrawText("Level: " + GSM.level.ToString(), (float)bounds.Width / 2 - 185, (float)bounds.Height - 35, Color.FromArgb(255, 255, 255, 255));
                    // Positions the score board during game
                    args.DrawingSession.DrawText("Score: " + MyScore.ToString(), (float)bounds.Width / 2 - 40, (float)bounds.Height - 35, Color.FromArgb(255, 255, 255, 255));
                    // Positions the highscore board during game
                    args.DrawingSession.DrawText("High Score: " + Storage.STRHighScore, (float)bounds.Width / 2 - 430, (float)bounds.Height - 35, Color.FromArgb(255, 255, 255, 255));
                    myShip.MoveShip();

                    //Displaying life count
                    args.DrawingSession.DrawText("Lives: ", (float)bounds.Width / 2 + 150, (float)bounds.Height - 35, Color.FromArgb(255, 255, 255, 255));
                    for (int i = 0; i < lives; i++)
                    {
                        args.DrawingSession.DrawImage(Scaling.img(Heart), (float)bounds.Width / 2 + (210 + (50 * i)), (float)bounds.Height - 40);
                    }

                    //displays the explosion of ship and alien or bullet and alien
                    if (boomX > 0 && boomY > 0 && boomCount > 0)
                    {
                        args.DrawingSession.DrawImage(Scaling.img(Boom), boomX, boomY);
                        boomCount--;
                    }
                    //otherwise resets coordinates
                    else
                    {
                        boomCount = 60;
                        boomX = 0;
                        boomY = 0;
                    }

                    //moving alien fleet
                    if (alienList.Count > 0)
                        alienList[0].MoveFleet();
                    //Enemies
                    for (int j = 0; j < alienList.Count; j++)
                    {
                        if (alienList[j].AlienType == 1)
                        {
                            ALIEN_IMG = Enemy1;
                        }
                        if (alienList[j].AlienType == 2)
                        {
                            ALIEN_IMG = Enemy2;
                        }
                        //moving aliens and drawing
                        alienList[j].MoveAlien();
                        args.DrawingSession.DrawImage(Scaling.img(ALIEN_IMG), alienList[j].AlienXPOS, alienList[j].AlienYPOS);

                        //alien projectiles
                        for (int a = 0; a < alienList[j].getShootX().Count; a++)
                        {
                            args.DrawingSession.DrawImage(Scaling.img(AlienLaser), alienList[j].getShootX()[a], alienList[j].getShootY()[a]);
                            //alien projectile collision
                            if (alienList[j].getShootX()[a] - (25 * scaleWidth) >= myShip.ShipXPOS && alienList[j].getShootX()[a] <= myShip.ShipXPOS + (110 * scaleWidth) && alienList[j].getShootY()[a] + (60 * scaleHeight) >= myShip.ShipYPOS && alienList[j].getShootY()[a] <= myShip.ShipYPOS + (110 * scaleHeight))
                            {
                                boomX = myShip.ShipXPOS;
                                boomY = myShip.ShipYPOS;

                                alienList[j].removeShoot(a);

                                lives--;

                                if (lives == 0)
                                {
                                    RoundEnded = true;
                                }
                            }
                        }



                    }
                    //Display Projectiles
                    for (int i = 0; i < myShip.getBulletX().Count; i++)
                    {
                        //Beam.png needs no dimension scaling
                        args.DrawingSession.DrawImage(Scaling.img(Photon), myShip.getBulletX()[i], myShip.getBulletY()[i]);

                        for (int h = 0; h < alienList.Count; h++)
                        {
                            //70 and 77 are dimensions from boom.png
                            if (myShip.getBulletX()[i] >= alienList[h].AlienXPOS && myShip.getBulletX()[i] <= alienList[h].AlienXPOS + (70 * scaleWidth) && myShip.getBulletY()[i] >= alienList[h].AlienYPOS && myShip.getBulletY()[i] <= alienList[h].AlienYPOS + (77 * scaleHeight))
                            {
                                //50 is half of boom.png width 100 and 91 is also from boom.png
                                boomX = alienList[h].AlienXPOS;
                                boomY = alienList[h].AlienYPOS;

                                MyScore = MyScore + alienList[h].AlienScore;        //increases score based on the alien type destroyed
                                liveScore = liveScore + alienList[h].AlienScore;    //keeps track of score for gaining lives

                                alienList.RemoveAt(h);
                                myShip.removeBullet(i);

                                //If not the first time receiving a bonus life then life is incremented every 1300000 points
                                if (liveScore >= 65000 && firstBonus == false && lives < 6)
                                {
                                    lives++;
                                    liveScore -= 65000;
                                }
                                if (liveScore >= 32000 && firstBonus == true)
                                {
                                    lives++;
                                    firstBonus = false;
                                }

                                if (alienList.Count == 0)
                                    RoundEnded = true;

                                break;
                            }
                        }
                    }
                    //Ship/alien collision and decremention of life. Ends game when lives get to zero
                    for (int i = 0; i < alienList.Count; i++)
                    {
                        if (alienList[i].AlienXPOS + (70 * scaleWidth) >= myShip.ShipXPOS && alienList[i].AlienXPOS <= myShip.ShipXPOS + (110 * scaleWidth) && alienList[i].AlienYPOS + (77 * scaleHeight) >= myShip.ShipYPOS && alienList[i].AlienYPOS <= myShip.ShipYPOS + (110 * scaleHeight))
                        {
                            boomX = myShip.ShipXPOS;
                            boomY = myShip.ShipYPOS;

                            alienList.RemoveAt(i);

                            lives--;

                            if (alienList.Count == 0)
                                RoundEnded = true;
                            else if (lives == 0)
                            {
                                RoundEnded = true;
                            }

                        }
                    }
                    //Draws ship
                    args.DrawingSession.DrawImage(Scaling.img(MyShip), myShip.ShipXPOS, myShip.ShipYPOS);
                }
            }

            //Redraws everything
            GameCanvas.Invalidate();
        }

        /// <summary>
        /// Method to determine what will happen when a button is pressed on each screen and what happens when 
        /// the round is ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (RoundEnded == true)
            {
                if (lives > 0)
                {
                    //Continues game on the Continue screen based on the button pixel position
                    if (((float)e.GetPosition(GameCanvas).X > 621 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1303 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 826 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 891 * scaleHeight)
                        GSM.nextLevel();

                    //returns to start screen on the Continue screen based on button pixel position
                    else if (lives > 0 && ((float)e.GetPosition(GameCanvas).X > 621 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1303 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 946 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 1008 * scaleHeight)
                    {
                        lives = 0;
                    }
                }
                else if (lives == 0)
                {
                    GSM.endGame();
                }
            }
            else
            {
                if (GameState != 2)
                {
                    //go to rules page using button pixel positions
                    if (((float)e.GetPosition(GameCanvas).X > 768 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1152 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 723 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 831 * scaleHeight)
                    {
                        GameState = 1;
                    }

                    //Return back to start screen from rules page using button pixel positions
                    if (((float)e.GetPosition(GameCanvas).X > 1417 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1836 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 907 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 1015 * scaleHeight)
                    {
                        GameState = 0;
                    }

                    //Button pixel positions on the startedit.png for the start game button
                    if (((float)e.GetPosition(GameCanvas).X > 270 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 656 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 479 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 589 * scaleHeight)
                    {
                        GameState = 2;
                        GSM.startGame();
                    }
                }
            }
        }
    }
}