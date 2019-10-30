using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Suffix Array
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SuffixArray<T> 
    : IEnumerable<IEnumerable<T>> {

    #region Private Data

    private List<T> m_Items;

    private int[] m_Indexes;

    #endregion Private Data

    #region Algorithm

    private int CompareIndexes(int left, int right) {
      if (left == right)
        return 0;

      int n = Math.Min(m_Items.Count - left, m_Items.Count - right);

      for (int i = 0; i < n; ++i) {
        int compare = Comparer.Compare(m_Items[left + i], m_Items[right + i]);

        if (compare != 0)
          return compare;
      }

      return left < right ? -1 : 1; 
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source</param>
    /// <param name="comparer">Comparer</param>
    public SuffixArray(IEnumerable<T> source, IComparer<T> comparer) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      if (null == comparer)
        comparer = Comparer<T>.Default;

      if (null == comparer)
        throw new ArgumentNullException(nameof(comparer), 
          $"No comparer provided for {typeof(T).Name}");

      Comparer = comparer;

      m_Items = source.ToList();

      m_Indexes = new int[m_Items.Count];

      for (int i = m_Items.Count - 1; i >= 0; --i)
        m_Indexes[i] = i;

      Array.Sort(m_Indexes, (left, right) => CompareIndexes(left, right));
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source</param>
    public SuffixArray(IEnumerable<T> source)
      : this(source, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Source
    /// </summary>
    public IReadOnlyList<T> Source => m_Items;

    /// <summary>
    /// Index
    /// </summary>
    /// <param name="index"></param>
    public int Index(int index) {
      if (index < 0 || index >= m_Indexes.Length)
        throw new ArgumentOutOfRangeException(nameof(index));

      return m_Indexes[index];
    }

    /// <summary>
    /// Suffix
    /// </summary>
    public IEnumerable<T> Suffix(int index) {
      if (index < 0 || index >= m_Indexes.Length)
        throw new ArgumentOutOfRangeException(nameof(index));

      return m_Items.Skip(m_Indexes[index]);
    }

    /// <summary>
    /// Comparer
    /// </summary>
    public IComparer<T> Comparer { get; }

    /// <summary>
    /// Count
    /// </summary>
    public int Count {
      get {
        return m_Indexes.Length;
      }
    }

    #endregion Public

    #region IEnumerable<IEnumerable<T>>

    /// <summary>
    /// Suffixes
    /// </summary>
    public IEnumerator<IEnumerable<T>> GetEnumerator() {
      foreach (int index in m_Indexes)
        yield return m_Items.Skip(m_Indexes[index]);
    }

    /// <summary>
    /// Enumerator (suffixes)
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IEnumerable<IEnumerable<T>>
  }
}
