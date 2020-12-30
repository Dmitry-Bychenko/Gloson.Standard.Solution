using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Heap Node
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class MasterHeapNode<T> {
    #region Private Data

    internal int Index { get; set; } = -1;

    #endregion Private Data

    #region Create

    /// <summary>
    /// 
    /// </summary>
    public MasterHeapNode(T value) {
      Value = value;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Heap
    /// </summary>
    public MasterHeap<T> Heap { get; internal set; }

    /// <summary>
    /// Value
    /// </summary>
    public T Value;

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove() {
      if (Heap is null)
        return false;

      Heap.Remove(this);

      return true;
    }

    /// <summary>
    /// Add 
    /// </summary>
    public void Add(MasterHeap<T> heap) {
      if (heap is null)
        throw new ArgumentNullException(nameof(heap));

      heap.Push(this);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Heap With Nodes
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class MasterHeap<T> {
    #region Private Data

    // Items
    protected readonly List<MasterHeapNode<T>> m_Items = new List<MasterHeapNode<T>>();

    #endregion Private Data

    #region Algorithm

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
    protected MasterHeap(IComparer<T> comparer, int capacity) {
      comparer ??= Comparer<T>.Default;

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer), $"No comparer provided when {typeof(T).Name} doesn't provide default one");

      m_Items = capacity < 0
        ? new List<MasterHeapNode<T>>()
        : new List<MasterHeapNode<T>>(capacity);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer (null for default)</param>
    protected MasterHeap(IComparer<T> comparer)
      : this(comparer, -1) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Expected capacity (-1 for default)</param>
    protected MasterHeap(int capacity)
      : this(null, capacity) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected MasterHeap()
      : this(null, -1) {
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
    public IReadOnlyList<MasterHeapNode<T>> Items => m_Items;

    /// <summary>
    /// Take min (max) value without removing it
    /// </summary>
    public MasterHeapNode<T> Peek() => m_Items.Count > 0
      ? m_Items[0]
      : throw new InvalidOperationException("Heap is empty");

    /// <summary>
    /// Try Peek
    /// </summary>
    public bool TryPeek(out MasterHeapNode<T> value) {
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
    public MasterHeapNode<T> Pop() {
      MasterHeapNode<T> result = m_Items.Count > 0
        ? m_Items[0]
        : throw new InvalidOperationException("Heap is empty");

      m_Items[0] = m_Items[^1];
      m_Items[0].Index = 0;

      m_Items.RemoveAt(m_Items.Count - 1);

      MoveDown(0);

      result.Heap = null;
      result.Index = -1;

      return result;
    }

    /// <summary>
    /// Try Pop
    /// </summary>
    /// <param name="value">Min (Max) value or default(T)</param>
    /// <returns>true if value returned</returns>
    public bool TryPop(out MasterHeapNode<T> value) {
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
    public void Push(MasterHeapNode<T> value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (value.Heap is not null)
        if (value.Heap != this && value.Heap is not null)
          value.Heap.Remove(value);

      m_Items.Add(value);
      value.Index = m_Items.Count - 1;

      value.Heap = this;

      MoveUp(m_Items.Count - 1);
    }

    /// <summary>
    /// Push
    /// </summary>
    public MasterHeapNode<T> Add(T value) {
      var node = new MasterHeapNode<T>(value);

      Push(node);

      return node;
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public void AddRange(IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (var item in source)
        Push(new MasterHeapNode<T>(item));
    }

    /// <summary>
    /// Remove 
    /// </summary>
    public bool Remove(MasterHeapNode<T> value) {
      if (null == value)
        return false;

      if (value.Heap != this)
        return false;

      m_Items[value.Index] = m_Items[m_Items.Count - 1];
      m_Items[value.Index].Index = value.Index;
      m_Items.RemoveAt(m_Items.Count - 1);

      if (value.Index != m_Items.Count) {
        MoveUp(value.Index);
        MoveDown(value.Index);
      }

      value.Heap = null;
      value.Index = -1;

      return true;
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      foreach (var item in m_Items) {
        item.Heap = null;
        item.Index = -1;
      }

      m_Items.Clear();
    }

    /// <summary>
    /// Consume
    /// </summary>
    public IEnumerable<MasterHeapNode<T>> Consume() {
      while (m_Items.Count > 0)
        yield return Pop();
    }

    /// <summary>
    /// Trim Excess
    /// </summary>
    public void TrimExcess() {
      m_Items.TrimExcess();
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Min Heap With Nodes
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MasterMaxHeap<T> : MasterHeap<T> {
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
        if (Comparer.Compare(m_Items[index].Value, m_Items[leftIndex].Value) < 0) {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[leftIndex];
          m_Items[leftIndex] = h;

          m_Items[index].Index = index;
          m_Items[leftIndex].Index = leftIndex;
        }

        return;
      }

      // Both indice exist
      if (Comparer.Compare(m_Items[index].Value, m_Items[leftIndex].Value) < 0) {
        if (Comparer.Compare(m_Items[leftIndex].Value, m_Items[rightIndex].Value) < 0) {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[rightIndex];
          m_Items[rightIndex] = h;

          m_Items[index].Index = index;
          m_Items[rightIndex].Index = rightIndex;

          MoveDown(rightIndex);
        }
        else {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[leftIndex];
          m_Items[leftIndex] = h;

          m_Items[index].Index = index;
          m_Items[leftIndex].Index = leftIndex;

          MoveDown(leftIndex);
        }
      }
      else if (Comparer.Compare(m_Items[index].Value, m_Items[rightIndex].Value) < 0) {
        MasterHeapNode<T> h = m_Items[index];
        m_Items[index] = m_Items[rightIndex];
        m_Items[rightIndex] = h;

        m_Items[index].Index = index;
        m_Items[rightIndex].Index = rightIndex;

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
      if (Comparer.Compare(m_Items[topIndex].Value, m_Items[index].Value) >= 0)
        return;

      int nextIndex = index % 2 == 0 ? index - 1 : index + 1;

      // No next index: swap top and current only
      if (nextIndex >= m_Items.Count) {
        MasterHeapNode<T> h = m_Items[topIndex];
        m_Items[topIndex] = m_Items[index];
        m_Items[index] = h;

        m_Items[index].Index = index;
        m_Items[topIndex].Index = topIndex;
      }
      else if (Comparer.Compare(m_Items[nextIndex].Value, m_Items[index].Value) > 0) {
        MasterHeapNode<T> h = m_Items[nextIndex];
        m_Items[topIndex] = m_Items[nextIndex];
        m_Items[nextIndex] = h;

        m_Items[index].Index = index;
        m_Items[nextIndex].Index = nextIndex;
      }
      else {
        MasterHeapNode<T> h = m_Items[topIndex];
        m_Items[topIndex] = m_Items[index];
        m_Items[index] = h;

        m_Items[index].Index = index;
        m_Items[topIndex].Index = topIndex;
      }

      MoveUp(topIndex);
    }

    #endregion Algorithm
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Max Heap With Nodes
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MasterMinHeap<T> : MasterHeap<T> {
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
        if (Comparer.Compare(m_Items[index].Value, m_Items[leftIndex].Value) > 0) {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[leftIndex];
          m_Items[leftIndex] = h;

          m_Items[index].Index = index;
          m_Items[leftIndex].Index = leftIndex;
        }

        return;
      }

      // Both indice exist
      if (Comparer.Compare(m_Items[index].Value, m_Items[leftIndex].Value) > 0) {
        if (Comparer.Compare(m_Items[leftIndex].Value, m_Items[rightIndex].Value) > 0) {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[rightIndex];
          m_Items[rightIndex] = h;

          m_Items[rightIndex].Index = rightIndex;
          m_Items[index].Index = index;

          MoveDown(rightIndex);
        }
        else {
          MasterHeapNode<T> h = m_Items[index];
          m_Items[index] = m_Items[leftIndex];
          m_Items[leftIndex] = h;

          m_Items[leftIndex].Index = leftIndex;
          m_Items[index].Index = index;

          MoveDown(leftIndex);
        }
      }
      else if (Comparer.Compare(m_Items[index].Value, m_Items[rightIndex].Value) > 0) {
        MasterHeapNode<T> h = m_Items[index];
        m_Items[index] = m_Items[rightIndex];
        m_Items[rightIndex] = h;

        m_Items[rightIndex].Index = rightIndex;
        m_Items[index].Index = index;

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
      if (Comparer.Compare(m_Items[topIndex].Value, m_Items[index].Value) <= 0)
        return;

      int nextIndex = index % 2 == 0 ? index - 1 : index + 1;

      // No next index: swap top and current only
      if (nextIndex >= m_Items.Count) {
        MasterHeapNode<T> h = m_Items[topIndex];
        m_Items[topIndex] = m_Items[index];
        m_Items[index] = h;

        m_Items[topIndex].Index = topIndex;
        m_Items[index].Index = index;
      }
      else if (Comparer.Compare(m_Items[nextIndex].Value, m_Items[index].Value) < 0) {
        MasterHeapNode<T> h = m_Items[nextIndex];
        m_Items[topIndex] = m_Items[nextIndex];
        m_Items[nextIndex] = h;

        m_Items[topIndex].Index = topIndex;
        m_Items[nextIndex].Index = nextIndex;
      }
      else {
        MasterHeapNode<T> h = m_Items[topIndex];
        m_Items[topIndex] = m_Items[index];
        m_Items[index] = h;

        m_Items[topIndex].Index = topIndex;
        m_Items[index].Index = index;
      }

      MoveUp(topIndex);
    }

    #endregion Algorithm
  }

}
