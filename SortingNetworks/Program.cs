using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using SortingNetworks.Parallel;

namespace SortingNetworks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using SortingNetworks.Utils;

    public class Program
    {
        // It's five million nets a lot to have in memory?
        private static int MAX_GENERATE_WITHOUT_BATCHES = 5000000;

        public static void Main(string[] args)
        {
            void SaveNetworks(ushort size, int i, IReadOnlyList<IComparatorNetwork> readOnlyList)
            {
                Trace.WriteLine($"Save Network for step {i}");
                System.IO.Directory.CreateDirectory("SavedNetworks");
                //var binarySerializer =
                //    new BinarySerializer($"SavedNetworks/nets_{size}_{i}_{DateTime.Now:yyyyMMddHHmmssfff}.dat");
                //binarySerializer.Serialize(readOnlyList);

                var jsonOptions = new JsonSerializerOptions()
                {
                    IncludeFields = true
                };

                var path = $"SavedNetworks/nets_{size}_{i}_{DateTime.Now:yyyyMMddHHmmssfff}.json";
                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.Write(JsonSerializer.SerializeToUtf8Bytes(readOnlyList, jsonOptions));
                }

                Trace.WriteLine($"Saved network in {path}");

                //var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            }

            ushort pause = 0;
            var copySteps = new List<ushort>();
            var batchSize = MAX_GENERATE_WITHOUT_BATCHES/2;
            IReadOnlyList<IComparatorNetwork> comparatorNets = null;
            IPruner pruner = new Pruner();
            IPruner.Threads = 1;

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
                    case "-l":
                        // log file
                        var traceFile = arg.Substring(3);
                        InitiateTracer(CreateDefaultListeners(traceFile));
                        break;
                    case "-p":
                        // step for pause
                        pause = Convert.ToUInt16(arg.Substring(3));
                        break;
                    case "-r":
                        var path = arg.Substring(3);
                        var serializer = new BinarySerializer(path);
                        comparatorNets = serializer.Deserialize<IReadOnlyList<IComparatorNetwork>>();
                        break;
                    case "-c":
                        copySteps.AddRange(arg.Substring(3).Split(",").Select(step => Convert.ToUInt16(step)));
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
                    default:
                        Trace.WriteLine("Usage:");
                        Trace.WriteLine("-s input size");
                        Trace.WriteLine("-k comparators number");
                        Trace.WriteLine("-l log file path");
                        Trace.WriteLine("-p pause step");
                        Trace.WriteLine("-r resume from binary file");
                        Trace.WriteLine("-c create copy file at c step comma separated");
                        break;
                }
            }

            var comparatorsGenerator = new ComparatorsGenerator();
            var sortingNetworksGenerator = new Generator();
            var comparators = comparatorsGenerator.GenerateComparators(Enumerable.Range(0, IComparatorNetwork.Inputs).ToArray());
            var generatorPruner = new GeneratePruneInBatches(batchSize);
            var stopWatch = Stopwatch.StartNew();

            comparatorNets ??= new List<IComparatorNetwork> { new ComparatorNetwork(new Comparator[1]{new Comparator(0,1)})};
            var currentComparator = comparatorNets[0].Comparators.Length;

            for (var i = currentComparator; i < IComparatorNetwork.NumComparators; i++)
            {
                Trace.WriteLine($"Adding Comparator {i + 1}");
                //if (copySteps.Contains((ushort)i))
                //{
                //    SaveNetworks(IComparatorNetwork.Inputs, i, comparatorNets);
                //}

                //if (comparatorNets.Count >= MAX_GENERATE_WITHOUT_BATCHES)
                //{
                //    Trace.WriteLine($"Generate and prune--------------");
                //    var generatePruneWatch = Stopwatch.StartNew();
                //    comparatorNets = generatorPruner.GeneratePrune(comparatorNets, comparators);
                //    Trace.WriteLine($"Length after Generate and Prune: {comparatorNets.Count}");
                //    Trace.WriteLine($"Prune time  {generatePruneWatch.Elapsed}");
                //}
                //else
                //{
                    Trace.WriteLine($"Generate--------------");
                    var generateWatch = Stopwatch.StartNew();
                    comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators);

                    if (comparatorNets.Count > 15000)
                    {
                        comparatorNets = HeuristicRemover.RemoveNetsWithMoreOutputs(comparatorNets);
                    }

                    Trace.WriteLine($"Length after Generate: {comparatorNets.Count}");
                    Trace.WriteLine($"Generate time  {generateWatch.Elapsed}");
                    var count = double.Parse(comparatorNets.Count.ToString());

                    if (copySteps.Contains((ushort)i))
                    {
                        SaveNetworks(IComparatorNetwork.Inputs, i, comparatorNets);
                    }

                    Trace.WriteLine($"Prune--------------");
                    var pruneWatch = Stopwatch.StartNew();
                    if (IPruner.Threads > 1)
                    {
                        var splitNets = comparatorNets.SplitList(Math.Max((int)Math.Ceiling(count / IPruner.Threads), 0)).ToList();
                        comparatorNets = pruner.Prune(splitNets);
                    }
                    else
                    {
                        comparatorNets = pruner.Prune(comparatorNets);
                    }

                    Trace.WriteLine($"Length after Prune: {comparatorNets.Count}");
                    Trace.WriteLine($"Prune time  {pruneWatch.Elapsed}");
                //}
#if DEBUG
                Trace.WriteLine($"Is subset: {IComparatorNetwork.IsSubset}");
                Trace.WriteLine($"Is subset dual: {IComparatorNetwork.IsSubsetDual}");
                Trace.WriteLine($"Try permutations call: {IComparatorNetwork.TryPermutationCall:N}");
                IComparatorNetwork.IsSubset = 0;
                IComparatorNetwork.IsSubsetDual = 0;
                IComparatorNetwork.TryPermutationCall = 0;
#endif
                Trace.WriteLine(string.Empty);

                if (pause != 0 && i + 1 == pause)
                {
                    SaveNetworks(IComparatorNetwork.Inputs, i + 1, comparatorNets);
                    Trace.WriteLine($"Stopped execution at step: {i + 1}");
                    break;
                }
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

            PrintSortingNetworks(comparatorNets.Where(x => x.IsSortingNetwork()).ToList());
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
                TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime
            };
            var ctl = new ConsoleTraceListener(false) { TraceOutputOptions = TraceOptions.DateTime };

            return new TraceListener[] { twtl, ctl };
        }

        private static void PrintSortingNetworks(IReadOnlyList<IComparatorNetwork> nets)
        {
            var sortingNets = nets.Where(x => x.IsSortingNetwork()).ToList();
            Trace.WriteLine($"{sortingNets.Count} Sorting Networks found with {IComparatorNetwork.Inputs} inputs and {nets[0].Comparators.Length} comparators");
            foreach (var n in sortingNets)
            {
                PrintComparatorNet(n);
            }
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
