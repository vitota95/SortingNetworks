using System;
using System.Collections.Generic;
using System.Linq;

namespace SortingNetworks
{
    class Program
    {
        static void Main(string[] args)
        {
            var n = 4;
            var k = 2;
            var range = Enumerable.Range(0, n).ToList();
            var combinations = GenerateCombinations(range, 2);
            var tupleCombinations = new List<Tuple<int, int>>();

            foreach (var comp in combinations)
            {
                if (comp[0] != comp[1])
                {
                    tupleCombinations.Add(Tuple.Create(comp[0], comp[1]));
                }
            }

            var comparatorNets = new List<IEnumerable<Tuple<int, int>>>();

            foreach (var t in tupleCombinations) 
            {
                comparatorNets.Add(new List<Tuple<int, int>>() { t });
            }

            for (var i=0; i < k - 1; i++)
            {
                comparatorNets = (List<IEnumerable<Tuple<int, int>>>)Generate(comparatorNets, tupleCombinations);
                //comparatorNets = (List<List<Tuple<int, int>>>)Prune(comparatorNets);

            }

            PrintAllNets(comparatorNets);
            Console.ReadKey();
            //GenerateComparatorNets(4, 8);
        }

        private static IEnumerable<IEnumerable<Tuple<int, int>>> Generate(IEnumerable<IEnumerable<Tuple<int, int>>> set, IEnumerable<Tuple<int,int>> combinations)
        {
            var newSet = new List<IEnumerable<Tuple<int, int>>>();

            foreach (var x in set) 
            {
                foreach (var c in combinations) 
                {
                    var x2 = new List<Tuple<int, int>>(x);
                    x2.Add(c);
                    newSet.Add(x2);
                }
            }

            return newSet;
        }

        private static IEnumerable<IEnumerable<Tuple<int, int>>> Prune(IEnumerable<IEnumerable<Tuple<int, int>>> set)
        {

            return set;
        }

        private static void PrintAllNets(IEnumerable<IEnumerable<Tuple<int, int>>> nets) 
        {
            foreach (var n in nets) 
            {
                PrintComparatorNet(n);
            }
        }

        private static void PrintComparatorNet(IEnumerable<Tuple<int,int>> net) 
        {
            foreach (var p in net)
            {
                Console.Write($"({p.Item1},{p.Item2})");
            }
            Console.WriteLine();
        }

        //private static void GenerateComparatorNets(int s, int c)
        //{
        //    var comparatorsMatrix = GenerateCombinations(Enumerable.Range(0,s).ToList(), 2);
        //    var comparators = new List<Tuple<int, int>>();

        //    foreach (var comp in comparatorsMatrix) {
        //        if (comp[0] != comp[1]) {
        //            comparators.Add(Tuple.Create(comp[0], comp[1]));
        //        }
        //    }

        //    var comparatorsCombi = GenerateCombinations(comparators, c);

        //    foreach (var perm in comparatorsCombi)
        //    {
        //        foreach (var p in perm)
        //        {
        //            Console.Write($"({p.Item1},{p.Item2})");
        //        }
        //        Console.WriteLine();
        //    }
        //    Console.ReadKey();
        //}

        private static List<List<T>> GenerateCombinations<T>(List<T> combinationList, int k)
        {
            var combinations = new List<List<T>>();

            if (k == 0)
            {
                var emptyCombination = new List<T>();
                combinations.Add(emptyCombination);

                return combinations;
            }

            if (combinationList.Count == 0)
            {
                return combinations;
            }

            T head = combinationList[0];
            var copiedCombinationList = new List<T>(combinationList);

            List<List<T>> subcombinations = GenerateCombinations(copiedCombinationList, k - 1);

            foreach (var subcombination in subcombinations)
            {
                subcombination.Insert(0, head);
                combinations.Add(subcombination);
            }

            combinationList.RemoveAt(0);
            combinations.AddRange(GenerateCombinations(combinationList, k));

            return combinations;
        }
    }
}
