namespace TheClimb.Models
{
    internal class Player : Ball
    {
        public string orientation;
        public int score, win;

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