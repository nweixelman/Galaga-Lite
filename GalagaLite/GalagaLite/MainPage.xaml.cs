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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GalagaLite
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static CanvasBitmap BG, Rules, StartScreen, Level1, ScoreScreen, Photon, Enemy1, Enemy2, ALIEN_IMG, MyShip, Boom;
        public static Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
        public static float DesignWidth = 1920;
        public static float DesignHeight = 1080;
        public static float scaleWidth, scaleHeight;
        public static float MyScore, boomX, boomY;
        public static int boomCount = 60;
        public static int totalEnemies = 3, holdEnemies = totalEnemies;
        public static bool RoundEnded = false;
        public static int lives = 1;

        public static int GameState = 0;
        //High Score
        public static string STRHighScore = "0";

        public static DispatcherTimer RoundTimer = new DispatcherTimer();
        public static DispatcherTimer EnemyTimer = new DispatcherTimer();

        public static Ship myShip;

        //Lists (Enemies)
        public static List<float> enemyXPOS = new List<float>();
        public static List<float> enemyYPOS = new List<float>();
        public static List<int> enemySHIP = new List<int>();
        public static List<string> enemyDIR = new List<string>();
        public static List<Alien> alienList = new List<Alien>();
        public static Boolean leftMovement = false;
        public static Boolean rightMovement = false;
        public static Boolean shoot = false;

        public MainPage()
        {

            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;

            Scaling.SetScale();
            RoundTimer.Tick += RoundTimer_Tick;
            RoundTimer.Interval = new TimeSpan(0, 0, 1);

            EnemyTimer.Tick += EnemyTimer_Tick;

            myShip = new Ship((float)bounds.Width / 2 - (46 * scaleWidth), (float)bounds.Height - (130 * scaleHeight));
        }


        private void EnemyTimer_Tick(object sender, object e)
        {
            for (int a = 0; a < totalEnemies + 20; a++)
            {
                if (totalEnemies > 0)
                {
                    Alien myAlien = new Alien((90 * a * scaleHeight), (50 + scaleHeight), 1);
                    alienList.Add(myAlien);
                }

                totalEnemies -= 1;
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
            StartScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/galaga_logo.png"));
            Level1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/background2.png"));
            ScoreScreen = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/scorescreen.png"));
            Rules = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/rules.png"));
            Photon = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/beam.png"));
            MyShip = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/spaceship.png"));
            Enemy1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien.png"));
            Enemy2 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/alien2.png"));
            Boom = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/Images/boom.png"));
        }
        private void GameCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            GSM.gameLevel();
            args.DrawingSession.DrawImage(Scaling.img(BG));
            args.DrawingSession.DrawText("Lives: " + lives.ToString(), new Vector2(200, 200), Colors.Orange);
            if (RoundEnded == true)
            {
                CanvasTextLayout textLayout1 = new CanvasTextLayout(args.DrawingSession, MyScore.ToString(), new CanvasTextFormat() { FontSize = (36 * scaleHeight), WordWrapping = CanvasWordWrapping.NoWrap }, 0.0f, 0.0f);
                args.DrawingSession.DrawTextLayout(textLayout1, ((DesignWidth * scaleWidth) / 2) - ((float)textLayout1.DrawBounds.Width / 2 - 20), 820 * scaleHeight, Colors.White);
                args.DrawingSession.DrawText("HighScores\n" + Convert.ToInt16(STRHighScore), new Vector2(200, 200), Color.FromArgb(255, 200, 150, 210));
            }
            else
            {
                if (GameState > 1)
                {
                    args.DrawingSession.DrawText("Score: " + MyScore.ToString(), (float)bounds.Width / 2, 10, Color.FromArgb(255, 255, 255, 255));
                    myShip.MoveShip();

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

                        args.DrawingSession.DrawImage(Scaling.img(Photon), myShip.getBulletX()[i] - (41 * scaleWidth), myShip.getBulletY()[i] - (63 * scaleHeight));

                        for (int h = 0; h < alienList.Count; h++)
                        {
                            if (myShip.getBulletX()[i] >= alienList[h].AlienXPOS && myShip.getBulletX()[i] <= alienList[h].AlienXPOS + (70 * scaleWidth) && myShip.getBulletY()[i] >= alienList[h].AlienYPOS && myShip.getBulletY()[i] <= alienList[h].AlienYPOS + (77 * scaleHeight))
                            {
                                boomX = myShip.getBulletX()[i] - (37 * scaleWidth);
                                boomY = myShip.getBulletY()[i] - (33 * scaleHeight);

                                MyScore = MyScore + alienList[h].AlienScore;

                                alienList.RemoveAt(h);
                                myShip.removeBullet(i);

                                break;
                            }
                        }
                    }
                    for(int i = 0; i < alienList.Count; i++)
                    {
                        if (myShip.ShipXPOS >= alienList[i].AlienXPOS && myShip.ShipXPOS <= alienList[i].AlienXPOS + (70 * scaleWidth) && myShip.ShipYPOS >= alienList[i].AlienYPOS && myShip.ShipYPOS <= alienList[i].AlienYPOS + (77 * scaleHeight))
                        {
                            boomX = myShip.ShipXPOS;
                            boomY = myShip.ShipYPOS;

                            lives--;

                            alienList.RemoveAt(i);
                            
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
                Storage.UpdateScore();

                if(lives > 0 && ((float)e.GetPosition(GameCanvas).X > 735 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1176 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 940 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 1005 * scaleHeight)
                {
                    GSM.nextLevel();
                }
                else if (lives < 1 && ((float)e.GetPosition(GameCanvas).X > 735 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1176 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 940 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 1005 * scaleHeight)
                {
                    GSM.endGame();
                }

            }
            else
            {
                if (GameState != 2)
                {
                    if (((float)e.GetPosition(GameCanvas).X > 1102 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 1383 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 803 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 870 * scaleHeight)
                    {
                        GameState = 1;
                    }

                    if (((float)e.GetPosition(GameCanvas).X > 258 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 624 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 670 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 805 * scaleHeight)
                    {
                        GameState = 0;
                    }

                    if (((float)e.GetPosition(GameCanvas).X > 546 * scaleWidth && (float)e.GetPosition(GameCanvas).X < 826 * scaleWidth) && (float)e.GetPosition(GameCanvas).Y > 799 * scaleHeight && (float)e.GetPosition(GameCanvas).Y < 865 * scaleHeight)
                    {
                        GameState = 2;
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