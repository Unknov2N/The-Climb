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

namespace aerocock
{
    /// <summary>
    /// Логика взаимодействия для Options_mirror.xaml
    /// </summary>
    public partial class Options_mirror : Window
    {
        public _const c;
        private Ellipse[,] ellMatrix = new Ellipse[24, 24];
        private readonly (SolidColorBrush Red, SolidColorBrush Green) color = (Brushes.Red, Brushes.LightGreen);

        public Options_mirror(ref _const c)
        {
            this.c = c;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Canva.Height = System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height;
            Canva.Width = Canva.Height;// System.Windows.Forms.Screen.AllScreens[1].WorkingArea.Height;
            pictureBox_Paint();
            grid_Paint();
        }
        private void pictureBox_Paint()
        {
            //((Window)this.Parent).WindowStartupLocation = WindowStartupLocation.
            Point point = new Point();
            double step = (Canva.Height / 24);//(int)
            for (int i = 1; i < 24; i++)
            {
                for (int j = 1; j < 24; j++)
                {
                    point.X = step * j;
                    point.Y = step * i;
                    ellMatrix[j, i] = new Ellipse { Fill = color.Red,
                        Visibility = (c.Matrix[i, j] != 0) ? Visibility.Visible : Visibility.Hidden
                };
                    ellMatrix[j, i].Width = ellMatrix[j, i].Height = 50;
                    Canvas.SetLeft(ellMatrix[j, i], step * j - ellMatrix[j, i].Width / 2);
                    Canvas.SetTop(ellMatrix[j, i], step * i - ellMatrix[j, i].Height / 2);
                    ellMatrix[j, i].Stroke = Brushes.White;// GreenYellow;
                    ellMatrix[j, i].StrokeThickness = 6;// ellMatrix[j, i].Height/2 - 8;
                    Canva.Children.Add(ellMatrix[j, i]);
                }
            }
        }
        public void Sync_Point(int x, int y)
        {
            ellMatrix[x, y].Visibility = (ellMatrix[x, y].Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden;
        }

        private void grid_Paint()
        {
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
                    Y1 = (double)i * Canva.Height / 24,//(i-0.5)  - смещение сетки, чтоб точки были по центру пересечения линий
                    Y2 = (double)i * Canva.Height / 24,
                    Stroke = Brushes.White,// Black,
                    StrokeThickness = 2
                };
                Line vertical_line = new Line
                {
                    X1 = (double)i * Canva.Width / 24,
                    X2 = (double)i * Canva.Width / 24,
                    Y1 = 0,
                    Y2 = i * Canva.Height,
                    Stroke = Brushes.White,// Black,
                    StrokeThickness = 2
                };
                Canva.Children.Add(horizontal_line);
                Canva.Children.Add(vertical_line);
            }
        }
    }
}
