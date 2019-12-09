using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace aerocock
{
    class Hook
    {
        //public Point point { get; }// = new List<Point>();
        //public int i { get; set; }//x-индекс зацепа от -11 до 11 (включая 0)
        //public int j { get; set; }
        public Vector coord { get; set; }
        public bool isBusy { get; set; }
        public Hook(double _x, double _y)
        {
            coord = new Vector(_x, _y);
            isBusy = false;
        }

        //   public void IsBusy() { }
    }
}
