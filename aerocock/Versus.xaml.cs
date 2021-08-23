using System;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using aerocock.Models;
using Microsoft.Kinect;
using WMPLib;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

//using aerocock.Game;

namespace aerocock
{
    //public class Temp : Game { }
    public partial class Versus : Window //Games
    {
        private readonly SoundPlayer collision_audio, score_audio, win_audio, time_audio;
        //private bool GameIsEnable
        //{
        //    get { return GameIsEnable; }
        //    set
        //    { 
        //        if (!value)
        //        {
        //            if (VStimer.IsEnabled)
        //            {
        //                VStimer.Stop();
        //            }
        //            else VStimer.Start();
        //            timetimer.;

        //        }
        //        GameIsEnable = value;
        //    }
        //}
        private readonly Ellipse[,] ellMatrix = new Ellipse[24, 24];

        private readonly DispatcherTimer
            Starttimer = new DispatcherTimer(),
            VStimer = new DispatcherTimer(),
            /*Continuetimer = new DispatcherTimer(),*/
            timetimer = new DispatcherTimer();

        private _const c;
        private double coeffAspectRatio; //побороть различные базовые классы
        private Game game;

        private GameBall gameball; //, test;
        private double offset;
        private readonly Parameters parameters;
        private Player player1, player2;
        private int pWidth, pHeight, secondsEnum;
        private WindowsMediaPlayer soundBackground;
        private int timeCountdown;

        public Versus(ref _const c, Parameters parameters)
        {
            secondsEnum = 3;
            this.c = c;

            this.parameters = parameters;

            collision_audio = new SoundPlayer();
            score_audio = new SoundPlayer();
            win_audio = new SoundPlayer();
            ///background_audio = new SoundPlayer();
            time_audio = new SoundPlayer();

            InitializeComponent();
            AudioInit();
            SecondScreenInit();
            GameFieldInit();
            BallsInit();
            TimersInit();
            SensorInit();

            ReadyPlayer1.Visibility = Visibility.Visible;
            ReadyPlayer2.Visibility = Visibility.Visible;
        }

        private void AudioInit()
        {
            var audio = aerocock.Resources.collision;
            collision_audio.Stream = audio;
            collision_audio.LoadAsync();
            audio = aerocock.Resources.vertical_border;
            score_audio.Stream = audio;
            score_audio.LoadAsync();
            audio = aerocock.Resources.win;
            win_audio.Stream = audio;
            win_audio.LoadAsync();
            audio = aerocock.Resources.timetick;
            time_audio.Stream = audio;
            time_audio.LoadAsync();

            soundBackground = new WindowsMediaPlayer();
            soundBackground.URL =
                Path.Combine(Directory.GetCurrentDirectory(), "Objects/Audio/VersusBackgroundMusic.mp3");
            soundBackground.settings.setMode("loop", true);
            soundBackground.controls.play();
        }

        private void SecondScreenInit()
        {
            if (Screen.AllScreens.Length == 2)
            {
                Back.Visibility = Visibility.Hidden;
                game = new Game(); // ref secondsEnum, ref Starttimer, ref VStimer, ref Continuetimer, ref timetimer);
                game.STOP += Stop_the_game;
                game.Show();
                pHeight = (int) (double) Screen.AllScreens[1].WorkingArea.Height;
                pWidth = (int) (c.width / c.height * Screen.AllScreens[1].WorkingArea.Height);
            }
            else
            {
                pHeight = (int) SystemParameters.PrimaryScreenHeight;
                pWidth = (int) (c.width / c.height * SystemParameters.PrimaryScreenHeight);
            }
        }

        private void GameFieldInit()
        {
            if (c.age == "child") offset = c.fieldVerticalOffset;
            //рисуем неоновые границы игрового окна в зависимости от размеров (3*2, 3*3, 4*2, 4*3 метра)
            box.Source = new BitmapImage(new Uri(Instructions.GameBox_Paint(ref c, ref offset)));
            coeffAspectRatio = pHeight / c.height;

            //CanvaForKinect.Width = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            //CanvaForKinect.Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            CanvaForKinect.Width = pWidth;
            CanvaForKinect.Height = pHeight;

            Canva.Width = Grid.Width = pWidth;
            Canva.Height = Grid.Height = pHeight;
            Canvas.SetTop(countdownText, (pHeight - countdownText.FontSize) / 2);
            Canvas.SetLeft(countdownText, pWidth / 2 - countdownText.FontSize);
            Grid.Margin = new Thickness(0, coeffAspectRatio * offset, 0, 0);
            Grid.Height = 1.0 * coeffAspectRatio * c.height * (1 - offset / c.height);
        }

