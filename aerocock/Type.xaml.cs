using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using aerocock.Models;
using Microsoft.Kinect;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace aerocock
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Type : Window
    {
        private _const c = new _const();
        public KinectSensor sensor = KinectSensor.GetDefault();

        public Type()
        {
            InitializeComponent();
            c.soundBackground.URL =
                Path.Combine(Directory.GetCurrentDirectory(),
                    "Objects/Audio/003-14 years girl.mp3"); //"C:/Users/v0v4y/Desktop/temp/ClimBall/aerocock/Objects/Audio/003-14 years girl.wav";//"pack://application:,,,/Objects/Audio/tada.wav";
            c.soundBackground.settings.volume = 80;
            c.soundBackground.settings.setMode("loop", true);
            c.soundBackground.controls.play();
            sensor.Open();
        }


        private void Type_Click(object sender, RoutedEventArgs e)
        {
            versus.IsChecked = cooperative.IsChecked = words.IsChecked /*=другие типы игр*/ = false;
            c.type = ((CheckBox) sender).Name;
            ((CheckBox) sender).IsChecked = true;
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
            var regime = new Regime(ref c, this);
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
                    var s1 = Screen.AllScreens[1];
                    var r1 = s1.WorkingArea;

                    test = new Kinect_test(ref c);
                    test.WindowState = WindowState.Normal;
                    test.WindowStartupLocation = WindowStartupLocation.Manual;
                    test.Top = r1.Top;
                    test.Left = r1.Left;
                    test.Show();
                    test.WindowState = WindowState.Maximized;
                }
                catch (Exception exc)
                {
                    //MessageBox.Show("Не подключен проектор", exc.Data.ToString());
                    //Instructions.Log.Error("[Options] Запуск без проектора");

                    test = new Kinect_test(ref c);
                    test.WindowState = WindowState.Normal;
                    test.WindowStartupLocation = WindowStartupLocation.Manual;
                    test.Show();
                    test.WindowState = WindowState.Maximized;
                }
            }

            if (e.Key == Key.C)
            {
                var options = new Options(ref c);
                options.Show();
            }

            if (e.Key == Key.Q)
                foreach (Window w in Application.Current.Windows)
                    w.Close();
        }
    }
}