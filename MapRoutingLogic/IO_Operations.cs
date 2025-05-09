using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapRoutingLogic
{
    public class IO_Operations
    {
        public string InputPath { get; set; }
        public string ActualOutputPath { get; set; }
        public string OutputPath { get; set; }

        public IO_Operations()
        {

        }
        // evaluate [1] Sample Cases
        public Map BuildMap(string mapfile)
        {
            string content = String.Join(" ",File.ReadAllLines(mapfile));
            string[] parts = content.Split(new[] { ' ' },
                                            StringSplitOptions.RemoveEmptyEntries);

            int numOfInter = Convert.ToInt32(parts[0]);
            int idxOfRoads = (numOfInter * 3) + 1;
            int numOfRoades = Convert.ToInt32(parts[idxOfRoads]);

            Map map = new Map();
            // fill intersections
            for (int k = 1; k < idxOfRoads; k += 3)
            {
                int Id = Convert.ToInt32(parts[k]);
                double X = Convert.ToDouble(parts[k + 1]);
                double Y = Convert.ToDouble(parts[k + 2]);
                map.CreateIntersection(new Intersection(Id, X, Y));
            }

            for (int r = idxOfRoads + 1; r < parts.Length; r += 4)
            {
                map.CreateRoad(Convert.ToInt32(parts[r]),
                               Convert.ToInt32(parts[r + 1]),
                               Convert.ToDouble(parts[r + 2]),
                               Convert.ToInt32(parts[r + 3])
                    );
            }
            //map.PrintMap();
            return map;
        }

        public List<Query> BuildQuery(string queryFile)
        {
            string content = String.Join(" ", File.ReadAllLines(queryFile));
            string[] parts = content.Split(new[] { ' ' },
                                            StringSplitOptions.RemoveEmptyEntries);

            int numOfQueries = Convert.ToInt32(parts[0]);


            List<Query> queries = new List<Query>();
            for (int k = 1; k < parts.Length; k += 5)
            {
                queries.Add(new Query(  Convert.ToDouble(parts[k]),
                                        Convert.ToDouble(parts[k + 1]),
                                        Convert.ToDouble(parts[k + 2]),
                                        Convert.ToDouble(parts[k + 3]),
                                        Convert.ToDouble(parts[k + 4])
                            ));
            }
            Console.WriteLine("_________Queries________\n");
            foreach(var q in queries)
            {
                Console.WriteLine(q);
            }
            Console.WriteLine("\n_________EDN________\n\n");
            return queries;
        }
        public void SampleCases()
        {
            // 1. load input files
            string InputFolder = @"..\..\..\TEST CASES\[1] Sample Cases\Input\";
            var mapPathes = Directory.EnumerateFiles(InputFolder, "map*.*", SearchOption.AllDirectories);
            foreach (var file in mapPathes)
            {
                // Build map
                Map map = BuildMap(file);
            }

            // 2. load quiries files

            var queryPathes = Directory.EnumerateFiles(InputFolder, "que*.*", SearchOption.AllDirectories);
            foreach(var p in queryPathes)
            {
                Console.WriteLine("query " + p);
            }
            List<List<Query>> QueriesForEveryMap = new List<List<Query>>();
            foreach(var queryFile in queryPathes)
            {
                QueriesForEveryMap.Add(BuildQuery(queryFile));
            }

            // 3. load output files

            // 4. build graph

            // 5. loop on quiries and calculate result and save in array of output object

            // 6. compare every output with acutal output
        }

        // evaluate [2] Medium Cases

        // [3] Large Cases

        // [4] BONUS Test Cases

    }
}

