using System;
using System.Windows;
using System.Windows.Controls;

namespace TheClimb.Models
{
    internal class CoopPoint : Ball
    {
        public delegate void Collect();

        public bool existence, blinkcounter;

        public double lifeTimeMs;

        public CoopPoint(Canvas _canva, double _radius, int _windowheight, int _windowwidth, double _coeff,
            double _offset, double _CoopRandHalfWidth, double _height, string _imagesource)
            : base(_radius, _windowheight, _windowwidth, _coeff, _imagesource)

        {
            var rd = new Random();
            lifeTimeMs = 0;
            existence = true;
            blinkcounter = false;
            Setpos(_canva, (_CoopRandHalfWidth - radius) * 2 * (0.5 - rd.NextDouble()),
                -_offset / 2 + ((_height - _offset) / 2 - 2 * radius) * 2 * (0.5 - rd.NextDouble()));
        }

        public event Collect collide;

        public void Blink(double _startBlinkMs)
        {
            if (lifeTimeMs > _startBlinkMs && existence)
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
            var dist = new Vector(_gameball.position.X - position.X, _gameball.position.Y - position.Y);
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
}