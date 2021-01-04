using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Prefix Sum Array
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PrefixSumArray : IReadOnlyList<long>, IEquatable<PrefixSumArray> {
    #region Private Data

    private readonly List<long> m_Sums = new List<long>();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor 
    /// </summary>
    public PrefixSumArray(IEnumerable<long> source) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      m_Sums.Add(0);

      long sum = 0;

      foreach (long item in source)
        m_Sums.Add(sum += item);
    }

    #endregion Create

    #region Publuc

    /// <summary>
    /// Items (sums)
    /// </summary>
    public IReadOnlyList<long> Sums => m_Sums;

    /// <summary>
    /// Sum (entire array)
    /// </summary>
    public long Sum() => m_Sums[^1];

    /// <summary>
    /// Sum [0 .. index]
    /// </summary>
    public long Sum(int index) => index >= 0 && index < m_Sums.Count - 1
      ? m_Sums[index + 1]
      : throw new ArgumentOutOfRangeException(nameof(index));

    /// <summary>
    /// Sum [from .. to] 
    /// </summary>
    public long Sum(int from, int to) {
      if (from < 0 || from >= Count || from > to)
        throw new ArgumentOutOfRangeException(nameof(from));
      if (to < 0 || to >= Count)
        throw new ArgumentOutOfRangeException(nameof(to));

      return m_Sums[to + 1] - m_Sums[from];
    }

    #endregion Public

    #region IReadOnlyList<long>

    /// <summary>
    /// Index
    /// </summary>
    public long this[int index] {
      get => (index >= 0 && index < m_Sums.Count)
        ? m_Sums[index + 1] - m_Sums[index]
        : 0;
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Sums.Count - 1;

    /// <summary>
    /// Typed enumerator
    /// </summary>
    public IEnumerator<long> GetEnumerator() {
      IEnumerable<long> Data() {
        for (int i = 1; i < m_Sums.Count; ++i)
          yield return m_Sums[i] - m_Sums[i - 1];
      }

      return Data().GetEnumerator();
    }

    /// <summary>
    /// Typeless Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<long>

    #region IEquatable<SumLongArray>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(PrefixSumArray other) {
      if (ReferenceEquals(this, other))
        return true;
      if (other is null)
        return false;

      return m_Sums.SequenceEqual(other.m_Sums);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as PrefixSumArray);

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode() => unchecked((int)m_Sums[^1]);

    #endregion IEquatable<SumLongArray>
  }

}
