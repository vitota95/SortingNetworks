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
            this.DifferentZeroPositions = new Dictionary<uint, bool[]>();
            this.Comparators = comparators;
            this.Inputs = inputs;
            this.Outputs = this.CalculateOutput();
            this.SequencecesWithKOnes = this.CalculateSequencesWithKOnes();
        }

        /// <inheritdoc/>
        public HashSet<short> Outputs { get; private set; }

        public Dictionary<uint, bool[]> DifferentZeroPositions { get; private set; }

        public Dictionary<uint, int> SequencecesWithKOnes { get; }

        /// <inheritdoc/>
        public short Inputs { get; private set; }

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
                var permutedSubset = new HashSet<short>();

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
                        permutedSubset.Add((short)permutedOutput[0]);
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
            if (CheckSequencesWithOnes(n1.SequencecesWithKOnes, n2.SequencecesWithKOnes))
            {
                return false;
            }

            if (CheckZeroPositions(n1.DifferentZeroPositions, n2.DifferentZeroPositions))
            {
                return false;
            }

            return true;
        }

        private static bool CheckSequencesWithOnes(Dictionary<uint, int> d1, Dictionary<uint, int> d2)
        {
            foreach (var (key, value) in d1)
            {
                if (d2.TryGetValue(key, out var y))
                {
                    if (value > y)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckZeroPositions(Dictionary<uint, bool[]> d1, Dictionary<uint, bool[]> d2)
        {
            foreach (var (key, value1) in d1)
            {
                if (d2.TryGetValue(key, out var value2))
                {
                    if (value1.Count(c => c) > value2.Count(c => c))
                    {
                        return true;
                    }
                }
                else // not sure about this else
                {
                    return true;
                }
            }

            return false;
        }

        private Dictionary<uint, int> CalculateSequencesWithKOnes()
        {
            var d = new Dictionary<uint, int>();

            using (var e = this.Outputs.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var setBits = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)e.Current);
                    IncrementInDictionary(setBits, ref d);
                }
            }

            return d;
        }

        private static void IncrementInDictionary(uint key, ref Dictionary<uint, int> dict, int value = 1)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] += value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        private HashSet<short> CalculateOutput() 
        {
            var total = Math.Pow(2, this.Inputs) - 1;
            var output = new HashSet<short>();
            
            for (short i = 1; i < total; i++) 
            {
                output.Add(this.ComputeOutput(i));
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
            var setBits = System.Runtime.Intrinsics.X86.Popcnt.PopCount((uint)newValue[0]);

            var zeroPositions = new bool[this.Inputs];
            for (var i = 0; i < arr.Length; i++)
            {
                if (!arr.Get(i))
                {
                    zeroPositions[i] = true;
                }
            }

            if (!this.DifferentZeroPositions.ContainsKey(setBits))
            {
                this.DifferentZeroPositions[setBits] = zeroPositions;
            }
            else
            {
                var temp = this.DifferentZeroPositions[setBits];

                for (var i = 0; i < temp.Length; i++)
                {
                    if (!temp[i] && zeroPositions[i])
                    {
                        temp[i] = true;
                    }
                }

                this.DifferentZeroPositions[setBits] = temp;
            }

            return (short)newValue[0];
        }
    }
}
