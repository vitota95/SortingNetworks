#define DUAL

namespace SortingNetworks
{
    using System;
    using System.Linq;
    using static System.Numerics.BitOperations;

    /// <inheritdoc cref="IComparatorNetwork"/>
    [Serializable]
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(Comparator[] comparators) 
        {
            this.Where0 = new int[IComparatorNetwork.Inputs];
            for (var i = 0; i < IComparatorNetwork.Inputs; i++)
            {
                this.Where0[i] = -1;
            }
            this.Where1 = new int[IComparatorNetwork.Inputs];
            this.Where0SetCount = new int[IComparatorNetwork.Inputs];
            this.SequencesWithOnes = new int[IComparatorNetwork.Inputs];

            this.Comparators = comparators;
            this.Outputs = this.CalculateOutput();
#if DUAL
            this.Where0Dual = new int[IComparatorNetwork.Inputs];
            this.Where1Dual = new int[IComparatorNetwork.Inputs];
            this.OutputsDual = this.DualizeOutputs(this.Outputs);
            this.Where0Dual = this.DualizeWhere(this.Where0);
            this.Where1Dual = this.DualizeWhere(this.Where1);
#endif
        }

        public int[] Outputs { get; private set; }
#if DUAL
        public int[] OutputsDual { get; }

        public int[] Where0Dual { get; private set; }

        public int[] Where1Dual { get; }
#endif
        public int OutputsPopCount { get; private set; }

        public int[] Where0 { get; private set; }

        public int[] Where0SetCount { get; private set; }

        public int[] Where1 { get; private set; }


        public int[] SequencesWithOnes { get; private set; }

        /// <inheritdoc/>
        public Comparator[] Comparators { get;  set; }

        /// <inheritdoc/>
        public bool IsSortingNetwork()
        {
            return this.OutputsPopCount == IComparatorNetwork.Inputs - 1;
        }

        /// <inheritdoc/>
        public bool IsRedundant(IComparatorNetwork n)
        {
            return this.Outputs.SequenceEqual(n.Outputs);
        }

        /// <inheritdoc/>
        public IComparatorNetwork CloneWithNewComparator(Comparator comparator)
        {
            var comparatorsSize = this.Comparators.Length + 1;
            var newComparators = new Comparator[comparatorsSize];
            Array.Copy(this.Comparators, newComparators, this.Comparators.Length);
            newComparators[comparatorsSize - 1] = comparator;

            return new ComparatorNetwork(newComparators);
        }

        /// <inheritdoc/>
        public bool IsSubsumed(IComparatorNetwork n)
        {
#if DEBUG
            IComparatorNetwork.SubsumeTotal++;
#endif
            if (!ShouldCheckSubsumption(n, this))
            {
#if DEBUG
                IComparatorNetwork.SubsumeNoCheckTotal++;
#endif
                return false;
            }

#if DEBUG
            IComparatorNetwork.SubsumeNumber++;
#endif

            // Create matrix for permutations
            var positions = GetPositions(this.Where0,  this.Where1, n.Where0, n.Where1);

            //if (positions == null)
            //{
            //    return false;
            //}
#if DUAL
            var positionsDual = GetPositions(this.Where0, this.Where1, n.Where0Dual, n.Where1Dual);

            var succeed = false;
            if (positions != null)
            {
                succeed = TryPermutations(positions, new int[IComparatorNetwork.Inputs], this.Outputs, n.Outputs, 0);
            }

            if (succeed)
            {
                return true;
            }

            if (positionsDual != null)
            {
                succeed = TryPermutations(positionsDual, new int[IComparatorNetwork.Inputs], n.OutputsDual, this.Outputs, 0);
            }
#endif

#if DEBUG
            if (succeed)
            {
                IComparatorNetwork.SubsumeSucceed++;
            }
#endif

            return succeed;
        }

        /// <summary>
        /// Counts the number of bits set for each output of both input comparator networks
        /// if the number of set bits is different the n1 cannot be subsumed by n2 and vice versa.
        /// </summary>
        /// <param name="n1">The comparator network 1.</param>
        /// <param name="n2">The comparator network 2.</param>
        /// <returns>True if subsume test should be done, False otherwise.</returns>
        private static bool ShouldCheckSubsumption(IComparatorNetwork n1, IComparatorNetwork n2)
        {
#if DEBUG
            if (n1.OutputsPopCount > n2.OutputsPopCount)
            {
                IComparatorNetwork.OutputCountBigger++;
                return false;
            }

            if (!SequencesAreCompatible(n1.SequencesWithOnes, n2.SequencesWithOnes))
            {
                IComparatorNetwork.SubsumeNoCheck1++;
                return false;
            }

            if (!SequencesAreCompatible(n1.Where0SetCount, n2.Where0SetCount))
            {
                IComparatorNetwork.SubsumeNoCheck2++;
                return false;
            }

            return true;

#endif
            if (n1.OutputsPopCount > n2.OutputsPopCount)
            {
                return false;
            }

            return SequencesAreCompatible(n1.SequencesWithOnes, n2.SequencesWithOnes) && 
                   SequencesAreCompatible(n1.Where0SetCount, n2.Where0SetCount);
        }

