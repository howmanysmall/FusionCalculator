using System.Collections;
using System.Runtime.CompilerServices;

// some notes for me
// `MethodImplOptions.AggressiveInlining` suggests to that the JIT compiler should
// inline this method if possible. Inlining a method can improve performance by
// reducing the overhead of calling the method. However, inlining too much can increase
// the size of the code and potentially cause cache misses, which can hurt performance.

// `MethodImplOptions.AggressiveOptimization`, is used to indicate that it contains
// hot code. When this attribute is applied to a method, it causes the fully optimizing
// JIT to be used right away for the method, which might produce better, more optimized
// code but would take more time to compile. This attribute is useful for hot path code
// where using more time at the beginning saves time later if runtime detects that it's
// on the hot path.

// Hot code refers to sections of code that are executed frequently during the runtime
// of a program. If you are assuming that this function will be called a lot, then you
// can use this attribute to tell the JIT compiler to optimize the code for performance.

namespace FusionCalculator.DataStructures
{
	internal class OpenAddressHashSet<T> : IHashSet<T>
	{
		public int Count { get; private set; }

		internal OpenAddressHashSet(int initialBucketSize = 2)
		{
			this.InitialBucketSize = initialBucketSize;
			this.HashArray = new HashSetNode<T>[initialBucketSize];
		}

		#region Methods

		/// <summary>
		/// Checks if the value is in the hash set.
		/// </summary>
		/// <param name="value">The value to check for.</param>
		/// <returns>The status of the value's existence.</returns>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public bool Contains(T value)
		{
			int hashCode = this.GetHash(value);
			int index = hashCode % this.BucketSize;

			if (this.HashArray[index] is null)
				return false;

			HashSetNode<T>? current = this.HashArray[index];
			T hitKey = current.Value;

			while (current is not null)
			{
				if (current.Value is null)
					throw new NullReferenceException("Current value is null");

				if (current.Value.Equals(value))
					return true;

				index += 1;
				if (index == this.BucketSize)
					index = 0;

				current = this.HashArray[index];
				if (current is not null)
				{
					if (current.Value is null)
						throw new NullReferenceException("Current value is null");

					if (current.Value.Equals(hitKey))
						return false;
				}
			}

			return false;
		}

		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public IEnumerator<T> GetEnumerator() =>
			new OpenAddressHashSetEnumerator<T>(this.HashArray, this.HashArray.Length);

		/// <summary>
		/// Adds a value to the hash set.
		/// </summary>
		/// <param name="value">The value you want to add.</param>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public void Add(T value)
		{
			this.Grow();
			int hashCode = this.GetHash(value);
			int index = hashCode % this.BucketSize;

			if (this.HashArray[index] is null)
				this.HashArray[index] = new HashSetNode<T>(value);
			else
			{
				HashSetNode<T>? current = this.HashArray[index];

				T hitKey = current.Value;

				while (current is not null)
				{
					if (current.Value is null)
						throw new NullReferenceException("Current value is null");

					if (current.Value.Equals(value))
						throw new Exception("Duplicate value");

					index += 1;
					if (index == this.BucketSize)
						index = 0;

					current = this.HashArray[index];
					if (current is not null)
					{
						if (current.Value is null)
							throw new NullReferenceException("Current value is null");

						if (current.Value.Equals(hitKey))
							throw new Exception("HashSet is full!");
					}
				}

				this.HashArray[index] = new HashSetNode<T>(value);
			}

			this.Count += 1;
		}

