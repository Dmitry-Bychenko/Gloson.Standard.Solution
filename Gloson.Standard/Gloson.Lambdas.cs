using System;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Func Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class FuncExtensions {
    #region Public

    /// <summary>
    /// Pipeline
    /// </summary>
    public static Func<T, T> Pipeline<T>(this Func<T, T> first, params Func<T, T>[] next) {
      if (null == first)
        throw new ArgumentNullException(nameof(first));
      else if (null == next)
        throw new ArgumentNullException(nameof(first));

      if (next.Length <= 0)
        return first;

      return (T v) => {
        T r = first(v);

        foreach (var f in next)
          r = f(r);

        return r;
      };
    }

    #endregion Public
  }
}
