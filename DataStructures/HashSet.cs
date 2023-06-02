using System.Collections;

namespace FusionCalculator.DataStructures
{
	/// <summary>
	/// The HashSet implementation type.
	/// </summary>
	public enum HashSetType
	{
		OpenAddressing,
		SeparateChaining,
	}

	/// <summary>
	/// A hash table implementation.
	/// </summary>
	/// <typeparam name="T">The value data type.</typeparam>
	public class HashSet<T> : IEnumerable<T>
	{
		/// <summary>
		/// The number of items in this HashSet.
		/// </summary>
		public int Count => this.Set.Count;

		/// <summary>
		/// Creates a new HashSet.
		/// </summary>
		/// <param name="hashSetType">The HashSet implementation to use.</param>
		/// <param name="initialBucketSize">The larger the bucket size lesser the collision, but memory matters!</param>
		public HashSet(
			HashSetType hashSetType = HashSetType.SeparateChaining,
			int initialBucketSize = 2
		)
		{
			if (initialBucketSize < 2)
				throw new ArgumentOutOfRangeException(
					nameof(initialBucketSize),
					2,
					"The initial bucket size must be at least 2."
				);

			if (hashSetType == HashSetType.SeparateChaining)
				this.Set = new SeparateChainingHashSet<T>(initialBucketSize);
			else
				this.Set = new OpenAddressHashSet<T>(initialBucketSize);
		}

		#region Methods
		public IEnumerator<T> GetEnumerator() => this.Set.GetEnumerator();

		/// <summary>
		/// Does this hash table contains the given value.<br/?
		/// Time complexity: O(1) amortized.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>True if this hashset contains the given value.</returns>
		public bool Contains(T value)
		{
			return this.Set.Contains(value);
		}

		/// <summary>
		/// Add a new value.<br/?
		/// Time complexity: O(1) amortized.
		/// </summary>
		/// <param name="value">The value to add.</param>
		public void Add(T value)
		{
			this.Set.Add(value);
		}

		/// <summary>
		/// Remove the given value.<br/?
		/// Time complexity: O(1) amortized.
		/// </summary>
		/// <param name="value">The value to remove.</param>
		public void Remove(T value)
		{
			this.Set.Remove(value);
		}

		/// <summary>
		/// Clear the hashtable.<br/?
		/// Time complexity: O(1).
		/// </summary>
		public void Clear()
		{
			this.Set.Clear();
		}

		#endregion

		private readonly IHashSet<T> Set;

		IEnumerator IEnumerable.GetEnumerator() => this.Set.GetEnumerator();
	}

	internal interface IHashSet<T> : IEnumerable<T>
	{
		int Count { get; }

		void Add(T item);
		bool Contains(T value);
		void Clear();
		void Remove(T key);
	}
}
