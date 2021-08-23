using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aerocock.Models;
using Microsoft.Kinect;

namespace aerocock
{
    /// <summary>
    ///     Логика взаимодействия для type.xaml
    /// </summary>
    public partial class Regime : Window
    {
        public _const c;
        public KinectSensor sensor = KinectSensor.GetDefault();
        private readonly Type typeWindow;

        public Regime(ref _const c, Type typeWindow)
        {
            this.typeWindow = typeWindow;
            InitializeComponent();
            this.c = c;
            sensor.Open();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            sensor.Close();
            typeWindow.sensor.Open();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                var options = new Options(ref c);
                options.Show();
            }

            if (e.Key == Key.Q)
                foreach (Window w in Application.Current.Windows)
                    w.Close();
        }

        private void Regime_Click(object sender, RoutedEventArgs e)
        {
            score.IsChecked = time.IsChecked /*=другие типы игр*/ = false;
            c.regime = ((CheckBox) sender).Name;
            ((CheckBox) sender).IsChecked = true;
            //if ((string)easy.Content==state)
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            sensor.Close();
            var play = new Parameters(ref c, this);
            play.Show();
        }
    }
}