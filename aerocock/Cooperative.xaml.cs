using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using aerocock.Models;
using Microsoft.Kinect;
using WMPLib;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

//using aerocock.Game;

/*  DONE сделать выход по завершении вермени
    DONE при игре на время нет максимума, есть итог после истечения времени
    DONE проверить корректность всех hidden  и visible 
    DONE отформатировать расположение времени и иксов и прочего
    DONE в меню параметров отцентровать "размер окна"
    DONE везде сделать кнопку назад
    DONE убрать черный фон наконец с меню
    сообразить заставку при простое (делаю несколько сталкивающихся шаров, сила тяжести небольшая - вверх)
    и сделать кнопку для открытия технических настроек
    расположение детям/взрослым по центру
    DONE не до конца (в самих играх осталось) DONE русифицировать все подписи
    DONE убрать ебучий спидбол. ебучую попеду тоже
    DDONE режим версус прикрутить всё что есть у кооператива (режимы, скорость)
    ??сделать рандомную скорость после удара о игрока при режиме рандом
*/
namespace aerocock
{
    public partial class Cooperative : Window
    {
        private _const c; // = new _const();

        private double
            coeffAspectRatio;

        private readonly double
            blinkMs = 1000d / 5;

        private double
            addBallTime,
            offset;
        //int msperframe;

        private readonly SoundPlayer collision_audio;
        private readonly SoundPlayer score_audio;
        private readonly SoundPlayer win_audio;
        private readonly SoundPlayer time_audio;
        private Game game;
        private GameBall gameball;
        private Ball player1, player2;

        private List<CoopPoint> points;

        //Vector ballvelocity;
        private int
            pWidth,
            pHeight,
            secondsEnum = 3,
            timeCountdown,
            score,
            maxscore,
            attempt;

        private WindowsMediaPlayer soundBackground;

        private readonly DispatcherTimer
            Starttimer = new DispatcherTimer();

        private readonly DispatcherTimer
            /*Continuetimer = new DispatcherTimer(),*/
            Pointstimer = new DispatcherTimer();

        private readonly DispatcherTimer
            /*Continuetimer = new DispatcherTimer(),*/
            COOPtimer = new DispatcherTimer();

        private readonly DispatcherTimer
            /*Continuetimer = new DispatcherTimer(),*/
            timetimer = new DispatcherTimer();

        private readonly DispatcherTimer
            /*Continuetimer = new DispatcherTimer(),*/
            pointsdelay = new DispatcherTimer();


        public Cooperative(ref _const c)
        {
            this.c = c;
            InitializeComponent();

            collision_audio = new SoundPlayer();
            score_audio = new SoundPlayer();
            win_audio = new SoundPlayer();
            time_audio = new SoundPlayer();
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
                Path.Combine(Directory.GetCurrentDirectory(),
                    "Objects/Audio/VersusBackgroundMusic.mp3"); //ConfigurationManager.AppSettings["ResoursePath"] + "VersusBackgroundMusic.wav"; ;//"C:/Users/v0v4y/Desktop/temp/ClimBall/aerocock/Objects/Audio/VersusBackgroundMusic.wav";//"pack://application:,,,/Objects/Audio/tada.wav";
            //soundBackground.settings.volume = 10;
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

            Canva.Height = Grid.Height = pHeight;
            Canva.Width = Grid.Width = pWidth;
            Canva.Margin = new Thickness(0, 0, 0, 0);
            Canvas.SetTop(countdownText, (pHeight - countdownText.FontSize) / 2);
            Canvas.SetLeft(countdownText, pWidth / 2 - countdownText.FontSize);
            Grid.Margin = new Thickness(0, coeffAspectRatio * offset, 0, 0);
            Grid.Height = 1.0 * coeffAspectRatio * c.height * (1 - offset / c.height);
        }

        private void TimersInit()
        {
            timeCountdown = c.coopMaxTime;

            Starttimer.Interval =
                timetimer.Interval =
                    /*//Continuetimer.Interval = */TimeSpan.FromSeconds(1d);
            Pointstimer.Interval = TimeSpan.FromMilliseconds(blinkMs);
            pointsdelay.Interval = TimeSpan.FromMilliseconds(c.lifeTimeMs / 2);
            COOPtimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / c.fps);

            timetimer.Tick += _timer_Tick;
            pointsdelay.Tick += Delay_Tick;
            COOPtimer.Tick += COOP_Tick;
            Starttimer.Tick += Start_Tick;
            Pointstimer.Tick += Point_Tick;

            if (c.regime == "time")
            {
                timerText.Text = "время " + timeCountdown + " ";
                //COOPtimer.Tick += COOPtime_Tick;
                one_x.Visibility =
                    two_x.Visibility =
                        three_x.Visibility = Visibility.Hidden;
            }

