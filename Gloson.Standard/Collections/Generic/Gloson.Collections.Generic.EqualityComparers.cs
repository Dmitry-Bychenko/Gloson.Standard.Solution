using System;
using System.Collections.Generic;
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

    #endregion Public
  }

}
