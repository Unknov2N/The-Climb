using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using aerocock.Models;
using System.Configuration;

namespace aerocock
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Type : Window
    {
        _const c = new _const();
        public KinectSensor sensor = KinectSensor.GetDefault();

        public Type()
        {
            InitializeComponent();
            c.soundBackground.URL = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Objects/Audio/003-14 years girl.mp3"); //"C:/Users/v0v4y/Desktop/temp/ClimBall/aerocock/Objects/Audio/003-14 years girl.wav";//"pack://application:,,,/Objects/Audio/tada.wav";
            c.soundBackground.settings.volume = 80;
            c.soundBackground.settings.setMode("loop", true);
            c.soundBackground.controls.play();
            sensor.Open();
          
        }



        private void Type_Click(object sender, RoutedEventArgs e)
        {
            versus.IsChecked = cooperative.IsChecked =words.IsChecked/*=другие типы игр*/ = false;
            c.type = (string)((CheckBox)sender).Name;
            ((CheckBox)sender).IsChecked = true;
            //if ((string)easy.Content==state)
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //SoundPlayerAction t = new SoundPlayerAction();
            /*try
             {
                 System.IO.Stream music = aerocock.Resources.BackgroundMusic;
                 SoundPlayer sp = new SoundPlayer(music);
                 sp.LoadAsync();
                 sp.PlayLooping();
             }
             catch {
                 string dir = System.IO.Directory.GetCurrentDirectory();
                 MessageBox.Show(dir); }*/
            sensor.Close();
            Regime regime = new Regime(ref c, this);
            regime.c = c;
            regime.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V)
            {
                Kinect_test test;
                try //if (System.Windows.Forms.Screen.AllScreens.Length == 2)
                {

                    System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.AllScreens[1];
                    System.Drawing.Rectangle r1 = s1.WorkingArea;

                    test = new Kinect_test(ref c);
                    test.WindowState = System.Windows.WindowState.Normal;
                    test.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    test.Top = r1.Top;
                    test.Left = r1.Left;
                    test.Show();
                    test.WindowState = System.Windows.WindowState.Maximized;
                }
                catch (Exception exc)
                {
                    //MessageBox.Show("Не подключен проектор", exc.Data.ToString());
                    //Instructions.Log.Error("[Options] Запуск без проектора");

                    test = new Kinect_test(ref c);
                    test.WindowState = System.Windows.WindowState.Normal;
                    test.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    test.Show();
                    test.WindowState = System.Windows.WindowState.Maximized;
                }
            }
            if (e.Key == Key.C)
            {
                Options options = new Options(ref this.c);
                options.Show();
            }

            if (e.Key == Key.Q)
                foreach (Window w in App.Current.Windows)
                    w.Close();
        }
    }
}