            scoreText.Text = "очки " + "0";
            //Continuetimer.Tick += Continue_Tick;


            /*if (c.regime == "score")
            {
                timerText.Visibility =
                    //leftScore.Visibility =
                    //rightScore.Visibility =
                    leftwinScore.Visibility =
                    rightwinScore.Visibility = Visibility.Hidden;
            }
            if (c.regime == "time")
            {
                timerText.Text = "время " + timeCountdown.ToString() + " ";
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
            Starttimer.Tick += Start_Tick;*/
        }

        private void BallsInit()
        {
            points = new List<CoopPoint>();
            gameball = new GameBall(c.gameballradius, pHeight, pWidth, coeffAspectRatio,
                "pack://application:,,,/Objects/Balls/Circles/magenta.png");
            player1 = new Player(c.radius, pHeight, pWidth, coeffAspectRatio, "left",
                "pack://application:,,,/Objects/Balls/Discs/red.png");
            player2 = new Player(c.radius, pHeight, pWidth, coeffAspectRatio, "right",
                "pack://application:,,,/Objects/Balls/Discs/blue.png");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Instructions.Log.GameCounter("Игра в режиме " + c.type + " " + c.speed + " (" + c.age + ") запущена",
                Instructions.CountType.START);
            AudioInit();
            SecondScreenInit();
            GameFieldInit();
            BallsInit();
            TimersInit();
            SensorInit();

            gameball.play += play_effect;
            gameball.win += Out_of_border;
            //points.

            countdownText.Text = "На старт...";
            //Starttimer.Start();
        }

        private void Delay_Tick(object sender, EventArgs e)
        {
            points.Add(new CoopPoint(Canva, c.gameballradius, pHeight, pWidth, coeffAspectRatio, offset,
                c.coopPointHalfWidth, c.height,
                "pack://application:,,,/Objects/Balls/Circles/Yellow.png"));
            points.Last().collide += Collected;
            //Canva.Children.Add(points.Last().image);

            var timer = (DispatcherTimer) sender;
            timer.Stop();
            Pointstimer.Start();

            if (c.regime == "time")
                timetimer.Start();
        }

        private void Point_Tick(object sender, EventArgs e)
        {
            if (addBallTime >= c.newPointsMs)
            {
                addBallTime = 0;
                points.Add(new CoopPoint(Canva, c.gameballradius, pHeight, pWidth, coeffAspectRatio, offset,
                    c.coopPointHalfWidth, c.height,
                    "pack://application:,,,/Objects/Balls/Circles/Yellow.png"));
                points.Last().collide += Collected;
                //Canva.Children.Add(points.Last().image);
            }

            foreach (var item in points)
                item.Blink(c.startBlinkMs);

            for (var i = points.Count - 1; i >= 0; i--)
                if (points[i].lifeTimeMs > c.lifeTimeMs)
                {
                    Canva.Children.Remove(points[i].image);
                    points.RemoveAt(i);
                }

            /*if (points.First().lifeTimeMs > c.lifeTimeMs)
            {
                Canva.Children.Remove(points.First().image);
                points.RemoveAt(0);
            }*/
            foreach (var item in points)
                item.lifeTimeMs += blinkMs;
            addBallTime += blinkMs;
        }

        private void Start_Tick(object sender, EventArgs e)
        {
            if (secondsEnum > 0) countdownText.Text = "" + secondsEnum;
            switch (secondsEnum)
            {
                case 0:
                    countdownText.Text = "Начали!";
                    break;

                case -1:
                    countdownText.Text = "";
                    Starttimer.Stop();
                    pointsdelay.Start();
                    COOPtimer.Start();

                    gameball.Setpos(Canva, 0, -offset / 2);
                    player1.Setpos(Canva, -10, -10);
                    player2.Setpos(Canva, -10, -12);
                    gameball.Randomvelocity(c.minVelocity, c.maxVelocity, c.multiplier, c.speed);
                    break;

                case -4:
                    countdownText.Text = "набрано очков\n\n" + maxscore * c.coopScoreMultiplier;
                    break;

                case -7:
                    Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
                    Stop_the_game();
                    break;
            }

            secondsEnum--;
        }

