using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SortingNetworks
{
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(short size) 
        {
            this.Output = CalculateOutput();
        }

        public HashSet<BitArray> Output { get; private set; }

        public short Size { get; private set; }

        public Tuple<short, short>[] Comparators { get;  set; }

        public bool IsEquivalent(IComparatorNetwork n)
        {
            return OutputsAreEqual(this.Output, n.Output);
        }

        private static bool OutputsAreEqual(HashSet<BitArray> ba1, HashSet<BitArray> ba2)
        {
            return ba1.Equals(ba2);
        }

        private HashSet<BitArray> CalculateOutput() 
        {
            var total = Math.Pow(2, this.Size);
            var output = new HashSet<BitArray>();
            
            for (var i = 0; i < total; i++) 
            {
                output.Add(ComputeOutput(i));
            }

            return output;
        }

        private BitArray ComputeOutput(int value) 
        {
            var arr = new BitArray(new int[] { value });
            // TODO: compute the output based on comparators
            return arr;
        }
    }
}
