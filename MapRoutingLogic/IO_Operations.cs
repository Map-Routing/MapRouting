using System;
using System.Collections.Generic;
using System.Globalization;
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
            string[] lines = File.ReadAllLines(mapfile);
            int lineIndex = 0;


            Map map = new Map();

            int numberOfIntersections = int.Parse(lines[lineIndex++]);

            for (int i = 0; i < numberOfIntersections; i++)
            {
                string[] line = lines[lineIndex++].Split(' ');
                map.CreateIntersection(new Intersection(int.Parse(line[0]), double.Parse(line[1]),
                    double.Parse(line[2])));
            }


            int numberOfRoads = int.Parse(lines[lineIndex++]);

            for (int i = 0; i < numberOfRoads; i++)
            {
                string[] line = lines[lineIndex++].Split(' ');
                map.CreateRoad(int.Parse(line[0]), int.Parse(line[1]),
                    double.Parse(line[2]), int.Parse(line[3]));
            }

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
            
            int partsCount = parts.Count;
            for (int k = 1; k < partsCount; k += 5)
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


        public void ClaculateOutput(TestCase testCase)
        {
            int queriesCount = testCase.Queries.Count;
            // Pre-size the list if possible
            testCase.Outputs = new List<Output>(queriesCount);


            // Initialize with nulls to reserve slots
            for (int i = 0; i < queriesCount; i++)
            {
                testCase.Outputs.Add(null);
            }
            bool parallelize = queriesCount >= 1000;
            if (parallelize)
            {
                Parallel.For(0, queriesCount, i =>
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
                        shortestDistance = shortpath.output.ShortestDistance,
                        TotalWalkingDistance = shortpath.output.TotalWalkingDistance,
                        TotalVehicleDistance = shortpath.output.TotalVehicleDistance
                    };

                    // Direct assignment by index - thread-safe!
                    testCase.Outputs[i] = output;
                });
            }
            else
            {
                for (int i = 0; i < queriesCount; i++)
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
                        shortestDistance = shortpath.output.ShortestDistance,
                        TotalWalkingDistance = shortpath.output.TotalWalkingDistance,
                        TotalVehicleDistance = shortpath.output.TotalVehicleDistance
                    };

                    
                    testCase.Outputs[i] = output;
                }
            }
        }

        
        public async void RunTestCases()
        {
            string sampleCasesInput = @"[1] Sample Cases\Input\";
            string MediumInput = @"[2] Medium Cases\Input\";
            string LargeInput = @"[3] Large Cases\Input\";
            
            string InputFolder = @"..\..\..\TEST CASES\";
            string OutputFolder = @"..\..\..\TEST CASES\";
            Console.WriteLine($"Choose Number of TestCases \n[1] Sample Cases\n[2] Medium Cases\n[3] Large Cases\n[4] Enter your own map file and quiries file");
            string mapfile = "";
            string queryfile ="";

            string choice = Console.ReadLine() ;
            int sz = 1;
            string outPath = "";
            string[] filename =new string[3];
            switch (choice)
            {
                case "1":
                    InputFolder += sampleCasesInput;
                    sz = 5;
                    filename[0] = "map";
                    filename[1] = "queries";
                    filename[2] = "output";
                    outPath = @$"..\..\..\Output\[1] Sample Cases\";
                    break; 
                case "2":
                    InputFolder += MediumInput;
                    sz = 1;
                    filename[0] = "OLMap";
                    filename[1] = "OLQueries";
                    filename[2] = "OLOutput";
                    outPath = @$"..\..\..\Output\[2] Medium Cases\";
                    break;
                case "3":
                    InputFolder += LargeInput;
                    sz = 1;
                    filename[0] = "SFMap";
                    filename[1] = "SFQueries";
                    filename[2] = "SFOutput";
                    outPath = @$"..\..\..\Output\[3] Large Cases\";
                    break;
                case "4":
                    Console.WriteLine("Enter map file path: ");
                    mapfile = Console.ReadLine();
                    Console.WriteLine("Enter Query file path: ");
                    queryfile = Console.ReadLine();
                    sz = 1;
                    filename[2] = "Ouput.txt";
                    outPath = @"..\..\..\Result\";
                    break;
                default: return;
            }
            int numOfTestCase = 1;

            if (sz > 1)
            {
                Console.Write($"Enter number of map test from {1} to {sz}: ");
                numOfTestCase = int.Parse(Console.ReadLine());
            }


            DirectoryInfo di = Directory.CreateDirectory(outPath);

            TestCase testCase = new TestCase();

            if(choice != "4" && sz == 1)
            {
                mapfile =InputFolder + $"{filename[0]}.txt" ;
                queryfile = InputFolder + $"{filename[1]}.txt";
            }
            else if (sz > 1 && choice != "4")
            {
                mapfile = InputFolder + $"{filename[0]}{numOfTestCase}.txt";
                queryfile = InputFolder + $"{filename[1]}{numOfTestCase}.txt";
            }

            var watchWithIO = System.Diagnostics.Stopwatch.StartNew();

            testCase.TestMap = LoadMap(mapfile);

            testCase.Queries = LoadQueries(queryfile);

            // Calculate Our Output
            var watch = System.Diagnostics.Stopwatch.StartNew();
            ClaculateOutput(testCase);
            watch.Stop();

            testCase.TotalExecNoIO = watch.ElapsedMilliseconds;

            string outfile = Path.Combine(outPath, $"output{numOfTestCase}.txt");

            File.WriteAllText(outfile, "");

            int outputsCount = testCase.Outputs.Count;
            for (int k = 0; k < outputsCount; k++)

            {
                File.AppendAllText(outfile,$"{string.Join(' ', testCase.Outputs[k].IdOfIntersections)}" +
                    $"\n" +
                    $"{testCase.Outputs[k].shortestTime:F} mins" +
                    $"\n" +
                    $"{testCase.Outputs[k].shortestDistance:F} km" +
                    $"\n" +
                    $"{testCase.Outputs[k].TotalWalkingDistance:F} km" +
                    $"\n" +
                    $"{testCase.Outputs[k].TotalVehicleDistance:F} km" +
                    $"\n\n");

            }

            watchWithIO.Stop();
            testCase.TotalExec = watchWithIO.ElapsedMilliseconds;
            File.AppendAllText(outfile, $"{testCase.TotalExecNoIO} ms" +
                $"\n\n" +
                $"{testCase.TotalExec} ms");


            Console.WriteLine("Execution Time = " + testCase.TotalExecNoIO);

            Console.WriteLine("Execution Time With IO = " + testCase.TotalExec);
        }

    }


}

