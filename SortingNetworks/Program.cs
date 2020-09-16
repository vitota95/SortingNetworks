namespace SortingNetworks
{
    using System.Diagnostics;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            short inputs = 7;
            var k = 16;

            var comparatorsGenerator = new ComparatorsGenerator();
            var sortingNetworksGenerator = new ComparatorNetworksGenerator();
            var pruner = new Pruner();
            var comparators = comparatorsGenerator.GenerateComparators(Enumerable.Range(0, inputs).ToArray());

            InitiateTracer();

            var stopWatch = Stopwatch.StartNew();
            var comparatorNets = new IComparatorNetwork[] { new ComparatorNetwork(inputs, new Comparator[0]) };

            for (var i = 0; i < k; i++)
            {
                Trace.WriteLine($"Adding Comparator {i+1}");

                Trace.WriteLine($"Generate--------------");
                comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators);
                Trace.WriteLine($"Length after Generate: {comparatorNets.Length} ");

                Trace.WriteLine($"Prune--------------");
                comparatorNets = pruner.Prune(comparatorNets);
                Trace.WriteLine($"Length after Prune: {comparatorNets.Length} ");
                Trace.WriteLine("");
            }

            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");
            Trace.WriteLine("");

            PrintSortingNetworks(comparatorNets, inputs, k);
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
            Trace.WriteLine("");
        }

        private static void InitiateTracer()
        {
            Trace.Listeners.Clear();
            var twtl = new TextWriterTraceListener("log.txt")
            {
                Name = "TextLogger",
                TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime
            };
            var ctl = new ConsoleTraceListener(false) { TraceOutputOptions = TraceOptions.DateTime };
            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;
        }
    }
}
