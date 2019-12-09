using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
using System.IO;
using System.Threading;
using aerocock.Models;
using System.ComponentModel;
using Microsoft.Kinect;

namespace aerocock
{
    /// <summary>
    /// Логика взаимодействия для Words.xaml
    /// </summary>


    public partial class Words : Window
    {

        Game game;
        private Ellipse[,] ellMatrix = new Ellipse[24, 24];
        List<string> words = new List<string>();
        List<TextBlock> charBlocks;
        List<Letter> letters;
        List<Hook> hooks = new List<Hook>();
        public delegate void AudioEffect(string _type);
        public event AudioEffect play;
        double coeffAspectRatio;

        private KinectSensor _sensor = KinectSensor.GetDefault();
        private BodyFrameReader bodyFrameReader, prepareFrameReader;
        private FrameDescription frameDescription;

        private void SensorInit()
        {
            this.ReadyPlayer.Visibility = Visibility.Visible;
            if (_sensor == null)
            {
                MessageBox.Show($"{nameof(_sensor)}==Кинект не подключен");
                this.KinectError.Visibility = Visibility.Visible;
            }
            frameDescription = this._sensor.DepthFrameSource.FrameDescription;
            prepareFrameReader = this._sensor.BodyFrameSource.OpenReader();
            prepareFrameReader.FrameArrived += PrepareToGame;
            _sensor.Open();
        }

        private async void AllPlayersReady()
        {
            prepareFrameReader.FrameArrived -= PrepareToGame;
            prepareFrameReader.Dispose();
            await Task.Factory.StartNew(() => { Thread.Sleep(400); });
            bodyFrameReader = this._sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
            //запуск игры
            NextWord();
        }

