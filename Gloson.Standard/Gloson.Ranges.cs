using System;
using System.Collections;
using System.Collections.Generic;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Range Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RangeExtensions {
    #region Inner Classes

    /// <summary>
    /// Range Enumerator
    /// </summary>
    public struct RangeEnumerator : IEnumerator<int> {
      #region Private Data

      private long m_Current;

      #endregion Private Data

      #region Create

      /// <summary>
      /// Standard Constructor
      /// </summary>
      /// <param name="range">Range To Enumerate</param>
      /// <exception cref="ArgumentException">When either Start or End are count from End</exception>
      public RangeEnumerator(Range range) {
        Range = range;

        m_Current = !range.End.IsFromEnd && !range.Start.IsFromEnd
          ? (long)(range.Start.Value) - 1
          : throw new ArgumentException("IsFromEnd bounds are not supported", nameof(range));
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Range to enumerate
      /// </summary>
      public Range Range { get; }

      #endregion Public

      #region IEnumerator<int>

      /// <summary>
      /// Current
      /// </summary>
      public int Current => m_Current >= Range.Start.Value
        ? (int)m_Current
        : throw new InvalidOperationException("Current is not defined");

      /// <summary>
      /// Typeless Current
      /// </summary>
      object IEnumerator.Current => Current;

      /// <summary>
      /// Dispose (do nothing)
      /// </summary>
      public void Dispose() { }

      /// <summary>
      /// Move Next
      /// </summary>
      public bool MoveNext() {
        if (m_Current >= Range.End.Value)
          return false;

        m_Current += 1;

        return true;
      }

      /// <summary>
      /// Reset
      /// </summary>
      public void Reset() {
        m_Current = (long)(Range.Start.Value) - 1;
      }

      #endregion IEnumerator<int>
    }

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Get Enumerator
    /// </summary>
    /// <param name="range">Range to get enumerator from</param>
    public static RangeEnumerator GetEnumerator(this Range range) => new (range);

    /// <summary>
    /// As Enumerable
    /// </summary>
    /// <param name="range"Range to Enumerate></param>
    public static IEnumerable<int> AsEnumerable(this Range range) {
      foreach (int item in range)
        yield return item;
    }

    #endregion Public
  }

}
