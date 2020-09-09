using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SortingNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            short inputs = 5;
            var k = 9;
            var range = Enumerable.Range(0, inputs).ToList();

            var combinationsGenerator = new CombinationsGenerator();
            var sortingNetworksGenerator = new SortingNetworksGenerator();
            var combinations = combinationsGenerator.GenerateCombinations(range, 2);
            var comparators = new List<Comparator>();

            InitiateTracer();

            foreach (var comp in combinations)
            {
                if (comp[0] != comp[1])
                {
                    comparators.Add(new Comparator((short)comp[0], (short)comp[1]));
                }
            }

            var stopWatch = Stopwatch.StartNew();
            var comparatorNets = new IComparatorNetwork[] { new ComparatorNetwork(inputs, new Comparator[0]) };

            for (var i = 0; i < k; i++)
            {
                Trace.WriteLine($"Generate--------------");
                comparatorNets = sortingNetworksGenerator.Generate(comparatorNets, comparators);
                Trace.WriteLine($"Length after Generate: {comparatorNets.Length} ");
                //Trace.WriteLine($"Prune--------------");
                //comparatorNets = Prune(comparatorNets);
                //Trace.WriteLine($"Length after Prune: {comparatorNets.Length} ");
                Trace.WriteLine("");
            }

            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");

            PrintSortingNetworks(comparatorNets, inputs, k);
        }
    
        private static IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            throw new NotImplementedException();
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
                Trace.Write($"({c.x},{c.y}) ");
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
