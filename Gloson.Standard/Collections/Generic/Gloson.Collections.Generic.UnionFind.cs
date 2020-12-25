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

    private readonly Dictionary<T, Tuple<T, int>> m_Items;

    private int m_CountMax = 1;

    private T m_IdMax = default;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public UnionFind(IEqualityComparer<T> comparer) {
      Comparer = comparer ?? EqualityComparer<T>.Default;

      m_Items = new Dictionary<T, Tuple<T, int>>(Comparer);
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
    /// Total Count
    /// </summary>
    public int CountTotal => m_Items.Count;

    /// <summary>
    /// Count Max (number of items in the largest disjoint set)
    /// </summary>
    public int CountMax => m_CountMax;

    /// <summary>
    /// Id Max (Id of the largest disjoint set)
    /// </summary>
    public T IdMax => m_IdMax;

    /// <summary>
    /// Id
    /// </summary>
    public T Id(T value) {
      T result = value;

      List<T> path = null;

      while (true) {
        if (!m_Items.TryGetValue(result, out var next))
          return result;
        else if (Comparer.Equals(next.Item1, result)) {
          if (path is not null) {
            int size = m_Items[result].Item2;

            foreach (T node in path)
              m_Items[node] = new Tuple<T, int>(result, size);
          }

          return result;
        }

        if (path is null)
          path = new List<T>();

        path.Add(result);

        result = next.Item1;
      }
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count(T value) {
      T id = Id(value);

      return m_Items.TryGetValue(id, out var rec) ? rec.Item2 : 1;
    }

    /// <summary>
    /// Find (if left and right are in the same disjoint set)
    /// </summary>
    public bool Find(T left, T right) =>
      ReferenceEquals(left, right) || Comparer.Equals(Id(left), Id(right));

    /// <summary>
    /// Union
    /// </summary>
    public bool Union(T left, T right) {
      if (Find(left, right))
        return false;

      if (!m_Items.TryGetValue(left, out var leftRec))
        m_Items.Add(left, leftRec = new Tuple<T, int>(left, 1));

      if (!m_Items.TryGetValue(right, out var rightRec))
        m_Items.Add(right, rightRec = new Tuple<T, int>(right, 1));

      T idLeft = Id(left);
      T idRight = Id(right);

      if (Comparer.Equals(idLeft, idRight))
        return false;

      if (leftRec.Item2 < rightRec.Item2) {
        m_Items[idLeft] = new Tuple<T, int>(idRight, leftRec.Item2 + rightRec.Item2);
        m_Items[idRight] = new Tuple<T, int>(idRight, leftRec.Item2 + rightRec.Item2);
      }
      else {
        m_Items[idRight] = new Tuple<T, int>(idLeft, leftRec.Item2 + rightRec.Item2);
        m_Items[idLeft] = new Tuple<T, int>(idLeft, leftRec.Item2 + rightRec.Item2);
      }

      int count = Count(idRight);

      if (count >= m_CountMax) {
        m_CountMax = count;
        m_IdMax = idRight;
      }

      return true;
    }

    #endregion Public
  }

}
