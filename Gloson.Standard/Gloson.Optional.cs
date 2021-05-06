using System;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Optional
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Optional<T>
    : IEquatable<Optional<T>>,
      IEquatable<T> {

    #region Private Data

    // Value
    private readonly T m_Value;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="exists">Exists</param>
    public Optional(T value, bool exists) {
      Exists = exists;
      m_Value = exists ? value : default;
    }

    /// <summary>
    /// Standard constructor (value exists)
    /// </summary>
    /// <param name="value">Value</param>
    public Optional(T value) : this(value, true) { }
    /// <summary>
    /// Standard constructor (not existing value)
    /// </summary>
    public Optional() : this(default, false) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public T Value => Exists
      ? m_Value
      : throw new InvalidOperationException("Value doesn't exist");

    /// <summary>
    /// If Value Exists
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return Exists
        ? $"Existsing {m_Value?.ToString()}"
        : $"Unexisting {typeof(T).Name}";
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Optional<T> left, Optional<T> right) {
      if (object.ReferenceEquals(left, right))
        return true;
      else if (left is null)
        return false;
      else if (right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Optional<T> left, Optional<T> right) {
      if (object.ReferenceEquals(left, right))
        return false;
      else if (left is null)
        return true;
      else if (right is null)
        return true;

      return !left.Equals(right);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Optional<T> left, T right) {
      if (left is null || !left.Exists)
        return false;

      return Object.Equals(left.m_Value, right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Optional<T> left, T right) {
      if (left is null || !left.Exists)
        return true;

      return !Object.Equals(left.m_Value, right);
    }

    /// <summary>
    /// From value
    /// </summary>
    public static implicit operator Optional<T>(T value) => new(value);

    #endregion Operators

    #region IEquatable<T>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Optional<T> other) {
      if (other is null)
        return false;

      if (other.Exists != Exists)
        return false;

      return !Exists || object.Equals(m_Value, other.m_Value);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(T other) {
      if (other is null)
        return false;

      if (!Exists)
        return false;

      return object.Equals(m_Value, other);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object o) {
      if (o is Optional<T> other)
        return Equals(other);
      else if (o is T otherItem)
        return Equals(otherItem);
      else
        return false;
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return
          !Exists ? int.MaxValue
        : m_Value is null ? 0
        : m_Value.GetHashCode();
    }

    #endregion IEquatable<T>
  }

}
