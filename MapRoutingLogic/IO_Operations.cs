using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
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
            var parts = new List<string>();
            using (var reader = new StreamReader(mapfile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var lineParts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    parts.AddRange(lineParts);
                }
            }

            int numOfInter = int.Parse(parts[0]);
            int idxOfRoads = (numOfInter * 3) + 1;
            int numOfRoades = int.Parse(parts[idxOfRoads]);

            Map map = new Map();
            // fill intersections

            Parallel.Invoke(
                () =>
                {
                    for (int k = 1; k < idxOfRoads; k += 3)
                    {
                        int Id = int.Parse(parts[k]);
                        double X = double.Parse(parts[k + 1]);
                        double Y = double.Parse(parts[k + 2]);
                        map.CreateIntersection(new Intersection(Id, X, Y));
                    }
                },
                () =>
                {
                    for (int r = idxOfRoads + 1; r < parts.Count; r += 4)
                    {
                        map.CreateRoad(int.Parse(parts[r]),
                                       int.Parse(parts[r + 1]),
                                       double.Parse(parts[r + 2]),
                                       int.Parse(parts[r + 3])
                            );
                    }
                }
            );


            //map.PrintMap();
            return map;
        }

        public List<Query> LoadQueries(string queryFile)
        {
            var parts = new List<string>();
            using (var reader = new StreamReader(queryFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var lineParts = line.Split(new[] { ' ' },
                                             StringSplitOptions.RemoveEmptyEntries);
                    parts.AddRange(lineParts);
                }
            }
            // Use parts as is or call ToArray() if needed
            int numOfQueries = int.Parse(parts[0]);


            List<Query> queries = new List<Query>();
            
            for (int k = 1; k < parts.Count; k += 5)
            {
                queries.Add(new Query(double.Parse(parts[k]),
                                        double.Parse(parts[k + 1]),
                                        double.Parse(parts[k + 2]),
                                        double.Parse(parts[k + 3]),
                                        double.Parse(parts[k + 4])
                            ));
            }


            return queries;
        }

        public void LoadActualOutputs(ref TestCase testCase,string OutputFile, int numOfOutputs)
        {
            List<string> lines = File.ReadLines(OutputFile).ToList() ;

            int sz = lines.Count;

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
                                                .Select(x => int.Parse(x)).ToList();

                output.shortestTime = double.Parse(
                                                        lines[i+1]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.shortestDistance = double.Parse(
                                                        lines[i + 2]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalWalkingDistance = double.Parse(
                                                        lines[i + 3]
                                                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .ElementAt(0)
                                                      );

                output.TotalVehicleDistance = double.Parse(
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


        }

        public void ClaculateOutput(TestCase testCase)
        {
            // Pre-size the list if possible
            testCase.Outputs = new List<Output>(testCase.Queries.Count);

            // Initialize with nulls to reserve slots
            for (int i = 0; i < testCase.Queries.Count; i++)
            {
                testCase.Outputs.Add(null);
            }

            Parallel.For(0, testCase.Queries.Count, i =>
            {
                var potentialNodes = new potentialNodes();
                var AllIntersection = testCase.TestMap.Intersections.Values.ToList();
                var startAndEndNodes = potentialNodes.findValidNodes(AllIntersection, testCase.Queries[i]);

                var shortpath = new shortestPath(startAndEndNodes.Item1, startAndEndNodes.Item2);
                shortpath.FinalResult(testCase.TestMap);

                var output = new Output()
                {
                    IdOfIntersections = shortpath.output.List,
                    shortestTime = shortpath.output.TotalTime,
                    shortestDistance = Math.Round(shortpath.output.ShortestDistance, 2, MidpointRounding.AwayFromZero),
                    TotalWalkingDistance = shortpath.output.TotalWalkingDistance,
                    TotalVehicleDistance = shortpath.output.TotalVehicleDistance
                };

                // Direct assignment by index - thread-safe!
                testCase.Outputs[i] = output;
            });
        }
        public void CompareResult(TestCase testCase)
        {
            int sz = testCase.ActualOutputs.Count;
            for (int i = 0; i < sz; i++)
            {
                string message = "";
                var result = testCase.Outputs[i].Equals(testCase.ActualOutputs[i], ref message);
                Console.WriteLine($"Result of Output {i + 1} = {result}\n{message}\n");
            }
        }

        
        public async void RunTestCases()
        {
            string sampleCasesInput = @"[1] Sample Cases\Input\";
            string sampleCasesOutput = @"[1] Sample Cases\Output\";
            string MediumInput = @"[2] Medium Cases\Input\";
            string MediumOutput = @"[2] Medium Cases\Output\";
            string LargeInput = @"[3] Large Cases\Input\";
            string LargeOutput = @"[3] Large Cases\Output\";
            
            string InputFolder = @"..\..\..\TEST CASES\";
            string OutputFolder = @"..\..\..\TEST CASES\";
            Console.WriteLine($"Choose Number of TestCases \n[1] Sample Cases\n[2] Medium Cases\n[3] Large Cases");

            string choice = Console.ReadLine() ;
            int sz = 1;
            string outPath = "";
            string[] filename =new string[3];
            switch (choice)
            {
                case "1":
                    InputFolder += sampleCasesInput;
                    OutputFolder += sampleCasesOutput;
                    sz = 5;
                    filename[0] = "map";
                    filename[1] = "queries";
                    filename[2] = "output";
                    outPath = @$"..\..\..\Output\[1] Sample Cases\";
                    break; 
                case "2":
                    InputFolder += MediumInput;
                    OutputFolder += MediumOutput;
                    sz = 1;
                    filename[0] = "OLMap";
                    filename[1] = "OLQueries";
                    filename[2] = "OLOutput";
                    outPath = @$"..\..\..\Output\[2] Medium Cases\";
                    break;
                case "3":
                    InputFolder += LargeInput;
                    OutputFolder += LargeOutput;
                    sz = 1;
                    filename[0] = "SFMap";
                    filename[1] = "SFQueries";
                    filename[2] = "SFOutput";
                    outPath = @$"..\..\..\Output\[3] Large Cases\";
                    break;

                default: return;
            }
            
            DirectoryInfo di = Directory.CreateDirectory(outPath);

            for (int i=1;i<=sz;i++)
            {
                var watchWithIO = System.Diagnostics.Stopwatch.StartNew();

                TestCase testCase = new TestCase();
                // map of test case                    

                string mapfile = (choice == "1") ? InputFolder + $"{filename[0]}{i}.txt" : InputFolder + $"{filename[0]}.txt";
                string queryfile = (choice == "1") ? InputFolder + $"{filename[1]}{i}.txt" : InputFolder + $"{filename[1]}.txt";
                string outputfile = (choice == "1") ? OutputFolder + $"{filename[2]}{i}.txt" : OutputFolder + $"{filename[2]}.txt";
                Map mapresult = null;
                List<Query> queryresult = null;

                Parallel.Invoke(
                    () => 
                        mapresult = LoadMap(mapfile)
                    ,
                    () => 
                        queryresult = LoadQueries(queryfile)
                    ,
                    () => 
                        LoadActualOutputs(ref testCase, outputfile, sz)
                    
                );



                testCase.TestMap = mapresult;
                testCase.Queries = queryresult;



                // Calculate Our Output
                var watch = System.Diagnostics.Stopwatch.StartNew();

                ClaculateOutput(testCase);

                watch.Stop();

                CompareResult(testCase);

                testCase.TotalExecNoIO = watch.ElapsedMilliseconds;
                Console.WriteLine("Execution Time = "+testCase.TotalExecNoIO+ " And the Acutal is "+ testCase.ActualTotalExecNoIO);

                watchWithIO.Stop();
                testCase.TotalExec = watchWithIO.ElapsedMilliseconds;
                
                Console.WriteLine("Execution Time With IO = "+testCase.TotalExec + " And the Acutal is " + testCase.ActualTotalExec);
                string outfile = Path.Combine(outPath, $"output{i}.txt");
                File.WriteAllText(outfile, "");
                for (int k = 0; k < testCase.Outputs.Count; k++)
                {
                    File.AppendAllText(outfile,$"{string.Join(' ', testCase.Outputs[k].IdOfIntersections)}" +
                        $"\n" +
                        $"{testCase.Outputs[k].shortestTime} mins" +
                        $"\n" +
                        $"{testCase.Outputs[k].shortestDistance} km" +
                        $"\n" +
                        $"{testCase.Outputs[k].TotalWalkingDistance} km" +
                        $"\n" +
                        $"{testCase.Outputs[k].TotalVehicleDistance} km" +
                        $"\n\n");

                }
                File.AppendAllText(outfile, $"{testCase.TotalExecNoIO} ms" +
                    $"\n\n" +
                    $"{testCase.TotalExec} ms");


                Console.WriteLine("\nALL IS DONE\n");

            }
        }

    }


}

