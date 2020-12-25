using System;
using System.Collections.Generic;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerator Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class EnumeratorExtensions {
    #region Public

    /// <summary>
    /// AsEnumerable 
    /// </summary>
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator) {
      if (enumerator is null)
        throw new ArgumentNullException(nameof(enumerator));

      using (enumerator) {
        while (enumerator.MoveNext())
          yield return enumerator.Current;
      }
    }

    /// <summary>
    /// AsEnumerable 
    /// </summary>
    public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator, bool keepOpen) {
      if (enumerator is null)
        throw new ArgumentNullException(nameof(enumerator));

      if (keepOpen)
        while (enumerator.MoveNext())
          yield return enumerator.Current;
      else
        using (enumerator) {
          while (enumerator.MoveNext())
            yield return enumerator.Current;
        }
    }

    #endregion Public
  }
}
