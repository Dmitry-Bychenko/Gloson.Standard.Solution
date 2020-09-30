using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Union Find  
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Disjoint-set_data_structure"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class UnionFind<T>
    : IReadOnlyList<IReadOnlyCollection<T>>,
      IEquatable<UnionFind<T>> {

    #region Internal Classes

    private class OrderedComparerClass : IEqualityComparer<UnionFind<T>> {
      /// <summary>
      /// Equals 
      /// </summary>
      public bool Equals(UnionFind<T> x, UnionFind<T> y) {
        if (ReferenceEquals(x, y))
          return true;
        else if (null == x || null == y)
          return false;

        if (x.ItemsComparer != y.ItemsComparer)
          return false;

        if (x.m_Items.Count != y.m_Items.Count)
          return false;

        for (int i = 0; i < x.m_Items.Count; ++i)
          if (!x.m_Items[i].SetEquals(y.m_Items[i]))
            return false;

        return true;
      }

      /// <summary>
      /// Hash Code
      /// </summary>
      public int GetHashCode(UnionFind<T> obj) => obj == null ? 0 : obj.Count;
    }

    private class UnOrderedComparerClass : IEqualityComparer<UnionFind<T>> {
      /// <summary>
      /// Equals 
      /// </summary>
      public bool Equals(UnionFind<T> x, UnionFind<T> y) {
        if (ReferenceEquals(x, y))
          return true;
        else if (null == x || null == y)
          return false;

        if (x.ItemsComparer != y.ItemsComparer)
          return false;

        if (x.m_Items.Count != y.m_Items.Count)
          return false;

        var data = x.m_Items
          .GroupBy(item => item.Count)
          .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var item in y.m_Items) {
          if (!data.TryGetValue(item.Count, out var list))
            return false;

          if (!list.Any(hs => hs.SetEquals(item)))
            return false;
        }

        return true;
      }

      /// <summary>
      /// Hash Code
      /// </summary>
      public int GetHashCode(UnionFind<T> obj) => obj == null ? 0 : obj.Count;
    }

    #endregion Internal Classes

    #region Private Data

    private readonly List<HashSet<T>> m_Items;

    #endregion Private Data 

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UnionFind(IEqualityComparer<T> itemComparer, int count) {
      ItemsComparer = itemComparer ?? EqualityComparer<T>.Default;
      m_Items = count <= 0 ? new List<HashSet<T>>() : new List<HashSet<T>>(count);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UnionFind(IEqualityComparer<T> itemComparer) : this(itemComparer, 0) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UnionFind(int count) : this(null, count) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public UnionFind() : this(null, 0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<T> ItemsComparer { get; }

    /// <summary>
    /// Ordered comparer
    /// </summary>
    public static IEqualityComparer<UnionFind<T>> OrderedComparer { get; } = new OrderedComparerClass();

    /// <summary>
    /// UnOrdered comparer
    /// </summary>
    public static IEqualityComparer<UnionFind<T>> UnOrderedComparer { get; } = new UnOrderedComparerClass();

    /// <summary>
    /// Try Union
    /// </summary>
    /// <param name="left">left index to union</param>
    /// <param name="right">right index to union</param>
    public bool TryUnion(int left, int right) {
      if (left < 0 || right < 0 || left >= m_Items.Count || right >= m_Items.Count)
        return false;
      else if (left == right)
        return true;

      if (left < right)
        m_Items[left].UnionWith(m_Items[right]);
      else
        m_Items[right].UnionWith(m_Items[left]);

      m_Items.RemoveAt(Math.Max(left, right));

      return true;
    }

    /// <summary>
    /// Union
    /// </summary>
    /// <param name="left">left index to union</param>
    /// <param name="right">right index to union</param>
    public void Union(int left, int right) {
      if (left < 0 || left >= m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(left));
      else if (right < 0 || right >= m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(right));
      else if (left == right)
        return;

      if (left < right)
        m_Items[left].UnionWith(m_Items[right]);
      else
        m_Items[right].UnionWith(m_Items[left]);

      m_Items.RemoveAt(Math.Max(left, right));
    }

    /// <summary>
    /// Find index which contains Value
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <return>Index or -1</return>
    public int Find(T value) {
      for (int i = m_Items.Count - 1; i >= 0; --i)
        if (m_Items[i].Contains(value))
          return i;

      return -1;
    }

    /// <summary>
    /// Find index which contains Value or create a new index if Value is not found
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <return>Index</return>
    public int FindOrCreate(T value) {
      for (int i = m_Items.Count - 1; i >= 0; --i)
        if (m_Items[i].Contains(value))
          return i;

      m_Items.Add(new HashSet<T>(ItemsComparer) { value });

      return m_Items.Count - 1;
    }

    /// <summary>
    /// Try adding pair; pair is added if and only if both left and right are NOT in the same set
    /// </summary>
    /// <param name="left">left item</param>
    /// <param name="right">right item</param>
    /// <param name="index">index of set the pair has been added</param>
    /// <returns>true if pair is added, false otherwise</returns>
    public bool TryAddPair(T left, T right, out int index) {
      int leftIndex = Find(left);
      int rightIndex = Find(right);

      if (leftIndex >= 0 && rightIndex >= 0) {
        index = Math.Min(leftIndex, rightIndex);

        if (leftIndex == rightIndex)
          return false;

        Union(leftIndex, rightIndex);

        return true;
      }
      else if (leftIndex >= 0) {
        index = leftIndex;

        m_Items[index].Add(left);

        return true;
      }
      else if (rightIndex >= 0) {
        index = rightIndex;

        m_Items[index].Add(left);

        return true;
      }

      m_Items.Add(new HashSet<T>(ItemsComparer) { left, right });

      index = m_Items.Count - 1;

      return true;
    }

    /// <summary>
    /// To String (debug) 
    /// </summary>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();

      int length = (m_Items.Count - 1).ToString().Length;

      for (int i = 0; i < m_Items.Count; ++i) {
        if (sb.Length > 0)
          sb.AppendLine();

        sb.Append("set #");
        sb.Append(i.ToString().PadLeft(length));
        sb.Append(" : {");
        sb.Append(string.Join(", ", m_Items[i]));
        sb.Append("}");
      }

      return sb.ToString();
    }

    #endregion Public

    #region IReadOnlyList<ReadOnlyCollection<T>>

    /// <summary>
    /// Count 
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Set by it's index
    /// </summary>
    /// <param name="index">index</param>
    /// <returns>Set as read only colelction</returns>
    public IReadOnlyCollection<T> this[int index] {
      get {
        if (index < 0 || index >= m_Items.Count)
          throw new ArgumentOutOfRangeException(nameof(index));

        return m_Items[index];
      }
    }

    /// <summary>
    /// Typed Enumerator
    /// </summary>
    public IEnumerator<IReadOnlyCollection<T>> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Typeless enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<ReadOnlyCollection<T>>

    #region IEquatable<UnionFind<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(UnionFind<T> other) => UnOrderedComparer.Equals(this, other);

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object o) => Equals(o as UnionFind<T>);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => UnOrderedComparer.GetHashCode(this);

    #endregion IEquatable<UnionFind<T>>
  }

}
