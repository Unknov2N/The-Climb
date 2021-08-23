using System.Windows;

namespace TheClimb.Models
{
    internal class Hook
    {
        public Hook(double _x, double _y)
        {
            coord = new Vector(_x, _y);
            isBusy = false;
        }

        //public Point point { get; }// = new List<Point>();
        //public int i { get; set; }//x-индекс зацепа от -11 до 11 (включая 0)
        //public int j { get; set; }
        public Vector coord { get; set; }
        public bool isBusy { get; set; }

        //   public void IsBusy() { }
    }
}