        /* void Continue_Tick(object sender, EventArgs e)// от -2 до -4: когда мы проиграли, потом -4:-7, -2:-7  когда выиграли POPEDA!S
         {
             switch (secondsEnum)
             {
                 case 1:
                     countdownText.Text = "внимание";
                     break;
 
                 case 0:
                     countdownText.Text = "Начали!";
                     break;
 
                 case -1:
                     countdownText.Text = "";
                     Continuetimer.Stop();//((DispatcherTimer)sender).Stop();
                     pointsdelay.Start();
                     COOPtimer.Start();
 
                     gameball.Setpos(Canva, 0, -offset / 2);
                     gameball.Randomvelocity(c.minVelocity, c.maxVelocity, c.multiplier, c.speed);
                     //ballvelocity = Randomvelocity(c.minVelocity, c.maxVelocity, c.speed);
                     break;
             }
         }*/

        private void _timer_Tick(object sender, EventArgs e)
        {
            timeCountdown--;
            if (timeCountdown == 10)
                timerText.Foreground = Brushes.Red;

            if (timeCountdown < 10)
                time_audio.Play();

            if (timeCountdown <= 0)
            {
                foreach (var _item in points)
                    Canva.Children.Remove(_item.image);
                points.Clear();

                Canva.Children.Remove(player1.image);
                Canva.Children.Remove(player2.image);
                Canva.Children.Remove(gameball.image);
                gameball.Remove_Trace(Canva);

                maxscore = score;
                secondsEnum = -2;

                soundBackground.controls.stop();

                if (maxscore < c.coopWinScore)
                {
                    countdownText.Text = "время вышло";
                    play_effect("lose");
                }
                else
                {
                    countdownText.Text = "Победа!";
                    play_effect("win");
                }

                scoreText.Text = "";
                timerText.Text = "";

                timetimer.Stop();
                Pointstimer.Stop();
                pointsdelay.Stop();
                COOPtimer.Stop();
                Starttimer.Start();
            }

            timerText.Text = "время " + timeCountdown + " ";
        }

        private void COOP_Tick(object sender, EventArgs e) //работа игры без действий
        {
            /*
                        Canva.Children.Remove(gameball.image);
                        Canva.Children.Remove(player1.image);
                        Canva.Children.Remove(player2.image);*/

            gameball.Changeposition(Canva, c.fps, offset);
            gameball.Collision(Canva, player1);
            gameball.Collision(Canva, player2);
            if (points.Count != 0)
                foreach (var item in points)
                    if (item.existence)
                        item.Collision(gameball, c.lifeTimeMs);
        }

