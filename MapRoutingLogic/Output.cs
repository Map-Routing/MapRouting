using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    public class Output
    {
        public List<int> IdOfIntersections { get; set; }
        public double shortestTime { get; set; } // in minutes
        public double shortestDistance { get; set; } // km
        public double TotalWalkingDistance { get; set; } // km
        public double TotalVehicleDistance { get; set; } //km

        public override string ToString()
        {
            return $"{string.Join(" ",IdOfIntersections)}\n" +
                $"{shortestTime} minues\n " +
                $"{shortestDistance} km \n" +
                $"{TotalWalkingDistance} km \n" +
                $"{TotalVehicleDistance} km\n";
        }

    }
}