        private static bool SequencesAreCompatible(int[] a1, int[] a2)
        {
            for (var i = 0; i < a1.Length; i++)
            {
                if (a1[i] > a2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryPermutations(int[] positions, int[] permutation, int[] o1, int[] o2, int index)
        {
            if (index == IComparatorNetwork.Inputs)
            {
                return false;
            }

            for (var j = 0; j < IComparatorNetwork.Inputs; j++)
            {
                if ((positions[j] & (1 << index)) == 0) continue;
                if (IsAlreadyAdded(permutation, j, index - 1)) continue;
                permutation[index] = j;
                var result = TryPermutations(positions, permutation, o1, o2, index + 1);
                if (result)
                {
                    return true;
                }
                // As permutation is passed by reference, when it returns from recursion we need to reset the values after index.
                // This is quicker and consumes less memory than cloning the array.
                permutation = ResetPositions(index + 1, permutation);
            }

            return index == IComparatorNetwork.Inputs - 1 && OutputIsSubset(permutation, o1, o2);
        }

        private static int[] ResetPositions(int start, int[] arr)
        {
            for (var i = start; i < arr.Length; i++)
            {
                arr[i] = -1;
            }

            return arr;
        }

        private static bool IsAlreadyAdded(int[] a, int value, int limit)
        {
            for (var i = 0; i <= limit; i++)
            {
                if (a[i] == value)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool OutputIsSubset(int[] permutation, int[] o1, int[] o2)
        {
            for (var output = 1; output < (1 << IComparatorNetwork.Inputs); output++)
            {
                if (!o2.GetBitValue(output)) continue;

                var newOutput = 0;

                // permute bits
                for (var j = 0; j < permutation.Length; j++)
                {
                    if ((output & (1 << permutation[j])) != 0) newOutput |= 1 << j;
                }

                if (!o1.GetBitValue(newOutput))
                {
                    return false;
                }
            }

            return true;
        }

        private static int[] GetPositions(int[] n1Where0, int[] n1Where1, int[] n2Where0, int[] n2Where1)
        {
            var positions = new int[IComparatorNetwork.Inputs];
            var prod = 1;
            positions.Populate(-1);
            for (var pos = 0; pos < IComparatorNetwork.Inputs; pos++)
            {
                for (var j = 1; j < IComparatorNetwork.Inputs; j++)
                {
                    var x = n2Where1[j] & (1 << pos);
                    if (x != 0)
                    {
                        positions[pos] &= n1Where1[j];
                    }

                    var y = n2Where0[j] & (1 << pos);
                    if (y != 0)
                    {
                        positions[pos] &= n1Where0[j];
                    }
                }

                prod *= PopCount((uint)(positions[pos] & ((1 << IComparatorNetwork.Inputs) - 1)));
            }

            if (prod == 0)
            {
                return null;
            }

            return positions;
        }

        private int[] CalculateOutput() 
        {
            var total = (1 << IComparatorNetwork.Inputs) - 1;
            var outputs = new int[IComparatorNetwork.OutputSize];

            for (ushort i = 1; i < total; i++)
            {
                var position = this.ComputeOutput(i, out var setBits);

                if (!outputs.GetBitValue(position))
                {
                    this.SequencesWithOnes[setBits]++;
                }

                outputs.SetBit(position);
            }

            for (var i = 0; i < outputs.Length; i++)
            {
                this.OutputsPopCount += PopCount((uint) outputs[i]);
            }

            // complement where 0
            for (var i = 0; i < this.Where0.Length; i++)
            {
                this.Where0[i] = ~this.Where0[i] & ((1 << IComparatorNetwork.Inputs) - 1);
                if (this.Where0[i] != -1)
                {
                    this.Where0SetCount[i] = PopCount((uint)this.Where0[i]);
                }
            }

            return outputs;
        }

        private int[] DualizeOutputs(int[] outputs)
        {
            var total = (1 << IComparatorNetwork.Inputs) - 1;
            var dual = new int[IComparatorNetwork.OutputSize];

            for (var i = 0; i < total; i++)
            {
                if(!outputs.GetBitValue(i))
                {
                    dual.SetBit(i);
                }
            }

            return dual;
        } 
        
        private int[] DualizeWhere(int[] where)
        {
            var dual = new int[where.Length];

            for (var i = 1; i < IComparatorNetwork.Inputs; i++)
            {
                if (!where.GetBitValue(i))
                {
                    dual.SetBit(i);
                }
            }

            //for (var i = 0; i < where.Length; i++)
            //{
            //    dual[i] = ~where[i];
            //}

            return dual;
        }

        private ushort ComputeOutput(ushort value, out ushort setBits) 
        {
            for (var i = 0; i < this.Comparators.Length; i++)
            {
                var pos1 = IComparatorNetwork.Inputs - this.Comparators[i].X - 1;
                var pos2 = IComparatorNetwork.Inputs - this.Comparators[i].Y - 1;
                var bit1 = (value >> pos1) & 1;
                var bit2 = (value >> pos2) & 1;

                if ((~bit1 & bit2) != 0)
                {
                    value = (ushort)(value & ~(1 << pos2));
                    value = (ushort)(value | (1 << pos1));
                }
            }

            setBits = (ushort)PopCount(value);
            this.Where1[setBits] |= value;
            this.Where0[setBits] &= value;

            return value;
        }
    }
}
