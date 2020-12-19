using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Union Find
  /// </summary>
  /// <see cref="https://www.cs.princeton.edu/~rs/AlgsDS07/01UnionFind.pdf"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class UnionFind<T> {
    #region Private Data

    private readonly Dictionary<T, (T root, int size)> m_Items;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public UnionFind(IEqualityComparer<T> comparer) {
      Comparer = comparer ?? EqualityComparer<T>.Default;

      m_Items = new Dictionary<T, (T root, int size)>(Comparer);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public UnionFind()
      : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyDictionary<T, (T root, int size)> Items => m_Items;

    /// <summary>
    /// Id
    /// </summary>
    public T Id(T value) {
      T result = value;

      List<T> path = null;

      while (true) {
        if (!m_Items.TryGetValue(result, out var next))
          return result;
        else if (Comparer.Equals(next.root, result)) {
          if (null != path) {
            int size = m_Items[result].size;

            foreach (T node in path)
              m_Items[node] = (result, size);
          }

          return result;
        }

        if (null == path)
          path = new List<T>();

        path.Add(result);

        result = next.root;
      }
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count(T value) {
      T id = Id(value);

      return m_Items.TryGetValue(id, out var rec) ? rec.size : 1;
    }

    /// <summary>
    /// Find (if left and right are in the same disjoint set)
    /// </summary>
    public bool Find(T left, T right) =>
      ReferenceEquals(left, right) || Comparer.Equals(Id(left), Id(right));

    /// <summary>
    /// Union
    /// </summary>
    public void Union(T left, T right) {
      if (Find(left, right))
        return;

      if (!m_Items.TryGetValue(left, out (T root, int size) leftRec))
        m_Items.Add(left, leftRec = (left, 1));

      if (!m_Items.TryGetValue(right, out (T root, int size) rightRec))
        m_Items.Add(right, rightRec = (right, 1));

      T id;

      if (leftRec.size < rightRec.size) {
        id = Id(right);

        m_Items[left] = (id, leftRec.size + rightRec.size);
      }
      else {
        id = Id(left);

        m_Items[right] = (Id(left), leftRec.size + rightRec.size);
      }

      m_Items[id] = (id, leftRec.size + rightRec.size);
    }

    #endregion Public
  }

}
