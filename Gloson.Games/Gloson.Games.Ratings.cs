using System;

namespace Gloson.Games {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Elo Rating
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Elo_rating_system"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EloRating : IEquatable<EloRating>, IComparable<EloRating> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public EloRating(int value) {
      Value = value > 0 && value < 1_000_000
        ? value
        : throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Standard Constructor, default novice rating
    /// </summary>
    public EloRating()
      : this(800) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(EloRating left, EloRating right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;

      return left.Value.CompareTo(right.Value);
    }

    /// <summary>
    /// Value
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Category
    /// </summary>
    public int Category {
      get {
        int v = (Value - 2250) / 25 + (Value % 25 == 0 ? 0 : 1);

        if (v <= 0)
          return 0;

        return v;
      }
    }

    /// <summary>
    /// Expected points
    /// </summary>
    /// <param name="opponentRating">Opponent Rating</param>
    /// <returns>Points (share) 0..1</returns>
    public double Expected(EloRating opponentRating) {
      if (null == opponentRating)
        throw new ArgumentNullException(nameof(opponentRating));

      return 1.0 / (1.0 + Math.Pow(10, (opponentRating.Value - Value) / 400.0));
    }

    /// <summary>
    /// Updated Rating
    /// </summary>
    /// <param name="opponentRating">Opponent rating</param>
    /// <param name="taken">Taken points</param>
    /// <param name="available">Available points</param>
    /// <param name="coefficient">Coefficient</param>
    public EloRating Update(EloRating opponentRating, double taken, double available, int coefficient) {
      if (null == opponentRating)
        throw new ArgumentNullException(nameof(opponentRating));
      else if (taken < 0)
        throw new ArgumentOutOfRangeException(nameof(taken));
      else if (available < 0)
        throw new ArgumentOutOfRangeException(nameof(available));
      else if (available < taken)
        throw new ArgumentOutOfRangeException(nameof(taken));
      else if (coefficient < 1 || coefficient > 100)
        throw new ArgumentOutOfRangeException(nameof(coefficient));

      int value = (int)(Value + coefficient * (taken - Expected(opponentRating) * available) + 0.5);

      return new EloRating(value);
    }

    /// <summary>
    /// Updated Rating
    /// </summary>
    /// <param name="opponentRating">Opponent rating</param>
    /// <param name="taken">Taken points</param>
    /// <param name="available">Available points</param>
    public EloRating Update(EloRating opponentRating, double taken, double available) {
      if (null == opponentRating)
        throw new ArgumentNullException(nameof(opponentRating));

      int v = Math.Min(Value, opponentRating.Value);

      int coef =
        v > 2400 ? 10 :
        v > 2200 ? 20 : 40;

      return Update(opponentRating, taken, available, coef);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Value.ToString();

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal 
    /// </summary>
    public static bool operator ==(EloRating left, EloRating right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equal 
    /// </summary>
    public static bool operator !=(EloRating left, EloRating right) => Compare(left, right) != 0;

    /// <summary>
    /// More or Equal 
    /// </summary>
    public static bool operator >=(EloRating left, EloRating right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less or Equal 
    /// </summary>
    public static bool operator <=(EloRating left, EloRating right) => Compare(left, right) <= 0;

    /// <summary>
    /// More 
    /// </summary>
    public static bool operator >(EloRating left, EloRating right) => Compare(left, right) > 0;

    /// <summary>
    /// Less 
    /// </summary>
    public static bool operator <(EloRating left, EloRating right) => Compare(left, right) < 0;

    #endregion Operators

    #region IEquatable<EloRating>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(EloRating other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Value == other.Value;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as EloRating);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Value;

    #endregion IEquatable<EloRating>

    #region IComparable<EloRating>

    /// <summary>
    /// Compare To 
    /// </summary>
    public int CompareTo(EloRating other) => Compare(this, other);

    #endregion IComparable<EloRating>
  }

}
