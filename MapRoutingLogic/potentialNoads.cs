using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    public class PotentialNoads
    {
        public double euclidean_Distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }

        public (Dictionary<int, double>, Dictionary<int, double>) findValidNodes(List<Intersection> nodes, Query query)
        {
            double R_Km = query.R / 1000;

            Dictionary<int, double> validStartNodes = new Dictionary<int, double>();
            Dictionary<int, double> validEndNodes = new Dictionary<int, double>();

            foreach (var node in nodes)
            {
                double distance = euclidean_Distance(query.SourceX, query.SourceY, node.X, node.Y);
                if (distance > R_Km)
                    continue;

                double walkingTime = distance / 5;
                validStartNodes[node.ID] = walkingTime;

            }

            foreach (var node in nodes)
            {
                double distance = euclidean_Distance(query.DestinationX, query.DestinationY, node.X, node.Y);
                if (distance > R_Km)
                    continue;

                double walkingTime = distance / 5;
                validEndNodes[node.ID] = walkingTime;

            }
            return (validStartNodes, validEndNodes);
        }
    }
}
