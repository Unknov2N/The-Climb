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
using System.Windows.Media.Animation;
using aerocock.Models;
using Microsoft.Kinect;

namespace aerocock
{
    /// <summary>
    /// Interaction logic for Parameters.xaml
    /// </summary>
    public partial class Parameters : Window
    {
        public _const c;
        Regime regime;
        public KinectSensor sensor = KinectSensor.GetDefault();
        //string state = "легкий", age = "взрослый";
      //  double height = 3;
        public Parameters(ref _const c, Regime regime) 
        {
            this.regime = regime;
            InitializeComponent();
            easy.IsChecked = true;
          //  c.speed = "easy";
            adult.IsChecked = true;
            //  c.age = "adult";
            this.c = c;
            sensor.Open();
        }
        private void Speed_Click(object sender, RoutedEventArgs e)
        {
            easy.IsChecked = medium.IsChecked = hard.IsChecked = false;// = speedball.IsChecked
            Canvas.SetZIndex(easy, 0); Canvas.SetZIndex(medium, 0); Canvas.SetZIndex(hard, 0);// Canvas.SetZIndex(speedball, 0);
            c.speed = (string)((CheckBox)sender).Name;
            ((CheckBox)sender).IsChecked = true;
            //if ((string)easy.Content==state)
            Canvas.SetZIndex(((CheckBox)sender), 1);
        }
        private void Age_Click(object sender, RoutedEventArgs e)
        {
            adult.IsChecked = child.IsChecked = false;
            c.age = (string)((CheckBox)sender).Name;
            ((CheckBox)sender).IsChecked = true;
            //((CheckBox)sender).
            switch (c.age)
            {
                case "adult":
                    {
                        Size.IsChecked = false;
                        break;
                        /* DoubleAnimation sizeanimation = new DoubleAnimation();
                         sizeanimation.From = Size.Height;
                         sizeanimation.To = Size.Height*1.5;
                         sizeanimation.Duration = TimeSpan.FromSeconds(0.3);
                         Size.BeginAnimation(Rectangle.HeightProperty, sizeanimation);
                         //height = 3;*/
                    }
                case "child":
                    {
                        Size.IsChecked = true;
                        break;
                        /*DoubleAnimation sizeanimation = new DoubleAnimation();
                        sizeanimation.From = Size.Height;
                        sizeanimation.To = Size.Height/1.5;
                        sizeanimation.Duration = TimeSpan.FromSeconds(0.3);
                        Size.BeginAnimation(Rectangle.HeightProperty, sizeanimation);
                        //height = 2;*/


                    }
            }

        }
        private void Go_Click(object sender, RoutedEventArgs e)
        {/* Game game = new Game(ref c);
            game.Show();*/
            //c.soundBackground.Stop();
            sensor.Close();
            c.soundBackground.controls.pause();
            switch (c.type)
            {
                case "versus":
                    {
                        Versus play = new Versus(ref c, this);
                        // play.c = c;
                        if (System.Windows.Forms.Screen.AllScreens.Length == 2)
                        {
                            //Game game = new Game();
                            //game.show();
                            System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.AllScreens[1];
                            System.Drawing.Rectangle r1 = s1.WorkingArea;
                            play.WindowState = System.Windows.WindowState.Normal;
                            play.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                            play.Top = r1.Top;
                            play.Left = r1.Left;
                        }
                        play.Show();
                        play.WindowState = System.Windows.WindowState.Maximized;

                        break;
                    }
                case "cooperative":
                    {
                        Cooperative play = new Cooperative(ref c);
                        //  play.c = c;
                        if (System.Windows.Forms.Screen.AllScreens.Length == 2)
                        {
                            //Game game = new Game();
                            //game.show();
                            System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.AllScreens[1];
                            System.Drawing.Rectangle r1 = s1.WorkingArea;
                            play.WindowState = System.Windows.WindowState.Normal;
                            play.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                            play.Top = r1.Top;
                            play.Left = r1.Left;
                        }
                        play.Show();
                        play.WindowState = System.Windows.WindowState.Maximized;
                        break;
                    }
                case "words":
                    {
                        Words play = new Words(ref c);
                        //  play.c = c;
                        if (System.Windows.Forms.Screen.AllScreens.Length == 2)
                        {
                            //Game game = new Game();
                            //game.show();
                            System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.AllScreens[1];
                            System.Drawing.Rectangle r1 = s1.WorkingArea;
                            play.WindowState = System.Windows.WindowState.Normal;
                            play.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                            play.Top = r1.Top;
                            play.Left = r1.Left;
                        }
                        play.Show();
                        play.WindowState = System.Windows.WindowState.Maximized;
                        break;
                    }
            }
            //System.Threading.Thread.Sleep(1300);
            go.IsChecked = false;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            sensor.Close();
            regime.sensor.Open();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
                foreach (Window w in App.Current.Windows)
                    w.Close();
        }
    }
}