        private void TimersInit()
        {
            timeCountdown = c.vsMaxTime;

            VStimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / c.fps);
            timetimer.Interval =
                Starttimer.Interval =
                    /*Continuetimer.Interval =*/
                    TimeSpan.FromSeconds(1d);

            if (c.regime == "score")
                timerText.Visibility =
                    //leftScore.Visibility =
                    //rightScore.Visibility =
                    leftwinScore.Visibility =
                        rightwinScore.Visibility = Visibility.Hidden;
            if (c.regime == "time")
            {
                timerText.Text = "время " + timeCountdown + " ";
                player1_one_x.Visibility =
                    player1_two_x.Visibility =
                        player1_three_x.Visibility =
                            player2_one_x.Visibility =
                                player2_two_x.Visibility =
                                    player2_three_x.Visibility = Visibility.Hidden;
            }

            VStimer.Tick += VS_Tick;
            timetimer.Tick += _timer_Tick;
            //Continuetimer.Tick += Continue_Tick;
            Starttimer.Tick += Start_Tick;
        }

        private void BallsInit()
        {
            gameball = new GameBall(c.gameballradius, pHeight, pWidth, coeffAspectRatio,
                "pack://application:,,,/Objects/Balls/Circles/magenta.png");
            player1 = new Player(c.radius, pHeight, pWidth, coeffAspectRatio, "left",
                "pack://application:,,,/Objects/Balls/Discs/red.png");
            player2 = new Player(c.radius, pHeight, pWidth, coeffAspectRatio, "right",
                "pack://application:,,,/Objects/Balls/Discs/blue.png");
            //ballvelocity = new Vector;// (0,0);
            //gameball.Setpos(ref Canva, 0, -offset / 2);
            //player1.Setpos(ref Canva, -10, -10);
            //player2.Setpos(ref Canva, -10, -12);
            //PictureBox_Paint();
            //ballvelocity =  Randomvelocity(c.minVelocity, c.maxVelocity, c.speed);
            //gameball.Randomvelocity(c.minVelocity, c.maxVelocity, c.speed);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Instructions.Log.GameCounter("Игра в режиме " + c.type + " " + c.speed + " (" + c.age + ") запущена",
                Instructions.CountType.START);


            gameball.play += play_effect;
            gameball.win += Out_of_border;

