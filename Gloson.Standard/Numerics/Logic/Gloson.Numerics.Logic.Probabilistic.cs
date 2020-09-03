using System;
using System.Globalization;

namespace Gloson.Numerics.Logic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Probablilistic Logical Value 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct ProbabilisticLogical
    : IComparable<ProbabilisticLogical>,
      IEquatable<ProbabilisticLogical> {

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ProbabilisticLogical(double value) {
      Value = value >= 0 && value <= 1.0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="value"></param>
    public ProbabilisticLogical(bool value) : this(value ? 1 : 0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare 
    /// </summary>
    public static int Compare(ProbabilisticLogical left, ProbabilisticLogical right) =>
      left.Value.CompareTo(right.Value);

    /// <summary>
    /// Value
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Is True
    /// </summary>
    public bool IsTrue => Value == 1;

    /// <summary>
    /// Is False
    /// </summary>
    public bool IsFalse => Value == 0;


    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
        Value == 0 ? "False (0)"
      : Value == 1 ? "True (1)"
      : $"Intermediary {Value.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Not
    /// </summary>
    public ProbabilisticLogical Not() => new ProbabilisticLogical(1 - Value);

    /// <summary>
    /// And
    /// </summary>
    public ProbabilisticLogical And(ProbabilisticLogical other) => new ProbabilisticLogical(Value * other.Value);

    /// <summary>
    /// Or
    /// </summary>
    public ProbabilisticLogical Or(ProbabilisticLogical other) =>
      new ProbabilisticLogical(Value + other.Value - Value * other.Value);

    /// <summary>
    /// Equality
    /// </summary>
    public ProbabilisticLogical Equ(ProbabilisticLogical other) {
      double x = Value;
      double y = other.Value;

      return new ProbabilisticLogical(x * y + (1 - x) * (1 - y) - x * y * (1 - x) * (1 - y));
    }

    /// <summary>
    /// Exclusive Or
    /// </summary>
    public ProbabilisticLogical Xor(ProbabilisticLogical other) {
      double x = Value;
      double y = other.Value;

      return new ProbabilisticLogical(1 - x * y + (1 - x) * (1 - y) + x * y * (1 - x) * (1 - y));
    }

    /// <summary>
    /// Implication 
    /// </summary>
    public ProbabilisticLogical Imp(ProbabilisticLogical other) =>
      new ProbabilisticLogical(1 - other.Value * (Value - 1));

    #endregion Public

    #region Operators

    #region Cast

    /// <summary>
    /// From Boolean
    /// </summary>
    public static implicit operator ProbabilisticLogical(bool value) => new ProbabilisticLogical(value);

    #endregion Cast

    #region Comparison

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) != 0;

    /// <summary>
    /// More Or Equal
    /// </summary>
    public static bool operator >=(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less Or Equal
    /// </summary>
    public static bool operator <=(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) <= 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(ProbabilisticLogical left, ProbabilisticLogical right) => Compare(left, right) < 0;


    #endregion Comparison 

    #region Arithmetics

    /// <summary>
    /// Not 
    /// </summary>
    public static ProbabilisticLogical operator ~(ProbabilisticLogical value) => value.Not();

    /// <summary>
    /// And 
    /// </summary>
    public static ProbabilisticLogical operator &(ProbabilisticLogical left, ProbabilisticLogical right)
      => left.And(right);

    /// <summary>
    /// Or 
    /// </summary>
    public static ProbabilisticLogical operator |(ProbabilisticLogical left, ProbabilisticLogical right)
      => left.Or(right);

    /// <summary>
    /// Xor 
    /// </summary>
    public static ProbabilisticLogical operator ^(ProbabilisticLogical left, ProbabilisticLogical right)
      => left.Xor(right);

    #endregion Arithmetics

    #endregion Operators

    #region IComparable<ProbabilisticLogical>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(ProbabilisticLogical other) => Compare(this, other);

    #endregion IComparable<ProbabilisticLogical>

    #region IEquatable<ProbabilisticLogical>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ProbabilisticLogical other) => Value == other.Value;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => obj is ProbabilisticLogical other && other.Value == Value;

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    #endregion IEquatable<ProbabilisticLogical>
  }

}
