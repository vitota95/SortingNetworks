namespace SortingNetworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IComparatorNetwork"/>
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(short inputs, Comparator[] comparators) 
        {
            this.Comparators = comparators;
            this.Inputs = inputs;
            this.Outputs = CalculateOutput();       
        }

        /// <inheritdoc/>
        public HashSet<short> Outputs { get; private set; }

        /// <inheritdoc/>
        public short Inputs { get; private set; }

        /// <inheritdoc/>
        public Comparator[] Comparators { get;  set; }

        /// <inheritdoc/>
        public bool IsSortingNetwork()
        {
            return this.Outputs.Count == this.Inputs + 1;
        }

        /// <inheritdoc/>
        public bool IsRedundant(IComparatorNetwork n)
        {
            return this.Outputs.SetEquals(n.Outputs);
        }

        /// <inheritdoc/>
        public IComparatorNetwork CloneWithNewComparator(Comparator comparator)
        {
            var comparatorsSize = this.Comparators.Length + 1;
            var newComparators = new Comparator[comparatorsSize];
            Array.Copy(this.Comparators, newComparators, this.Comparators.Length);
            newComparators[comparatorsSize - 1] = comparator;

            return new ComparatorNetwork(this.Inputs, newComparators);
        }

        /// <inheritdoc/>
        public bool IsSubsumed(IComparatorNetwork n)
        {
            var isSubsumed = false;
            if (!ShouldCheckSubsumption(n, this)) return false;

            var permutations = Enumerable.Range(0, this.Inputs).GetPermutations().ToArray();
            for (var i = 0; i < permutations.Length; i++)
            {
                var p = permutations[i];
                var clone = CreateCloneWithPermutation(
                    (ComparatorNetwork)n,
                    (short)p.ElementAt(0),
                    (short)p.ElementAt(1));

                if (OutputIsSubset(clone, this))
                {
                    isSubsumed = true;
                    break;
                }
            }

            return isSubsumed;
        }

        private static bool ShouldCheckSubsumption(IComparatorNetwork n1, IComparatorNetwork n2 )
        {
            var d1 = new Dictionary<uint, int>();
            var d2 = new Dictionary<uint, int>();

            using (var e1 = n1.Outputs.GetEnumerator())
            using (var e2 = n2.Outputs.GetEnumerator())
            {
                for (var i = 0; i < n1.Outputs.Count; i++)
                {
                    var setBits1 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)e1.Current);
                    var setBits2 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)e2.Current);
                
                    IncrementInDictionary(setBits1, ref d1);
                    IncrementInDictionary(setBits2, ref d2);

                    e1.MoveNext();
                    e2.MoveNext();
                }
            }

            foreach (var x in d1)
            {
                if (d2.TryGetValue(x.Key, out var y))
                {
                    if (x.Value > y) return false;
                }
            }

            return true;
        }

        private static void IncrementInDictionary(uint key, ref Dictionary<uint,int> dict)
        {
            if (key == 0)
            {
                return;
            }

            if (dict.ContainsKey(key))
            {
                dict[key]++;
            }
            else
            {
                dict.Add(key, 1);
            }
        }

        private static bool OutputIsSubset(IComparatorNetwork c1, IComparatorNetwork c2) 
        {
            return c1.Outputs.IsSubsetOf(c2.Outputs);
        }
           
        private static IComparatorNetwork CreateCloneWithPermutation(ComparatorNetwork n, short x1, short x2) 
        {
            var comparators = new Comparator[n.Comparators.Length];

            for (var i = 0; i < n.Comparators.Length; i++) 
            {
                var x = n.Comparators[i].X;
                var y = n.Comparators[i].Y;
                var comparator = new Comparator(x, y);

                if (x == x1) 
                {
                    comparator.X = x2;
                }
                else if (x == x2) 
                {
                    comparator.X = x1;
                }

                if (y == x1)
                {
                    comparator.Y = x2;
                }
                else if (y == x2)
                {
                    comparator.Y = x1;
                }

                comparators[i] = comparator;
            }

            return new ComparatorNetwork(n.Inputs, comparators);
        }

        private HashSet<short> CalculateOutput() 
        {
            var total = Math.Pow(2, this.Inputs);
            var output = new HashSet<short>();
            
            for (short i = 0; i < total; i++) 
            {
                output.Add(ComputeOutput(i));
            }

            return output;
        }

        private short ComputeOutput(short value) 
        {
            var arr = new BitArray(new int[] { value }) { Length = this.Inputs };
            var length = arr.Length - 1;

            for (var i = 0; i < this.Comparators.Length; i++) 
            {
                var b1 = arr.Get(length - this.Comparators[i].X);
                var b2 = arr.Get(length - this.Comparators[i].Y);

                if (!b1 && b2) 
                {
                    var temp = b1;
                    arr.Set(length - this.Comparators[i].X, b2);
                    arr.Set(length - this.Comparators[i].Y, temp);
                }
            }

            var newValue = new int[1];
            arr.CopyTo(newValue, 0);

            return (short)newValue[0];
        }
    }
}
