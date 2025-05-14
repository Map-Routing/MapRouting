using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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
                //in queue add the start nodes with their walking time as the distances
                result.Distances[pair.Key] = pair.Value;
                priorityQueue.Enqueue((pair.Key, pair.Value), pair.Value);
            }


            while (priorityQueue.Count > 0)
            {
                var current = priorityQueue.Dequeue();
                //take the current distance from the current node in the queue
                double currentDistance = current.distance;
                int currentNode = current.node;

                //if current > distance of node so skip
                if (currentDistance > result.Distances[currentNode])
                    continue;


                // relax neighbors
                foreach (Road road in graph.Roads[currentNode])
                {
                    int neighbor = road.DestinationIntersection;
                    //double newDistance = currentDistance + road.Time;
                    double timeInHours = road.Length / road.Speed;
                    double newDistance = currentDistance + timeInHours;

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

            //Console.WriteLine($"End nodes: {string.Join(",", EndNodes.Keys)}");

            foreach (var pair in EndNodes)
            {
                //Console.WriteLine($"End node {pair.Key}, Distance: {result.Distances[pair.Key]}, Parent: {result.Parents[pair.Key]}");
                //d[endnode]+walkingTime(value)
                double total = result.Distances[pair.Key] + pair.Value;
                if (total < min)
                {
                  min = total;
                  bestEnd = pair.Key;
                }
                
            }

            //here we return best End node and it's total with walking time
            return (bestEnd, min);
        }

        public List<int>  constructPath(int endNode)
        {
            var path = new List<int>();
            if (result.Parents[endNode] == -1)
            {
                //Console.WriteLine($"No path exists to end node {endNode}");
                return path; // No path exists
            }

            
            int current = endNode;
            while (current != -1)
            {
                path.Add(current);
                current = result.Parents[current];
                
            }

            path.Reverse(); // Now ordered: StartNode → ... → EndNode
                            // int startNode = path[0]; // First node = StartNode
            //Console.WriteLine($"Constructed path: {string.Join("→", path)}");
            return path;
        }

        public (List<int> path, double totalTime, double totalWalkingDistance, double totalVehicleDistance) FinalResult(Map graph)
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

            double totalTime = min * 60; //shortest time
            double totalTimeRounded = Math.Round(totalTime, 2);
            //debugging
            //Console.WriteLine($"Best end selected: {bestEnd}");

            List<int> l;
            l = constructPath(bestEnd);
            if (!l.Contains(bestEnd))
                l.Add(bestEnd);

            double totalWalkingDistance = (EndNodes[bestEnd] * 5) + (StartNodes[l[0]] * 5);
            double totalWalkingDistanceRounded = Math.Round(totalWalkingDistance, 2);
            double totalVehicleDistance = 0;
            for (int i = 0; i < l.Count - 1; i++)
            {
                int currentIntersection = l[i];
                int nextIntersection = l[i + 1];

                // Find the road between current and next intersections
                Road connectingRoad = graph.Roads[currentIntersection]
                    .FirstOrDefault(road => road.DestinationIntersection == nextIntersection);

                if (connectingRoad != null)
                {
                    totalVehicleDistance += connectingRoad.Length;
                }
            }
            double totalVehicleDistanceRounded = Math.Round(totalVehicleDistance, 2);
            return (l, totalTimeRounded, totalWalkingDistanceRounded, totalVehicleDistanceRounded);
        }


    }
}
