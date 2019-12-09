using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace aerocock
{
    class Ball
    {
        public Vector position = new Vector();//, velocity = new Vector();
        public double radius, coeff;
        public int pwindowheight, pwindowwidth;
        public Image image = new Image();

       

        public Ball(double _radius, int _windowheight, int _windowwidth, double _coeff,string _imagesource)
        {
            radius = _radius;
            coeff = _coeff;
            pwindowheight = _windowheight;
            pwindowwidth = _windowwidth;
            image.Height = image.Width = 2 * _radius * _coeff;
            image.Source = new BitmapImage(new Uri(_imagesource));
            
        }
      
        public void Rand(double maxposition)//от центра в метрах
        {
            Random randomposition = new Random();
            position.X = (double)pwindowwidth / 2 / coeff + maxposition * (1 - 2 * randomposition.NextDouble()) * Math.Cos(randomposition.NextDouble() * 2 * Math.PI);
            position.Y = (double)pwindowheight / 2 / coeff - maxposition * (1 - 2 * randomposition.NextDouble()) * Math.Sin(randomposition.NextDouble() * 2 * Math.PI);

        }
        public void Setpos(Canvas _canva, double _x, double _y)//от центра в метрах
        {
            position.X = _x;
            position.Y = _y;
            Paint(_canva);
        }

        public void Setpospix(Canvas _canva, int _px, int _py)//от центра в пикселях
        {
            position.X = (-pwindowwidth / 2 + _px) / coeff;
            position.Y = (pwindowheight / 2 - _py) / coeff;
            Paint(_canva);
        }
        public void Paint(Canvas _canva)//coeff - перевод из метров в пиксели
        {
            if (_canva.Children.Contains(image))
                _canva.Children.Remove(image);
            Canvas.SetLeft(image, (pwindowwidth / 2) + position.X * coeff - image.Width / 2);
            Canvas.SetTop(image, (pwindowheight / 2) - position.Y * coeff - image.Height / 2);
            _canva.Children.Add(image);
        }
        public void Paint(Canvas _canva, Image  _ell)//coeff - перевод из метров в пиксели
        {
           /* if (_canva.Children.Contains(_ell))
                _canva.Children.Remove(_ell);*/
            Canvas.SetLeft(_ell, (pwindowwidth / 2) + position.X * coeff - image.Width / 2);
            Canvas.SetTop(_ell, (pwindowheight / 2) - position.Y * coeff - image.Height / 2);
            _canva.Children.Add(_ell);
        }
        /*public Vector Changeposition(int _fps, Vector _velocity)//проверочка на горизонтальные стенки, простые отражения
        {
            double dt = 1.0 / (double)_fps;
            
            if (Math.Abs((position + _velocity * dt).X) > (double)pwindowwidth / 2 / coeff - radius)
            {
                play("collision");
                _velocity.X *= -1;
            }

            if (Math.Abs((position + _velocity * dt).Y) > (double)pwindowheight / 2 / coeff - radius)
            {
                play("collision");
                _velocity.Y *= -1;
            }

            position += _velocity * dt;
            return _velocity;
        }*/

    }

    class CoopPoint : Ball
    {
        public delegate void Collect();
        public event Collect collide;

        public double lifeTimeMs;
        public bool existence, blinkcounter;
        public CoopPoint(Canvas _canva, double _radius, int _windowheight, int _windowwidth, double _coeff,
            double _offset, double _CoopRandHalfWidth, double _height, string _imagesource)
            : base(_radius, _windowheight, _windowwidth, _coeff, _imagesource)

        {
            Random rd = new Random();
            lifeTimeMs = 0;
            existence = true;
            blinkcounter = false;
            Setpos(_canva, (_CoopRandHalfWidth - radius) * 2 * (0.5 - rd.NextDouble()),
                -_offset / 2 + ((_height - _offset) / 2 - 2 * radius) * 2 * (0.5 - rd.NextDouble()));
        }
        public void Blink(double _startBlinkMs)
        {
            if ((lifeTimeMs > _startBlinkMs) && existence)
            {
                switch (blinkcounter)
                {
                    case false:
                        image.Opacity = 0.7;
                        break;

                    case true:
                        image.Opacity = 1;
                        break;
                }
                blinkcounter = !blinkcounter;
            }
        }

        public void Collision(GameBall _gameball, double _lifeTimeMs)
        {
           
        Vector dist = new Vector(_gameball.position.X - position.X, _gameball.position.Y - position.Y);
            double distance = dist.Length,
             mindistance = radius + _gameball.radius;
            if (distance <= mindistance)
            {
                lifeTimeMs = _lifeTimeMs * (3.7d / 5);
                image.Opacity = 0.3;
                existence = false;
                collide();
            }
        }
    }

    class GameBall : Ball
    {
        Vector velocity;
        double maxvelocity, minvelocity;
        //string border;

        public delegate void AudioEffect(string _type);
        public event AudioEffect play;
        public delegate void OutOfBorder(string _border);
        public event OutOfBorder win;
        //private Canvas canvas;

        List<Image> lines;
        public GameBall(double _radius, int _windowheight, int _windowwidth, 
            double _coeff, string _imagesource)
            : base(_radius, _windowheight, _windowwidth, _coeff, _imagesource)
        {
           // this.canvas = 
            //velocity = _velocity;
             //border = "empty";
            lines = new List<Image>();
        }

        public void Changeposition(Canvas _canva, int _fps, double _offset)//перемещение игрового шара
        {
            double dt = 1.0 / (double)_fps;
            //проверочка на вертикальные стенки
            if ((position + velocity * dt).X > (double)pwindowwidth / 2 / coeff - radius)
            {
                foreach (Image item in lines)
                    //lines.Remove(item);
                    _canva.Children.Remove(item);
                lines.Clear();
                play("vertical border");
                win("right");
                //border = "right";
                return;
            }
            if ((position + velocity * dt).X < -((double)pwindowwidth / 2 / coeff - radius))
            {
                foreach (Image item in lines)
                    _canva.Children.Remove(item);
                lines.Clear();
                play("vertical border");
                win("left");
                //border = "left";
                return;
            }

            //проверочка на горизонтальные стенки
            if ((position + velocity * dt).Y < -((double)pwindowheight / 2 / coeff - radius))
            {
                velocity.Y *= -1;
                play("collision");
            }
            if ((position + velocity * dt).Y > (double)pwindowheight / 2 / coeff - radius - _offset)
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
            lines.Add(new Image()
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
            foreach (Image item in lines)
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

        public void Collision(Canvas _canva, Ball _player/*double _x, double _y,
            Vector _velocity*/)//проверка на столкновения с игроком игрового шара
        {
            Vector dist = new Vector(_player.position.X - position.X, _player.position.Y - position.Y);
            double distance = dist.Length;
            double mindistance = radius + _player.radius;
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
                    Random rd = new Random();
                    magnitude = Math.Min(_minvelocity, _maxvelocity) + Math.Abs(_maxvelocity - _minvelocity) * rd.NextDouble();
                    break;

            }
            minvelocity = magnitude;
            maxvelocity = magnitude * _multiplier;

            Random rda = new Random();
            double angle = rda.NextDouble() * 2 * Math.PI;
            this.velocity.X = magnitude * Math.Cos(angle);
            this.velocity.Y = magnitude * Math.Sin(angle);
        }

        public void Remove_Trace(Canvas _canva)
        {
            foreach (var item in lines)
                _canva.Children.Remove(item);
        }
    }

    class Player : Ball
    {
        public int score, win;
        public string orientation;
        public Player(double _radius, int _windowheight, int _windowwidth, double _coeff,
            string _orientation, string _imagesource)
            : base(_radius, _windowheight, _windowwidth, _coeff, _imagesource)
        {
            win = 0;
            score = 0;
            orientation = _orientation;
        }
    }
}
