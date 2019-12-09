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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using aerocock.Models;
//using System.Windows.Forms.Screen;


namespace aerocock
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class Standby : Window
    {
        //Random rd = new Random();
        DispatcherTimer time = new DispatcherTimer();
        List<Ball> balls; Ball ball;
        int pWidth, pHeight, msperframe;
        double coeffAspectRatio;
        public _const c = new _const();

        public Standby()
        {
            //MultiScreen.ScreenInformation abab = new MultiScreen.ScreenInformation();


            InitializeComponent();

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q) Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

     /*   void Time_Tick(object sender, EventArgs e)//работа игры без действий
        {
            foreach (Balls ball in balls)
            {
                Canva.Children.Remove(ball.ell);
                //ballvelocity = ball.changeposition(c.fps, ballvelocity);
                //ballvelocity = ball.collision(player1.position.X, player1.position.Y, ballvelocity);
                ball.Paint();
                Canva.Children.Add(ball.ell);
            }
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length == 2)
            {
                pHeight = (int)(double)(System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height);
                pWidth = (int)(c.width / c.height * (double)System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height);
            }
            else
            {
                pHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                pWidth = (int)(c.width / c.height * System.Windows.SystemParameters.PrimaryScreenHeight);
            }

            coeffAspectRatio = (double)pHeight / c.height;
            balls = new List<Ball>();
            for (double _X = -(c.height / 2 - c.radius); _X < c.height / 2 - c.radius; _X += 5 * c.radius)
                for (double _Y = -(c.height / 2 - c.radius); _Y < c.height / 2 - c.radius; _Y += 5 * c.radius)
                {
                    Balls item = new Balls(c.radius, Brushes.OrangeRed, pHeight, pWidth,
                        coeffAspectRatio, c.height, c.minVelocity, c.maxVelocity);
                    item.Setpos(_X, _Y);
                    balls.Add(item);
                    Canva.Children.Add(item.ell);
                }

            ball = new Ball(c.radius, Brushes.Black, pHeight, pWidth, coeffAspectRatio);
            ball.Setpos(0, 0);
            Canva.Children.Add(ball.ell);

            msperframe = 1000 / c.fps;
            Canva.Height = pHeight;
            Canva.Width = pWidth;
            Canva.Margin = new Thickness(0, 0, 0, 0);

            Grid.Height = pHeight;
            Grid.Width = pWidth;
            Grid.Margin = new Thickness(0, 0, 0, 0);
            //Grid.Height = 1.0 * coeffAspectRatio * c.height;
            Canvas.SetTop(countdownText, (pHeight - countdownText.FontSize) / 2);
            Canvas.SetLeft(countdownText, pWidth / 2 - countdownText.FontSize);
            countdownText.Text = "hooooy";

            time.Tick += Time_Tick;
            time.Start();
        }

    }

    class Balls : Ball
    {
        public Vector velocity = new Vector();
        public Balls(double _radius, Brush _color, int _windowheight, int _windowwidth, double _coeff, double _height, double _minvelocity, double _maxvelocity)
            : base(_radius, _color, _windowheight, _windowwidth, _coeff)
        {
            Random rd = new Random(); Random rda = new Random();
            velocity.X = _minvelocity + (_maxvelocity - _minvelocity) *
                (1 - 2 * rd.NextDouble()) * Math.Cos(rda.NextDouble() * 2 * Math.PI);
            velocity.Y = _minvelocity + (_maxvelocity - _minvelocity) *
                (1 - 2 * rd.NextDouble()) * Math.Sin(rda.NextDouble() * 2 * Math.PI);
        }
    }*/
}
