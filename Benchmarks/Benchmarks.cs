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
        [Arguments(7, 16)]
        public void SingleCore(int inputs, int comparators)
        {
            var args = $@"-s:{inputs} -k:{comparators} -l:benchmarks.log".Split(' ');
            Program.Main(args);
        }
    }
}
