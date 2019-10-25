using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gloson;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SlidingWindowChangedEventArgs<T> : EventArgs {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public SlidingWindowChangedEventArgs(
      SlidingWindow<T> window,
      Optional<T> current,
      Optional<T> currentPrevious,
      Optional<T> itemAdded,
      Optional<T> itemRemoved,
      bool completed,
      bool completedPrevious) {

      Window = window;
      Current = current;
      CurrentPrevious = currentPrevious;
      ItemAdded = itemAdded;
      ItemRemoved = itemRemoved;
      Completed = completed;
      CompletedPrevious = completedPrevious;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Sliding window itself
    /// </summary>
    public SlidingWindow<T> Window { get; }

    /// <summary>
    /// Current Item
    /// </summary>
    public Optional<T> Current { get; }

    /// <summary>
    /// Current Item before changing
    /// </summary>
    public Optional<T> CurrentPrevious { get; }

    /// <summary>
    /// Item added
    /// </summary>
    public Optional<T> ItemAdded { get; }

    /// <summary>
    /// Item removed
    /// </summary>
    public Optional<T> ItemRemoved { get; }

    /// <summary>
    /// If Sliding Window Completed
    /// </summary>
    public bool Completed { get; }

    /// <summary>
    /// If Sliding Window was Completed before changing
    /// </summary>
    public bool CompletedPrevious { get; }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Sliding Window
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class SlidingWindow<T> 
    : IEnumerable<T>,
      IReadOnlyList<T> {

    #region Private Data

    // Circular buffer
    private readonly List<T> m_Buffer;
    // Current index
    private int m_InsertIndex = 0;
    // Deleted count
    private int m_Deleted = 0;

    #endregion Private Data

    #region Algorithm

    /// <summary>
    /// Enqueue
    /// </summary>
    protected internal void Enqueue(T value) {
      EventHandler<SlidingWindowChangedEventArgs<T>> changed = Changed;

      Optional<T> currentPrevious = m_Buffer.Count > 0
        ? new Optional<T>(Current)
        : new Optional<T>();

      bool wasCompleted = IsCompleted;

      Optional<T> itemRemoved = new Optional<T>();

      if (m_Buffer.Count < m_Buffer.Capacity) {
        m_Buffer.Add(value);
        itemRemoved = new Optional<T>();
      }
      else {
        itemRemoved = new Optional<T>(m_Buffer[m_InsertIndex]);
        m_Buffer[m_InsertIndex] = value;
      }

      m_InsertIndex = (m_InsertIndex + 1) % m_Buffer.Capacity;

      if (changed != null) {
        SlidingWindowChangedEventArgs<T> args = new SlidingWindowChangedEventArgs<T>(
          this,
          new Optional<T>(Current),
          currentPrevious,
          new Optional<T>(value),
          itemRemoved,
          IsCompleted,
          wasCompleted
        );

        changed(this, args);
      }
    }

    /// <summary>
    /// Dequeue if possible
    /// </summary>
    protected internal bool Dequeue() {
      if (AfterCount <= 0)
        return false;

      bool wasCompleted = IsCompleted;

      Optional<T> currentPrevious = m_Buffer.Count > 0
        ? new Optional<T>(Current)
        : new Optional<T>();

      Optional<T> itemRemoved = new Optional<T>(this[-BeforeCount]);

      m_Deleted += 1;

      EventHandler<SlidingWindowChangedEventArgs<T>> changed = Changed;

      if (changed != null) {
        SlidingWindowChangedEventArgs<T> args = new SlidingWindowChangedEventArgs<T>(
          this,
          new Optional<T>(Current),
          currentPrevious,
          new Optional<T>(),
          itemRemoved,
          IsCompleted,
          wasCompleted
        );

        changed(this, args);
      }

      return true;
    }

    /// <summary>
    /// Current Index
    /// </summary>
    protected int CurrentIndex {
      get {
        if (m_Buffer.Count < m_Buffer.Capacity)
          if (m_Buffer.Count <= AfterCapacity + 1)
            return 0;
          else
            return m_Buffer.Count - (AfterCapacity + 1) + m_Deleted;
        else
          return (m_InsertIndex + BeforeCapacity + m_Deleted) % m_Buffer.Count;
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="beforeCapacity">Before Count</param>
    /// <param name="afterCapacity">After Count</param>
    public SlidingWindow(int beforeCapacity, int afterCapacity) {
      if (beforeCapacity < 0)
        throw new ArgumentOutOfRangeException(nameof(beforeCapacity), $"{beforeCapacity} must be non-negative.");
      else if (afterCapacity < 0)
        throw new ArgumentOutOfRangeException(nameof(afterCapacity), $"{afterCapacity} must be non-negative.");

      BeforeCapacity = beforeCapacity;
      AfterCapacity = afterCapacity;

      m_Buffer = new List<T>(BeforeCapacity + AfterCapacity + 1);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Current
    /// </summary>
    public T Current {
      get {
        if (m_Buffer.Count <= 0)
          throw new InvalidOperationException("Sliding window is empty");

        int index = CurrentIndex;

        return m_Buffer[index];
      }
    }

    /// <summary>
    /// Before Count
    /// </summary>
    public int BeforeCapacity { get; }

    /// <summary>
    /// Before Count
    /// </summary>
    public int BeforeCount {
      get {
        if (m_Buffer.Count <= AfterCapacity + 1)
          return 0;
        else
          return m_Buffer.Count - (AfterCapacity + 1);
      }
    }

    /// <summary>
    /// Before Count
    /// </summary>
    public int AfterCapacity { get; }

    /// <summary>
    /// After Count
    /// </summary>
    public int AfterCount {
      get {
        if (m_Buffer.Count <= 1)
          return 0;
        else if (m_Buffer.Count <= AfterCapacity + 1)
          return m_Buffer.Count - 1;
        else
          return AfterCapacity - m_Deleted;
      }
    }

    /// <summary>
    /// Capacity
    /// </summary>
    public int Capacity => BeforeCapacity + AfterCapacity + 1;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Buffer.Count;

    /// <summary>
    /// Is window completed
    /// </summary>
    public bool IsCompleted => Capacity == Count && m_Deleted == 0;

    /// <summary>
    /// Indexer [-BeforeCapacity..AfterCount], 0 is current item 
    /// </summary>
    public T this[int index] {
      get {
        if (index > AfterCount)
          throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} is over AfterCount");
        else if (index < -BeforeCapacity)
          throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} if under BeforeCount");

        if (index == 0 && m_Buffer.Count <= 0)
          throw new ArgumentOutOfRangeException(nameof(index), $"Window is empty");

        return m_Buffer[(CurrentIndex + index + m_Buffer.Count) % m_Buffer.Count];
      }
    }

    /// <summary>
    /// Items
    /// </summary>
    public IEnumerable<T> AllItems() {
      int index = CurrentIndex - BeforeCount + m_Buffer.Count;

      for (int i = 0; i < m_Buffer.Count - m_Deleted; ++i)
        yield return m_Buffer[(i + index) % m_Buffer.Count];
    }

    /// <summary>
    /// Items before (backward) 
    /// </summary>
    /// <param name="includeCurrent">Should current item be included</param>
    /// <returns></returns>
    public IEnumerable<T> BeforeItems(bool includeCurrent) {
      if (m_Buffer.Count <= 0)
        yield break;

      if (includeCurrent)
        yield return Current;

      for (int i = 1; i <= BeforeCount; ++i)
        yield return this[-i];
    }

    /// <summary>
    /// Items before (backward) 
    /// </summary>
    public IEnumerable<T> BeforeItems() => BeforeItems(false);

    /// <summary>
    /// Items after (forward) 
    /// </summary>
    /// <param name="includeCurrent">Should current item be included</param>
    /// <returns></returns>
    public IEnumerable<T> AfterItems(bool includeCurrent) {
      if (m_Buffer.Count <= 0)
        yield break;

      if (includeCurrent)
        yield return Current;

      for (int i = 1; i <= AfterCount; ++i)
        yield return this[i];
    }

    /// <summary>
    /// Items after (forward)
    /// </summary>
    public IEnumerable<T> AfterItems() => AfterItems(false);

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (IsCompleted)
        return $"Before: {BeforeCount}; After: {AfterCount}";

      StringBuilder sb = new StringBuilder();

      sb.Append($"Before: { BeforeCount}");

      if (BeforeCount < BeforeCapacity)
        sb.Append("(expected: { BeforeCapacity})");

      sb.Append("; ");

      sb.Append($"After: {AfterCount}");

      if (AfterCount < BeforeCapacity)
        sb.Append($"(expected: { BeforeCapacity})");

      return sb.ToString();
    }

    /// <summary>
    /// Changed
    /// </summary>
    public event EventHandler<SlidingWindowChangedEventArgs<T>> Changed;

    #endregion Public

    #region IEnumerable<T>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() => AllItems().GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => AllItems().GetEnumerator();

    #endregion IEnumerable<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Sliding Window
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Sliding windows
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source</param>
    /// <param name="beforeCapacity">Before Capacity</param>
    /// <param name="afterCapacity">After Capacity</param>
    /// <returns>Sliding Window</returns>
    public static IEnumerable<SlidingWindow<T>> SlidingWindow<T>(this IEnumerable<T> source,
                                                                 int beforeCapacity,
                                                                 int afterCapacity,
                                                                 bool completedOnly,
                                                                 EventHandler<SlidingWindowChangedEventArgs<T>> changed) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (beforeCapacity < 0)
        throw new ArgumentOutOfRangeException(nameof(beforeCapacity));
      else if (afterCapacity < 0)
        throw new ArgumentOutOfRangeException(nameof(afterCapacity));

      SlidingWindow<T> window = new SlidingWindow<T>(beforeCapacity, afterCapacity);

      if (null != changed)
        window.Changed += changed;

      try {
        foreach (T item in source) {
          window.Enqueue(item);

          if (completedOnly && window.IsCompleted)
            yield return window;
        }

        while (window.Dequeue())
          if (completedOnly && window.IsCompleted)
            yield return window;
      }
      finally {
        if (null != changed)
          window.Changed -= changed;
      }
    }

    /// <summary>
    /// Sliding windows
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source</param>
    /// <param name="beforeCapacity">Before Capacity</param>
    /// <param name="afterCapacity">After Capacity</param>
    /// <returns>Sliding Window</returns>
    public static IEnumerable<SlidingWindow<T>> SlidingWindow<T>(this IEnumerable<T> source,
                                                                 int beforeCapacity,
                                                                 int afterCapacity, 
                                                                 bool completedOnly) {
      return SlidingWindow(source, beforeCapacity, afterCapacity, completedOnly, null);
    }

    /// <summary>
    /// Sliding windows
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">Source</param>
    /// <param name="beforeCapacity">Before Capacity</param>
    /// <param name="afterCapacity">After Capacity</param>
    /// <returns>Sliding Window</returns>
    public static IEnumerable<SlidingWindow<T>> SlidingWindow<T>(this IEnumerable<T> source,
                                                                 int beforeCapacity,
                                                                 int afterCapacity) {
      return SlidingWindow(source, beforeCapacity, afterCapacity, false, null);
    }

    #endregion Public
  }

}
