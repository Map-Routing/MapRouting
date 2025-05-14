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
        public Map LoadMap(string mapfile)
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

        public void LoadActualOutputs(ref TestCase testCase,string OutputFile, int numOfOutputs)
        {
            List<string> lines = File.ReadLines(OutputFile).ToList();

            int sz = lines.Count();
            for(int i=0;i<sz-2;)
            {
                if(string.IsNullOrWhiteSpace(lines[i]))
                {
                    i++;
                    continue;
                }
                if (lines[i].Contains("ms"))
                {
                    break;
                }

                Output output = new Output();
                output.IdOfIntersections = lines[i]
                                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                .Select(x => Convert.ToInt32(x)).ToList();

                output.shortestTime = Convert.ToDouble(
                                                        lines[i+1]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.shortestDistance = Convert.ToDouble(
                                                        lines[i + 2]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalWalkingDistance = Convert.ToDouble(
                                                        lines[i + 3]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalVehicleDistance = Convert.ToDouble(
                                                        lines[i+4]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                i = i + 5;
                testCase.ActualOutputs.Add(output);
            }


            testCase.ActualTotalExecNoIO = Convert.ToDouble(
                                                        lines[sz - 3]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );


            testCase.ActualTotalExec = Convert.ToDouble(
                                                        lines[sz - 1]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );


            // For Debugging
            //Console.WriteLine("\n___________Start________\n");
            //foreach (var o in testCase.ActualOutputs)
            //{
            //    Console.WriteLine(o);
                
            //    Console.WriteLine("**********************");
            //}
            //Console.WriteLine($"-------------------------- \n" +
            //                  $"| Exec with no IO = {testCase.TotalExecNoIO}        \n|" +
            //                  $"| Exec IO = {testCase.ActualTotalExec}        \n|" +
            //                  $"---------------------------\n");
        }
        
        public void ClaculateOutput(ref TestCase testCase)
        {
            List<Output> OutputResult = new List<Output>();
           
            for(int i = 0; i < testCase.Queries.Count(); i++)
            {
                /*
                 1. path
                2. shortestTime
                3. shortest Distance
                4. TotalWalkingDistance
                5. TotalvehicleDistance
                 */

                var potentialNodes = new potentialNodes();
                var AllIntersection = testCase.TestMap.Intersections.Values.ToList();
                var startAndEndNodes = potentialNodes.findValidNodes(AllIntersection, testCase.Queries[i]);

                var shortpath = new shortestPath(startAndEndNodes.Item1,startAndEndNodes.Item2);

                //var OurOutput = shortestPath
                shortpath.FinalResult(testCase.TestMap);
                var FinalResult = shortpath.output;
                var shortestDistanceRounded = Math.Round(FinalResult.ShortestDistance, 2, MidpointRounding.AwayFromZero);
                
                testCase.Outputs.Add(new Output()
                {
                    IdOfIntersections = FinalResult.List, 
                    shortestTime= FinalResult.TotalTime,
                    shortestDistance = (shortestDistanceRounded),
                    TotalWalkingDistance = FinalResult.TotalWalkingDistance,
                    TotalVehicleDistance = FinalResult.TotalVehicleDistance,
                }); // replace it by calling shortest path method
                
            }

            // IO time
            // time without IO (in for loop)

        }
        public void CompareResult(TestCase testCase)
        {
            int sz = testCase.ActualOutputs.Count();
            for (int i = 0; i < sz; i++)
            {
                string message = "";
                var result = testCase.Outputs[i].Equals(testCase.ActualOutputs[i], ref message);
                Console.WriteLine($"Result of Output {i + 1} = {result}\n{message}\n");
            }
        }
        
        public void RunTestCases()
        {
            string sampleCases = @"[1] Sample Cases\";
            string Medium = @"[2] Medium Cases\";
            string Large = @"[3] Large Cases\";

            string InputFolder = @"..\..\..\TEST CASES\";
            Console.WriteLine($"Choose Number of TestCases \n{sampleCases}\n{Medium}\n{Large}");

            string choice = Console.ReadLine() ;
            
            switch (choice)
            {
                case "1":
                    InputFolder += sampleCases;
                    break;
                case "2":
                    InputFolder += Medium;
                    break;
                case "3":
                    InputFolder += Large;
                    break;

                default: return;
            }
            var mapPathes = Directory.EnumerateFiles(InputFolder, "*ap*.*", SearchOption.AllDirectories).ToList();
            var queryPathes = Directory.EnumerateFiles(InputFolder, "*ries*.*", SearchOption.AllDirectories).ToList();
            var OutputPathes = Directory.EnumerateFiles(InputFolder, "*utput*.*", SearchOption.AllDirectories).ToList();
            
            List< TestCase> TestCases = new List<TestCase>();
            for(int i=0;i<mapPathes.Count();i++)
            {
                var watchWithIO = System.Diagnostics.Stopwatch.StartNew();

                TestCase testCase = new TestCase();
                // map of test case
                testCase.TestMap = LoadMap(mapPathes[i]);

                // queires of test case;

                testCase.Queries = LoadQueries(queryPathes[i]);

                // actual outputs 
                LoadActualOutputs(ref testCase,OutputPathes[i], testCase.Queries.Count());

                // Calculate Our Output
                var watch = System.Diagnostics.Stopwatch.StartNew();

                ClaculateOutput(ref testCase);

                watch.Stop();
                testCase.TotalExecNoIO = watch.ElapsedMilliseconds;
                Console.WriteLine("Execution Time = "+testCase.TotalExecNoIO);

                watchWithIO.Stop();
                testCase.TotalExec = watchWithIO.ElapsedMilliseconds;
                
                Console.WriteLine("Execution Time With IO = "+testCase.TotalExec);


                CompareResult(testCase);

            }
        }

    }
}

