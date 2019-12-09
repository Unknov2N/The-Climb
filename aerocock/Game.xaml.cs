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
using System.Windows.Threading;

//разобраться с говном в коментах и  Versus:Game
namespace aerocock//.Game
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game//: Window
    {
        public delegate void stop_the_game();
        public event stop_the_game STOP;
        /*int secondsEnum;
        //int timeCountdown;
        DispatcherTimer
             Starttimer = new DispatcherTimer(),
             VStimer = new DispatcherTimer(),
             Continuetimer = new DispatcherTimer(),
             timetimer = new DispatcherTimer();*/

        public Game() { }
        /*public Game(ref int secondsEnum, ref DispatcherTimer Starttimer, ref DispatcherTimer VStimer, 
            ref DispatcherTimer Continuetimer, ref DispatcherTimer timetimer)
        {

            InitializeComponent();
            this.secondsEnum = secondsEnum;
            this.Starttimer = Starttimer;
            this.VStimer = VStimer;
            this.Continuetimer = Continuetimer;
            this.timetimer = timetimer;
        }*/
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (System.Windows.Forms.Screen.AllScreens.Length == 1)
                //Close();//  this.Opacity = 0;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            /* //if(Starttimer.IsEnabled==true)
                 Starttimer.Stop();
             //if (VStimer.IsEnabled == true)
                 VStimer.Stop();
             //if (Continuetimer.IsEnabled == true)
                 Continuetimer.Stop();
             //if (timetimer.IsEnabled == true)
                 timetimer.Stop();
             secondsEnum = -3;
             Starttimer.Start();*/
            STOP();
        }
    }
}
