using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SortingNetworks
{
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(short inputs, Comparator[] comparators) 
        {
            this.Comparators = comparators;
            this.Inputs = inputs;
            this.Outputs = CalculateOutput();       
        }

        public HashSet<short> Outputs { get; private set; }

        public short Inputs { get; private set; }

        public Comparator[] Comparators { get;  set; }

        public bool IsMarked { get; private set; }      

        public bool IsSortingNetwork()
        {
            return this.Outputs.Count == this.Inputs + 1;
        }

        public bool IsRedundant(IComparatorNetwork n)
        {
            this.IsMarked = this.Outputs.SetEquals(n.Outputs);
            return this.IsMarked;
        }

        public IComparatorNetwork CloneWithNewComparator(Comparator comparator)
        {
            var comparatorsSize = this.Comparators.Length + 1;
            var newComparators = new Comparator[comparatorsSize];
            Array.Copy(this.Comparators, newComparators, this.Comparators.Length);
            newComparators[comparatorsSize-1] = comparator;

            return new ComparatorNetwork(this.Inputs, newComparators);
        }

        public void MarkIfSubsumed(IComparatorNetwork n)
        {
            if(!ShouldCheckSubsumption(n, this)) return;
           
            foreach (var p in Enumerable.Range(0, this.Inputs).GetPermutations())
            {
                var clone = CreateCloneWithPermutation((ComparatorNetwork) n, (short)p.ElementAt(0), (short)p.ElementAt(1));
                if(OutputIsSubset(clone, this))
                {
                    this.IsMarked = true;
                    break;
                }
            }
        }

        private static bool ShouldCheckSubsumption(IComparatorNetwork n1, IComparatorNetwork n2 )
        {
            var d1 = new Dictionary<uint, int>();
            var d2 = new Dictionary<uint, int>();
            var e1 = n1.Outputs.GetEnumerator();
            var e2 = n2.Outputs.GetEnumerator();

         
            for (var i=0; i<n1.Outputs.Count; i++)
            {
                var setBits1 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)e1.Current);
                var setBits2 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)e2.Current);
                
                IncrementInDictionary(setBits1, ref d1);
                IncrementInDictionary(setBits2, ref d2);

                e1.MoveNext();
                e2.MoveNext();
            }

            foreach (var x in d1)
            {
                if (d2.TryGetValue(x.Key, out int y))
                {
                    if (x.Value > y) return false;
                }
            }

            return true;
        }

        private static void IncrementInDictionary(uint key, ref Dictionary<uint,int> dict)
        {
            if (key == 0) return;

            if (dict.ContainsKey(key))
            {
                dict[key]++;
            }
            else
            {
                dict.Add(key, 1);
            }
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
            var arr = new BitArray(new int[] { value });
            arr.Length = this.Inputs;
            var length = arr.Length - 1;

            for (var i = 0; i < this.Comparators.Length; i++) 
            {
                var b1 = arr.Get(length - this.Comparators[i].x);
                var b2 = arr.Get(length - this.Comparators[i].y);

                if (!b1 && b2) 
                {
                    var temp = b1;
                    arr.Set(length - this.Comparators[i].x, b2);
                    arr.Set(length - this.Comparators[i].y, temp);
                }
            }

            var newValue = new int[1];
            arr.CopyTo(newValue, 0);

            return (short)newValue[0];
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
                var x = n.Comparators[i].x;
                var y = n.Comparators[i].y;
                var comparator = new Comparator(x, y);

                if (x == x1) 
                {
                    comparator.x = x2;
                }
                else if(x == x2) 
                {
                    comparator.x = x1;
                }

                if (y == x1)
                {
                    comparator.y = x2;
                }
                else if (y == x2)
                {
                    comparator.y = x1;
                }

                comparators[i] = comparator;
            }

            return new ComparatorNetwork(n.Inputs, comparators);
        }
    }
}
