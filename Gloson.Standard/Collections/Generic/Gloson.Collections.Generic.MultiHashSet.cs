using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// MultiHashSet collection
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class MultiHashSet<T>
    : IEquatable<MultiHashSet<T>>,
      IEnumerable<T> {

    #region Private Data

    // Items
    private readonly Dictionary<T, long> m_Items;

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer</param>
    public MultiHashSet(IEqualityComparer<T> comparer) {
      if (comparer is null)
        comparer = EqualityComparer<T>.Default;

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer),
          $"No default equality comparer is found for {typeof(T).Name}.");

      m_Items = new Dictionary<T, long>(Comparer);
    }

    /// <summary>
    /// Standard constructor with default comparer
    /// </summary>
    public MultiHashSet() : this(comparer: null) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="comparer">Comparer</param>
    public MultiHashSet(IEnumerable<T> source, IEqualityComparer<T> comparer) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer),
          $"No default equality comparer is found for {typeof(T).Name}.");

      m_Items = new Dictionary<T, long>(Comparer);

      foreach (var item in source)
        if (m_Items.TryGetValue(item, out long count))
          m_Items[item] = count + 1;
        else
          m_Items.Add(item, 1);
    }

    /// <summary>
    /// Standard constructor from source
    /// </summary>
    /// <param name="source">Source</param>
    public MultiHashSet(IEnumerable<T> source) : this(source, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Items (item and its count)
    /// </summary>
    public IReadOnlyDictionary<T, long> Items => m_Items;

    /// <summary>
    /// All Items
    /// </summary>
    public IEnumerable<T> AllItems {
      get {
        foreach (var pair in m_Items) {
          for (long i = 0; i < pair.Value; ++i)
            yield return pair.Key;
        }
      }
    }

    /// <summary>
    /// Count (distinct items)
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Total count
    /// </summary>
    public long AllCount => m_Items.Sum(pair => pair.Value);

    /// <summary>
    /// Counts
    /// </summary>
    public long this[T item] {
      get {
        return m_Items.TryGetValue(item, out long count) ? count : 0;
      }
      set {
        if (value < 0)
          throw new ArgumentOutOfRangeException(nameof(value));

        if (m_Items.TryGetValue(item, out long prior)) {
          if (value == 0)
            m_Items.Remove(item);
          else if (value != prior)
            m_Items[item] = value;
        }
        else {
          if (value > 0)
            m_Items.Add(item, value);
        }
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public long Add(T value) {
      if (m_Items.TryGetValue(value, out long count)) {
        count += 1;

        m_Items[value] = count;

        return count;
      }
      else {
        m_Items.Add(value, 1);

        return 1;
      }
    }

    /// <summary>
    /// Add items
    /// </summary>
    public long Add(T value, long count) {
      if (0 == count)
        return 0;

      if (m_Items.TryGetValue(value, out long actual)) {
        actual += count;

        if (actual <= 0) {
          m_Items.Remove(value);

          return 0;
        }
        else
          m_Items[value] = actual;

        return actual;
      }
      else if (count <= 0)
        return 0;
      else {
        m_Items.Add(value, count);

        return count;
      }
    }

    /// <summary>
    /// Add range
    /// </summary>
    /// <param name="source">Source</param>
    public void AddRange(IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (var item in source)
        if (m_Items.TryGetValue(item, out long count))
          m_Items[item] = count + 1;
        else
          m_Items.Add(item, 1);
    }

    /// <summary>
    /// Remove Item
    /// </summary>
    public long Remove(T value) {
      if (m_Items.TryGetValue(value, out long count)) {
        count -= 1;

        if (count == 0)
          m_Items.Remove(value);
        else
          m_Items[value] = count;

        return count;
      }

      return 0;
    }

    /// <summary>
    /// Remove Items
    /// </summary>
    public long Remove(T value, long count) {
      if (0 == count)
        return 0;

      if (m_Items.TryGetValue(value, out long actual)) {
        actual -= count;

        if (actual <= 0) {
          m_Items.Remove(value);

          return 0;
        }
        else
          m_Items[value] = actual;

        return actual;
      }
      else if (count >= 0)
        return 0;
      else {
        m_Items.Add(value, -count);

        return count;
      }
    }

    /// <summary>
    /// Remove Range
    /// </summary>
    public void RemoveRange(IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (var item in source)
        if (m_Items.TryGetValue(item, out long count)) {
          if (count <= 1)
            m_Items.Remove(item);
          else
            m_Items[item] = count - 1;
        }
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      m_Items.Clear();
    }

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(T item) => m_Items.ContainsKey(item);

    /// <summary>
    /// Intersect with
    /// </summary>
    public void IntersectWith(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      var list = m_Items.ToList();

      foreach (var item in list) {
        long count = Math.Min(item.Value, other[item.Key]);

        if (count <= 0)
          m_Items.Remove(item.Key);
        else
          m_Items[item.Key] = count;
      }
    }

    /// <summary>
    /// Except with
    /// </summary>
    public void ExceptWith(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      foreach (var pair in other.m_Items) {
        if (m_Items.TryGetValue(pair.Key, out long value)) {
          value -= pair.Value;

          if (value <= 0)
            m_Items.Remove(pair.Key);
          else
            m_Items[pair.Key] = value;
        }
      }
    }

    /// <summary>
    /// Symmetric Except with
    /// </summary>
    public void SymmetricExceptWith(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      foreach (var pair in other.m_Items) {
        if (m_Items.TryGetValue(pair.Key, out long value)) {
          value -= pair.Value;

          if (value == 0)
            m_Items.Remove(pair.Key);
          else
            m_Items[pair.Key] = Math.Abs(value);
        }
        else
          m_Items.Add(pair.Key, pair.Value);
      }
    }

    /// <summary>
    /// Union with
    /// </summary>
    public void UnionWith(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      foreach (var pair in other.m_Items)
        if (m_Items.TryGetValue(pair.Key, out long value))
          m_Items[pair.Key] = value + pair.Value;
        else
          m_Items.Add(pair.Key, pair.Value);
    }

    /// <summary>
    /// If subset of
    /// </summary>
    public bool IsSubsetOf(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      foreach (var pair in m_Items)
        if (other[pair.Key] < pair.Value)
          return false;

      return true;
    }

    /// <summary>
    /// If superset of
    /// </summary>
    public bool IsSupersetOf(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      return other.IsSubsetOf(this);
    }

    /// <summary>
    /// If proper subset of
    /// </summary>
    public bool IsProperSubsetOf(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      bool proper = false;

      foreach (var pair in m_Items) {
        long currentCount = pair.Value;
        long otherCount = other[pair.Key];

        if (otherCount < currentCount)
          return false;
        else if (otherCount > currentCount)
          proper = true;
      }

      return proper;
    }

    /// <summary>
    /// If proper superset of
    /// </summary>
    public bool IsProperSupersetOf(MultiHashSet<T> other) {
      if (other is null)
        throw new ArgumentNullException(nameof(other));

      return other.IsProperSubsetOf(this);
    }

    #endregion Public

    #region IEquatable<MultiSet<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(MultiHashSet<T> other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      if (m_Items.Count != other.m_Items.Count)
        return false;

      foreach (var pair in m_Items)
        if (!m_Items.TryGetValue(pair.Key, out long otherCount) || otherCount != pair.Value)
          return false;

      return true;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as MultiHashSet<T>);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => m_Items.Count;

    #endregion IEquatable<MultiSet<T>>

    #region IEnumerable<T>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<T> GetEnumerator() => AllItems.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IEnumerable<T>
  }

  //-----------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// MultiHashSet Extensions
  /// </summary>
  //
  //-----------------------------------------------------------------------------------------------

  public static partial class MultiHashSetExtensions {
    #region Public

    /// <summary>
    /// Intersect
    /// </summary>
    public static MultiHashSet<T> Intersect<T>(this MultiHashSet<T> left, MultiHashSet<T> right) {
      if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (right is null)
        throw new ArgumentNullException(nameof(right));

      MultiHashSet<T> result = new MultiHashSet<T>(left.Comparer);

      foreach (var leftPair in left.Items) {
        long rightCount = right[leftPair.Key];

        if (rightCount > 0)
          result.Add(leftPair.Key, Math.Min(leftPair.Value, rightCount));
      }

      return result;
    }

    /// <summary>
    /// Except
    /// </summary>
    public static MultiHashSet<T> Except<T>(this MultiHashSet<T> left, MultiHashSet<T> right) {
      if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (right is null)
        throw new ArgumentNullException(nameof(right));

      MultiHashSet<T> result = new MultiHashSet<T>(left.Comparer);

      foreach (var leftPair in left.Items) {
        long count = leftPair.Value - right[leftPair.Key];

        if (count > 0)
          result.Add(leftPair.Key, Math.Min(leftPair.Value, count));
      }

      return result;
    }

    /// <summary>
    /// Symmetric Except
    /// </summary>
    public static MultiHashSet<T> SymmetricExcept<T>(this MultiHashSet<T> left, MultiHashSet<T> right) {
      if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (right is null)
        throw new ArgumentNullException(nameof(right));

      MultiHashSet<T> result = new MultiHashSet<T>(left.Comparer);

      foreach (var leftPair in left.Items) {
        long count = leftPair.Value - right[leftPair.Key];

        if (count != 0)
          result.Add(leftPair.Key, Math.Min(leftPair.Value, Math.Abs(count)));
      }

      foreach (var rightPair in right.Items)
        if (!left.Contains(rightPair.Key))
          result.Add(rightPair.Key, rightPair.Value);

      return result;
    }

    /// <summary>
    /// Union
    /// </summary>
    public static MultiHashSet<T> Union<T>(this MultiHashSet<T> left, MultiHashSet<T> right) {
      if (left is null)
        throw new ArgumentNullException(nameof(left));
      else if (right is null)
        throw new ArgumentNullException(nameof(right));

      MultiHashSet<T> result = new MultiHashSet<T>(left.Comparer);

      foreach (var leftPair in left.Items) {
        long count = leftPair.Value + right[leftPair.Key];

        result.Add(leftPair.Key, Math.Min(leftPair.Value, count));
      }

      foreach (var rightPair in right.Items)
        if (!left.Contains(rightPair.Key))
          result.Add(rightPair.Key, rightPair.Value);

      return result;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To Multiset
    /// </summary>
    public static MultiHashSet<T> ToMultiHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer) =>
      new MultiHashSet<T>(source, comparer);

    /// <summary>
    /// To Multiset
    /// </summary>
    public static MultiHashSet<T> ToMultiHashSet<T>(this IEnumerable<T> source) =>
      new MultiHashSet<T>(source, null);

    /// <summary>
    /// If sequencies are equal as multisets
    /// </summary>
    public static bool MultiSetEqual<T>(this IEnumerable<T> source,
                                             IEnumerable<T> other,
                                             IEqualityComparer<T> comparer) {
      if (ReferenceEquals(source, other))
        return true;
      else if (source is null)
        return false;
      else if (other is null)
        return false;

      var left = new MultiHashSet<T>(source, comparer);
      var right = new MultiHashSet<T>(other, comparer);

      return left.Equals(right);
    }

    /// <summary>
    /// If sequencies are equal as multisets
    /// </summary>
    public static bool MultiSetEqual<T>(this IEnumerable<T> source,
                                             IEnumerable<T> other) =>
      MultiSetEqual(source, other, null);

    #endregion Public
  }
}
