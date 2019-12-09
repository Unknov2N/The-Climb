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
using aerocock.Models;
using Microsoft.Kinect;

namespace aerocock
{
    /// <summary>
    /// Логика взаимодействия для type.xaml
    /// </summary>
    public partial class Regime : Window
    {
        public _const c;
        public KinectSensor sensor = KinectSensor.GetDefault();
        Type typeWindow;
        
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
                Options options = new Options(ref this.c);
                options.Show();
            }
            if (e.Key == Key.Q)
                foreach (Window w in App.Current.Windows)
                    w.Close();
        }

        private void Regime_Click(object sender, RoutedEventArgs e)
        {
            score.IsChecked = time.IsChecked /*=другие типы игр*/ = false;
            c.regime = (string)((CheckBox)sender).Name;
            ((CheckBox)sender).IsChecked = true;
            //if ((string)easy.Content==state)
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            sensor.Close();
            Parameters play = new Parameters(ref c, this);
            play.Show();
        }
    }
}
