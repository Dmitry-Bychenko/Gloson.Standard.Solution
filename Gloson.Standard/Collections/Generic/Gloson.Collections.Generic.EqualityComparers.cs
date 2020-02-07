using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Equality Comparer Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EqualityComparerBuilder {
    #region Internal Classes

    private sealed class EqualityComparerFunc<T> : IEqualityComparer<T> {
      private Func<T, T, bool> m_Equals;
      private Func<T, int> m_Hash;

      public EqualityComparerFunc(Func<T, T, bool> equals, Func<T, int> hash) {
        m_Equals = equals;
        m_Hash = hash;
      }

      public bool Equals(T x, T y) {
        if (ReferenceEquals(x, y))
          return true;
        else if (ReferenceEquals(null, y))
          return false;
        else if (ReferenceEquals(x, null))
          return false;

        return m_Equals(x, y);
      }

      public int GetHashCode(T obj) {
        if (ReferenceEquals(null, obj))
          return 0;

        return m_Hash(obj);
      }
    }

    private sealed class EqualityComparerEmpty<T> : IEqualityComparer<T> {
      public bool Equals(T x, T y) => true;

      public int GetHashCode(T obj) => 0;
    }

    #endregion Internal Classes

    #region Public

    /// <summary>
    /// From Funcs
    /// </summary>
    /// <param name="equals">Equals</param>
    /// <param name="hash">Hash</param>
    /// <returns></returns>
    public static IEqualityComparer<T> FromFunc<T>(Func<T, T, bool> equals, Func<T, int> hash) {
      if (null == equals)
        throw new ArgumentNullException(nameof(equals));
      else if (null == hash)
        throw new ArgumentNullException(nameof(hash));

      return new EqualityComparerFunc<T>(equals, hash);
    }

    /// <summary>
    /// Empty
    /// </summary>
    public static IEqualityComparer<T> Empty<T>() => new EqualityComparerEmpty<T>();

    /// <summary>
    /// Default
    /// </summary>
    public static IEqualityComparer<T> Default<T>() => EqualityComparer<T>.Default;

    /// <summary>
    /// Default (Required)
    /// </summary>
    public static IEqualityComparer<T> DefaultRequired<T>() {
      IEqualityComparer<T> result = EqualityComparer<T>.Default;

      if (null == result)
        throw new InvalidOperationException($"Type {typeof(T).Name} doesn't have any default Equality Comparer.");

      return result;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Equality Comparer Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EqualityComparerExtensions {
    #region Inner Classes

    public sealed class EqualityComparerCombined<T> : IEqualityComparer<T> {
      #region Private Data

      private List<IEqualityComparer<T>> m_Items = new List<IEqualityComparer<T>>();

      #endregion Private Data

      #region Create

      /// <summary>
      /// Standard Constructor
      /// </summary>
      public EqualityComparerCombined(IEnumerable<IEqualityComparer<T>> comparers) {
        if (null == comparers)
          throw new ArgumentNullException(nameof(comparers));

        m_Items = comparers
          .Select(item => item ?? EqualityComparerBuilder.DefaultRequired<T>())
          .Distinct()
          .ToList();
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Items (inner comparers)
      /// </summary>
      public IReadOnlyList<IEqualityComparer<T>> Items => m_Items;

      #endregion Public

      #region IEqualityComparer<T>

      /// <summary>
      /// Comparison
      /// </summary>
      public bool Equals(T x, T y) {
        foreach (var comparer in m_Items)
          if (!comparer.Equals(x, y))
            return false;

        return true;
      }

      /// <summary>
      /// Hash Code
      /// </summary>
      public int GetHashCode(T obj) {
        unchecked {
          int result = 0;

          foreach (var comparer in m_Items)
            result = comparer.GetHashCode(obj) * 17 + 23 * result;

          return result;
        }
      }

      #endregion IEqualityComparer<T>
    }

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Combine Equality Comparers
    /// </summary>
    public static IEqualityComparer<T> ThenBy<T>(this IEqualityComparer<T> master, params IEqualityComparer<T>[] others) {
      if (null == master)
        master = EqualityComparerBuilder.DefaultRequired<T>();

      if (null == others)
        throw new ArgumentNullException(nameof(others));

      if (others.Length <= 0)
        return master;

      return new EqualityComparerCombined<T>(new IEqualityComparer<T>[] { master}.Concat(others));
    }

    #endregion Public

  }

}
