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
        public int[] Parents { set; get; }
    }
   internal class shortestPath
    {
        //function startpoints endpoints 
        //List<Intersection> nodes = new List<Intersection>();
        //Query query;

        Dictionary<int, double> StartNodes = new Dictionary<int, double>();
        Dictionary<int, double> EndNodes = new Dictionary<int, double>();
        Result result;

        public shortestPath(Dictionary<int, double> StartNodes, Dictionary<int, double> EndNodes)
        {
            this.StartNodes = StartNodes;
            this.EndNodes = EndNodes;
            this.result = new Result();
        }


        public  void Dijkstra(Map graph)
        {
            int nodesCount = graph.Intersections.Count();
            result.Distances = new double[nodesCount];
            result.Parents = new int[nodesCount];

            for (int i = 0; i < nodesCount; i++)
            {
                result.Distances[i] = double.PositiveInfinity;
                result.Parents[i] = -1;
            }

            var priorityQueue = new PriorityQueue<(int node, double distance), double>();
            foreach (var pair in StartNodes)
            {
                result.Distances[pair.Key] = pair.Value;
                priorityQueue.Enqueue((pair.Key, pair.Value), pair.Value);
            }


            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();
                double currentDistance = current.distance;
                int currentNode = current.node;

                //if current > distance of node so skip
                if (currentDistance > result.Distances[currentNode])
                    continue;

           
                // relax neighbors
                foreach (Road road in graph.Roads[currentNode])
                {
                    int neighbor = road.DestinationIntersection;
                    double newDistance = currentDistance + road.Length;

                    if (newDistance < result.Distances[neighbor])
                    {
                        result.Distances[neighbor] = newDistance;
                        result.Parents[neighbor] = currentNode;
                        priorityQueue.Enqueue((neighbor, newDistance), newDistance);
                    }
                }
            }
        }

        public (int,double) GetBestEndNode()
        {
            int bestEnd = -1;
            double min = double.PositiveInfinity;
            foreach(var pair in EndNodes)
            {
                //d[endnode]+walkingTime(value)
                double total= result.Distances[pair.Key]+pair.Value;
                if(total< min)
                {
                    min=total;
                    bestEnd= pair.Key;
                }
            }

            //here we return best End node and it's total with walking time
            return (bestEnd, min);
        }

        public List<int>  constructPath(int endNode)
        {
            var path = new List<int>();
            if (result.Parents[endNode] == -1)
                return path; // No path exists

            int current = endNode;
            while (current != -1)
            {
                path.Add(current);
                current = result.Parents[current];
            }

            path.Reverse(); // Now ordered: StartNode → ... → EndNode
           // int startNode = path[0]; // First node = StartNode
            return path;
        }

        public List<int> FinalResult(Map graph)
        {
            /*
               1. path   --> constructPath
               2. shortestTime -->
               3. shortest Distance
               4. TotalWalkingDistance
               5. TotalvehicleDistance
                */
            Dijkstra(graph);
            var (bestEnd, min) = GetBestEndNode();
            List<int> l;
            l=constructPath(bestEnd);
             return l;
        }


    }
}
