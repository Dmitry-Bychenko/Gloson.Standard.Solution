using System.Collections.Generic;
using System.Linq;

namespace Gloson.Collections.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Sequence Equality Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SequenceEqualityComparer<T>
    : IEqualityComparer<IEnumerable<T>> {

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SequenceEqualityComparer(IEqualityComparer<T> comparer) {
      InnerComparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SequenceEqualityComparer() : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Inner Comparer
    /// </summary>
    public IEqualityComparer<T> InnerComparer { get; }

    #endregion Public

    #region IEqualityComparer<IEnumerable<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IEnumerable<T> x, IEnumerable<T> y) =>
      Enumerable.SequenceEqual(x, y, InnerComparer);

    /// <summary>
    /// Hash Code
    /// </summary>
    public int GetHashCode(IEnumerable<T> obj) {
      if (null == obj)
        return -1;
      else if (obj is IReadOnlyList<T> list)
        return list.Count;
      else if (obj is ICollection<T> collection)
        return collection.Count;
      else if (obj is IReadOnlyCollection<T> rc)
        return rc.Count;
      else if (obj is T[] arr)
        return arr.Length;

      return -2;
    }

    #endregion IEqualityComparer<IEnumerable<T>>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Set Equality Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SetEqualityComparer<T> : IEqualityComparer<IEnumerable<T>> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SetEqualityComparer(IEqualityComparer<T> comparer) {
      InnerComparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public SetEqualityComparer() : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Inner Comparer
    /// </summary>
    public IEqualityComparer<T> InnerComparer { get; }

    #endregion Public

    #region IEqualityComparer<ISet<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IEnumerable<T> x, IEnumerable<T> y) {
      if (ReferenceEquals(x, y))
        return true;
      else if (null == x || null == y)
        return false;

      if (x.Except(y, InnerComparer).Any())
        return false;
      else if (y.Except(x, InnerComparer).Any())
        return false;

      return true;
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public int GetHashCode(IEnumerable<T> obj) {
      if (null == obj)
        return -1;
      else if (obj is IReadOnlyList<T> list)
        return list.Count;
      else if (obj is ICollection<T> collection)
        return collection.Count;
      else if (obj is IReadOnlyCollection<T> rc)
        return rc.Count;
      else if (obj is T[] arr)
        return arr.Length;

      return -2;
    }

    #endregion IEqualityComparer<ISet<T>>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Set Equality Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> {
    #region IEqualityComparer<ISet<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(HashSet<T> x, HashSet<T> y) {
      if (ReferenceEquals(x, y))
        return true;
      else if (null == x || null == y)
        return false;

      return x.SetEquals(y);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public int GetHashCode(HashSet<T> obj) => null == obj ? -1 : obj.Count;

    #endregion IEqualityComparer<ISet<T>>
  }

}
