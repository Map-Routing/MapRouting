using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    internal class Map
    {
        public Dictionary<int, Intersection> Intersections {get; } = new Dictionary<int, Intersection>();
        public Dictionary<int, List<Road>> Roads {get; } = new Dictionary<int, List<Road>>();

        public bool IsDynamic { set; get; } = false;
        public void CreateIntersection(Intersection intersection) { Intersections[intersection.ID] = intersection; }
        public void CreateRoad(int source, int destination, double length, int speed)
        {
            if (!Roads.ContainsKey(source))
                Roads[source] = new List<Road>();
            Roads[source].Add(new Road(source, destination, length, speed));
                
            if (!Roads.ContainsKey(destination))
                Roads[destination] = new List<Road>();
            Roads[destination].Add(new Road(destination, source, length, speed));

        }

        public List<int> BFS(int startIntersection)
        {
            if (!Intersections.ContainsKey(startIntersection))
                throw new ArgumentException($"start intersection {startIntersection} not found");

            bool[] visited = new bool[Intersections.Keys.Max() + 1];
            Queue<int> queue = new Queue<int>();

            queue.Enqueue(startIntersection);
            visited[startIntersection] = true;

            List<int> result = new List<int>();
            int currentIntersection;
            while (queue.Count != 0)
            {
                currentIntersection = queue.Dequeue();
                result.Add(currentIntersection);
                if(Roads.ContainsKey(currentIntersection)) 
                {
                    foreach (var road in Roads[currentIntersection])
                    {
                        if (!visited[road.DestinationIntersection])
                        {
                            queue.Enqueue(road.DestinationIntersection);
                            visited[road.DestinationIntersection] = true;
                        }
                    }
                }
            }
            return result;
        }

        public List<int> DFS(int startIntersection)
        {
            if (!Intersections.ContainsKey(startIntersection))
                throw new ArgumentException($"start intersection {startIntersection} not found");

            bool[] visited = new bool[Intersections.Keys.Max() + 1];
            Stack<int> stack = new Stack<int>();

            stack.Push(startIntersection);
            List<int> result = new List<int>();
            int currentIntersection;
            while(stack.Count != 0)
            {
                currentIntersection = stack.Pop();
                if(!visited[currentIntersection])
                {
                    visited[currentIntersection] = true;
                    result.Add(currentIntersection);

                    if(Roads.ContainsKey(currentIntersection))
                    {
                        foreach (var road in Roads[currentIntersection])
                        {
                            if (!visited[road.DestinationIntersection])
                                stack.Push(road.DestinationIntersection);
                        }
                    }
                }
            }
            return result;
        }
        public void PrintMap()
        {
            foreach (var intersection in Intersections)
            {
                Console.WriteLine($"Intersection {intersection.Key} at ({intersection.Value.X}, {intersection.Value.Y}):");

                if (Roads.ContainsKey(intersection.Key))
                {
                    foreach (var road in Roads[intersection.Key])
                    {
                        Console.WriteLine($"  Connected to {road.DestinationIntersection} (time: {road.Time:F3} minutes)");
                    }
                }

                Console.WriteLine();
            }

        }
    }
}
