using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-----------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Circular Buffer Interface  
  /// </summary>
  /// <typeparam name="T">Item type</typeparam>
  //
  //-----------------------------------------------------------------------------------------------------------------

  public interface ICircularBuffer<T> : IReadOnlyList<T> {
    /// <summary>
    /// Add item into Circular buffer
    /// </summary>
    /// <param name="item"></param>
    void Add(T item);

    /// <summary>
    /// Clear Circular Buffer
    /// </summary>
    void Clear();

    /// <summary>
    /// Circular buffer capacity
    /// </summary>
    int Capacity { get; }

    /// <summary>
    /// Add Range of items into the circular buffer
    /// </summary>
    /// <param name="values">Values to Add</param>
    /// <exception cref="ArgumentNullException">When values is null</exception>
    public void AddRange(IEnumerable<T> values) {
      if (values is null)
        throw new ArgumentNullException(nameof(values));

      foreach (var item in values)
        Add(item);
    }
  }

  //-----------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Circular Buffer (Array based) 
  /// </summary>
  /// <typeparam name="T">Item type</typeparam>
  //
  //-----------------------------------------------------------------------------------------------------------------

  public sealed class CircularBuffer<T> : ICircularBuffer<T> {
    #region Private Data

    private readonly T[] m_Items;

    #endregion Private Data

    #region Algorithm

    // Offset, where to write the next item
    internal int Offset { get; private set; }

    // Where To Start Enumeration when reading items
    internal int StartAt => Count == Capacity ? Offset % Capacity : 0;

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="capacity">Capacity, must be positive</param>
    /// <exception cref="ArgumentOutOfRangeException">When capacity is not positive</exception>
    public CircularBuffer(int capacity) {
      if (capacity <= 0)
        throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} must be positive");

      m_Items = new T[capacity];
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Capacity, must be positive</param>
    /// <param name="items">Items to add to the buffer</param>
    public CircularBuffer(int capacity, IEnumerable<T> items)
        : this(capacity) {
      (this as ICircularBuffer<T>).AddRange(items);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="items">Items to be create buffer from; must not be </param>
    /// <exception cref="ArgumentNullException">When items is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">When items is empty</exception>
    public CircularBuffer(IEnumerable<T> items) {
      if (items is null)
        throw new ArgumentNullException(nameof(items));

      m_Items = items.ToArray();

      if (m_Items.Length == 0)
        throw new ArgumentOutOfRangeException(nameof(items), "Empty collections are not allowed");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Add an item to the buffer
    /// </summary>
    /// <param name="value">Value to Add</param>
    public void Add(T value) {
      m_Items[Offset] = value;

      Count = Math.Clamp(Count + 1, 0, Capacity);
      Offset = (Offset + 1) % Capacity;
    }

    /// <summary>
    /// Clear All Items
    /// </summary>
    public void Clear() {
      Count = 0;
      Offset = 0;
    }

    /// <summary>
    /// Circular Buffer Capacity
    /// </summary>
    public int Capacity => m_Items.Length;

    #endregion Public

    #region IReadOnlyList<T>

    /// <summary>
    /// Count (Number of Items which are in the Circular Buffer) 
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="index">Item index (0 is the head of the buffer)</param>
    /// <returns>Value by its index</returns>
    /// <exception cref="ArgumentOutOfRangeException">When index is out of [0 .. Count - 1] range</exception>
    public T this[int index] {
      get {
        int i = (index % Capacity + StartAt + Capacity) % Capacity;

        if (i >= Count)
          throw new ArgumentOutOfRangeException(nameof(index));

        return m_Items[i];
      }
    }

    /// <summary>
    /// Typed Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() {
      for (int i = 0; i < Count; ++i)
        yield return m_Items[(i % Capacity + StartAt) % Capacity];
    }

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<T>
  }

  //-----------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Circular Buffer (Linked List based)
  /// </summary>
  /// <typeparam name="T">Item type</typeparam>
  //
  //-----------------------------------------------------------------------------------------------------------------

  public sealed class CircularBufferLinked<T> : ICircularBuffer<T> {
    #region Private Data

    private readonly LinkedList<T> m_Items = new();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="capacity">Capacity, must be positive</param>
    /// <exception cref="ArgumentOutOfRangeException">When capacity is not positive</exception>
    public CircularBufferLinked(int capacity) {
      if (capacity <= 0)
        throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(capacity)} must be positive");

      Capacity = capacity;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="capacity">Capacity, must be positive</param>
    /// <param name="items">Items to add to the buffer</param>
    public CircularBufferLinked(int capacity, IEnumerable<T> items)
        : this(capacity) {
      (this as ICircularBuffer<T>).AddRange(items);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="items">Items to be create buffer from; must not be </param>
    /// <exception cref="ArgumentNullException">When items is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">When items is empty</exception>
    public CircularBufferLinked(IEnumerable<T> items) {
      if (items is null)
        throw new ArgumentNullException(nameof(items));

      foreach (var item in items)
        m_Items.AddLast(item);

      if (m_Items.Count == 0)
        throw new ArgumentOutOfRangeException(nameof(items), "Empty collections are not allowed");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Add an item to the buffer
    /// </summary>
    /// <param name="value">Value to Add</param>
    public void Add(T value) {
      m_Items.AddLast(value);

      if (m_Items.Count > Capacity)
        m_Items.RemoveFirst();
    }

    /// <summary>
    /// Clear All Items
    /// </summary>
    public void Clear() => m_Items.Clear();

    #endregion Public

    #region IReadOnlyList<T>

    /// <summary>
    /// Count (Number of Items which are in the Circular Buffer) 
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Indexer
    /// </summary>
    /// <param name="index">Item index (0 is the head of the buffer)</param>
    /// <returns>Value by its index</returns>
    /// <exception cref="ArgumentOutOfRangeException">When index is out of [0 .. Count - 1] range</exception>
    public T this[int index] {
      get {
        int at = (index % Capacity + Capacity) % Capacity;

        if (at >= Count)
          throw new ArgumentOutOfRangeException(nameof(index));

        var node = m_Items.First!;

        for (int i = 0; i < at; ++i)
          node = node.Next!;

        return node.Value;
      }
    }

    /// <summary>
    /// Typed Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<T>
  }

}
