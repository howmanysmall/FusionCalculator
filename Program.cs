using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using MyHashSet = FusionCalculator.DataStructures.HashSet<string>;

namespace FusionCalculator
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// To run the benchmark, do `dotnet run -c release`.
			BenchmarkRunner.Run<BenchmarkHashSet>();
		}

		[SimpleJob(RuntimeMoniker.Net70)]
		[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
		public class BenchmarkHashSet
		{
			#region Benchmarks
			[Benchmark(Description = "HashSet (Open Addressing - base allocation=5)")]
			public MyHashSet BenchmarkOpenHashSetMinimal()
			{
				var hashSet = new MyHashSet(DataStructures.HashSetType.OpenAddressing, 5);

				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}

			[Benchmark(Description = "HashSet (Open Addressing - base allocation=RUN_ATTEMPTS)")]
			public MyHashSet BenchmarkOpenHashSet()
			{
				var hashSet = new MyHashSet(
					DataStructures.HashSetType.OpenAddressing,
					RUN_ATTEMPTS
				);

				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}

			[Benchmark(Description = "HashSet (Separate Chaining - base allocation=5)")]
			public MyHashSet BenchmarkSeparateHashSetMinimal()
			{
				var hashSet = new MyHashSet(DataStructures.HashSetType.SeparateChaining, 5);
				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}

			[Benchmark(Description = "HashSet (Separate Chaining - base allocation=RUN_ATTEMPTS)")]
			public MyHashSet BenchmarkSeparateHashSet()
			{
				var hashSet = new MyHashSet(
					DataStructures.HashSetType.SeparateChaining,
					RUN_ATTEMPTS
				);

				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}

			[Benchmark(Description = "Internal HashSet (base allocation=5)")]
			public HashSet<string> BenchmarkCSharpHashSetMinimal()
			{
				var hashSet = new HashSet<string>(5);
				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}

			[Benchmark(
				Baseline = true,
				Description = "Internal HashSet (base allocation=RUN_ATTEMPTS)"
			)]
			public HashSet<string> BenchmarkCSharpHashSet()
			{
				var hashSet = new HashSet<string>(RUN_ATTEMPTS);
				for (int index = 0; index < RUN_ATTEMPTS; index += 1)
					hashSet.Add(index.ToString());

				return hashSet;
			}
			#endregion

			public const int RUN_ATTEMPTS = 100_000;
		}
	}
}
