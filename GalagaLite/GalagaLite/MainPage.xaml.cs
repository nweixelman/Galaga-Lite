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
using System.Numerics;
using System.Diagnostics;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GalagaLite
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static CanvasBitmap Continue, GameOver, BG, Rules, StartScreen, Level1, Photon, Enemy1, Enemy2, ALIEN_IMG, MyShip, Boom, Heart;
        public static Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
        public static float DesignWidth = 1920;
        public static float DesignHeight = 1080;
        public static float scaleWidth, scaleHeight;
        public static float MyScore, boomX, boomY;
        public static int boomCount = 60, count = 1;
        public static bool RoundEnded = false;
        public static int lives = 1, liveScore = 0;
        public static Boolean firstBonus = true;

        public static int GameState = 0;
        //High Score
        public static string STRHighScore = "0";

        public static DispatcherTimer RoundTimer = new DispatcherTimer();
        public static DispatcherTimer EnemyTimer = new DispatcherTimer();

        public static Ship myShip;

        //Lists (Enemies)
        public static List<Alien> alienList = new List<Alien>();

        public MainPage()
        {

            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;

            Scaling.SetScale();
            RoundTimer.Tick += RoundTimer_Tick;
            RoundTimer.Interval = new TimeSpan(0, 0, 1);

            EnemyTimer.Tick += EnemyTimer_Tick;

            Storage.CreateFile();
            Storage.ReadFile();

            myShip = new Ship((float)bounds.Width / 2 - (64 * scaleWidth), (float)bounds.Height - (200 * scaleHeight));
        }


        private void EnemyTimer_Tick(object sender, object e)
        {
            for (int a = 0; a < GSM.holdEnemies; a++)
            {
                if (GSM.totalEnemies > 0)
                {
                    Alien myAlien = new Alien((90 * a * scaleHeight), (50 + scaleHeight), 1);
                    alienList.Add(myAlien);
                }

                GSM.totalEnemies -= 1;
            }
        }
        private void RoundTimer_Tick(object sender, object e)
        {
            if (alienList.Count < 1)
            {
                RoundTimer.Stop();
                RoundEnded = true;
            }
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            Scaling.SetScale();
        }

        private void GameCanvas_CreateResources(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        async Task CreateResourcesAsync(CanvasControl sender)
        {
            StartScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/startedit.png"));
            Level1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/background2edit.png"));
            Continue = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/continueedit.png"));
            GameOver = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/gameoveredit.png"));
            Rules = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/rulesedit.png"));
            Photon = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/beam.png"));
            MyShip = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/spaceship.png"));
            Enemy1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien.png"));
            Enemy2 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien2.png"));
            Boom = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/boom.png"));
            Heart = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/lifecount.png"));
        }
        private void GameCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            GSM.gameLevel();
            args.DrawingSession.DrawImage(Scaling.img(BG));
            if (RoundEnded == true)
            {
                if (lives < 0 || count == 0)
                    Storage.UpdateScore();

                Storage.ReadFile();

                CanvasTextLayout textLayout1 = new CanvasTextLayout(args.DrawingSession, MyScore.ToString(), new CanvasTextFormat() { FontSize = (90 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                //Positions the highscore board after game
                args.DrawingSession.DrawTextLayout(textLayout1, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2), 685 * scaleHeight, Colors.White);
                args.DrawingSession.DrawText("High Score: " + Storage.highScore.ToString(), (float)bounds.Width / 2 + 400, 200, Color.FromArgb(255, 255, 255, 255));
            }
            else
            {
                if (GameState > 1)
                {
                    //Positions the level number during game
                    args.DrawingSession.DrawText("Level: " + GSM.level.ToString(), (float)bounds.Width / 2 - 440, (float)bounds.Height - 45, Color.FromArgb(255, 255, 255, 255));
                    // Positions the score board during game
                    args.DrawingSession.DrawText("Score: " + MyScore.ToString(), (float)bounds.Width / 2 - 40, (float)bounds.Height - 45, Color.FromArgb(255, 255, 255, 255));
                    // Positions the highscore board during game
                    args.DrawingSession.DrawText("High Score: " + Storage.highScore.ToString(), (float)bounds.Width / 2 - 760, (float)bounds.Height - 45, Color.FromArgb(255, 255, 255, 255));
                    myShip.MoveShip();

                    //Displaying life count
                    args.DrawingSession.DrawText("Lives: ", (float)bounds.Width / 2 + 400, (float)bounds.Height - 45, Color.FromArgb(255, 255, 255, 255));
                    for (int i = 0; i < lives; i++)
                    {
                        args.DrawingSession.DrawImage(Scaling.img(Heart), (float)bounds.Width / 2 + (450 + (60 * i)), (float)bounds.Height - 55);
                    }

                    if (boomX > 0 && boomY > 0 && boomCount > 0)
                    {
                        args.DrawingSession.DrawImage(Scaling.img(Boom), boomX, boomY);
                        boomCount--;
                    }
                    else
                    {
                        boomCount = 60;
                        boomX = 0;
                        boomY = 0;
                    }

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

                        alienList[j].MoveAlien();
                        args.DrawingSession.DrawImage(Scaling.img(ALIEN_IMG), alienList[j].AlienXPOS, alienList[j].AlienYPOS);

                    }
                    //Display Projectiles
                    for (int i = 0; i < myShip.getBulletX().Count; i++)
                    {

                        //Beam.png needs no dimension scaling
                        args.DrawingSession.DrawImage(Scaling.img(Photon), myShip.getBulletX()[i], myShip.getBulletY()[i]);

                        for (int h = 0; h < alienList.Count; h++)
                        {
                            //100 and 91 are dimensions from boom.png
                            if (myShip.getBulletX()[i] >= alienList[h].AlienXPOS && myShip.getBulletX()[i] <= alienList[h].AlienXPOS + (100 * scaleWidth) && myShip.getBulletY()[i] >= alienList[h].AlienYPOS && myShip.getBulletY()[i] <= alienList[h].AlienYPOS + (91 * scaleHeight))
                            {
                                //50 is half of boom.png width 100 and 91 is also from boom.png
                                boomX = myShip.getBulletX()[i] - (50 * scaleWidth);
                                boomY = myShip.getBulletY()[i] - (91 * scaleHeight);

                                MyScore = MyScore + alienList[h].AlienScore;
                                liveScore = liveScore + alienList[h].AlienScore;

                                alienList.RemoveAt(h);
                                myShip.removeBullet(i);

                                if (liveScore >= 130000 && firstBonus == false)
                                {

                                    liveScore -= 130000;
                                    lives++;
                                }
                                else if(liveScore >= 65000 && firstBonus == true)
                                {
                                    firstBonus = false;
                                    lives++;
                                }

                                break;
                            }
                        }
                    }
                    for (int i = 0; i < alienList.Count; i++)
                    {
                        if (myShip.ShipXPOS >= alienList[i].AlienXPOS && myShip.ShipXPOS <= alienList[i].AlienXPOS + (70 * scaleWidth) && myShip.ShipYPOS >= alienList[i].AlienYPOS && myShip.ShipYPOS <= alienList[i].AlienYPOS + (77 * scaleHeight))
                        {
                            boomX = myShip.ShipXPOS;
                            boomY = myShip.ShipYPOS;

                            alienList.RemoveAt(i);

                            lives--;

                            if(lives == 0)
                            {
                                GSM.endGame();
                            }
                        }
                    }
                    args.DrawingSession.DrawImage(Scaling.img(MyShip), myShip.ShipXPOS, myShip.ShipYPOS);
                }
            }

            GameCanvas.Invalidate();
        }

        private void GameCanvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (RoundEnded == true)
            {
                if (lives > 0 && (((float)e.GetPosition(GameCanvas).X > 621 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1303 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 826 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 891 * scaleHeight))
                {
                    GSM.nextLevel();
                }
                else if (lives < 1 || (lives > 0 && ((float)e.GetPosition(GameCanvas).X > 621 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1303 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 946 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 1008 * scaleHeight))
                {
                    count = 0;
                    GSM.endGame();
                }
            }
            else
            {
                if (GameState != 2)
                {
                    if (((float)e.GetPosition(GameCanvas).X > 768 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1152 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 723 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 831 * scaleHeight)
                    {
                        GameState = 1;
                    }

                    //Button pixel positions on the rulesedit.png
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
                else if (GameState == 2)
                {
                    GSM.startGame();
                }
            }
        }
    }
}