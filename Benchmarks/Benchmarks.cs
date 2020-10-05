namespace Benchmarks
{
    using System;

    using BenchmarkDotNet;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    using SortingNetworks;

    public class Benchmarks
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmarks>();
        }

        [Benchmark]
        [Arguments(6, 12)]
        [Arguments(7, 16)]
        public void SingleCore(int inputs, int comparators)
        {
            var args = $@"{inputs} {comparators} benchmarks.log".Split(' ');
            Program.Main(args);
        }
    }
}
