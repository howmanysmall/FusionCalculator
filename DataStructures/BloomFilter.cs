using System.Runtime.CompilerServices;

namespace FusionCalculator.DataStructures
{
	/// <summary>
	/// A bloom filter is a space-efficient probabilistic data structure designed to test whether an element is present in a set.
	/// It is designed to be blazingly fast and use minimal memory at the cost of potential false positives.
	/// False positive matches are possible, but false negatives are not – in other words, a query returns either "possibly in set" or "definitely not in set".<br/>
	///
	/// Bloom proposed the technique for applications where the amount of source data would require an impractically large amount of memory if "conventional" error-free hashing techniques were applied.<br/>
	///
	/// This is useful for showing players something that they may not have seen yet. Once they've seen it, you can insert it into the filter and it will no longer show up.
	/// </summary>
	public class BloomFilter
	{
		public int Size;
		internal readonly BloomFilterStore Storage;

		public BloomFilter(int size)
		{
			this.Size = size;
			this.Storage = new BloomFilterStore(size);
		}

		/// <summary>
		/// Inserts an item into the filter.
		/// </summary>
		/// <param name="item">The item you want to insert.</param>
		public void Insert(string item)
		{
			foreach (int hashValue in this.GetHashValuesArray(item))
				if (!this.Storage.GetValue(hashValue))
					this.Storage.SetValue(hashValue);
		}

		/// <summary>
		/// Checks if the item might be in the filter.
		/// </summary>
		/// <param name="item">The item to check for.</param>
		/// <returns>Whether or not the is maybe in the filter.</returns>
		public bool MayContain(string item)
		{
			foreach (int hashValue in this.GetHashValuesArray(item))
				if (!this.Storage.GetValue(hashValue))
					return false;

			return true;
		}

		/// <summary>
		/// Runs all 3 hash functions on the input and returns a tuple of results.
		/// </summary>
		/// <param name="item">The value you want to hash.</param>
		/// <returns>The hashed tuple.</returns>
		public (int, int, int) GetHashValues(string item) =>
			(this.Hash1(item), this.Hash2(item), this.Hash3(item));

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private int[] GetHashValuesArray(string item) =>
			new int[] { this.Hash1(item), this.Hash2(item), this.Hash3(item) };

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private int Hash1(string item)
		{
			int hash = 0;

			for (int index = 0; index < item.Length; index += 1)
			{
				int byteCode = Convert.ToInt32(item[index]);
				hash = (hash << 5) + hash + byteCode;
				hash &= hash;
				hash = Math.Abs(hash);
			}

			return hash % this.Size;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private int Hash2(string item)
		{
			int hash = 5381;
			for (int index = 0; index < item.Length; index += 1)
				hash = (hash << 5) + hash + Convert.ToInt32(item[index]);

			return Math.Abs(hash % this.Size);
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		private int Hash3(string item)
		{
			int hash = 0;
			for (int index = 0; index < item.Length; index += 1)
			{
				hash = (hash << 5) - hash;
				hash += Convert.ToInt32(item[index]);
				hash &= hash;
			}

			return Math.Abs(hash % this.Size);
		}
	}

	internal class BloomFilterStore
	{
		private readonly bool[] Storage;

		public BloomFilterStore(int size)
		{
			this.Storage = new bool[size];
			for (int index = 0; index < size; index += 1)
				this.Storage[index] = false;
		}

		public bool GetValue(int index) => this.Storage[index];

		public void SetValue(int index) => this.Storage[index] = true;
	}
}
