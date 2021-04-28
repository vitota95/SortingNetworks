//#define SAVEALL
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using SortingNetworks.Extensions;
using SortingNetworks.Parallel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SortingNetworks
{
    public class Program
    {
        // It's five million nets a lot to have in memory?
        private static int MAX_GENERATE_WITHOUT_BATCHES = 5000000;

        private static int[] stateOfTheArtBestSizes = {0, 1, 3, 5, 9, 12, 16, 19, 25, 29, 35, 39, 45, 51, 56, 60, 71, 77, 85, 91, 100, 107, 115, 120, 132, 139, 150, 155, 165, 172, 180, 185 };

        public static void Main(string[] args)
        {
            var copySteps = new List<int>();
            var batchSize = MAX_GENERATE_WITHOUT_BATCHES/2;
            var heuristicPopulation = 0;
            IReadOnlyList<IComparatorNetwork> comparatorNets = null;
            IPruner pruner = new Pruner();
            IPruner.Threads = 1;
            bool heuristicTest = false;

            foreach (var arg in args)
            {
                switch (arg.Substring(0, 2).ToLower())
                {
                    case "-s":
                        // size
                        var size = Convert.ToUInt16(arg.Substring(3));
                        IComparatorNetwork.Inputs = size;
                        break;
                    case "-k":
                        // comparators
                        IComparatorNetwork.NumComparators = Convert.ToUInt16(arg.Substring(3));
                        break;
                    case @"-l":
                        // log file
                        var traceFile = arg.Substring(3);
                        InitiateTracer(CreateDefaultListeners(traceFile));
                        break;
                    case "-r":
                        var path = arg.Substring(3);
                        using (StreamReader file = File.OpenText(path))
                        {
                            comparatorNets = JsonSerializer.Deserialize<IReadOnlyList<ComparatorNetwork>>(file.ReadToEnd());
                        }

                        break;
                    case "-t":
                        IPruner.Threads = Convert.ToUInt16(arg.Substring(3));
                        if (IPruner.Threads > 1)
                        {
                            pruner = new ParallelPruner();
                        }
                        break;
                    case "-b":
                        batchSize = Convert.ToInt32(arg.Substring(3));
                        break;  
                    case "-h":
                        heuristicPopulation = Convert.ToInt32(arg.Substring(3));
                        break;
                    case "-p":
                        heuristicPopulation = Convert.ToInt32(arg.Substring(3));
                        heuristicTest = true;
                        break;
                    default:
                        Trace.WriteLine("Usage:");
                        Trace.WriteLine(@"-s input size");
                        Trace.WriteLine(@"-k comparators number");
                        Trace.WriteLine(@"-l log file path");
                        Trace.WriteLine(@"-r resume from binary file");
                        Trace.WriteLine(@"-c create copy file at c step comma separated");
                        Trace.WriteLine(@"-t number of threads");
                        Trace.WriteLine(@"-b batch size (for very large sets)");
                        Trace.WriteLine(@"-h heuristic maximum population size");
                        break;
                }
            }

            if (heuristicTest)
            {
                for (var i = heuristicPopulation; i > 1; i--)
                {
                    if (!FindSortingNetwork(batchSize, comparatorNets, pruner, i, copySteps))
                    {
                        Console.WriteLine($"The minimum heuristic population is: {i+1}");
                        break;
                    }
                }
            }
            else
            {
                FindSortingNetwork(batchSize, comparatorNets, pruner, heuristicPopulation, copySteps);
            }
        }

        private static bool FindSortingNetwork(int batchSize, IReadOnlyList<IComparatorNetwork> comparatorNets, IPruner pruner,
            int heuristicPopulation, List<int> copySteps)
        {
            var comparatorsGenerator = new ComparatorsGenerator();
            var sortingNetworksGenerator = new Generator();
            var comparators =
                comparatorsGenerator.GenerateComparators(Enumerable.Range(0, IComparatorNetwork.Inputs).ToArray());
            var generatorPruner = new GeneratePruneInBatches(batchSize);
            var stopWatch = Stopwatch.StartNew();

            //comparatorNets ??= new List<IComparatorNetwork> {new ComparatorNetwork(new Comparator[1] {new Comparator(0, 1)})};
            comparatorNets ??= new List<IComparatorNetwork>();

            if (comparatorNets.Count == 0)
            {
                var firstNets = new List<IComparatorNetwork>();
                for (var i = 0; i < comparators.Count; i++)
                {
                    firstNets.Add(new ComparatorNetwork(new[] {comparators[i]}));
                }

                comparatorNets = firstNets;
            }

            var currentComparator = comparatorNets.Count > 0 ? comparatorNets[0].Comparators.Length : 0;

            var rand = new Random();

            for (var i = currentComparator; i < IComparatorNetwork.NumComparators; i++)
            {
                Trace.WriteLine($"Adding Comparator {i + 1}");
                Trace.TraceInformation($"Generate");
                var generateWatch = Stopwatch.StartNew();

                if (comparatorNets.Count > batchSize)
                {
                    comparatorNets = generatorPruner.GeneratePrune(comparatorNets, comparators);
                    Trace.WriteLine($"Length after prune: {comparatorNets.Count}");
                    // Sorting network found
                    if (comparatorNets.Count == 1)
                    {
                        break;
                    }

                    continue;
                }

                comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators).OrderBy(x => rand.Next()).ToList();
                //var newNets = sortingNetworksGenerator.Generate(comparatorNets, comparators).ToList();
                //comparatorNets = newNets;

                Trace.WriteLine($"Length after Generate: {comparatorNets.Count}");
                Trace.WriteLine($"Generate time  {generateWatch.Elapsed}");
                var count = double.Parse(comparatorNets.Count.ToString());

                Trace.TraceInformation($"Prune");
                var pruneWatch = Stopwatch.StartNew();
                if (IPruner.Threads > 1)
                {
                    var splitNets = comparatorNets.SplitList(Math.Max((int) Math.Ceiling(count / IPruner.Threads), 10000))
                        .ToList();
                    comparatorNets = pruner.Prune(splitNets);
                }
                else
                {
                    comparatorNets = pruner.Prune(comparatorNets);
                }

                if (heuristicPopulation > 0 && comparatorNets.Count > heuristicPopulation)
                {
                    //comparatorNets = HeuristicRemover.RemoveNetsWithMoreBadZeroes(comparatorNets, heuristicPopulation);
                    comparatorNets = HeuristicRemover.RemoveNetsWithMoreOutputs(comparatorNets, heuristicPopulation);
                }


                Trace.WriteLine($"Length after Prune: {comparatorNets.Count}");
                Trace.WriteLine($"Prune time  {pruneWatch.Elapsed}");
#if DEBUG
                Trace.WriteLine($"Is subset: {IComparatorNetwork.IsSubset}");
                Trace.WriteLine($"Is subset dual: {IComparatorNetwork.IsSubsetDual}");
                Trace.WriteLine($"Try permutations call: {IComparatorNetwork.TryPermutationCall:N}");
                IComparatorNetwork.IsSubset = 0;
                IComparatorNetwork.IsSubsetDual = 0;
                IComparatorNetwork.TryPermutationCall = 0;
#endif

                Trace.TraceInformation($"Saving Nets");
                var path =
                    $"SavedNetworks/nets_{IComparatorNetwork.Inputs}_{comparatorNets[0].Comparators.Length}_{DateTime.Now:yyyyMMddHHmmssfff}.json";
#if SAVEALL
                comparatorNets.SaveToFile(path);
#endif
                if (copySteps.Contains((int) i))
                {
                    comparatorNets.SaveToFile(path);
                }

                Trace.WriteLine(string.Empty);

                // Sorting network found
                if (comparatorNets.Count == 1 && comparatorNets[0].IsSortingNetwork())
                {
                    break;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            // Trace.WriteLine($"Subsume no check: {IComparatorNetwork.SubsumeNoCheck} ");
            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");

#if DEBUG
            Trace.WriteLine(string.Empty);
            Debug.WriteLine($"Subsume total:{IComparatorNetwork.SubsumeTotal:N}");
            Debug.WriteLine($"Subsume no check 1:{IComparatorNetwork.SubsumeNoCheck1:N}");
            Debug.WriteLine($"Subsume no check 2:{IComparatorNetwork.SubsumeNoCheck2:N}");
            Debug.WriteLine($"Output count bigger:{IComparatorNetwork.OutputCountBigger:N}");
            Debug.WriteLine($"Subsume no check total:{IComparatorNetwork.SubsumeNoCheckTotal:N}");
            Debug.WriteLine($"Subsume number:{IComparatorNetwork.SubsumeNumber:N}");
            Debug.WriteLine($"Subsume succeed:{IComparatorNetwork.SubsumeSucceed:N}");
            Debug.WriteLine($"Permutations performed:{IComparatorNetwork.PermutationsNumber:N}");
            Debug.WriteLine($"Permutations walk:{IComparatorNetwork.PermutationsWalk:N}");
#endif
            Trace.WriteLine(string.Empty);

            PrintSortingNetworks(comparatorNets.Where(x => x.IsSortingNetwork2N()).ToList(), IComparatorNetwork.Inputs);

            return comparatorNets.Any(x => x.IsSortingNetwork2N());
        }

#if DEBUG
        private static void ResetCounters()
        {
            IComparatorNetwork.SubsumeNoCheck1 = 0;
            IComparatorNetwork.SubsumeNoCheck2 = 0;
            IComparatorNetwork.OutputCountBigger = 0;
            IComparatorNetwork.PermutationsNumber = 0;
            IComparatorNetwork.PermutationsWalk = 0;
            IComparatorNetwork.SubsumeNumber = 0;
            IComparatorNetwork.SubsumeSucceed= 0;
        }
#endif

        public static void InitiateTracer(TraceListener[] listeners)
        {
            Trace.Listeners.Clear();

            foreach (var traceListener in listeners)
            {
                Trace.Listeners.Add(traceListener);
            }

            Trace.AutoFlush = true;
        }

        private static TraceListener[] CreateDefaultListeners(string traceFile)
        {
            var twtl = new TextWriterTraceListener(traceFile)
            {
                Name = "TextLogger",
                TraceOutputOptions = TraceOptions.DateTime
            };
            var ctl = new ConsoleTraceListener(false) { TraceOutputOptions = TraceOptions.DateTime };

            return new TraceListener[] { twtl, ctl };
        }

        private static void PrintSortingNetworks(IReadOnlyList<IComparatorNetwork> nets, int size)
        {
            if (nets == null)
            {
                Trace.WriteLine("--------No Sorting networks found.");
                return;
            }

            var sortingNets = nets.Where(x => x.IsSortingNetwork()).ToList();

            if (sortingNets.Count > 0)
            {

                if (sortingNets[0].Comparators.Length < stateOfTheArtBestSizes[size - 1])
                {
                    Trace.WriteLine($"NEW SORTING NETWORK SIZE BOUND {sortingNets[0].Comparators.Length}");
                    var text = $"New Sorting network found with size {size} and {sortingNets[0].Comparators.Length} comparators: ";

                    foreach (var c in sortingNets[0].Comparators)
                    {
                        text += $"({c.X},{c.Y}) ";

                    }

                    text += "\n";

                    File.AppendAllText("NewSortingNetworksFound.txt", text);
                }

                Trace.WriteLine($"{sortingNets.Count} Sorting Networks found with {IComparatorNetwork.Inputs} inputs and {nets[0]?.Comparators.Length} comparators");
                foreach (var n in sortingNets)
                {
                    PrintComparatorNet(n);
                }
                return;
            }

            Trace.WriteLine("No Sorting networks found");
        }

        private static void PrintComparatorNet(IComparatorNetwork net)
        {
            foreach (var c in net.Comparators)
            {
                Trace.Write($"({c.X},{c.Y}) ");
            }
            Trace.WriteLine(string.Empty);
        }
    }
}
