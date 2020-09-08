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
            short size = 5;
            var k = 9;
            var range = Enumerable.Range(0, size).ToList();
            var combinationsGenerator = new CombinationsGenerator();
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
            Trace.WriteLine($"Generate first level--------------");
            var comparatorNets = CreateFirstLevelComparatorNetworks(size, comparators.ToArray());
            Trace.WriteLine($"Length after Generate: {comparatorNets.Length} ");
            //comparatorNets = Prune(comparatorNets);
            //Trace.WriteLine($"Length after Prune: {comparatorNets.Length} ");

            for (var i = 0; i < k - 1; i++)
            {
                Trace.WriteLine($"Generate--------------");
                comparatorNets = Generate(comparatorNets, comparators);
                Trace.WriteLine($"Length after Generate: {comparatorNets.Length} ");
                //Trace.WriteLine($"Prune--------------");
                //comparatorNets = Prune(comparatorNets);
                //Trace.WriteLine($"Length after Prune: {comparatorNets.Length} ");
                Trace.WriteLine("");
            }

            Trace.WriteLine($"Elapsed Time: {stopWatch.Elapsed} ");

            PrintSortingNetworks(comparatorNets, size, k);
        }

        private static IComparatorNetwork[] Generate(IComparatorNetwork[] nets, IList<Comparator> comparators)
        {
            var newSet = new IComparatorNetwork[nets.Length * comparators.Count];
            var index = 0;
            for (var i = 0; i < nets.Length; i++)
            {
                var net = nets[i];
                for (var j = 0; j < comparators.Count; j++)
                {
                    var newNet = net.CloneWithNewComparator(comparators[j]);
                    newSet[index] = newNet;
                    index++;
                }
            }

            return RemoveRedundantNetworks(newSet.ToArray());
        }

        private static IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            throw new NotImplementedException();
        }

        private static IComparatorNetwork[] CreateFirstLevelComparatorNetworks(short size, Comparator[] comparators) 
        {
            var comparatorNets = new ComparatorNetwork[comparators.Length];
            for (var i = 0; i< comparators.Length; i++)
            {
                comparatorNets[i] = new ComparatorNetwork(size, new Comparator[] { new Comparator(comparators[i].x, comparators[i].y) });
            }

            return RemoveRedundantNetworks(comparatorNets);
        }

        private static IComparatorNetwork[] RemoveRedundantNetworks(IComparatorNetwork[] nets) 
        {
            for (var i = 0; i < nets.Length; i++) 
            {
                if (nets[i].IsMarked) continue;
                for (var j = i + 1; j < nets.Length - 1; j++)
                {
                    if (nets[j].IsMarked) continue;
                    nets[j].MarkIfRedundant(nets[i]);
                }
            }

            return nets.Where(x => !x.IsMarked).ToArray();
        }

        private static void PrintSortingNetworks(IComparatorNetwork[] nets, int size, int k) 
        {
            var sortingNets = nets.Where(x => x.IsSortingNetwork()).ToList();
            Trace.WriteLine($"{sortingNets.Count} Sorting Networks found with size {size} and {k} comparators");
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
            File.Delete("log.txt");
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
