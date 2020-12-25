using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Comparer Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ComparerBuilder {
    #region Internal Classes

    private sealed class EmptyComparer<T> : IComparer<T> {
      public int Compare(T x, T y) => 0;
    }

    #endregion Internal Classes

    #region Public

    /// <summary>
    /// From Func
    /// </summary>
    public static IComparer<T> FromFunc<T>(Func<T, T, int> compare) {
      if (compare is null)
        throw new ArgumentNullException(nameof(compare));

      return Comparer<T>.Create((x, y) => compare(x, y));
    }

    /// <summary>
    /// Nulls First
    /// </summary>
    public static IComparer<T> NullsFirst<T>() {
      return Comparer<T>.Create((x, y) => {
        if (ReferenceEquals(x, y))
          return 0;
        else if (x is null)
          return -1;
        else if (y is null)
          return 1;

        return 0;
      });
    }

    /// <summary>
    /// Nulls Last
    /// </summary>
    public static IComparer<T> NullsLast<T>() {
      return Comparer<T>.Create((x, y) => {
        if (ReferenceEquals(x, y))
          return 0;
        else if (x is null)
          return 1;
        else if (y is null)
          return -1;

        return 0;
      });
    }

    /// <summary>
    /// Empty (all are equal to one another)
    /// </summary>
    public static IComparer<T> Empty<T>() => new EmptyComparer<T>();

    /// <summary>
    /// Default
    /// </summary>
    public static IComparer<T> Default<T>() => Comparer<T>.Default;

    /// <summary>
    /// Default (Required)
    /// </summary>
    public static IComparer<T> DefaultRequired<T>() {
      IComparer<T> result = Comparer<T>.Default;

      if (result is null)
        throw new InvalidOperationException($"Type {typeof(T).Name} doesn't have any default Comparer.");

      return result;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Comparer Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ComparerExtensions {
    #region Inner Classes

    private class ReversedComparer<T> : IComparer<T> {
      private readonly IComparer<T> m_Comparer;

      public ReversedComparer(IComparer<T> comparer) {
        m_Comparer = comparer;
      }

      public int Compare(T x, T y) {
        int result = m_Comparer.Compare(x, y);

        if (result < 0)
          return 1;
        else if (result > 0)
          return -1;
        else
          return 0;
      }
    }

    private class ThenByComparer<T> : IComparer<T> {
      private readonly IComparer<T> m_First;
      private readonly IComparer<T> m_Next;

      public ThenByComparer(IComparer<T> first, IComparer<T> next) {
        m_First = first;
        m_Next = next;
      }

      public int Compare(T x, T y) {
        int result = m_First.Compare(x, y);

        return result != 0
          ? result
          : m_Next.Compare(x, y);
      }
    }

    private class ThenByDescendingComparer<T> : IComparer<T> {
      private readonly IComparer<T> m_First;
      private readonly IComparer<T> m_Next;

      public ThenByDescendingComparer(IComparer<T> first, IComparer<T> next) {
        m_First = first;
        m_Next = next;
      }

      public int Compare(T x, T y) {
        int result = m_First.Compare(x, y);

        if (result != 0)
          return result;

        result = m_Next.Compare(x, y);

        if (result < 0)
          return 1;
        else if (result > 0)
          return -1;
        else
          return 0;
      }
    }

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Reverse
    /// </summary>
    public static IComparer<T> Reverse<T>(this IComparer<T> comparer) {
      if (comparer is null)
        throw new ArgumentNullException(nameof(comparer));

      return new ReversedComparer<T>(comparer);
    }

    /// <summary>
    /// Then By
    /// </summary>
    public static IComparer<T> ThenBy<T>(this IComparer<T> comparer, IComparer<T> nextComparer) {
      if (comparer is null)
        throw new ArgumentNullException(nameof(comparer));
      else if (nextComparer is null)
        throw new ArgumentNullException(nameof(nextComparer));

      if (comparer == nextComparer)
        return comparer;

      return new ThenByComparer<T>(comparer, nextComparer);
    }

    /// <summary>
    /// Then By Descending
    /// </summary>
    public static IComparer<T> ThenByDescending<T>(this IComparer<T> comparer, IComparer<T> nextComparer) {
      if (comparer is null)
        throw new ArgumentNullException(nameof(comparer));
      else if (nextComparer is null)
        throw new ArgumentNullException(nameof(nextComparer));

      if (comparer == nextComparer)
        return comparer;

      return new ThenByDescendingComparer<T>(comparer, nextComparer);
    }

    #endregion Public
  }

}
