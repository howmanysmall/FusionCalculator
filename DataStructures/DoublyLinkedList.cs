using System.Runtime.CompilerServices;
using System.Collections;

namespace FusionCalculator.DataStructures
{
	public class DoublyLinkedList<T> : IEnumerable<T>
	{
		public DoublyLinkedListNode<T>? Head;
		public DoublyLinkedListNode<T>? Tail;

		public IEnumerator<T> GetEnumerator() => new DoublyLinkedListEnumerator<T>(ref this.Head!);

		/// <summary>
		/// Inserts a new node at the start of a list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="data">The data to insert with.</param>
		/// <returns>The new node.</returns>
		public DoublyLinkedListNode<T> InsertFirst(T data)
		{
			DoublyLinkedListNode<T> newNode = new DoublyLinkedListNode<T>(data);
			if (this.Head is not null)
				this.Head.Previous = newNode;

			newNode.Next = this.Head;
			newNode.Previous = null;

			this.Head = newNode;
			if (this.Tail is null)
				this.Tail = this.Head;

			return newNode;
		}

		/// <summary>
		/// Insert right after this node.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="node">The node to insert after.</param>
		/// <param name="data">The data to insert.</param>
		/// <returns>The inserted node.</returns>
		public DoublyLinkedListNode<T> InsertAfter(
			DoublyLinkedListNode<T> node,
			DoublyLinkedListNode<T> data
		)
		{
			if (node is null)
				throw new ArgumentNullException(nameof(node), "Empty reference node!");

			if (node == this.Head && node == this.Tail)
			{
				node.Next = data;
				node.Previous = null;

				data.Previous = node;
				data.Next = null;

				this.Head = node;
				this.Tail = data;

				return data;
			}

			if (node != this.Tail)
			{
				data.Previous = node;
				data.Next = node.Next;

				node.Next!.Previous = data;
				node.Next = data;
			}
			else
			{
				data.Previous = node;
				data.Next = null;

				node.Next = data;
				this.Tail = data;
			}

			return data;
		}

		/// <summary>
		/// Insert right before this node.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="node">The node to insert before.</param>
		/// <param name="data">The data to insert.</param>
		/// <returns>The inserted node.</returns>
		public DoublyLinkedListNode<T> InsertBefore(
			DoublyLinkedListNode<T> node,
			DoublyLinkedListNode<T> data
		)
		{
			if (node is null)
				throw new ArgumentNullException(nameof(node), "Empty reference node!");

			if (node == this.Head && node == this.Tail)
			{
				node.Previous = data;
				node.Next = null;
				this.Tail = node;

				data.Previous = null;
				data.Next = node;

				this.Head = data;
				return data;
			}

			if (node != this.Head)
			{
				data.Previous = null;
				data.Next = node;

				node.Previous = data;
				this.Head = data;
			}
			else
			{
				data.Previous = node.Previous;
				data.Next = node;

				node.Previous!.Next = data;
				node.Previous = data;
			}

			return data;
		}

		/// <summary>
		/// Inserts a new node at the end of the list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="data">The data to insert with.</param>
		/// <returns>The inserted node.</returns>
		public DoublyLinkedListNode<T> InsertLast(T data)
		{
			if (this.Tail is null)
				return this.InsertFirst(data);

			DoublyLinkedListNode<T> newNode = new DoublyLinkedListNode<T>(data);
			this.Tail.Next = newNode;

			newNode.Previous = this.Tail;
			newNode.Next = null;

			this.Tail = newNode;
			return newNode;
		}

		/// <summary>
		/// Deletes the first value in the list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <returns>The deleted node.</returns>
		public T DeleteFirst()
		{
			if (this.Head is null)
				throw new InvalidOperationException("The list is empty!");

			T headData = this.Head.Data;
			if (this.Head == this.Tail)
			{
				this.Head = null;
				this.Tail = null;
			}
			else
			{
				this.Head.Next!.Previous = null;
				this.Head = this.Head.Next;
			}

			return headData;
		}

		/// <summary>
		/// Deletes the last value in the list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <returns>The deleted node.</returns>
		public T DeleteLast()
		{
			if (this.Tail is null)
				throw new InvalidOperationException("The list is empty!");

			T tailData = this.Tail.Data;
			if (this.Tail == this.Head)
			{
				this.Head = null;
				this.Tail = null;
			}
			else
			{
				this.Tail.Previous!.Next = null;
				this.Tail = this.Tail.Previous;
			}

			return tailData;
		}

