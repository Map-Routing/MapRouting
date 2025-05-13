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
        public Output()
        {
            IdOfIntersections = new List<int>();
            shortestTime = 0.0;
            shortestDistance = 0.0;
            TotalWalkingDistance = 0.0;
            TotalVehicleDistance = 0.0;
        }
        public override string ToString()
        {
            return $"{string.Join(" ",IdOfIntersections)}\n" +
                $"{shortestTime} minues\n " +
                $"{shortestDistance} km \n" +
                $"{TotalWalkingDistance} km \n" +
                $"{TotalVehicleDistance} km\n";
        }

        public  bool Equals(Output ActualOutput, ref string message)
        {
            int sz1 = ActualOutput.IdOfIntersections.Count();
            int sz2 = this.IdOfIntersections.Count();
            bool result = true;
            if (sz1 != sz2)
            {
                message += $"Expected Path = {string.Join(',', ActualOutput.IdOfIntersections)}" +
                            $" And Found = {string.Join(',', this.IdOfIntersections)}" +
                            $"\n";
                result = false ;
                return result;
            }
            for(int i = 0;i<sz1;i++)
            {
                if (ActualOutput.IdOfIntersections[i] != this.IdOfIntersections[i])
                {
                    message += $"Expected Path = {string.Join(',', ActualOutput.IdOfIntersections)}" +
                                $" And Found = {string.Join(',', this.IdOfIntersections)}" +
                                $"\n";
                    result = false;
                    break;
                }
            }

            // shortest time
            if(ActualOutput.shortestTime != this.shortestTime)
            {
                message += $"Expected shortest Time = {ActualOutput.shortestTime}" +
                    $" And Found {this.shortestTime}" +
                    $"\n";
                result = false ;
            }

            // shortest Distance
            if(ActualOutput.shortestDistance != this.shortestDistance)
            {
                message += $"Expected shortest Distance Time = {ActualOutput.shortestDistance}" +
                    $" And Found {this.shortestDistance}" +
                    $"\n";
                result = false ;
            }

            // TotalWalkingDistance 
            if (ActualOutput.TotalWalkingDistance != this.TotalWalkingDistance)
            {
                message += $"Expected Total Walking Distance = {ActualOutput.TotalWalkingDistance}" +
                    $" And Found {this.TotalWalkingDistance}" +
                    $"\n";
                result = false ;
            }

            // TotalVehicleDistance 
            if (ActualOutput.TotalVehicleDistance != this.TotalVehicleDistance)
            {
                message += $"Expected Total Vehicle Distance = {ActualOutput.TotalVehicleDistance}" +
                    $" And Found {this.TotalVehicleDistance}" +
                    $"\n";
                result = false ;
            }
            if (result)
            {
                message += "Passed";
            }
            return result;
        }
    }
}