        private void PrepareToGame(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        AllPlayersReady();
                        foreach (JointType jointType in body.Joints.Keys)
                        {
                            switch (jointType)
                            {
                                case JointType.HandTipLeft:
                                    CheckLetters(body.Joints[jointType]);
                                    break;
                                case JointType.HandTipRight:
                                    CheckLetters(body.Joints[jointType]);
                                    break;
                                case JointType.FootLeft:
                                    //CheckLetters(body.Joints[jointType], true);
                                    break;
                                case JointType.FootRight:
                                    //CheckLetters(body.Joints[jointType], true);
                                    break;
                                default:
                                    CheckLetters(body.Joints[jointType]);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void Ready(TextBlock Ready)
        {
            Ready.Visibility = Visibility.Hidden;
            AllPlayersReady();
        }

        private void _sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            if (e.IsAvailable)
                this.KinectError.Visibility = Visibility.Hidden;
            //else // ставить игру на паузу
            //this.KinectError.Visibility = Visibility.Visible;
        }

        private Body[] bodies = null;

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                CanvaForKinect.Children.Clear();
                SolidColorBrush PlayerColor = Brushes.Azure;
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        foreach (JointType jointType in body.Joints.Keys)
                        {
                            switch (jointType)
                            {
                                case JointType.HandTipLeft:
                                    CheckLetters(body.Joints[jointType], true);
                                    break;
                                case JointType.HandTipRight:
                                    CheckLetters(body.Joints[jointType], true);
                                    break;
                                case JointType.FootLeft:
                                    //CheckLetters(body.Joints[jointType], true);
                                    break;
                                case JointType.FootRight:
                                    //CheckLetters(body.Joints[jointType], true);
                                    break;
                                default:
                                    //CheckLetters(body.Joints[jointType]);
                                    break;
                            }
                        }

                    }
                }
            }
        }

        private void CheckLetters(Joint point, bool check = false)
        {

            double x = -point.Position.X * 3.7 / point.Position.Z,
                y = point.Position.Y * 3.7 / point.Position.Z;
            try
            {
                Ball PointBall;
                PointBall = new Ball(c.radius, pHeight, pWidth, coeffAspectRatio, "pack://application:,,,/Objects/Balls/Discs/red.png");
                PointBall.Setpos(CanvaForKinect, x, y);
                if (check)
                {
                    if (point.Position.Z > 3)
                    {
                        NextLetter(x, y);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        SoundPlayer collision_audio, score_audio, win_audio, time_audio;
        WMPLib.WindowsMediaPlayer soundBackground;

        DispatcherTimer
              Starttimer = new DispatcherTimer(),
              timetimer = new DispatcherTimer();

        int pWidth, pHeight, numWord, numLetter;
        TimeSpan _remainingTime;
        public TimeSpan remainingTime { get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                timeRemaining.Text = remainingTime./*Minutes.*/ToString("mm':'ss");// + ":" + remainingTime.Seconds.ToString();
            } }
        double offset = 0;
        _const c;
        public Words(ref _const _c)
        {
            collision_audio = new SoundPlayer();
            score_audio = new SoundPlayer();
            win_audio = new SoundPlayer();
            ///background_audio = new SoundPlayer();
            time_audio = new SoundPlayer();
            this.c = _c;
            InitializeComponent();
            SecondScreenInit();
            GameFieldInit();
            TimerInit();
            AudioInit();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            play += play_effect;
            numWord = 0;
            CountOfPoints();
            GetWords();
            //PictureBox_Paint();
            SensorInit();
            //grid_Paint();
            //NextWord();
            //timetimer.Start();
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

            soundBackground = new WMPLib.WindowsMediaPlayer();
            soundBackground.URL = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Objects/Audio/VersusBackgroundMusic.mp3");
            soundBackground.settings.setMode("loop", true);
            soundBackground.controls.play();
        }
        private void TimerInit()
        {
            remainingTime = TimeSpan.FromSeconds(c.wordsGameTime);
            timetimer.Interval = TimeSpan.FromSeconds(1d);
            timetimer.Tick += timer_Tick;
        }
        private async void timer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1d));
            if (remainingTime == TimeSpan.FromSeconds(10d))
            {
                timeRemaining.Foreground = Brushes.Red;
            }
            if (remainingTime < TimeSpan.FromSeconds(10d))
                play("tick");
                if (remainingTime == TimeSpan.Zero) 
            {
                letterGrid.Children.Clear();
                timeRemaining.Visibility = Visibility.Hidden;
                //foreach (var item in letters)
                //    textGrid.Children.Remove(item.textBlock);
                //foreach (var item in charBlocks)
                //    textGrid.Children.Remove(item);
                play("lose");
                countdownText.Text = "Игра окончена\nПройдено слов: " + numWord.ToString();
                //Ваше время: " + (TimeSpan.FromSeconds(c.wordsGameTime) - remainingTime).ToString() + 
                //DateTime temp = new DateTime(1, 1, 1, 1, 1, 1);
                //temp.ToShortDatimeRemainingring();
                Delay(3);// await Task.Factory.StartNew(() => Thread.Sleep(3000));
                Stop_the_game();
            }
        }
        private void SecondScreenInit()
        {
            if (System.Windows.Forms.Screen.AllScreens.Length == 2)
            {
                Back.Visibility = Visibility.Hidden;
                game = new Game();
                game.STOP += Stop_the_game;
                game.Show();
                pHeight = (int)(double)(System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height);
                pWidth = (int)(c.width / c.height * (double)System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height);
            }
            else
            {
                pHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                pWidth = (int)(c.width / c.height * System.Windows.SystemParameters.PrimaryScreenHeight);
            }
        }
        private void GameFieldInit()
        {
            if (c.age == "child")
            {
                offset = c.fieldVerticalOffset;
            }
            //рисуем неоновые границы игрового окна в зависимости от размеров (3*2, 3*3, 4*2, 4*3 метра)
            //box.Source = new BitmapImage(new Uri(Instructions.GameBox_Paint(ref c, ref offset)));
            coeffAspectRatio = (double)pHeight / c.height;

            CanvaForKinect.Width = pWidth;
            CanvaForKinect.Height = pHeight;

            Canva.Width = Grid.Width = pWidth;
            Canva.Height = Grid.Height = pHeight;
            Canvas.SetTop(countdownText, (pHeight - countdownText.FontSize) / 2);
            Canvas.SetLeft(countdownText, pWidth / 2 - countdownText.FontSize);
            Grid.Margin = new Thickness(0, coeffAspectRatio * offset, 0, 0);
            Grid.Height = 1.0 * coeffAspectRatio * (c.height) * (1 - offset / c.height);
        }
        void CountOfPoints()
        {
            for (int i = 2 + (int)(offset / (c.height / 24)); i < 24; i++) //цифра 2 получена опытным путём, чтобы буквы рисовались в необходимом поле
            {
                for (int j = 1; j < 24; j++)
                    if (c.Matrix[i, j] != 0)
                        hooks.Add(new Hook(0.125 * (j - 12), 0.125 * (12 - i)));
            }
        }
        private async void NextWord()
        {
            numLetter = 0;
            letterGrid.Children.Clear();
            //if (numWord > 0)
            //{
            //    charBlocks.Clear();
            //    letters.Clear();
            //}
            List<Hook> Busy = hooks.FindAll(x => x.isBusy);
            foreach (var item in Busy)
                item.isBusy = false;
            if (numWord < words.Count)
            {
                charBlocks = new List<TextBlock>(words[numWord].Length);
                letters = new List<Letter>(words[numWord].Length);
                if (words[numWord].Length < hooks.Count)
                    for (int i = 0; i < words[numWord].Length; i++)
                    {
                        charBlocks.Add(new TextBlock());
                        TextFormat(charBlocks.Last(), i, words[numWord].Length, words[numWord][i].ToString());
                        letters.Add(new Letter(letterGrid, hooks, charBlocks.Last(), pHeight));
                        play("new letter");
                        Delay(0.1);// await Task.Factory.StartNew(() => { Thread.Sleep(50); }); 
                    }
                timetimer.Start();
            }
        }
        private async void Delay(double _seconds)
        {
            await Task.Factory.StartNew(() => Thread.Sleep((Int32)_seconds * 1000));
        }
        private async void NextLetter(double _x, double _y)
        {
            double _offset = 0.5;
            int i = (int)Math.Floor(_x * 8 + _offset);
            int j = (int)Math.Floor(_y * 8 + _offset);
            double x = (double)i * 0.125, y = (double)j * 0.125;
            xCoord.Text = i.ToString();
            yCoord.Text = j.ToString();


            Letter taken = letters.Find(item =>
                ((int)(item.coord.X * 1000) == (int)(x * 1000) &&
                ((int)(item.coord.Y * 1000) == (int)(y * 1000) /*&&
                item.textBlock.Text == charBlocks[numLetter].Text*/)));

            if (taken != null) 
            {
                if (taken.textBlock.Text != charBlocks[numLetter].Text)
                {
                    play("taked wrong letter");
                    Delay(0.5);// await Task.Factory.StartNew(() => Thread.Sleep(500));
                }
                else
                {
                    play("taked right letter");
                    letterGrid.Children.Remove(taken.textBlock);
                    letters.Remove(taken);
                    charBlocks[numLetter].Foreground = Brushes.Crimson;// Aqua;
                    numLetter++;
                    Delay(0.5);// await Task.Factory.StartNew(() => Thread.Sleep(500));
                    if (numLetter == words[numWord].Length)
                    {
                        numWord++;
                        if (numWord < words.Count)
                        {
                            play("new word");
                            countdownText.Text = "Молодец!\nОсталось слов: " + (words.Count - numWord).ToString();
                            timetimer.Stop();
                            //Delay(3);
                             await Task.Factory.StartNew(() => { Thread.Sleep(3000); });
                            countdownText.Text = "";
                            //timetimer.Start();
                        }
                        else
                        {
                            timeRemaining.Visibility = Visibility.Hidden;
                            letterGrid.Children.Clear();
                            //foreach (var item in letters)
                            //    textGrid.Children.Remove(item.textBlock);
                            //foreach (var item in charBlocks)
                            //    textGrid.Children.Remove(item);

                            soundBackground.controls.stop();
                            play("win");
                            countdownText.Text = "Умница!\nТы прошёл уровень";// "\nза " + time + " секунд";
                            timetimer.Stop();
                            //Delay(5);
                            await Task.Factory.StartNew(() => { Thread.Sleep(5000); });
                            //timetimer.Start();
                            Stop_the_game();
                        }
                        NextWord();
                    }
                }
            }
        }
        private  void TextFormat(TextBlock _block,int _pos,int _len, string _text)
        {
            _block.VerticalAlignment = VerticalAlignment.Top;
            _block.HorizontalAlignment = HorizontalAlignment.Center;
            int fontsize = 40;
            _block.FontSize = fontsize;
            _block.Margin = new Thickness(fontsize * (_pos - _len / 2), 0, 0, 0);
            _block.Foreground = Brushes.White;//LightSkyBlue;
            _block.Text = _text;
            letterGrid.Children.Add(_block);
        }
        void GetWords()
        {
            try
            {
                string path = @"C:\TheCliff\Words\" + c.speed + ".txt";
                if (!File.Exists(path))
                    throw new Exception("Отсутствует список слов: " + c.speed);
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                        words.Add(reader.ReadLine());
                    //string readText = reader.ReadToEnd();
                    //var splitedText = readText.Split(' ');
                    //for(int i =0; i<splitedText.Length; i++)
                    //{
                    //    words.Add(splitedText[i]);
                    //}
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                Stop_the_game();
            }
        }
        private void PictureBox_Paint()
        {
            Point point = new Point();
            double step = (Canva.Height / 24);
            int diameter = 32;
            for (int i = 2 + (int)(offset * 24 / c.height); i < 24; i++)
            {
                for (int j = 1; j < 24; j++)
                {
                    point.X = step * j;
                    point.Y = step * i;
                    ellMatrix[j, i] = new Ellipse
                    {
                        Visibility = (c.Matrix[i, j] != 0) ? Visibility.Visible : Visibility.Hidden
                    };
                    ellMatrix[j, i].Width = ellMatrix[j, i].Height = diameter;
                    Canvas.SetLeft(ellMatrix[j, i], step * j + diameter / 2 - ellMatrix[j, i].Width);
                    Canvas.SetTop(ellMatrix[j, i], step * i - ellMatrix[j, i].Height / 2);
                    ellMatrix[j, i].Stroke = Brushes.DarkCyan;////White;// GreenYellow;
                    ellMatrix[j, i].StrokeThickness = 2;
                    Canva.Children.Add(ellMatrix[j, i]);
                }
            }
            Ellipse center = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Aqua

            };
            Canvas.SetLeft(center, pWidth / 2 - 5);
            Canvas.SetTop(center, pHeight / 2 - 5);
            Canva.Children.Add(center);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point mousepos = Mouse.GetPosition(Canva);
                double x = c.width * ((mousepos.X - (pHeight - pWidth) / 2) / pWidth - 0.5);
                double y = c.height * (0.5 - mousepos.Y / pWidth);
                countdownText.Text= x.ToString() + "\n" + y.ToString();

                NextLetter(x, y);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                Stop_the_game();
                if (_sensor.IsAvailable)
                    _sensor.Close();
                foreach (Window w in App.Current.Windows)
                    w.Close();
            }
        }
        public void Stop_the_game()
        {
            soundBackground.controls.stop();
            c.soundBackground.controls.play();

            Starttimer.Stop();
            timetimer.Stop();

            Instructions.Log.GameCounter("Игра завершена.", Instructions.CountType.STOP);
            if (_sensor != null)
                _sensor.Close();

            if (game != null) game.Close();
            Close();
        }
        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            numLetter = 0;
            letterGrid.Children.Clear();
            List<Hook> Busy = hooks.FindAll(x => x.isBusy);
            foreach (var item in Busy)
                item.isBusy = false;
            if (numWord < words.Count)
            {

                charBlocks = new List<TextBlock>(words[numWord].Length);
                letters = new List<Letter>(words[numWord].Length);
                if (words[numWord].Length < hooks.Count)
                    for (int i = 0; i < words[numWord].Length; i++)
                    {
                        charBlocks.Add(new TextBlock());
                        TextFormat(charBlocks.Last(), i, words[numWord].Length, words[numWord][i].ToString());
                        letters.Add(new Letter(letterGrid, hooks, charBlocks.Last(), pHeight));
                    }
            }
            else {
                Close();
                if (_sensor.IsAvailable)
                    _sensor.Close();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stop_the_game();
        }
        private void grid_Paint()
        {
            double _offset = 0.5;
            Rectangle rect = new Rectangle
            {
                Height = Canva.Height,
                Width = Canva.Width,
                Stroke = Brushes.Yellow,
                StrokeThickness = 5
            };
            Canva.Children.Add(rect);
            for (int i = 0; i < 24; ++i)
            {
                Line horizontal_line = new Line
                {
                    X1 = 0,
                    X2 = Canva.Width,
                    Y1 = ((double)i + _offset) * Canva.Height / 24,
                    Y2 = ((double)i + _offset) * Canva.Height / 24,
                    Stroke = Brushes.Aqua,//White,// Black,
                    StrokeThickness = 2
                };
                Line vertical_line = new Line
                {
                    X1 = ((double)i + _offset) * Canva.Width / 24,
                    X2 = ((double)i + _offset) * Canva.Width / 24,
                    Y1 = 0,
                    Y2 = i * Canva.Height,
                    Stroke = Brushes.Aqua, //White,// Black,
                    StrokeThickness = 2
                };
                Canva.Children.Add(horizontal_line);
                Canva.Children.Add(vertical_line);
            }
        }
        public void play_effect(string _event)
        {
            try
            {
                switch (_event)
                {/*
                    case "tick":
                        tick_audio.Play();
                        break;
                    case "lose":
                        lose_audio.Play();
                        break;
                    case "win":
                        win_audio.Play();
                        break;
                    case "new letter":
                        newLetter_audio.Play();
                        break;
                    case "taked wrong letter":
                        wrongLetter_audio.Play();
                        break;
                    case "taked right letter":
                        rightLetter_audio.Play();
                        break;
                /*    case "collected":
                        collision_audio.Play();
                        break;

                    case "vertical border":
                        score_audio.Play();
                        break;
                        */
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
                string dir = System.IO.Directory.GetCurrentDirectory();
                MessageBox.Show(dir);
            }
        }
    }
}
