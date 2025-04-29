using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    internal class Intersection
    {
        public int ID { set; get; }
        public double X { set; get; }
        public double Y { set; get; }

        public Intersection(int ID, double X, double Y)
        {
            this.ID = ID;
            this.X = X;
            this.Y = Y;
        }
    }
}
