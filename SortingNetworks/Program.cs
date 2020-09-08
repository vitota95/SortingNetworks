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
            short size = 4;
            var k = 5;
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

            var comparatorNets = CreateFirstLevelComparatorNetworks(size, comparators.ToArray());

            for (var i = 0; i < k - 1; i++)
            {
                comparatorNets = Generate(comparatorNets, comparators);
                //comparatorNets = (List<List<Tuple<int, int>>>)Prune(comparatorNets);
            }

            PrintComparatorNetworks(comparatorNets);
        }

        private static IComparatorNetwork[] Generate(IComparatorNetwork[] nets, IList<Comparator> comparators)
        {
            var newSet = new IComparatorNetwork[nets.Length*comparators.Count];
            var index = 0;
            for (var i = 0; i < nets.Length; i++)
            {
                for (var j = 0; j < comparators.Count; j++)
                {
                    newSet[index] = nets[i].CloneWithNewComparator(comparators[j]);
                    index++;
                }
            }

            return newSet;
        }

        private static IComparatorNetwork[] Prune(IComparatorNetwork[] nets)
        {
            return nets;
        }

        private static IComparatorNetwork[] CreateFirstLevelComparatorNetworks(short size, Comparator[] comparators) 
        {
            var comparatorNets = new ComparatorNetwork[comparators.Length];
            for (var i = 0; i< comparators.Length; i++)
            {
                comparatorNets[i] = new ComparatorNetwork(size, new Comparator[] { new Comparator(comparators[i].x, comparators[i].y) });
            }

            return comparatorNets;
        }

        private static void PrintComparatorNetworks(IComparatorNetwork[] nets) 
        {
            foreach (var n in nets) 
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
            Trace.WriteLine($"Is Sorting Network {net.IsSortingNetwork()}");
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
