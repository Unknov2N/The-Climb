using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TheClimb.Models
{
    internal class GameBall : Ball
    {
        //string border;

        public delegate void AudioEffect(string _type);

        public delegate void OutOfBorder(string _border);
        //private Canvas canvas;

        private readonly List<Image> lines;
        private double maxvelocity, minvelocity;
        private Vector velocity;

        public GameBall(double _radius, int _windowheight, int _windowwidth,
            double _coeff, string _imagesource)
            : base(_radius, _windowheight, _windowwidth, _coeff, _imagesource)
        {
            // this.canvas = 
            //velocity = _velocity;
            //border = "empty";
            lines = new List<Image>();
        }

        public event AudioEffect play;
        public event OutOfBorder win;

        public void Changeposition(Canvas _canva, int _fps, double _offset) //перемещение игрового шара
        {
            var dt = 1.0 / _fps;
            //проверочка на вертикальные стенки
            if ((position + velocity * dt).X > (double) pwindowwidth / 2 / coeff - radius)
            {
                foreach (var item in lines)
                    //lines.Remove(item);
                    _canva.Children.Remove(item);
                lines.Clear();
                play("vertical border");
                win("right");
                //border = "right";
                return;
            }

            if ((position + velocity * dt).X < -((double) pwindowwidth / 2 / coeff - radius))
            {
                foreach (var item in lines)
                    _canva.Children.Remove(item);
                lines.Clear();
                play("vertical border");
                win("left");
                //border = "left";
                return;
            }

            //проверочка на горизонтальные стенки
            if ((position + velocity * dt).Y < -((double) pwindowheight / 2 / coeff - radius))
            {
                velocity.Y *= -1;
                play("collision");
            }

            if ((position + velocity * dt).Y > (double) pwindowheight / 2 / coeff - radius - _offset)
            {
                velocity.Y *= -1;
                play("collision");
            }

            DrawTrace(_canva);
            //Vector oldpos = position;
            position += velocity * dt;
            if (velocity.Length > minvelocity) //можжем  бесконечно ускорить если часто ударять
                velocity -= velocity / velocity.Length * 0.01;
            Setpos(_canva, position.X, position.Y);
        }

        private void DrawTrace(Canvas _canva)
        {
            lines.Add(new Image
            {
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Objects/Balls/Circles/magenta.png")),
                Width = 2 * radius * coeff,
                Height = 2 * radius * coeff
            });
            Paint(_canva, lines.Last());
            if (lines.First().Opacity < 0.1)
            {
                _canva.Children.Remove(lines.First());
                lines.RemoveAt(0);
            }

            foreach (var item in lines)
            {
                item.Width *= 0.985;
                item.Height *= 0.985;
                item.Opacity -= 0.008;
            }
        }
        /*public async void animation(Canvas _canva)
        {
            //System.Windows.Forms.Invalidate();
            /*Image ell = new Image()
            {
                Fill = Brushes.Aquamarine,
                Height = 2 * radius * coeff,
                Width = 2 * radius * coeff
            };
            /*Image line = new Image
            {

            };
            Canvas.SetLeft(ell, (pwindowwidth / 2) + position.X * coeff - ell.Width / 2);
            Canvas.SetTop(ell, (pwindowheight / 2) - position.Y * coeff - ell.Height / 2);
            _canva.Children.Add(ell);
             await Application.Current.Dispatcher.Invoke(() =>
             {
                 for (double i = 1; i > 0.1; i *= 0.9)
                 {
                     Thread.Sleep(30);
                     ell.Opacity = i;
                 }
                 _canva.Children.Remove(ell);
             });
            //ThreadStart start = new ThreadStart(changeopacity());
            //var thread = new Thread(start);
            //thread.Start(ell);
            for (double i = 1; i > 0.10; i *= 0.85)
            {
                await Task.Factory.StartNew(() => { Thread.Sleep(30); });
                ell.Opacity = i;
               // ell.Height *= i;
               // ell.Width *= i;
            }
            _canva.Children.Remove(ell);
            
            //await Task.Factory.StartNew(() =>
            //{

            //    for (double i = 0.9; i > 0.10; i *= 0.9)
            //    {
            //        Thread.Sleep(30);
            //        ell.Opacity = i;
            //    }
            //    //_canva.Children.Remove(ell);
            //});
        }*/

        public void Collision(Canvas _canva, Ball _player /*double _x, double _y,
            Vector _velocity*/) //проверка на столкновения с игроком игрового шара
        {
            var dist = new Vector(_player.position.X - position.X, _player.position.Y - position.Y);
            var distance = dist.Length;
            var mindistance = radius + _player.radius;
            if (distance <= mindistance)
            {
                //double absvelocity = velocity.Length;

                dist.Normalize();
                velocity.Normalize();
                velocity -= 2 * dist;
                velocity.Normalize();
                velocity *= maxvelocity;
                play("collision");

                /*await Task.Run(() =>
                {
                    Vector decrease=
                    for (double multiplier = 0; multiplier < 1; multiplier -= 0.01)
                    {
                        velocity -= multiplier;
                        Thread.Sleep(30);
                    }

                });*/
            }
            //Setpos(_canva, position.X, position.Y);
        }

        public void Randomvelocity(double _minvelocity, double _maxvelocity, double _multiplier, string _speed)
        {
            double magnitude = 0;
            switch (_speed)
            {
                case "easy":
                    magnitude = _minvelocity;
                    break;

                case "medium":
                    magnitude = _minvelocity + Math.Abs(_maxvelocity - _minvelocity) / 2.0;
                    break;

                case "hard":
                    magnitude = _maxvelocity;
                    break;

                case "speedball":
                    var rd = new Random();
                    magnitude = Math.Min(_minvelocity, _maxvelocity) +
                                Math.Abs(_maxvelocity - _minvelocity) * rd.NextDouble();
                    break;
            }

            minvelocity = magnitude;
            maxvelocity = magnitude * _multiplier;

            var rda = new Random();
            var angle = rda.NextDouble() * 2 * Math.PI;
            velocity.X = magnitude * Math.Cos(angle);
            velocity.Y = magnitude * Math.Sin(angle);
        }

        public void Remove_Trace(Canvas _canva)
        {
            foreach (var item in lines)
                _canva.Children.Remove(item);
        }
    }
}