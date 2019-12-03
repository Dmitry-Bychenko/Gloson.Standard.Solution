using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// Symmetric Except
    /// </summary>
    public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> source, 
                                                         IEnumerable<T> other, 
                                                         IEqualityComparer<T> comparer) {
      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, other))
        throw new ArgumentNullException(nameof(other));

      if (ReferenceEquals(null, comparer))
        comparer = EqualityComparer<T>.Default;

      if (ReferenceEquals(null, comparer))
        throw new ArgumentNullException(nameof(comparer), 
          $"{typeof(T).Name} doesn't provide default IEqualityComparer<{typeof(T).Name}>");

      HashSet<T> hs = new HashSet<T>(source, comparer);

      hs.SymmetricExceptWith(other);

      foreach (T item in hs)
        yield return item;
    }

    /// <summary>
    /// Symmetric Except
    /// </summary>
    public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> source, IEnumerable<T> other) =>
      SymmetricExcept(source, other, null);

    #endregion Public
  }
}
