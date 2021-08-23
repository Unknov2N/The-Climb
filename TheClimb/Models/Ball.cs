using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TheClimb.Models
{
    internal class Ball
    {
        public Image image = new Image();
        public Vector position; //, velocity = new Vector();
        public int pwindowheight, pwindowwidth;
        public double radius, coeff;


        public Ball(double _radius, int _windowheight, int _windowwidth, double _coeff, string _imagesource)
        {
            radius = _radius;
            coeff = _coeff;
            pwindowheight = _windowheight;
            pwindowwidth = _windowwidth;
            image.Height = image.Width = 2 * _radius * _coeff;
            image.Source = new BitmapImage(new Uri(_imagesource));
        }

        public void Rand(double maxposition) //от центра в метрах
        {
            var randomposition = new Random();
            position.X = (double) pwindowwidth / 2 / coeff + maxposition * (1 - 2 * randomposition.NextDouble()) *
                Math.Cos(randomposition.NextDouble() * 2 * Math.PI);
            position.Y = (double) pwindowheight / 2 / coeff - maxposition * (1 - 2 * randomposition.NextDouble()) *
                Math.Sin(randomposition.NextDouble() * 2 * Math.PI);
        }

        public void Setpos(Canvas _canva, double _x, double _y) //от центра в метрах
        {
            position.X = _x;
            position.Y = _y;
            Paint(_canva);
        }

        public void Setpospix(Canvas _canva, int _px, int _py) //от центра в пикселях
        {
            position.X = (-pwindowwidth / 2 + _px) / coeff;
            position.Y = (pwindowheight / 2 - _py) / coeff;
            Paint(_canva);
        }

        public void Paint(Canvas _canva) //coeff - перевод из метров в пиксели
        {
            if (_canva.Children.Contains(image))
                _canva.Children.Remove(image);
            Canvas.SetLeft(image, pwindowwidth / 2 + position.X * coeff - image.Width / 2);
            Canvas.SetTop(image, pwindowheight / 2 - position.Y * coeff - image.Height / 2);
            _canva.Children.Add(image);
        }

        public void Paint(Canvas _canva, Image _ell) //coeff - перевод из метров в пиксели
        {
            /* if (_canva.Children.Contains(_ell))
                 _canva.Children.Remove(_ell);*/
            Canvas.SetLeft(_ell, pwindowwidth / 2 + position.X * coeff - image.Width / 2);
            Canvas.SetTop(_ell, pwindowheight / 2 - position.Y * coeff - image.Height / 2);
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
}