		/// <summary>
		/// Deletes the node with the matching data from the list.<br/>
		/// Time complexity: O(n)
		/// </summary>
		/// <param name="data">The data you want to delete.</param>
		public void Delete(T data)
		{
			if (this.Head is null)
				throw new InvalidOperationException("The list is empty!");

			if (this.Head == this.Tail)
			{
				if (this.Head.Data!.Equals(data))
					this.DeleteFirst();

				return;
			}

			DoublyLinkedListNode<T>? current = this.Head;
			while (current is not null)
			{
				if (current.Data!.Equals(data))
				{
					if (current.Previous is null)
					{
						current.Next!.Previous = null;
						this.Head = current.Next;
					}
					else if (current.Next is null)
					{
						current.Previous.Next = null;
						this.Tail = current.Previous;
					}
					else
					{
						current.Previous.Next = current.Next;
						current.Next.Previous = current.Previous;
					}

					break;
				}

				current = current.Next;
			}
		}

		/// <summary>
		/// Deletes the node from the list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="node">The node you want to delete.</param>
		public void Delete(DoublyLinkedListNode<T> node)
		{
			if (this.Head is null)
				throw new InvalidOperationException("The list is empty!");

			if (node == this.Head && node == this.Tail)
			{
				this.Head = null;
				this.Tail = null;
				return;
			}

			if (node == this.Head)
			{
				node.Next!.Previous = null;
				this.Head = node.Next;
			}
			else if (node == this.Tail)
			{
				node.Previous!.Next = null;
				this.Tail = node.Previous;
			}
			else
			{
				node.Previous!.Next = node.Next;
				node.Next!.Previous = node.Previous;
			}
		}

		/// <summary>
		/// Checks if the list is empty.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <returns>Whether or not the list is empty.</returns>
		public bool IsEmpty() => this.Head is null;

		public void Clear()
		{
			if (this.Head is null)
				throw new InvalidOperationException("The list is empty!");

			this.Head = null;
			this.Tail = null;
		}

		internal void InsertFirst(DoublyLinkedListNode<T> newNode)
		{
			if (this.Head is not null)
				this.Head.Previous = newNode;

			newNode.Next = this.Head;
			newNode.Previous = null;

			this.Head = newNode;
			if (this.Head is null)
				this.Tail = this.Head;
		}

		/// <summary>
		/// Unions this list with another list.<br/>
		/// Time complexity: O(1)
		/// </summary>
		/// <param name="newList">The list you want to union with.</param>
		internal void Union(DoublyLinkedList<T> newList)
		{
			if (this.Head is null)
			{
				this.Head = newList.Head;
				this.Tail = newList.Tail;
				return;
			}

			if (newList.Head is null)
				return;

			this.Head.Previous = newList.Tail;
			newList.Tail!.Next = this.Head;
			this.Head = newList.Head;
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}

	/// <summary>
	/// A node in a doubly linked list.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	public class DoublyLinkedListNode<T>
	{
		public T Data;
		public DoublyLinkedListNode<T>? Next;
		public DoublyLinkedListNode<T>? Previous;

		public DoublyLinkedListNode(T data)
		{
			this.Data = data;
		}
	}

	internal class DoublyLinkedListEnumerator<T> : IEnumerator<T>
	{
		public T Current => this.CurrentNode!.Data;
		internal DoublyLinkedListNode<T>? CurrentNode;
		internal DoublyLinkedListNode<T>? HeadNode;

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		internal DoublyLinkedListEnumerator(ref DoublyLinkedListNode<T> headNode)
		{
			this.HeadNode = headNode;
		}

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public bool MoveNext()
		{
			if (this.HeadNode is null)
				return false;

			if (this.CurrentNode is null)
			{
				this.CurrentNode = this.HeadNode;
				return true;
			}

			if (this.CurrentNode.Next is not null)
			{
				this.CurrentNode = this.CurrentNode.Next;
				return true;
			}

			return false;
		}

		public void Dispose()
		{
			this.HeadNode = null;
			this.CurrentNode = null;
		}

		public void Reset()
		{
			this.CurrentNode = this.HeadNode;
		}

		object IEnumerator.Current => this.CurrentNode!;
	}
}
