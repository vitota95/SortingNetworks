namespace SortingNetworks
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            var inputs = Convert.ToUInt16(args[0]);
            var k = Convert.ToInt16(args[1]);
            var traceFile = args[2];

            var comparatorsGenerator = new ComparatorsGenerator();
            var sortingNetworksGenerator = new ComparatorNetworksGenerator();
            var pruner = new Pruner();
            var comparators = comparatorsGenerator.GenerateComparators(Enumerable.Range(0, inputs).ToArray());

            if (args.Length < 4)
            {
                InitiateTracer(CreateDefaultListeners(traceFile));
            }

            var stopWatch = Stopwatch.StartNew();
            var comparatorNets = new IComparatorNetwork[] { new ComparatorNetwork(inputs, new Comparator[0]) };

            for (var i = 0; i < k; i++)
            {
                Trace.WriteLine($"Adding Comparator {i+1}");
                Trace.WriteLine($"Generate--------------");
                var generateWatch = Stopwatch.StartNew();
                comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators);
                Trace.WriteLine($"Length after Generate: {comparatorNets.Length}");
                Trace.WriteLine($"Generate time  {generateWatch.Elapsed}");

                Trace.WriteLine($"Prune--------------");
                var pruneWatch = Stopwatch.StartNew();
                comparatorNets = pruner.Prune(comparatorNets);
                Trace.WriteLine($"Length after Prune: {comparatorNets.Length}");
                Trace.WriteLine($"Prune time  {pruneWatch.Elapsed}");
                Trace.WriteLine(string.Empty);
            }

            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");
            Trace.WriteLine(string.Empty);

            PrintSortingNetworks(comparatorNets, inputs, k);
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

        private static void PrintSortingNetworks(IComparatorNetwork[] nets, int inputs, int k) 
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
