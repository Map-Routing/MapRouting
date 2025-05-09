using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
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

        public List<Query> LoadQueries(string queryFile)
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

            /*FOR DEBUGGING*/
            //Console.WriteLine("_________Queries________\n");
            //foreach(var q in queries)
            //{
            //    Console.WriteLine(q);
            //}
            //Console.WriteLine("\n_________EDN________\n\n");

            return queries;
        }

        public List<Output> LoadActualOutputs(string OutputFile, int numOfOutputs)
        {
            IEnumerable<string> lines = File.ReadLines(OutputFile);

            List<Output> outputs = new List<Output>(numOfOutputs);

            for(int i=0;i<lines.Count()-2;)
            {
                if(string.IsNullOrWhiteSpace(lines.ElementAt(i)))
                {
                    i++;
                    continue;
                }
                if (lines.ElementAt(i).Contains("ms"))
                {
                    break;
                }

                Output output = new Output();
                output.IdOfIntersections = lines.ElementAt(i)
                                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => Convert.ToInt32(x)).ToList();

                output.shortestTime = Convert.ToDouble(
                                                        lines.ElementAt(i + 1)
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.shortestDistance = Convert.ToDouble(
                                                        lines.ElementAt(i + 2)
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalWalkingDistance = Convert.ToDouble(
                                                        lines.ElementAt(i + 3)
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalVehicleDistance = Convert.ToDouble(
                                                        lines.ElementAt(i + 4)
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                i = i + 5;
                outputs.Add(output);
            }


            // For Debugging
            Console.WriteLine("\n___________Start________\n");
            //foreach(var o in outputs)
            //{
            //    Console.WriteLine(o);
            //    Console.WriteLine("**********************");
            //}
            return outputs;
        }
        
        public List<Output> ClaculateOutput(in TestCase testCase)
        {
            List<Output> OutputResult = new List<Output>();

            return OutputResult;
        }

        
        public void SampleCases()
        {
            // 1. load input files
            string InputFolder = @"..\..\..\TEST CASES\[1] Sample Cases\";
            var mapPathes = Directory.EnumerateFiles(InputFolder, "map*.*", SearchOption.AllDirectories).ToList();
            var queryPathes = Directory.EnumerateFiles(InputFolder, "que*.*", SearchOption.AllDirectories).ToList();
            var OutputPathes = Directory.EnumerateFiles(InputFolder, "output*.*", SearchOption.AllDirectories).ToList();

            List< TestCase> TestCases = new List<TestCase>();
            for(int i=0;i<mapPathes.Count();i++)
            {
                TestCase testCase = new TestCase();
                // map of test case
                testCase.TestMap = BuildMap(mapPathes[i]);

                // queires of test case;

                testCase.Queries = LoadQueries(queryPathes[i]);

                // actual outputs 
                testCase.Outputs = LoadActualOutputs(OutputPathes[i], testCase.Queries.Count);

                // Calculate Our Output
            
            
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

