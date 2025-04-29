using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    internal class Road
    {
        public int SourceIntersection { set; get; }
        public int DestinationIntersection { set; get; }
        public double Length { set; get; }
        public int Speed { set; get; }
        public double Time => (Length / Speed) * 60;

        public Road(int Source, int Destination, double Length , int Speed)
        {
            this.SourceIntersection = Source;
            this.DestinationIntersection = Destination;
            this.Length = Length;
            this.Speed = Speed;
        }
    }
}
