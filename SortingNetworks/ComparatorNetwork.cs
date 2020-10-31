namespace SortingNetworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        }

        /// <inheritdoc/>
        public HashSet<ushort> Outputs { get; private set; }

        public int[] Where0 { get; private set; }

        public int[] Where0SetCount { get; }

        public int[] Where1 { get; private set; }

        public int[] SequencesWithOnes { get; private set; }

        /// <inheritdoc/>
        public Comparator[] Comparators { get;  set; }

        /// <inheritdoc/>
        public bool IsSortingNetwork()
        {
            return this.Outputs.Count == IComparatorNetwork.Inputs - 1;
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

            return new ComparatorNetwork(newComparators);
        }

        /// <inheritdoc/>
        public bool IsSubsumed(IComparatorNetwork n, IEnumerable<int>[] permutations)
        {
            if (!ShouldCheckSubsumption(n, this))
            {
                //IComparatorNetwork.SubsumeNoCheck++;
                return false;
            }

            // Create matrix for permutations
            var positions = this.GetPositions(n);
            var values = new int[IComparatorNetwork.Inputs];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = i;
            }

            if (positions == null)
            {
                return false;
            }

            return this.ApplyPermutations(n, permutations, positions);
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
            return SequencesAreCompatible(n1.SequencesWithOnes, n2.SequencesWithOnes) &&
            SequencesAreCompatible(n1.Where0SetCount, n2.Where0SetCount);
        }

        private static bool SequencesAreCompatible(int[] a1, int[] a2)
        {
            for (var i = 0; i < a1.Length; i++)
            {
                if (a1[i] != 0 && a1[i] > a2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool ApplyPermutations(IComparatorNetwork n, IEnumerable<int>[] permutations, int[] positions)
        {
            for (var i = 0; i < permutations.Length; i++)
            {
                var permutation = permutations[i].ToArray();
                var isValidPermutation = true;
                for (var j = 0; j < permutation.Length; j++)
                {
                    if ((positions[permutation[j]] & (1 << j)) == 0)
                    {
                        isValidPermutation = false;
                        break;
                    }
                }

                if (isValidPermutation)
                {
                    var isSubset = true;

                    using (var enumerator = n.Outputs.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var output = enumerator.Current;
                            var newOutput = 0;

                            // permute bits
                            for (var j = 0; j < permutation.Length; j++)
                            {
                                if ((output & (1 << permutation[j])) > 0) newOutput |= 1 << j;
                            }

                            if (!this.Outputs.Contains((ushort)newOutput))
                            {
                                isSubset = false;
                                break;
                            }
                        }
                    }

                    if (isSubset)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int[] GetPositions(IComparatorNetwork n)
        {
            var positions = new int[IComparatorNetwork.Inputs];
            var prod = 1;
            positions.Populate(-1);
            for (var pos = 0; pos < IComparatorNetwork.Inputs; pos++)
            {
                for (ushort j = 1; j < IComparatorNetwork.Inputs; j++)
                {
                    var x = n.Where1[j] & (1 << pos);
                    if (x != 0)
                    {
                        positions[pos] &= this.Where1[j];
                    }

                    var y = n.Where0[j] & (1 << pos);
                    if (y != 0)
                    {
                        positions[pos] &= this.Where0[j];
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

        private HashSet<ushort> CalculateOutput() 
        {
            var total = (1 << IComparatorNetwork.Inputs) - 1;
            var output = new HashSet<ushort>();

            for (ushort i = 1; i < total; i++)
            {
                if (output.Add(this.ComputeOutput(i, out var setBits)))
                {
                    this.SequencesWithOnes[setBits]++;
                }
            }

            // complement where 0
            for (var i = 0; i < this.Where0.Length; i++)
            {
                this.Where0[i] = ~this.Where0[i] & ((1 << IComparatorNetwork.Inputs) - 1);
                if (this.Where0[i] != -1)
                {
                    this.Where0SetCount[i] = (int)PopCount((uint)this.Where0[i]);
                }
            }

            return output;
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
