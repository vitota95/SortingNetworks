using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
            throw new NotImplementedException();
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
            for (var i = 0; i < this.Comparators.Length; i++) 
            {
                var b1 = arr.Get(this.Comparators[i].x);
                var b2 = arr.Get(this.Comparators[i].y);

                if (!b1 && b2) 
                {
                    var temp = b1;
                    arr.Set(this.Comparators[i].x, b2);
                    arr.Set(this.Comparators[i].y, temp);
                }
            }

            var newValue = new int[1];
            arr.CopyTo(newValue, 0);

            return (short)newValue[0];
        }     
    }
}
