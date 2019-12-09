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
    /// Логика взаимодействия для Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public _const c;
        public Ellipse[,] ellMatrix = new Ellipse[24, 24];
        private readonly (SolidColorBrush Red, SolidColorBrush Green) color = (Brushes.Red, Brushes.LightGreen);
        Options_mirror mirror;

        public Options(ref _const c)
        {
            InitializeComponent();
            this.c = c;
        }

        private void pictureBox_Paint()
        {
            //((Window)this.Parent).WindowStartupLocation = WindowStartupLocation.
            Point point = new Point();
            for (int i = 1; i < 24; i++)
            {
                for (int j = 1; j < 24; j++)
                {
                    point.X = Canva.Height / 24 * j;
                    point.Y = Canva.Height / 24 * i;
                    ellMatrix[i, j] = new Ellipse { Fill = (c.Matrix[i, j] == 0) ? color.Red : color.Green };
                    ellMatrix[i, j].Width = ellMatrix[i, j].Height = 20;
                    Canvas.SetLeft(ellMatrix[i, j], Canva.Height / 24 * j - ellMatrix[i, j].Width / 2);
                    Canvas.SetTop(ellMatrix[i, j], Canva.Height / 24 * i - ellMatrix[i, j].Height / 2);
                    ellMatrix[i, j].Stroke = Brushes.White;
                    ellMatrix[i, j].StrokeThickness = 2;
                    Canva.Children.Add(ellMatrix[i, j]);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point mousepos = Mouse.GetPosition(Canva);
                int x = (int)((Instructions.Arounding(mousepos.X + Canva.Height / 48, Canva.Height / 24)) / (Canva.Height / 24)),
                    y = (int)((Instructions.Arounding(mousepos.Y + Canva.Height / 48, Canva.Height / 24)) / (Canva.Height / 24));
                try
                {
                    c.Matrix[y, x] = (c.Matrix[y, x] == 0) ? 1 : 0;
                    ellMatrix[y, x].Fill = (ellMatrix[y, x].Fill == color.Red) ? color.Green : color.Red;
                    if (mirror != null)
                    {
                        mirror.Sync_Point(x, y);
                    } //try-catch { }// finally { };


                    Instructions.ChangeMatrixInFile(c.Matrix);
                    Instructions.SetMatrix(c.Matrix, 12.5);

                }
                catch (Exception exc)
                {
                    Instructions.Log.Error("[Options]" + exc.Message);
                    MessageBox.Show("Нажатие вне поля");
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try //if (System.Windows.Forms.Screen.AllScreens.Length == 2)
            {

                System.Windows.Forms.Screen s1 = System.Windows.Forms.Screen.AllScreens[1];
                System.Drawing.Rectangle r1 = s1.WorkingArea;

                mirror = new Options_mirror(ref c);
                mirror.WindowState = System.Windows.WindowState.Normal;
                mirror.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                mirror.Top = r1.Top;
                mirror.Left = r1.Left;
                mirror.Show();
                mirror.WindowState = System.Windows.WindowState.Maximized;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не подключен проектор", exc.Data.ToString());
                Instructions.Log.Error("[Options] Запуск без проектора");
                //throw exc;
            }

            Canva.Height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
            Canva.Width = Canva.Height;
            pictureBox_Paint();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mirror != null)
                mirror.Close();
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
                foreach (Window w in App.Current.Windows)
                    w.Close();

            if (e.Key == Key.C)
                Close();

        }
    }
}
// Отрисовка сетки
//Line line1 = new Line //горизонтальные линии
//{
//    X1 = 0,
//    X2 = Canva.Width,
//    Y1 = i * Canva.Height / 24,
//    Y2 = i * Canva.Height / 24,
//    Stroke = Brushes.Black
//};
//Line line2 = new Line // вертикальные линии
//{
//    X1 = i * Canva.Width / 24,
//    X2 = i * Canva.Width / 24,
//    Y1 = 0,
//    Y2 = i * Canva.Height,
//    Stroke = Brushes.Black
//};
//Canva.Children.Add(line1);
//Canva.Children.Add(line2);