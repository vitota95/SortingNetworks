namespace SortingNetworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http.Headers;

    /// <inheritdoc cref="IComparatorNetwork"/>
    public class ComparatorNetwork : IComparatorNetwork
    {
        public ComparatorNetwork(short inputs, Comparator[] comparators) 
        {
            this.DifferentZeroPositions = new bool[inputs][];
            this.DifferentOnePositions = new bool[inputs][];
            this.OutputsDictionary = new Dictionary<uint, HashSet<short>>();
            this.Comparators = comparators;
            this.Inputs = inputs;
            this.Outputs = this.CalculateOutput();
            this.W = CalculateW(this.OutputsDictionary, inputs);
        }

        /// <inheritdoc/>
        public HashSet<short> Outputs { get; private set; }

        public bool[][] DifferentZeroPositions { get; private set; }

        public bool[][] DifferentOnePositions { get; private set; }

        public Dictionary<uint, HashSet<short>> OutputsDictionary { get; }

        public Dictionary<uint, bool[]>[] W { get; }

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
        public bool IsSubsumedBy(IComparatorNetwork n, IEnumerable<int>[] permutations)
        {
            if (!ShouldCheckSubsumption(n, this)) return false;

            if (n.Outputs.IsSubsetOf(this.Outputs)) return true;

            //foreach (var (key, value) in this.OutputsDictionary)
            //{
            //    permutations = this.GetUsefulPermutations(this.W[0][key], n.W[0][key], permutations);
            //    permutations = this.GetUsefulPermutations(this.W[1][key], n.W[1][key], permutations);
            //}

            // Skip first permutation, as it is already check
            for (var i = 1; i < permutations.Length; i++)
            {
                var permutedSubset = new HashSet<short>();
                
                var permutation = permutations[i].ToArray();
                
                using (var enumerator = n.Outputs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
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
            if (OutputPositionsAreCompatible(n1.DifferentOnePositions, n2.DifferentOnePositions))
            {
                return false;
            }

            if (OutputPositionsAreCompatible(n1.DifferentZeroPositions, n2.DifferentZeroPositions))
            {
                return false;
            }

            return true;
        }

        private static bool OutputPositionsAreCompatible(bool[][] s1, bool[][] s2)
        {
            for (var i = 0; i < s1.GetLength(0); i++)
            {
                if (s1[i] != null && s2[i] != null)
                {
                    if (s1[i].Length > s2[i].Length)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        private static Dictionary<uint, bool[]>[] CalculateW(Dictionary<uint, HashSet<short>> net1Outputs, short length)
        {
            var w0 = new Dictionary<uint, bool[]>();
            var w1 = new Dictionary<uint, bool[]>();

            foreach (var (key, value) in net1Outputs)
            {
                var positions0 = new bool[length];
                var positions1 = new bool[length];
                using (var enumerator = value.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var arr = new BitArray(new int[] { enumerator.Current }) { Length = length };
                        for (var i = 0; i < arr.Count; i++)
                        {
                            if (arr.Get(i))
                            {
                                positions1[i] = true;
                            }
                            else
                            {
                                positions0[i] = true;
                            }
                        }
                    }
                }

                w0.Add(key, positions0);
                w1.Add(key, positions1);
            }

            return new[] { w0, w1 };
        }

        private static bool[,] ComputeWMatrix(bool[] w1, bool[] w2)
        {
            var matrix = new bool[w1.Length, w2.Length];

            for (var i = 0; i < w1.Length; i++)
            {
                var marked = 0;
                for (var j = 0; j < w2.Length; j++)
                {
                    if (w1[i]  && !w2[j])
                    {
                        matrix[i, j] = true;
                        marked++;
                    }
                }

                if (marked == w1.Length)
                {
                    return null;
                }
            }

            //PrintMatrix(matrix);

            return matrix;
        }

        private static void PrintMatrix(bool[,] matrix)
        {
            Trace.WriteLine("------------------------------------");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Trace.Write($"{matrix[i, j]}\t");
                }
                Trace.WriteLine(string.Empty);
            }
            Trace.WriteLine("------------------------------------");
            Trace.WriteLine(string.Empty);
        }

        /// <summary>
        /// Discards permutations to apply in subsumption test in application of lemma 6.
        /// </summary>
        /// <param name="arr1">The positions vector of net 1.</param>
        /// <param name="arr2">The positions vector of net 2.</param>
        /// <param name="permutations">The all permutations array.</param>
        /// <returns>Returns a dictionary with key number of set bits in Outputs and value permutations to discard</returns>
        private IEnumerable<int>[] GetUsefulPermutations(bool[] arr1, bool[] arr2, IEnumerable<int>[] permutations)
        {
            var matrix = ComputeWMatrix(arr1, arr2);

            if (matrix != null)
            {
                for (var i = 0; i < arr1.Length; i++)
                {
                    for (var j = 0; j < arr1.Length; j++)
                    {
                        if (matrix[i, j])
                        {
                            permutations = permutations.Where(
                                x =>
                                    {
                                        var temp = x.ToArray();
                                        return temp[i] != j;
                                    }).ToArray();
                        }
                    }
                }
            }
            //else
            //{
            //    return null;
            //}
            
            return permutations;
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

            if (this.OutputsDictionary.ContainsKey(setBits))
            {
                this.OutputsDictionary[setBits].Add((short)newValue[0]);
            }
            else
            {
                this.OutputsDictionary[setBits] = new HashSet<short>();
            }

            this.CalculateDifferentZeroPositions(arr, setBits);

            return (short)newValue[0];
        }

        private void CalculateDifferentZeroPositions(BitArray arr, uint setBits)
        {
            var zeroPositions = new bool[this.Inputs];
            var onePositions = new bool[this.Inputs];
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr.Get(i))
                {
                    onePositions[i] = true;
                }
                else
                {
                    zeroPositions[i] = true;
                }
            }

            this.DifferentZeroPositions = this.AddToPositionsMatrix(setBits, zeroPositions, this.DifferentZeroPositions);
            this.DifferentOnePositions = this.AddToPositionsMatrix(setBits, onePositions, this.DifferentOnePositions);
        }

        private bool[][] AddToPositionsMatrix(uint setBits, bool[] positions, bool[][] matrix)
        {
            if (matrix[setBits] == null)
            {
                matrix[setBits] = positions;
            }
            else
            {
                var temp = matrix[setBits];

                for (var i = 0; i < temp.Length; i++)
                {
                    if (!temp[i] && positions[i])
                    {
                        temp[i] = true;
                    }
                }

                matrix[setBits] = temp;
            }

            return matrix;
        }
    }
}