		/// <summary>
		/// Removes a value from the hash set.
		/// </summary>
		/// <param name="value">The value you want to remove.</param>
		public void Remove(T value)
		{
			int hashCode = this.GetHash(value);
			int currentIndex = hashCode % this.BucketSize;

			if (this.HashArray[currentIndex] is null)
				throw new Exception("No such item for given value");

			HashSetNode<T> current = this.HashArray[currentIndex];
			T hitKey = current.Value;

			HashSetNode<T>? target = null;
			while (current is not null)
			{
				T currentValue = current.Value;
				if (currentValue is null)
					throw new NullReferenceException("Current value is null");

				if (currentValue.Equals(value))
				{
					target = current;
					break;
				}

				currentIndex += 1;
				if (currentIndex == this.BucketSize)
					currentIndex = 0;

				current = this.HashArray[currentIndex];
				if (current is not null)
				{
					T newCurrentValue = current.Value;
					if (newCurrentValue is null)
						throw new NullReferenceException("Current value is null");

					if (newCurrentValue.Equals(hitKey))
						throw new Exception("No such item for given value");
				}
			}

			if (target is null)
				throw new Exception("No such item for given value");

			this.HashArray[currentIndex] = null!;
			currentIndex += 1;

			if (currentIndex == this.BucketSize)
				currentIndex = 0;

			current = this.HashArray[currentIndex];

			while (current is not null)
			{
				this.HashArray[currentIndex] = null!;
				this.Add(current.Value);

				currentIndex += 1;
				if (currentIndex == this.BucketSize)
					currentIndex = 0;

				current = this.HashArray[currentIndex];
			}

			this.Count -= 1;
			this.Shrink();
		}

		/// <summary>
		/// Completely clears the hash set.
		/// </summary>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public void Clear()
		{
			this.HashArray = new HashSetNode<T>[this.InitialBucketSize];
			this.Count = 0;
		}
		#endregion

		#region Privates
		private int BucketSize => this.HashArray.Length;
		private HashSetNode<T>[] HashArray;
		private readonly int InitialBucketSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		// This is aggressively optimized because of the fact it will be called
		// every time something is added, and since I'm anticipating me adding
		// some 52,000 items to this hash set, I want to make sure it's as fast
		// as possible.
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		private void Grow()
		{
			if (!(this.BucketSize * 0.7 <= Count))
				return;

			int originalBucketSize = this.BucketSize;
			HashSetNode<T>[] currentArray = this.HashArray;

			this.HashArray = new HashSetNode<T>[this.BucketSize * 2];
			for (int index = 0; index < originalBucketSize; index += 1)
			{
				HashSetNode<T>? current = currentArray[index];
				if (current is not null)
				{
					this.Add(current.Value);
					this.Count -= 1;
				}
			}

			currentArray = null!;
		}

		private void Shrink()
		{
			if (this.Count <= this.BucketSize * 0.3 && this.BucketSize / 2 > this.InitialBucketSize)
			{
				int originalBucketSize = this.BucketSize;
				HashSetNode<T>[] currentArray = this.HashArray;

				this.HashArray = new HashSetNode<T>[this.BucketSize / 2];
				for (int index = 0; index < originalBucketSize; index += 1)
				{
					HashSetNode<T>? current = currentArray[index];
					if (current is not null)
					{
						this.Add(current.Value);
						this.Count -= 1;
					}
				}

				// free memory
				currentArray = null!;
			}
		}

		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		private int GetHash(T value) => value is not null ? Math.Abs(value.GetHashCode()) : 0;
		#endregion
	}

	// really wish I could use a struct 😵‍💫
	internal class HashSetNode<T>
	{
		internal T Value;

		internal HashSetNode(T value)
		{
			this.Value = value;
		}
	}

	internal class OpenAddressHashSetEnumerator<T> : IEnumerator<T>
	{
		public T Current
		{
			get
			{
				try
				{
					return this.HashArray[this.Position].Value;
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		internal HashSetNode<T>[] HashArray;

		internal OpenAddressHashSetEnumerator(HashSetNode<T>[] hashArray, int length)
		{
			this.HashArray = hashArray;
			this.Length = length;
		}

		public void Dispose()
		{
			this.Length = 0;
			this.HashArray = null!;
		}

		public bool MoveNext()
		{
			this.Position += 1;
			while (this.Position < this.Length && this.HashArray[this.Position] is null)
				this.Position += 1;

			return this.Position < this.Length;
		}

		public void Reset()
		{
			this.Position = -1;
		}

		object IEnumerator.Current => this.Current!;

		private int Length;
		private int Position = -1;
	}
}
