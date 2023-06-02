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
	internal class SeparateChainingHashSet<T> : IHashSet<T>
	{
		public int Count { get; private set; }

		internal SeparateChainingHashSet(int initialBucketSize = 3)
		{
			this.InitialBucketSize = initialBucketSize;
			this.HashArray = new DoublyLinkedList<HashSetNode<T>>[initialBucketSize];
		}

		#region Methods

		/// <summary>
		/// Checks if the value is in the hash set.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <returns>Whether or not the value is in the hash set.</returns>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public bool Contains(T value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value), "Can't check null value!");

			int index = Math.Abs(value.GetHashCode()) % this.BucketSize;
			if (this.HashArray[index] is null)
				return false;

			DoublyLinkedListNode<HashSetNode<T>>? current = this.HashArray[index].Head;
			while (current is not null)
			{
				if (current.Data.Value!.Equals(value))
					return true;

				current = current.Next;
			}

			return false;
		}

		/// <summary>
		/// Adds the value to the hash set.
		/// </summary>
		/// <param name="value">The value you want to add.</param>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public void Add(T value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value), "Can't add null value!");

			this.Grow();
			int index = Math.Abs(value.GetHashCode()) % this.BucketSize;

			if (this.HashArray[index] is null)
			{
				this.HashArray[index] = new DoublyLinkedList<HashSetNode<T>>();
				this.HashArray[index].InsertFirst(new HashSetNode<T>(value));
				this.FilledBuckets += 1;
			}
			else
			{
				DoublyLinkedListNode<HashSetNode<T>>? current = this.HashArray[index].Head;
				while (current is not null)
				{
					if (current.Data.Value!.Equals(value))
						throw new InvalidOperationException("Value already exists!");

					current = current.Next;
				}

				this.HashArray[index].InsertFirst(new HashSetNode<T>(value));
			}

			this.Count += 1;
		}

		/// <summary>
		/// Removes the value from the hash set.
		/// </summary>
		/// <param name="value">The value you want to remove.</param>
		public void Remove(T value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value), "Can't remove null value!");

			int index = Math.Abs(value.GetHashCode()) % this.BucketSize;
			if (this.HashArray[index] is null)
				throw new InvalidOperationException("Index is null!");

			DoublyLinkedListNode<HashSetNode<T>>? current = this.HashArray[index].Head;
			DoublyLinkedListNode<HashSetNode<T>>? previous = null;
			while (current is not null)
			{
				if (current.Data.Value!.Equals(value))
				{
					previous = current;
					break;
				}

				current = current.Next;
			}

			if (previous is null)
				throw new Exception("Value not found!");

			this.HashArray[index].Delete(previous);
			if (this.HashArray[index].Head is null)
			{
				this.HashArray[index] = null!;
				this.FilledBuckets -= 1;
			}

			this.Count -= 1;
			this.Shrink();
		}

		/// <summary>
		/// Clears the hash set.
		/// </summary>
		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public void Clear()
		{
			this.HashArray = new DoublyLinkedList<HashSetNode<T>>[this.InitialBucketSize];
			this.Count = 0;
			this.FilledBuckets = 0;
		}

		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		public IEnumerator<T> GetEnumerator() =>
			new SeparateChainingHashSetEnumerator<T>(this.HashArray, this.BucketSize);
		#endregion

		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

		#region Private
		private int BucketSize => this.HashArray.Length;
		private const double Tolerance = 0.1;
		private readonly int InitialBucketSize;
		private int FilledBuckets;
		private DoublyLinkedList<HashSetNode<T>>[] HashArray = null!;

		private void SetValue(T value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value), "Can't add null value!");

			int index = Math.Abs(value.GetHashCode()) % this.BucketSize;
			if (this.HashArray[index] is null)
				throw new InvalidOperationException("Index is null!");

			DoublyLinkedListNode<HashSetNode<T>>? current = this.HashArray[index].Head;
			while (current is not null)
			{
				if (current.Data.Value!.Equals(value))
				{
					this.Remove(value);
					this.Add(value);
					return;
				}

				current = current.Next;
			}

			throw new Exception("Value not found!");
		}

		[MethodImpl(
			MethodImplOptions.AggressiveOptimization & MethodImplOptions.AggressiveInlining
		)]
		private void Grow()
		{
			if (this.FilledBuckets >= this.BucketSize * 0.7)
			{
				this.FilledBuckets = 0;

				int newBucketSize = this.BucketSize * 2;
				DoublyLinkedList<HashSetNode<T>>[] newHashArray = new DoublyLinkedList<
					HashSetNode<T>
				>[newBucketSize];

				for (int index = 0; index < this.BucketSize; index += 1)
				{
					DoublyLinkedList<HashSetNode<T>>? item = this.HashArray[index];
					if (item is not null)
						if (item.Head is not null)
						{
							DoublyLinkedListNode<HashSetNode<T>>? current = item.Head;
							while (current is not null)
							{
								DoublyLinkedListNode<HashSetNode<T>>? next = current.Next;
								int newIndex =
									Math.Abs(current.Data.Value!.GetHashCode()) % newBucketSize;

								if (newHashArray[newIndex] is null)
								{
									this.FilledBuckets += 1;
									newHashArray[newIndex] = new DoublyLinkedList<HashSetNode<T>>();
								}

								newHashArray[newIndex].InsertFirst(current);
								current = next;
							}
						}
				}

				this.HashArray = newHashArray;
			}
		}

		private void Shrink()
		{
			if (
				Math.Abs(this.FilledBuckets - this.BucketSize * 0.3) < Tolerance
				&& this.BucketSize / 2 > this.InitialBucketSize
			)
			{
				this.FilledBuckets = 0;
				int newBucketSize = this.BucketSize / 2;
				DoublyLinkedList<HashSetNode<T>>[] newHashArray = new DoublyLinkedList<
					HashSetNode<T>
				>[newBucketSize];

				for (int index = 0; index < this.BucketSize; index += 1)
				{
					DoublyLinkedList<HashSetNode<T>>? item = this.HashArray[index];
					if (item?.Head is not null)
					{
						DoublyLinkedListNode<HashSetNode<T>>? current = item.Head;
						while (current is not null)
						{
							DoublyLinkedListNode<HashSetNode<T>>? next = current.Next;
							int newIndex =
								Math.Abs(current.Data.Value!.GetHashCode()) % newBucketSize;

							if (newHashArray[newIndex] is null)
							{
								this.FilledBuckets += 1;
								newHashArray[newIndex] = new DoublyLinkedList<HashSetNode<T>>();
							}

							newHashArray[newIndex].InsertFirst(current);
							current = next;
						}
					}
				}

				this.HashArray = newHashArray;
			}
		}
		#endregion
	}

	internal class SeparateChainingHashSetEnumerator<T> : IEnumerator<T>
	{
		public T Current
		{
			get
			{
				try
				{
					return this.CurrentNode!.Data.Value;
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
			}
		}

		internal SeparateChainingHashSetEnumerator(
			DoublyLinkedList<HashSetNode<T>>[] hashList,
			int length
		)
		{
			this.HashList = hashList;
			this.Length = length;
		}

		public bool MoveNext()
		{
			if (this.CurrentNode?.Next is not null)
			{
				this.CurrentNode = this.CurrentNode.Next;
				return true;
			}

			while (this.CurrentNode?.Next is null)
			{
				this.Position += 1;
				if (this.Position < this.Length)
				{
					if (this.HashList[this.Position] is null)
						continue;

					this.CurrentNode = this.HashList[this.Position].Head;
					if (this.CurrentNode is null)
						continue;

					return true;
				}

				break;
			}

			return false;
		}

		public void Dispose()
		{
			this.Length = 0;
			this.HashList = null!;
		}

		public void Reset()
		{
			this.Position = -1;
			this.CurrentNode = null;
		}

		object IEnumerator.Current => this.Current!;

		internal DoublyLinkedList<HashSetNode<T>>[] HashList;

		private int Length;
		private int Position = -1;
		private DoublyLinkedListNode<HashSetNode<T>>? CurrentNode;
	}
}
