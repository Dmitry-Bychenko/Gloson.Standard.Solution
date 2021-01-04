using System;
using System.Collections.Generic;
using System.Numerics;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Big Rational Number
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct BigRational : IEquatable<BigRational>, IComparable<BigRational> {
    #region Create

    /// <summary>
    /// Standard rational constructor
    /// </summary>
    /// <param name="numerator">numerator</param>
    /// <param name="denominator">denominator</param>
    public BigRational(BigInteger numerator, BigInteger denominator) {
      if (numerator < 0 && denominator < 0) {
        numerator = -numerator;
        denominator = -denominator;
      }
      else if (numerator == 0) {
        if (denominator != 0)
          denominator = 1;
      }
      else if (denominator == 0) {
        if (numerator < 0)
          numerator = -1;
        else
          numerator = 1;
      }
      else if (denominator < 0) {
        numerator = -numerator;
        denominator = -denominator;
      }

      if (denominator == 0) {
        Numerator = numerator;
        Denominator = denominator;
      }
      else {
        var gcd = denominator.Gcd(numerator < 0 ? -numerator : numerator);

        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
      }
    }

    /// <summary>
    /// Standard rational constuctor (from integer)
    /// </summary>
    /// <param name="value">integer value</param>
    public BigRational(BigInteger value) : this(value, 1) { }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out BigRational result) {
      result = BigRational.NaN;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      value = value.Trim();

      if ("NaN".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = BigRational.NaN;

        return true;
      }
      else if ("+Inf".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = BigRational.PositiveInfinity;

        return true;
      }
      else if ("-Inf".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = BigRational.NegativeInfinity;

        return true;
      }

      string[] parts = value.Split(new char[] { '/', '\\', ':' });

      if (parts.Length > 2)
        return false;

      if (parts.Length == 1) {
        if (BigInteger.TryParse(value, out BigInteger v)) {
          result = new BigRational(v);

          return true;
        }
        else
          return false;
      }

      if (BigInteger.TryParse(parts[0], out BigInteger a) &&
          BigInteger.TryParse(parts[1], out BigInteger b)) {
        result = new BigRational(a, b);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static BigRational Parse(string value) => TryParse(value, out var result)
      ? result
      : throw new FormatException("Not a valid fraction");

    /// <summary>
    /// Zero
    /// </summary>
    public static BigRational Zero => new BigRational(0, 1);

    /// <summary>
    /// One
    /// </summary>
    public static BigRational One => new BigRational(1, 1);

    /// <summary>
    /// NaN
    /// </summary>
    public static BigRational NaN => new BigRational(0, 0);

    /// <summary>
    /// Positive infinity
    /// </summary>
    public static BigRational PositiveInfinity => new BigRational(1, 0);

    /// <summary>
    /// Negative infinity
    /// </summary>
    public static BigRational NegativeInfinity => new BigRational(-1, 0);

    #endregion Create

    #region Public

    /// <summary>
    /// Farey sequence
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Farey_sequence"/>
    public static IEnumerable<BigRational> Farey(BigInteger n) {
      if (n <= 0)
        throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)} must be positive");

      BigInteger a = 0;
      BigInteger b = 1;
      BigInteger c = 1;
      BigInteger d = n;

      yield return new BigRational(a, b);
      yield return new BigRational(c, d);

      if (n == 1)
        yield break;

      while (true) {
        BigInteger z = (n + b) / d;

        BigInteger p = z * c - a;
        BigInteger q = z * d - b;

        yield return new BigRational(p, q);

        if (p == 1 && q == 1)
          break;

        a = c;
        b = d;
        c = p;
        d = q;
      }
    }

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(BigRational left, BigRational right) {
      var value = left.Numerator * right.Denominator - left.Denominator * right.Numerator;

      if (value < 0)
        return -1;
      else if (value > 0)
        return 1;
      else
        return 0;
    }

    /// <summary>
    /// Numerator
    /// </summary>
    public BigInteger Numerator { get; }

    /// <summary>
    /// Denominator
    /// </summary>
    public BigInteger Denominator { get; }

    /// <summary>
    /// Is NaN
    /// </summary>
    public bool IsNaN => Numerator == 0 && Denominator == 0;

    /// <summary>
    /// Is Infinity (either positive or negative)
    /// </summary>
    public bool IsInfinity => Numerator != 0 && Denominator == 0;

    /// <summary>
    /// Is Positive Infinity 
    /// </summary>
    public bool IsPositiveInfinity => Numerator > 0 && Denominator == 0;

    /// <summary>
    /// Is Negative Infinity 
    /// </summary>
    public bool IsNegativeInfinity => Numerator < 0 && Denominator == 0;

    /// <summary>
    /// Is Integer (no fractional part)
    /// </summary>
    public bool IsInteger => Denominator == 1;

    /// <summary>
    /// Absolute Value 
    /// </summary>
    public BigRational Abs() => Numerator < 0
      ? new BigRational(-Numerator, Denominator)
      : this;

    /// <summary>
    /// Power
    /// </summary>
    public BigRational Pow(int exponent) {
      if (Denominator == 0)
        return this;
      else if (0 == exponent)
        return One;
      else if (0 == Numerator)
        return Zero;

      if (exponent > 0)
        return new BigRational(BigInteger.Pow(Numerator, exponent), BigInteger.Pow(Denominator, exponent));
      else if (exponent == int.MinValue)
        throw new ArgumentOutOfRangeException(nameof(exponent));
      else
        return new BigRational(BigInteger.Pow(Denominator, -exponent), BigInteger.Pow(Numerator, -exponent));
    }

    /// <summary>
    /// Truncate (integer part) 
    /// </summary>
    public BigRational Trunc() => Denominator == 0
      ? this
      : new BigRational(Numerator / Denominator, 1);

    /// <summary>
    /// Fractional part  
    /// </summary>
    public BigRational Frac() => Denominator == 0
      ? this
      : new BigRational(Numerator % Denominator, 1);

    public BigRational Floor() {
      if (Denominator == 0)
        return this;

      if (Numerator >= 0)
        return Numerator / Denominator;

      return Numerator / Denominator - (Numerator % Denominator == 0 ? 0 : 1);
    }

    /// <summary>
    /// Ceiling
    /// </summary>
    public BigRational Ceiling() {
      if (Denominator == 0)
        return this;

      if (Numerator <= 0)
        return Numerator / Denominator;

      return Numerator / Denominator + (Numerator % Denominator == 0 ? 0 : 1);
    }

    /// <summary>
    /// Round
    /// </summary>
    public BigRational Round(MidpointRounding mode) {
      if (Denominator == 0)
        return this;

      BigInteger integer = Numerator / Denominator;
      BigInteger frac = 2 * ((Numerator < 0 ? -Numerator : Numerator) % Denominator);

      int sign = Numerator < 0 ? -1 : 1;

      if (frac < Denominator)
        return integer;
      else if (frac > Denominator)
        return sign < 0 ? integer - 1 : integer + 1;

      if (mode == MidpointRounding.AwayFromZero)
        return sign < 0 ? integer - 1 : integer + 1;
      else if (mode == MidpointRounding.ToZero)
        return integer;
      else if (mode == MidpointRounding.ToNegativeInfinity)
        return integer - 1;
      else if (mode == MidpointRounding.ToPositiveInfinity)
        return integer + 1;

      if (integer % 2 == 0)
        return integer;
      else
        return sign < 0 ? integer - 1 : integer + 1;
    }

    /// <summary>
    /// Round
    /// </summary>
    public BigRational Round() => Round(MidpointRounding.ToEven);

    /// <summary>
    /// Fractional digits
    /// e.g. 1/7 returns 1, 4, 2, 8, 5, 7, 1, 4 ,2 ...
    /// </summary>
    public IEnumerable<int> FractionalDigits() {
      if (Denominator <= 1)
        yield break;

      for (BigInteger value = ((Numerator < 0 ? -Numerator : Numerator) % Denominator) * 10;
           value != 0;
           value = (value % Denominator) * 10)
        yield return (int)(value / Denominator);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (Denominator == 0)
        if (Numerator == 0)
          return "NaN";
        else if (Numerator > 0)
          return "+Inf";
        else
          return "-Inf";
      else if (Denominator == 1)
        return Numerator.ToString();

      return $"{Numerator} / {Denominator}";
    }

    #endregion Public

    #region Operators

    #region Cast 

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(BigInteger value) => new BigRational(value);

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(int value) => new BigRational(value);

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(long value) => new BigRational(value);

    #endregion Cast

    #region Comparison

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(BigRational left, BigRational right) => left.Equals(right);

    /// <summary>
    /// Not Equals 
    /// </summary>
    public static bool operator !=(BigRational left, BigRational right) => !left.Equals(right);

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(BigRational left, BigRational right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(BigRational left, BigRational right) => Compare(left, right) < 0;

    /// <summary>
    /// More or Equal
    /// </summary>
    public static bool operator >=(BigRational left, BigRational right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less or Equal
    /// </summary>
    public static bool operator <=(BigRational left, BigRational right) => Compare(left, right) <= 0;

    #endregion Comparison

    #region Arithmetics

    /// <summary>
    /// Unary +
    /// </summary>
    public static BigRational operator +(BigRational value) => value;

    /// <summary>
    /// Unary -
    /// </summary>
    public static BigRational operator -(BigRational value) => new BigRational(-value.Numerator, value.Denominator);

    /// <summary>
    /// Binary +
    /// </summary>
    public static BigRational operator +(BigRational left, BigRational right) =>
      new BigRational(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary -
    /// </summary>
    public static BigRational operator -(BigRational left, BigRational right) =>
      new BigRational(left.Numerator * right.Denominator - right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary *
    /// </summary>
    public static BigRational operator *(BigRational left, BigRational right) =>
      new BigRational(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary /
    /// </summary>
    public static BigRational operator /(BigRational left, BigRational right) =>
      new BigRational(left.Numerator * right.Denominator, left.Denominator * right.Numerator);

    /// <summary>
    /// Remainder
    /// </summary>
    public static BigRational operator %(BigRational left, BigRational right) =>
      new BigRational((left.Numerator * right.Denominator) % (right.Numerator * left.Denominator),
                       left.Denominator * right.Denominator);

    #endregion Arithmetics

    #endregion Operators

    #region IEquatable<BigRational>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(BigRational other) =>
      Numerator == other.Numerator && Denominator == other.Denominator;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) =>
      obj is BigInteger other && Equals(other);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Numerator.GetHashCode() ^ Denominator.GetHashCode();

    #endregion IEquatable<BigRational>

    #region IComparable<BigRational>

    /// <summary>
    /// CompareTo
    /// </summary>
    public int CompareTo(BigRational other) => Compare(this, other);

    #endregion IComparable<BigRational>
  }

}
