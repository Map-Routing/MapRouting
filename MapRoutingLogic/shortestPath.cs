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
        List<Node> nodes = new List<Node>();
        Query query;

        Dictionary<int, double> StartNodes = new Dictionary<int, double>();
        Dictionary<int, double> EndNodes = new Dictionary<int, double>();
        Result result;

        public shortestPath(Dictionary<int, double> StartNodes, Dictionary<int, double> EndNodes)
        {
            this.StartNodes = StartNodes;
            this.EndNodes = EndNodes;
            this.result = new Result();
        }

        //we might need it in testing
        //public static void Ge
        //{
        //    var potentialNodes = new potentialNoads();
        //    var (StartNodes, EndNodes) = potentialNodes.findValidNodes(nodes, query);


        //}


        public  Result Dijkstra(Map graph)
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
                    double newDistance = currentDistance + road.Time;

                    if (newDistance < result.Distances[neighbor])
                    {
                        result.Distances[neighbor] = newDistance;
                        result.Parents[neighbor] = currentNode;
                        priorityQueue.Enqueue((neighbor, newDistance), newDistance);
                    }
                }
            }

            return result;
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


        public static List<int> constructPath(int bestEnd)
        {
            int[] parents = result.Parents;
            var finalPath = new List<int>();

            if (parents[bestEnd] == -1)
                return null; //no path

            int currentNode = bestEnd;
            while (currentNode != -1)
            {
                finalPath.Add(currentNode);
                currentNode = previousNodes[currentNode];
            }
            //add the first node with its parent = -1
            finalPath.Add(currentNode);

            finalPath.Reverse();
            return finalPath;

        }




        /*this function we've taken frim chatGPT together ya shahd if it will help */

        //public static List<int> GetShortestPath(int endNode, int[] previousNodes)
        //{
        //    var path = new List<int>();
        //    if (previousNodes[endNode] == -1)
        //        return path; // No path exists

        //    int current = endNode;
        //    while (current != -1)
        //    {
        //        path.Add(current);
        //        current = previousNodes[current];
        //    }

        //    path.Reverse();
        //    return path;
        //}






    }
}
