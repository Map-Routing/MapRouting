using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace MapRoutingLogic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}


//def find_shortest_time_path(graph, S, E, R, walking_speed):
//    # Find all nodes within R of S and E
//    start_nodes = [node for node in graph.nodes if distance(S, node) <= R]
//    end_nodes = [node for node in graph.nodes if distance(node, E) <= R]

//    min_time = infinity
//    best_path = None

//    for start_node in start_nodes:
//        T_start = distance(S, start_node) / walking_speed

//        for end_node in end_nodes:
//            T_end = distance(end_node, E) / walking_speed

//            # Skip if walking alone is worse than current best
//            if (T_start + T_end) >= min_time:
//                continue

//            # Compute shortest path on graph
//            graph_time, graph_path = dijkstra(graph, start_node, end_node)

//            total_time = T_start + graph_time + T_end

//            if total_time < min_time:
//                min_time = total_time
//                best_path = {
//    'start_walk': (S, start_node),
//                    'graph_path': graph_path,
//                    'end_walk': (end_node, E),
//                    'total_time': total_time
//                }

//return best_path
