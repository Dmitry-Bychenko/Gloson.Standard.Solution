﻿#region Description

//---------------------------------------------------------------------------------------------------------------------
//
// Min/Max Heap and Priority Queue
//
//---------------------------------------------------------------------------------------------------------------------

#endregion Description

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Heap (either Min or Max Heap)
  /// </summary>
  /// <typeparam name="T"></typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseHeap<T>
    : IEnumerable<T>,
      IReadOnlyCollection<T> {

    #region Private Data

    // Items
    protected List<T> m_Items;

    #endregion Private Data

    #region Algorithm

    private void Heapify(int n, int i, int sign) {
      int largest = i; // Initialize largest as root 
      int l = 2 * i + 1; // left = 2*i + 1 
      int r = 2 * i + 2; // right = 2*i + 2 

      // If left child is larger than root 
      if (l < n && Comparer.Compare(m_Items[l], m_Items[largest]) * sign > 0)
        largest = l;

      // If right child is larger than largest so far 
      if (r < n && Comparer.Compare(m_Items[r], m_Items[largest]) * sign > 0)
        largest = r;

      // If largest is not root 
      if (largest != i) {
        (m_Items[i], m_Items[largest]) = (m_Items[largest], m_Items[i]);


        // Recursively heapify the affected sub-tree 
        Heapify(n, largest, sign);
      }
    }

    // Function to build a Max-Heap from the Array 
    private void BuildHeap(int sign) {
      int n = m_Items.Count;

      // Index of last non-leaf node 
      int startIdx = (n / 2) - 1;

      // Perform reverse level order traversal 
      // from last non-leaf node and heapify 
      // each node 
      for (int i = startIdx; i >= 0; i--)
        Heapify(n, i, sign);
    }

    /// <summary>
    /// Heapify Move Down
    /// </summary>
    protected abstract void MoveDown(int index);

    /// <summary>
    /// Heapify Move Up
    /// </summary>
    protected abstract void MoveUp(int index);

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    protected BaseHeap(IComparer<T> comparer, int capacity) {
      comparer ??= Comparer<T>.Default;

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer), $"No comparer provided when {typeof(T).Name} doesn't provide default one");

      m_Items = capacity < 0 ? new List<T>() : new List<T>(capacity);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    protected BaseHeap(IComparer<T> comparer)
      : this(comparer, -1) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    protected BaseHeap(int capacity)
      : this(null, capacity) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected BaseHeap()
      : this(null, -1) {
    }

    protected BaseHeap(IEnumerable<T> source, IComparer<T> comparer, int sign) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      comparer ??= Comparer<T>.Default;

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer), $"No comparer provided when {typeof(T).Name} doesn't provide default one");

      m_Items = new List<T>(source);

      BuildHeap(sign);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer to use
    /// </summary>
    public IComparer<T> Comparer {
      get;
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<T> Items => m_Items;

    /// <summary>
    /// Take min (max) value without removing it
    /// </summary>
    public T Peek() => m_Items.Count > 0
      ? m_Items[0]
      : throw new InvalidOperationException("Heap is empty");

    /// <summary>
    /// Try Peek
    /// </summary>
    public bool TryPeek(out T value) {
      if (m_Items.Count <= 0) {
        value = default;

        return false;
      }

      value = m_Items[0];

      return true;
    }

    /// <summary>
    /// Take min (max) value and remove it
    /// </summary>
    public T Pop() {
      T result = m_Items.Count > 0
        ? m_Items[0]
        : throw new InvalidOperationException("Heap is empty");

      m_Items[0] = m_Items[^1];
      m_Items.RemoveAt(m_Items.Count - 1);

      MoveDown(0);

      return result;
    }

    /// <summary>
    /// Try Pop
    /// </summary>
    /// <param name="value">Min (Max) value or default(T)</param>
    /// <returns>true if value returned</returns>
    public bool TryPop(out T value) {
      if (m_Items.Count <= 0) {
        value = default;

        return false;
      }

      value = Pop();

      return true;
    }

    /// <summary>
    /// Push value to the Heap
    /// </summary>
    /// <param name="value">Value to push into heap</param>
    public void Add(T value) {
      m_Items.Add(value);

      MoveUp(m_Items.Count - 1);
    }

    /// <summary>
    /// Add (Push) values into heap
    /// </summary>
    /// <param name="values">Values to push into heap</param>
    public void AddRange(IEnumerable<T> values) {
      if (values is null)
        throw new ArgumentNullException(nameof(values));

      foreach (var value in values)
        Add(value);
    }

    /// <summary>
    /// Remove At index
    /// </summary>
    public void RemoveAt(int index) {
      if (index < 0 || index >= m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(index));

      m_Items[index] = m_Items[^1];
      m_Items.RemoveAt(m_Items.Count - 1);

      MoveUp(index);
      MoveDown(index);
    }

    /// <summary>
    /// Remove single value
    /// </summary>
    public bool Remove(T value) {
      int index = IndexOf(value);

      if (index < 0)
        return false;

      m_Items[index] = m_Items[^1];
      m_Items.RemoveAt(m_Items.Count - 1);

      MoveUp(index);
      MoveDown(index);

      return true;
    }

    /// <summary>
    /// Index of value (-1 if value is not found)
    /// </summary>
    /// <param name="value">value to find</param>
    /// <returns>index</returns>
    public int IndexOf(T value) {
      for (int i = m_Items.Count - 1; i >= 0; --i)
        if (Comparer.Compare(value, m_Items[i]) == 0)
          return i;

      return -1;
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      m_Items.Clear();
    }

    /// <summary>
    /// Trim Excess
    /// </summary>
    public void TrimExcess() {
      m_Items.TrimExcess();
    }

    /// <summary>
    /// Consume
    /// </summary>
    public IEnumerable<T> Consume() {
      while (m_Items.Count > 0)
        yield return Pop();
    }

    /// <summary>
    /// Copy
    /// </summary>
    public T[] Copy() {
      return m_Items.ToArray();
    }

    #endregion Public

    #region IEnumerable<T>

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
      return m_Items.GetEnumerator();
    }

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return m_Items.GetEnumerator();
    }

    #endregion IEnumerable<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Min Heap 
  /// </summary>
  /// <typeparam name="T">Heap Item type</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MinHeap<T>
    : BaseHeap<T> {

    #region Algorithm

    /// <summary>
    /// Heapify Move Down
    /// </summary>
    protected override void MoveDown(int index) {
      if (index >= m_Items.Count)
        return;

      int leftIndex = 2 * index + 1;

      // No Left item - do nothing
      if (leftIndex >= m_Items.Count)
        return;

      int rightIndex = leftIndex + 1;

      // Just right index - compare with left and stop 
      if (rightIndex >= m_Items.Count) {
        if (Comparer.Compare(m_Items[index], m_Items[leftIndex]) > 0)
          (m_Items[index], m_Items[leftIndex]) = (m_Items[leftIndex], m_Items[index]);

        return;
      }

      // Both indice exist
      if (Comparer.Compare(m_Items[index], m_Items[leftIndex]) > 0) {
        if (Comparer.Compare(m_Items[leftIndex], m_Items[rightIndex]) > 0) {
          (m_Items[index], m_Items[rightIndex]) = (m_Items[rightIndex], m_Items[index]);

          MoveDown(rightIndex);
        }
        else {
          (m_Items[index], m_Items[leftIndex]) = (m_Items[leftIndex], m_Items[index]);

          MoveDown(leftIndex);
        }
      }
      else if (Comparer.Compare(m_Items[index], m_Items[rightIndex]) > 0) {
        (m_Items[index], m_Items[rightIndex]) = (m_Items[rightIndex], m_Items[index]);

        MoveDown(rightIndex);
      }
    }

    /// <summary>
    /// Heapify Move Up
    /// </summary>
    protected override void MoveUp(int index) {
      if (index <= 0)
        return;

      int topIndex = (index - 1) / 2;

      // Correct order
      if (Comparer.Compare(m_Items[topIndex], m_Items[index]) <= 0)
        return;

      int nextIndex = index % 2 == 0 ? index - 1 : index + 1;

      // No next index: swap top and current only
      if (nextIndex >= m_Items.Count)
        (m_Items[topIndex], m_Items[index]) = (m_Items[index], m_Items[topIndex]);
      else if (Comparer.Compare(m_Items[nextIndex], m_Items[index]) < 0)
        (m_Items[nextIndex], m_Items[topIndex]) = (m_Items[topIndex], m_Items[nextIndex]);
      else
        (m_Items[topIndex], m_Items[index]) = (m_Items[index], m_Items[topIndex]);

      MoveUp(topIndex);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    public MinHeap(IComparer<T> comparer, int capacity)
      : base(comparer, capacity) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    public MinHeap(IComparer<T> comparer)
      : base(comparer, -1) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    public MinHeap(int capacity)
      : base(null, capacity) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public MinHeap()
      : base(null, -1) {
    }

    /// <summary>
    /// Standard constructor 
    /// </summary>
    public MinHeap(IEnumerable<T> source, IComparer<T> comparer) : base(source, comparer, -1) { }

    /// <summary>
    /// Standard constructor 
    /// </summary>
    public MinHeap(IEnumerable<T> source) : base(source, null, -1) { }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Max Heap 
  /// </summary>
  /// <typeparam name="T">Heap Item type</typeparam>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MaxHeap<T>
    : BaseHeap<T> {

    #region Algorithm

    /// <summary>
    /// Heapify Move Down
    /// </summary>
    protected override void MoveDown(int index) {
      if (index >= m_Items.Count)
        return;

      int leftIndex = 2 * index + 1;

      // No Left item - do nothing
      if (leftIndex >= m_Items.Count)
        return;

      int rightIndex = leftIndex + 1;

      // Just right index - compare with left and stop 
      if (rightIndex >= m_Items.Count) {
        if (Comparer.Compare(m_Items[index], m_Items[leftIndex]) < 0)
          (m_Items[index], m_Items[leftIndex]) = (m_Items[leftIndex], m_Items[index]);

        return;
      }

      // Both indice exist
      if (Comparer.Compare(m_Items[index], m_Items[leftIndex]) < 0) {
        if (Comparer.Compare(m_Items[leftIndex], m_Items[rightIndex]) < 0) {
          (m_Items[index], m_Items[rightIndex]) = (m_Items[rightIndex], m_Items[index]);

          MoveDown(rightIndex);
        }
        else {
          (m_Items[index], m_Items[leftIndex]) = (m_Items[leftIndex], m_Items[index]);

          MoveDown(leftIndex);
        }
      }
      else if (Comparer.Compare(m_Items[index], m_Items[rightIndex]) < 0) {
        (m_Items[index], m_Items[rightIndex]) = (m_Items[rightIndex], m_Items[index]);

        MoveDown(rightIndex);
      }
    }

    /// <summary>
    /// Heapify Move Up
    /// </summary>
    protected override void MoveUp(int index) {
      if (index <= 0)
        return;

      int topIndex = (index - 1) / 2;

      // Correct order
      if (Comparer.Compare(m_Items[topIndex], m_Items[index]) >= 0)
        return;

      int nextIndex = index % 2 == 0 ? index - 1 : index + 1;

      // No next index: swap top and current only
      if (nextIndex >= m_Items.Count)
        (m_Items[topIndex], m_Items[index]) = (m_Items[index], m_Items[topIndex]);
      else if (Comparer.Compare(m_Items[nextIndex], m_Items[index]) > 0)
        (m_Items[nextIndex], m_Items[topIndex]) = (m_Items[topIndex], m_Items[nextIndex]);
      else
        (m_Items[topIndex], m_Items[index]) = (m_Items[index], m_Items[topIndex]);

      MoveUp(topIndex);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    public MaxHeap(IComparer<T> comparer, int capacity) : base(comparer, capacity) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    public MaxHeap(IComparer<T> comparer) : base(comparer, -1) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    public MaxHeap(int capacity) : base(null, capacity) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public MaxHeap() : base(null, -1) { }

    /// <summary>
    /// Standard constructor 
    /// </summary>
    public MaxHeap(IEnumerable<T> source, IComparer<T> comparer) : base(source, comparer, 1) { }

    /// <summary>
    /// Standard constructor 
    /// </summary>
    public MaxHeap(IEnumerable<T> source) : base(source, null, 1) { }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Priority Queue (the higher priority the sooner)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class QueueWithPriority<T>
    : IReadOnlyCollection<T> {

    #region Internal Classes 

    private readonly struct QueueItem : IComparable<QueueItem> {
      internal QueueItem(T value, double priority) {
        Value = value;
        Priority = priority;
      }

      public T Value { get; }
      public double Priority { get; }

      public int CompareTo(QueueItem other) {
        return Priority.CompareTo(other.Priority);
      }
    }

    #endregion Internal Classes

    #region Private Data

    private readonly MaxHeap<QueueItem> m_Heap;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="capacity">Expected Capacity</param>
    public QueueWithPriority(int capacity) {
      m_Heap = new MaxHeap<QueueItem>(capacity);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public QueueWithPriority()
      : this(-1) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Enqueue
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="priority">Priority</param>
    public void Enqueue(T value, double priority) {
      m_Heap.Add(new QueueItem(value, priority));
    }

    /// <summary>
    /// Dequeue
    /// </summary>
    public T Dequeue() => m_Heap.Pop().Value;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Heap.Count;

    /// <summary>
    /// Peek
    /// </summary>
    public T Peek() => m_Heap.Peek().Value;

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() => m_Heap.Clear();

    /// <summary>
    /// Try Peek
    /// </summary>
    public bool TryPeek(out T value) {
      if (m_Heap.Count <= 0) {
        value = default;

        return false;
      }

      value = m_Heap.Peek().Value;

      return true;
    }

    /// <summary>
    /// Try Dequeue
    /// </summary>
    public bool TryDequeue(out T value) {
      if (m_Heap.Count <= 0) {
        value = default;

        return false;
      }

      value = m_Heap.Pop().Value;

      return true;
    }

    /// <summary>
    /// Trim Excess
    /// </summary>
    public void TrimExcess() {
      m_Heap.TrimExcess();
    }

    /// <summary>
    /// Consume
    /// </summary>
    public IEnumerable<T> Consume() {
      while (m_Heap.Count > 0)
        yield return m_Heap.Pop().Value;
    }

    #endregion Public

    #region IEnumerable<T>

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => m_Heap.Select(item => item.Value).GetEnumerator();

    /// <summary>
    /// Enumerable
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Heap.Select(item => item.Value).GetEnumerator();

    #endregion IEnumerable<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Priority Queue (the lower priority the sooner)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PriorityQueue<T> {
    #region Private Data

    private readonly MinHeap<T> m_Heap;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public PriorityQueue(IComparer<T> comparer, int capacity) {
      Comparer ??= Comparer<T>.Default
               ?? throw new ArgumentNullException($"Type {typeof(T).Name} doesn't have default comparer.");

      Capacity = capacity < 0 ? -1 : capacity;
      m_Heap = new MinHeap<T>(comparer, capacity);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public PriorityQueue(IComparer<T> comparer) : this(comparer, -1) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public PriorityQueue(int capacity) : this(null, capacity) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public PriorityQueue() : this(null, -1) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IComparer<T> Comparer { get; }

    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Enqueue
    /// </summary>
    public void Enqueue(T value) {
      m_Heap.Add(value);
    }

    /// <summary>
    /// Dequeue
    /// </summary>
    public T Dequeue() => m_Heap.Pop();

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Heap.Count;

    /// <summary>
    /// Peek
    /// </summary>
    public T Peek() => m_Heap.Peek();

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() => m_Heap.Clear();

    /// <summary>
    /// Try Peek
    /// </summary>
    public bool TryPeek(out T value) {
      if (m_Heap.Count <= 0) {
        value = default;

        return false;
      }

      value = m_Heap.Peek();

      return true;
    }

    /// <summary>
    /// Try Dequeue
    /// </summary>
    public bool TryDequeue(out T value) {
      if (m_Heap.Count <= 0) {
        value = default;

        return false;
      }

      value = m_Heap.Pop();

      return true;
    }

    /// <summary>
    /// Remove single value
    /// </summary>
    public bool Remove(T value) {
      return m_Heap.Remove(value);
    }

    /// <summary>
    /// Trim Excess
    /// </summary>
    public void TrimExcess() {
      m_Heap.TrimExcess();
    }

    /// <summary>
    /// Consume
    /// </summary>
    public IEnumerable<T> Consume() {
      while (m_Heap.Count > 0)
        yield return m_Heap.Pop();
    }

    /// <summary>
    /// Copy
    /// </summary>
    public T[] Copy() {
      return m_Heap.Copy();
    }

    #endregion Public
  }

}
