namespace SortingNetworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc cref="IComparatorNetwork"/>
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(Comparator[] comparators) 
        {
            this.Where0 = new Dictionary<ushort, int>();
            this.Where1 = new Dictionary<ushort, int>();
            this.SequencesWithOnes = new Dictionary<ushort, int>();

            this.Comparators = comparators;
            this.Outputs = this.CalculateOutput();
        }

        /// <inheritdoc/>
        public HashSet<ushort> Outputs { get; private set; }

        public Dictionary<ushort, int> Where0 { get; private set; }

        public Dictionary<ushort, int> Where1 { get; private set; }

        public Dictionary<ushort, int> SequencesWithOnes { get; private set; }

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
            if (!ShouldCheckSubsumption(n, this)) return false;

            // Create matrix for permutations
            var positions = this.GetPositions(n);
            var values = new int[IComparatorNetwork.Inputs];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = i;
            }

            if (positions == null)
            {
                return false;
            }

            //var toPermute = RestrictPermutations(values.GetPermutations().ToArray(), positions).ToArray();
            var toPermute = permutations.ToArray();
            //var toPermute = permutations;

            for (var i = 0; i < toPermute.GetLength(0); i++)
            {
                var permutation = toPermute[i].ToArray();
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

            return false;
        }

        private int[] GetPositions(IComparatorNetwork n)
        {
            var positions = new int[IComparatorNetwork.Inputs];
            ulong prod = 1;
            positions.Populate(-1);
            for (var pos = 0; pos < IComparatorNetwork.Inputs; pos++)
            {
                for (ushort j = 1; j < IComparatorNetwork.Inputs; j++)
                {
                    if (n.Where1.ContainsKey(j))
                    {
                        var x = n.Where1[j] & (1 << pos);
                        if (x != 0)
                        {
                            positions[pos] &= this.Where1[j];
                        }
                    }

                    if (n.Where0.ContainsKey(j))
                    {
                        var y = n.Where0[j] & (1 << pos);
                        if (y != 0)
                        {
                            positions[pos] &= this.Where0[j];
                        }
                    }
                }

                prod *= System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)(positions[pos] & ((1 << IComparatorNetwork.Inputs) - 1)));
            }

            if (prod == 0)
            {
                return null;
            }

            return positions;
        }

        private int[][] RestrictPermutations(IEnumerable<int>[] permutations, int[] positions)
        {
            var newPermutations = new List<int[]>();
            var bitPositions = new BitArray[positions.Length];
            for (var i = 0; i < positions.Length; i++)
            {
                bitPositions[i] = new BitArray(new int[] { positions[i] }) { Length = positions.Length };
            }

            for (var i = 1; i < permutations.Length; i++)
            {
                var permutation = permutations[i].ToArray();
                var shouldAdd = true;
                for (var j = 0; j < permutation.Length; j++)
                {
                    if (!bitPositions[permutation[j]].Get(j))
                    {
                        shouldAdd = false;
                        break;
                    }
                }

                if (shouldAdd)
                {
                    newPermutations.Add(permutation);
                }
            }

            return newPermutations.ToArray();
        }

        private static ulong CountBits(ulong l)
        {
            return (l * 0x_200040008001UL & 0x_111111111111111UL) % 0x_f;
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
                   CheckWhere0SetCount(n1.Where0, n2.Where0); // &&
        }

        private static bool SequencesAreCompatible(Dictionary<ushort, int> d1, Dictionary<ushort, int> d2)
        {
            foreach (var (key, value1) in d1)
            {
                if (d2.TryGetValue(key, out var value2))
                {
                    if (value1 > value2)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckWhere0SetCount(Dictionary<ushort, int> d1, Dictionary<ushort, int> d2)
        {
            foreach (var (key, value1) in d1)
            {
                if (d2.TryGetValue(key, out var value2))
                {
                    var s1 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)value1);
                    var s2 = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)value2);

                    if (s1 > s2)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private HashSet<ushort> CalculateOutput() 
        {
            var total = (1 << IComparatorNetwork.Inputs) - 1;
            var output = new HashSet<ushort>();

            for (ushort i = 1; i < total; i++)
            {
                if (output.Add(this.ComputeOutput(i, out var setBits)))
                {
                    if (this.SequencesWithOnes.ContainsKey(setBits))
                    {
                        this.SequencesWithOnes[setBits] ++;
                    }
                    else
                    {
                        this.SequencesWithOnes.Add(setBits, 1);
                    }
                }
            }

            // complement where 0
            foreach (var key in this.Where0.Keys.ToList())
            {
                this.Where0[key] = ~this.Where0[key] & ((1 << IComparatorNetwork.Inputs) - 1);
            }

            return output;
        }

        private ushort ComputeOutput(ushort value, out ushort setBits) 
        {
            var arr = new BitArray(new int[] { value }) { Length = IComparatorNetwork.Inputs };
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

            var outputValue = new uint[1];
            arr.CopyTo(outputValue, 0);
            setBits = (ushort)System.Runtime.Intrinsics.X86.Popcnt.PopCount(outputValue[0]);
            this.CalculateDifferentOnePositions((int)outputValue[0], setBits);
            this.CalculateDifferentZeroPositions((int)outputValue[0], setBits);

            return (ushort)outputValue[0];
        }

        private void CalculateDifferentOnePositions(int value, ushort setBits)
        {
            if (!this.Where1.ContainsKey(setBits))
            {
                this.Where1[setBits] = 0;
            }
            this.Where1[setBits] |= value;
        }

        private void CalculateDifferentZeroPositions(int value, ushort setBits)
        {
            if (!this.Where0.ContainsKey(setBits))
            {
                this.Where0[setBits] = -1;
            }
            this.Where0[setBits] &= value;
        }
    }
}