        public void Collected()
        {
            //play_effect("win");

            score++;
            scoreText.Text = "очки " + c.coopScoreMultiplier * score;
            if (score >= c.coopWinScore)
            {
                foreach (var _item in points)
                    Canva.Children.Remove(_item.image);
                //points.Clear();
                Canva.Children.Remove(player1.image);
                Canva.Children.Remove(player2.image);
                Canva.Children.Remove(gameball.image);
                gameball.Remove_Trace(Canva);

                secondsEnum = -5;
                soundBackground.controls.stop();
                countdownText.Text = "Победа!";
                play_effect("win");

                timetimer.Stop();
                pointsdelay.Stop();
                Pointstimer.Stop();
                COOPtimer.Stop();
                Starttimer.Start();
            }

            play_effect("vertical border");
        }
        /*void COOPtime_Tick(object sender, EventArgs e)//работа игры без действий
        {
             Canva.Children.Remove(gameball.image);
             Canva.Children.Remove(player1.image);
             Canva.Children.Remove(player2.image);

            gameball.Changeposition(Canva, c.fps, offset);
            gameball.Collision(Canva, player1);
            gameball.Collision(Canva, player2);

            foreach (CoopPoint _item in points)
                if (_item.existence)
                {
                    Vector dist = new Vector(gameball.position.X - _item.position.X, gameball.position.Y - _item.position.Y);
                    double distance = dist.Length,
                     mindistance = _item.radius + gameball.radius;
                    if (distance <= mindistance)
                    {
                        _item.lifeTimeMs = c.lifeTimeMs * (3.7d / 5);
                        _item.image.Opacity = 0.3;
                        _item.existence = false;
                        score++;
                        scoreText.Text = "очки " + (c.coopScoreMultiplier * score).ToString();
                    }
                }


        }*/

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousepos = Mouse.GetPosition(Canva);
                player1.Setpos(Canva, (-pWidth / 2 + mousepos.X) / coeffAspectRatio,
                    (pHeight / 2 - mousepos.Y) / coeffAspectRatio);
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                var mousepos = Mouse.GetPosition(Canva);
                player2.Setpos(Canva, (-pWidth / 2 + mousepos.X) / coeffAspectRatio,
                    (pHeight / 2 - mousepos.Y) / coeffAspectRatio);
            }
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
            //soundBackground.controls.stop();
            c.soundBackground.controls.play();

            Starttimer.Stop();
            COOPtimer.Stop();
            timetimer.Stop();
            Pointstimer.Stop();
            pointsdelay.Stop();


            Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
            if (_sensor != null)
                _sensor.Close();

            if (game != null) game.Close();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                Stop_the_game();
                //Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
                Instructions.Log.Info("Окно игры закрыто пользователем");
                foreach (Window w in Application.Current.Windows)
                    w.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stop_the_game();
            /*if (_sensor != null)
                _sensor.Close();
            Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
            Close();*/
        }

        /*public Vector Randomvelocity(double _minvelocity, double _maxvelocity, string _speed)
        {
            double velocity = 0;
            switch (_speed)
            {
                case "easy":
                    velocity = _minvelocity;
                    break;

                case "medium":
                    velocity = _minvelocity + Math.Abs(_maxvelocity - _minvelocity) / 2.0;
                    break;

                case "hard":
                    velocity = _maxvelocity;
                    break;

                case "speedball":
                    Random rd = new Random(); 
                    velocity = Math.Min(_minvelocity, _maxvelocity) + Math.Abs(_maxvelocity - _minvelocity) * rd.NextDouble();
                    break;
            }
            Random rda = new Random();
            double angle = rda.NextDouble() * 2 * Math.PI;

            Vector _vel = new Vector()
            {
                X = velocity * Math.Cos(angle),
                Y = velocity * Math.Sin(angle)
            };
            return _vel;
        }*/
        public void Out_of_border(string _border)
        {
            foreach (var _item in points)
                Canva.Children.Remove(_item.image);
            points.Clear();

            Canva.Children.Remove(gameball.image);
            Canva.Children.Remove(player1.image);
            Canva.Children.Remove(player2.image);

            maxscore = maxscore < score ? score : maxscore;
            scoreText.Text = "очки " + (score = 0);

            if (c.regime == "score")
            {
                ++attempt;

                switch (attempt)
                {
                    case 1:
                        one_x.Visibility = Visibility.Hidden;
                        one_x_red.Visibility = Visibility.Visible;
                        break;

                    case 2:
                        two_x.Visibility = Visibility.Hidden;
                        two_x_red.Visibility = Visibility.Visible;
                        break;

                    case 3:
                        three_x.Visibility = Visibility.Hidden;
                        three_x_red.Visibility = Visibility.Visible;
                        break;
                }

                if (attempt > c.coopMaxAttempts)
                {
                    soundBackground.controls.stop();
                    play_effect("lose");

                    secondsEnum = -2;
                    countdownText.Text = "Вы проиграли!";
                    scoreText.Visibility =
                        one_x_red.Visibility =
                            two_x_red.Visibility =
                                three_x_red.Visibility =
                                    Visibility.Hidden;
                }
                else
                {
                    secondsEnum = 3;
                    countdownText.Text = "вылет";
                    play_effect("vertical border");
                }

                if (maxscore >= c.coopWinScore)
                {
                    //scoreText.Text = "";
                    secondsEnum = -5;
                    countdownText.Text = "Победа!";
                    soundBackground.controls.stop();
                }
                //timetimer.Stop();
            }

            if (c.regime == "time")
            {
                secondsEnum = 3;
                countdownText.Text = "вылет";
                play_effect("vertical border");
                timetimer.Stop();
            }

            Pointstimer.Stop();
            pointsdelay.Stop();
            COOPtimer.Stop();
            Starttimer.Start();
        }

        #region Kinect

        private KinectSensor _sensor = KinectSensor.GetDefault();
        private BodyFrameReader bodyFrameReader, prepareFrameReader;
        private FrameDescription frameDescription;

        private void SensorInit()
        {
            if (_sensor == null)
                MessageBox.Show($"{nameof(_sensor)}==Кинект не подключен");
            //Close();
            //this.KinectError.Visibility = Visibility.Visible;
            frameDescription = _sensor.DepthFrameSource.FrameDescription;
            //CanvaForKinect.Height = frameDescription.Height;
            //CanvaForKinect.Width = frameDescription.Height;
            prepareFrameReader = _sensor.BodyFrameSource.OpenReader();
            prepareFrameReader.FrameArrived += PrepareToGame;
            _sensor.Open();
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
            {
                //this.KinectError.Visibility = Visibility.Hidden;
            }
            //else // ставить игру на паузу
            //this.KinectError.Visibility = Visibility.Visible;
        }

        private Body[] bodies;

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
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
                var PlayerColor = Brushes.Azure;
                foreach (var body in bodies)
                    if (body.IsTracked)
                    {
                        if (body.Joints[JointType.SpineMid].Position.X > 0)
                            PlayerColor = Brushes.Red;
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