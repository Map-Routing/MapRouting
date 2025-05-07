using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace MapRoutingLogic
{

    public class Result
    {
        public double[] Distances { set; get; }
        public int[] parents { set; get; }
    }
   internal class shortestPath
    {
        //function startpoints endpoints 
        public static findShortest(Map graph,)
        {
            var potentialNodes = new potentialNoads();
            var (StartNodes, EndNodes) = potentialNodes.findValidNodes(nodes, query);


        }


        public static Result Dijkstra(Map graph, int SNode)
        {
            int nodesCount=graph.Intersections.Count();
            double[] distances = new double[nodesCount];
            int[] previousNodes = new int[nodesCount];

            for (int i = 0; i < nodesCount; i++)
            {
                distances[i] = double.PositiveInfinity;
                previousNodes[i] = -1;
            }
            //assign start to 0
            distances[SNode] = 0;

            // Priority queue (min-heap) of (distance, node)
            var priorityQueue = new PriorityQueue<(double distance, int node), double>();

            priorityQueue.Enqueue((0, SNode), 0);

            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue(); //min distance
                double currentDistance = current.distance;
                int currentNode = current.node;

                // Skip if we already found a better path
                if (currentDistance > distances[currentNode])
                    continue;

                // Explore neighbors
                foreach (Road road in graph.Roads[currentNode])
                {
                    int neighbor = road.DestinationIntersection;
                    double newDistance = currentDistance + road.Time;

                    if (newDistance < distances[neighbor])
                    {
                        distances[neighbor] = newDistance;
                        previousNodes[neighbor] = currentNode;
                        priorityQueue.Enqueue((newDistance, neighbor), newDistance);
                    }
                }
            }

            return new Result { Distances = distances, parents = previousNodes };
        }

        public static List<int> GetShortestPath(int endNode, int[] previousNodes)
        {
            var path = new List<int>();
            if (previousNodes[endNode] == -1)
                return path; // No path exists

            int current = endNode;
            while (current != -1)
            {
                path.Add(current);
                current = previousNodes[current];
            }

            path.Reverse();
            return path;
        }







        //input graph,startpoints,endp
        //time,path
        //graph
        Map Graph =new Map();
        List<Node> nodes=new List<Node>();
        Query query;
        Dictionary<int, double>  StartNodes = new Dictionary<int, double>();
        Dictionary<int, double>  EndNodes = new Dictionary<int, double>();
        public void SomeMethod()
        {
            
        }

       






    }
}
