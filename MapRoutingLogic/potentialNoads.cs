using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    // initially classes from chat-gpt
    class Node
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Node(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }

    class Query
    {
        public double SourceX { get; set; }
        public double SourceY { get; set; }
        public double DestinationX { get; set; }
        public double DestinationY { get; set; }
        public double R { get; set; }

        public Query(double sx, double sy, double dx, double dy, double r)
        {
            SourceX = sx;
            SourceY = sy;
            DestinationX = dx;
            DestinationY = dy;
            R = r;
        }
    }
    internal class potentialNoads
    {
        public double euclidean_Distance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return distance;
        }

        public (Dictionary<int, double>, Dictionary<int, double>) findValidNodes(List<Node> nodes, Query query)
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
                validStartNodes[node.Id] = walkingTime;

            }

            foreach (var node in nodes)
            {
                double distance = euclidean_Distance(query.DestinationX, query.DestinationY, node.X, node.Y);
                if (distance > R_Km)
                    continue;

                double walkingTime = distance / 5;
                validEndNodes[node.Id] = walkingTime;

            }
            return (validStartNodes, validEndNodes);
        }
    }
}
