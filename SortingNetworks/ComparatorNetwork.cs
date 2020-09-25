namespace SortingNetworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using SortingNetworks.Extensions;

    /// <inheritdoc cref="IComparatorNetwork"/>
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(ushort inputs, Comparator[] comparators) 
        {
            this.DifferentZeroPositions = new Dictionary<ushort, ushort>();
            this.SequencesWithKOnes = new Dictionary<ushort, ushort>();

            this.Comparators = comparators;
            this.Inputs = inputs;
            this.Outputs = this.CalculateOutput();
        }

        /// <inheritdoc/>
        public HashSet<ushort> Outputs { get; private set; }

        public Dictionary<ushort, ushort> DifferentZeroPositions { get; private set; }

        public Dictionary<ushort, ushort> SequencesWithKOnes { get; private set; }

        /// <inheritdoc/>
        public ushort Inputs { get; private set; }

        /// <inheritdoc/>
        public Comparator[] Comparators { get;  set; }

        /// <inheritdoc/>
        public bool IsSortingNetwork()
        {
            return this.Outputs.Count == this.Inputs - 1;
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
        public bool IsSubsumed(IComparatorNetwork n, IEnumerable<int>[] permutations)
        {
            if (!ShouldCheckSubsumption(n, this)) return false;

            if (n.Outputs.IsSubsetOf(this.Outputs))
            {
                return true;
            }

            // Skip first permutation, as it is already check
            for (var i = 1; i < permutations.Length; i++)
            {
                var permutation = permutations[i].ToArray();
                var permutedSubset = new HashSet<ushort>();

                using (var enumerator = n.Outputs.GetEnumerator())
                {
                    enumerator.MoveNext();
                    do
                    {
                        var outputBits = new BitArray(new int[] { enumerator.Current }) { Length = n.Inputs };
                        var permutedOutputBits = new BitArray(n.Inputs);

                        for (var j = 0; j < permutation.Length; j++)
                        {
                            permutedOutputBits[j] = outputBits[permutation[j]];
                        }

                        var permutedOutput = new int[1];
                        permutedOutputBits.CopyTo(permutedOutput, 0);
                        permutedSubset.Add((ushort)permutedOutput[0]);
                    }
                    while (enumerator.MoveNext());
                }

                if (permutedSubset.IsSubsetOf(this.Outputs))
                {
                    return true;
                }
            }

            return false;
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

            return SequencesAreCompatible(n1.SequencesWithKOnes, n2.SequencesWithKOnes) &&
                   SequencesAreCompatible(n1.DifferentZeroPositions, n2.DifferentZeroPositions);
        }

        private static bool SequencesAreCompatible(Dictionary<ushort, ushort> d1, Dictionary<ushort, ushort> d2)
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

        private static Dictionary<ushort, ushort> IncrementInDictionary(ushort key, Dictionary<ushort, ushort> dict, ushort value = 1)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += value;
            }
            else
            {
                dict.Add(key, value);
            }

            return dict;
        }

        private HashSet<ushort> CalculateOutput() 
        {
            var total = Math.Pow(2, this.Inputs) - 1;
            var output = new HashSet<ushort>();
            var differentZeroPositions = new Dictionary<ushort, BitArray>();
            ushort setBits;

            for (ushort i = 1; i < total; i++) 
            {
                if (output.Add(this.ComputeOutput(i, ref differentZeroPositions, out setBits)))
                {
                    this.SequencesWithKOnes = IncrementInDictionary(setBits, this.SequencesWithKOnes);
                }
            }

            foreach (var (key, value) in differentZeroPositions)
            {
                this.DifferentZeroPositions.Add(key, value.SetCount());
            }

            return output;
        }

        private ushort ComputeOutput(ushort value, ref Dictionary<ushort, BitArray> differentZeroPositions, out ushort setBits) 
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

            var outputValue = new uint[1];
            arr.CopyTo(outputValue, 0);
            setBits = (ushort)System.Runtime.Intrinsics.X86.Popcnt.PopCount(outputValue[0]);
            this.CalculateDifferentZeroPositions(ref differentZeroPositions, arr, setBits);

            return (ushort)outputValue[0];
        }

        private void CalculateDifferentZeroPositions(ref Dictionary<ushort, BitArray> differentZeroPositions, BitArray arr, ushort setBits)
        {
            if (differentZeroPositions.ContainsKey(setBits))
            {
                differentZeroPositions[setBits] = differentZeroPositions[setBits].Or(arr);
            }
            else
            {
                differentZeroPositions[setBits] = arr;
            }
        }
    }
}