            countdownText.Text = "На старт";
            //Starttimer.Start();
            //AllPlayersReady();
        }

        private void Start_Tick(object sender, EventArgs e)
        {
            //test = 0;
            if (c.regime == "score" && (player1.win == c.vsMaxWins || player2.win == c.vsMaxWins) && c.vsMaxWins != 0 &&
                secondsEnum > -2)
            {
                secondsEnum = -6;
                //Canva.Children.Clear();
                if (player1.win == c.vsMaxWins) countdownText.Text = "Красный\nпобедил!";
                else countdownText.Text = "Синий\nпобедил!";
                soundBackground.controls.stop();
                play_effect("win");
            }

            if (secondsEnum > 0)
            {
                countdownText.Text = "" + secondsEnum;
                play_effect("tick");
            }

            switch (secondsEnum)
            {
                case 0:
                    countdownText.Text = "Начали!";
                    //leftScore.Text = rightScore.Text = "0";
                    //leftScore.Visibility = rightScore.Visibility = Visibility.Visible;
                    break;

                case -1:
                    countdownText.Text = "";
                    // leftborder.Visibility = rightborder.Visibility = Visibility.Visible;
                    var timer = (DispatcherTimer) sender;
                    timer.Stop();

                    VStimer.Start();
                    if (c.regime == "time")
                        timetimer.Start();

                    player1.Setpos(Canva, -10, -10);
                    player2.Setpos(Canva, -10, -12);
                    gameball.Setpos(Canva, 0, -offset / 2);
                    gameball.Randomvelocity(c.minVelocity, c.maxVelocity, c.multiplier,
                        c.speed); // ballvelocity = Randomvelocity(c.minVelocity, c.maxVelocity, c.speed);
                    break;

                case -4:
                    //countdownText.FontSize = 60;
                    //countdownText.Text = "набрано очков\n\n";// + maxscore * c.coopScoreMultiplier;
                    countdownText.Text = "счёт\n" + player1.win + " - " + player2.win;
                    break;

                case -9:
                    //..Type menu = new Type();
                    //menu.Show();
                    Stop_the_game();
                    break;
            }

            secondsEnum--;
        }

        private void VS_Tick(object sender, EventArgs e) //работа игры без действий
        {
            gameball.Changeposition(Canva, c.fps,
                offset); // ballvelocity = ball.VSchangeposition(c.fps, ballvelocity, ref border, offset);

            /* if (c.regime=="time" && timeCountdown <= 0)
             {
                 
             }*/

            gameball.Collision(Canva, player1);
            gameball.Collision(Canva, player2);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            timeCountdown--;
            if (timeCountdown == 10) timerText.Foreground = Brushes.Red;
            if (timeCountdown < 10)
                //soundBackground.settings.volume = timeCountdown;
                play_effect("tick");
            if (timeCountdown <= 0)
            {
                Canva.Children.Remove(gameball.image);
                //gameball.Remove_Trace(Canva); почему при окончании раунда всё хорошо?
                Canva.Children.Remove(player1.image);
                Canva.Children.Remove(player2.image);

                secondsEnum = -2;
                timerText.Text = ""; // "time " + "";
                countdownText.Text = "время вышло";
                timetimer.Stop();
                VStimer.Stop();
                Starttimer.Start();
                return;
            }

            timerText.Text = "время " + timeCountdown + " ";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                Stop_the_game();
                foreach (Window w in Application.Current.Windows)
                    w.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stop_the_game();
        }

        public void play_effect(string _event)
        {
            try
            {
                switch (_event)
                {
                    case "collision":
                        collision_audio.Play();
                        break;

                    case "vertical border":
                        score_audio.Play();
                        break;

                    case "win":
                        win_audio.Play();
                        break;
                    case "tick":
                        time_audio.Play();
                        break;
                }
            }
            catch
            {
                var dir = Directory.GetCurrentDirectory();
                MessageBox.Show(dir);
            }
        }

        public void Stop_the_game()
        {
            soundBackground.controls.stop();
            c.soundBackground.controls.play();

            Starttimer.Stop();
            VStimer.Stop();
            timetimer.Stop();

            Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
            if (_sensor != null)
                _sensor.Close();

            if (parameters != null) parameters.sensor.Open();

            if (game != null) game.Close();
            Close();
        }

        public void Out_of_border(string _border)
        {
            if (_border == "right")
                ++player1.score;
            if (_border == "left")
                ++player2.score;
            if (player1.score == c.vsMaxScore)
            {
                leftwinScore.Text = (++player1.win).ToString();
                countdownText.Text = "Красный выиграл";
                if (c.regime == "score")
                    switch (player1.win)
                    {
                        case 1:
                            player2_one_x.Visibility = Visibility.Hidden;
                            player2_one_x_red.Visibility = Visibility.Visible;
                            break;

                        case 2:
                            player2_two_x.Visibility = Visibility.Hidden;
                            player2_two_x_red.Visibility = Visibility.Visible;
                            //Canva.Children.Remove(player1_one_x_red);
                            break;

                        case 3:
                            player2_three_x.Visibility = Visibility.Hidden;
                            player2_three_x_red.Visibility = Visibility.Visible;
                            break;
                    }
            }
            else
            {
                rightwinScore.Text = (++player2.win).ToString();
                countdownText.Text = "Выиграл синий";
                if (c.regime == "score")
                    switch (player2.win)
                    {
                        case 1:
                            player1_one_x.Visibility = Visibility.Hidden;
                            player1_one_x_red.Visibility = Visibility.Visible;
                            break;

                        case 2:
                            player1_two_x.Visibility = Visibility.Hidden;
                            player1_two_x_red.Visibility = Visibility.Visible;
                            break;

                        case 3:
                            player1_three_x.Visibility = Visibility.Hidden;
                            player1_three_x_red.Visibility = Visibility.Visible;
                            break;
                    }
            }

            Canva.Children.Remove(gameball.image);
            Canva.Children.Remove(player1.image);
            Canva.Children.Remove(player2.image);

            secondsEnum = 3;
            player2.score = player1.score = 0;
            if (c.regime == "time") timetimer.Stop();
            Starttimer.Start();
            VStimer.Stop();
        }

        #region Kinect

        private readonly KinectSensor _sensor = KinectSensor.GetDefault();
        private BodyFrameReader bodyFrameReader, prepareFrameReader;
        private FrameDescription frameDescription;

        private void SensorInit()
        {
            frameDescription = _sensor.DepthFrameSource.FrameDescription;
            prepareFrameReader = _sensor.BodyFrameSource.OpenReader();
            prepareFrameReader.FrameArrived += PrepareToGame;
            _sensor.Open();
            _sensor.IsAvailableChanged += _sensor_IsAvailableChanged;
        }

        private void AllPlayersReady()
        {
            prepareFrameReader.Dispose();
            bodyFrameReader = _sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
            //запуск игры
            Starttimer.Start();
        }

        private void PrepareToGame(object sender, BodyFrameArrivedEventArgs e)
        {
            var dataReceived = false;

            using (var bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null) bodies = new Body[bodyFrame.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                CanvaForKinect.Children.Clear();
                foreach (var body in bodies)
                    if (body.IsTracked)
                    {
                        double LeftX = body.Joints[JointType.WristLeft].Position.X,
                            LeftY = body.Joints[JointType.WristLeft].Position.Y,
                            LeftZ = body.Joints[JointType.WristLeft].Position.Z,
                            RightX = body.Joints[JointType.WristRight].Position.X,
                            RightY = body.Joints[JointType.WristRight].Position.Y,
                            RightZ = body.Joints[JointType.WristRight].Position.Z;

                        if (LeftY > 11 / 30.0 * 1.5 && LeftY < 2 / 3.0 * 1.5 ||
                            RightY > 11 / 30.0 * 1.5 && RightY < 2 / 3.0 * 1.5)
                        {
                            if (LeftX > 1 / 4.0 * 1.5 && LeftX < 3 / 4.0 * 1.5 ||
                                RightX > 1 / 4.0 * 1.5 && RightX < 3 / 4.0 * 1.5)
                                Ready(ReadyPlayer1);
                            if (LeftX < -1 / 4.0 * 1.5 && LeftX > -3 / 4.0 * 1.5 ||
                                RightX < -1 / 4.0 * 1.5 && RightX > -3 / 4.0 * 1.5)
                                Ready(ReadyPlayer2);
                        }
                    }
            }
        }

        private void Ready(TextBlock Ready)
        {
            Ready.Visibility = Visibility.Hidden;
            if (ReadyPlayer1.Visibility == Visibility.Hidden && ReadyPlayer2.Visibility == Visibility.Hidden)
                AllPlayersReady();
        }

        private void _sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (e.IsAvailable)
                KinectError.Visibility = Visibility.Hidden;
            else // ставить игру на паузу
                KinectError.Visibility = Visibility.Visible;
        }

        private Body[] bodies;

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var dataReceived = false;
            var playerCounter = 0;

            using (var bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null) bodies = new Body[bodyFrame.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                CanvaForKinect.Children.Clear();
                var PlayerColor = Brushes.Azure;
                if (bodies.Length < 2) GamePause();
                foreach (var body in bodies)
                    if (body.IsTracked)
                    {
                        playerCounter++;
                        if (body.Joints[JointType.SpineMid].Position.X > 0)
                            PlayerColor = Brushes.Red;
                        else
                            PlayerColor = Brushes.Azure;
                        foreach (var jointType in body.Joints.Keys)
                            switch (jointType)
                            {
                                case JointType.HandTipLeft:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.HandTipRight:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.FootLeft:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.FootRight:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                            }
                    }
            }
        }

        private void GamePause()
        {
        }

        private void SetJointToCanva(Joint point, SolidColorBrush color)
        {
            double x =
                    -point.Position.X * 3.7 /
                    point.Position
                        .Z, //*/ -Instructions.Arounding(point.Position.X * 3.7 / point.Position.Z, 0.125),//125),
                y = point.Position.Y * 3.7 /
                    point.Position
                        .Z; //*/ Instructions.Arounding(point.Position.Y * 3.7 / point.Position.Z, 0.125);// 125);
            //double x1 = -Instructions.Arounding(point.Position.X, 0.125),//125)
            //    y1 = Instructions.Arounding(point.Position.Y, 0.125);// 125);
            try
            {
                //if (c.Matrix[(int)(-y * 8 + 12), (int)(24 - (-x * 8 + 12))] != 0)
                //if (c.Matrix[(int)(-y1 * 8 + 12), (int)(24 - (-x1 * 8 + 12))] != 0)
                //if (true)
                //if (c.Matrix[(int)(-Instructions.Arounding(y, 0.125) * 8 + 12), (int)(24 - (Instructions.Arounding(x, 0125) * 8 + 12))] != 0)
                {
                    Ball PointBall;
                    if (color == Brushes.Red)
                        PointBall = new Ball(c.radius, pHeight, pWidth, coeffAspectRatio,
                            "pack://application:,,,/Objects/Balls/Discs/red.png");
                    else
                        PointBall = new Ball(c.radius, pHeight, pWidth, coeffAspectRatio,
                            "pack://application:,,,/Objects/Balls/Discs/blue.png");
                    PointBall.Setpos(CanvaForKinect, x, y);
                    gameball.Collision(CanvaForKinect, PointBall);
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}