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
    public class FinalOutput
    {
        public List<int> List { set; get; } = new();
        public double TotalTime { set; get; }
        public double TotalWalkingDistance { set; get; }
        public double TotalVehicleDistance { set; get; }
        public double ShortestDistance { set; get; }
    }
    internal class shortestPath
    {
        //function startpoints endpoints 
        //List<Intersection> nodes = new List<Intersection>();
        //Query query;

        Dictionary<int, double> StartNodes = new Dictionary<int, double>();
        Dictionary<int, double> EndNodes = new Dictionary<int, double>();
        Result result;
        public FinalOutput output { set; get; }
        public shortestPath(Dictionary<int, double> StartNodes, Dictionary<int, double> EndNodes)
        {
            this.StartNodes = StartNodes;
            this.EndNodes = EndNodes;
            this.result = new Result();
            this.output = new FinalOutput();
        }


        public void Dijkstra(Map graph)
        {
            int nodesCount = graph.Intersections.Count;
            result.Distances = new double[nodesCount];
            result.Parents = new int[nodesCount];
            const double Infinity = double.PositiveInfinity;
            for (int i = 0; i < nodesCount; i++)
            {
                result.Distances[i] = Infinity;
                result.Parents[i] = -1;
            }

            var priorityQueue = new PriorityQueue<int, double>();
            foreach (var pair in StartNodes)
            {
                //in queue add the start nodes with their walking time as the distances
                result.Distances[pair.Key] = pair.Value;
                priorityQueue.Enqueue(pair.Key, pair.Value);
            }


            while (priorityQueue.Count > 0)
            {
                //var current = priorityQueue.Dequeue();
                priorityQueue.TryDequeue(out int currentNode, out double currentDistance);
                //take the current distance from the current node in the queue
                //double currentDistance = current.distance;
                //int currentNode = current.node;

                //if current > distance of node so skip
                if (currentDistance > result.Distances[currentNode])
                    continue;


                // relax neighbors
                foreach (Road road in graph.Roads[currentNode])
                {
                    int neighbor = road.DestinationIntersection;
                    double timeInHours = road.Length / road.Speed;
                    double newDistance = currentDistance + timeInHours;

                    if (newDistance < result.Distances[neighbor])
                    {
                        result.Distances[neighbor] = newDistance;
                        result.Parents[neighbor] = currentNode;
                        priorityQueue.Enqueue(neighbor, newDistance);
                    }
                }
            }
        }

        public (int, double) GetBestEndNode()
        {
            int bestEnd = -1;
            double min = double.PositiveInfinity;

            foreach (var pair in EndNodes)
            {
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

        public List<int> constructPath(int endNode)
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

        public void FinalResult(Map graph)
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

            output.TotalTime = Math.Round(min * 60, 2);

            output.List = constructPath(bestEnd);
            if (!output.List.Contains(bestEnd))
                output.List.Add(bestEnd);

            output.TotalWalkingDistance = 5 * (EndNodes[bestEnd] + StartNodes[output.List[0]]);
            output.TotalVehicleDistance = 0;

            int outputCount = output.List.Count - 1;
            for (int i = 0; i < outputCount; i++)
            {
                int currentIntersection = output.List[i];
                int nextIntersection = output.List[i + 1];

                // Find the road between current and next intersections
                Road connectingRoad = graph.Roads[currentIntersection].FirstOrDefault(road => road.DestinationIntersection == nextIntersection);

                if (connectingRoad != null)
                {
                    output.TotalVehicleDistance += connectingRoad.Length;
                }
            }
            output.ShortestDistance = output.TotalWalkingDistance + output.TotalVehicleDistance;
            //output.TotalWalkingDistance = output.TotalWalkingDistance;
            //output.TotalVehicleDistance = output.TotalVehicleDistance;
            //output.ShortestDistance = output.ShortestDistance;
        }


    }
}
