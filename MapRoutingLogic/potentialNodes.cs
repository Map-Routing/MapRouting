using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    public class potentialNodes
    {
        public double euclidean_Distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }

        public (Dictionary<int, double>, Dictionary<int, double>) findValidNodes(List<Intersection> nodes, Query query)
        {
            double R_Km = query.R / 1000;

            var validStartNodes = new Dictionary<int, double>();
            var validEndNodes = new Dictionary<int, double>();

            double querySourceX = query.SourceX;
            double querySourceY = query.SourceY;
            double queryDestinationX = query.DestinationX;
            double queryDestinationY = query.DestinationY;



            foreach (var intersection in nodes)
            {
                double destxStart = intersection.X - querySourceX;
                double destyStart = intersection.Y - querySourceY;
                double distanceStart = Math.Sqrt(destxStart * destxStart + destyStart * destyStart);


                if (distanceStart <= R_Km)
                    validStartNodes[intersection.ID] = distanceStart / 5;

                double destxEnd = intersection.X - queryDestinationX;
                double destyEnd = intersection.Y - queryDestinationY;
                double distanceEnd = Math.Sqrt(destxEnd * destxEnd + destyEnd * destyEnd);

                if (distanceEnd <= R_Km)
                    validEndNodes[intersection.ID] = distanceEnd / 5;
            }

            return (validStartNodes, validEndNodes);

        }
    }
}
