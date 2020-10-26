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
        public static void Main(string[] args)
        {
            void SaveNetworks(ushort size, int i, IReadOnlyList<IComparatorNetwork> readOnlyList)
            {
                Trace.WriteLine($"Save Network for step {i}"); 
                System.IO.Directory.CreateDirectory("SavedNetworks");
                var binarySerializer =
                    new BinarySerializer($"SavedNetworks/nets_{size}_{i}_{DateTime.Now:yyyyMMddHHmmssfff}.dat");
                binarySerializer.Serialize(readOnlyList);
            }

            ushort k = 0;
            ushort pause = 0;
            ushort copySteps = 0;
            IReadOnlyList<IComparatorNetwork> comparatorNets = null;

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
                        k = Convert.ToUInt16(arg.Substring(3));
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
                        copySteps = Convert.ToUInt16(arg.Substring(3));
                        break;
                    default:
                        Trace.WriteLine("Usage:");
                        Trace.WriteLine("-s input size");
                        Trace.WriteLine("-k comparators number");
                        Trace.WriteLine("-l log file path");
                        Trace.WriteLine("-p pause step");
                        Trace.WriteLine("-r resume from binary file");
                        Trace.WriteLine("-c create copy file each c steps");
                        break;
                }
            }

            var comparatorsGenerator = new ComparatorsGenerator();
            var sortingNetworksGenerator = new Generator();
            var pruner = new ParallelPruner();
            var comparators = comparatorsGenerator.GenerateComparators(Enumerable.Range(0, IComparatorNetwork.Inputs).ToArray());
            var stopWatch = Stopwatch.StartNew();

            comparatorNets ??= new List<IComparatorNetwork> { new ComparatorNetwork(new Comparator[0]) } ;
            var numComparators = comparatorNets[0].Comparators.Length;
            var copy = copySteps;

            for (var i = numComparators; i < k; i++)
            {
                if (i == copy && copy != 0)
                {
                    SaveNetworks(k, i, comparatorNets);
                    copy += copySteps;
                }

                Trace.WriteLine($"Adding Comparator {i+1}");
                Trace.WriteLine($"Generate--------------");
                var generateWatch = Stopwatch.StartNew();
                comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators);
                Trace.WriteLine($"Length after Generate: {comparatorNets.Count}");
                Trace.WriteLine($"Generate time  {generateWatch.Elapsed}");

                Trace.WriteLine($"Prune--------------");
                var pruneWatch = Stopwatch.StartNew();
                comparatorNets = pruner.Prune(comparatorNets, 12);
                Trace.WriteLine($"Length after Prune: {comparatorNets.Count}");
                Trace.WriteLine($"Prune time  {pruneWatch.Elapsed}");
                Trace.WriteLine(string.Empty);

                if (pause != 0 && i+1 == pause)
                {
                    SaveNetworks(k, i+1, comparatorNets);
                    Trace.WriteLine($"Stopped execution at step: {i+1}");
                    break;
                }
            }

            // Trace.WriteLine($"Subsume no check: {IComparatorNetwork.SubsumeNoCheck} ");
            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");
            Trace.WriteLine(string.Empty);

            PrintSortingNetworks(comparatorNets, IComparatorNetwork.Inputs, k);
        }

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

        private static void PrintSortingNetworks(IReadOnlyList<IComparatorNetwork> nets, int inputs, int k) 
        {
            var sortingNets = nets.Where(x => x.IsSortingNetwork()).ToList();
            Trace.WriteLine($"{sortingNets.Count} Sorting Networks found with {inputs} inputs and {k} comparators");
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
