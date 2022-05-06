using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Big Rational Number
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct BigRational : IEquatable<BigRational>, IComparable<BigRational>, ISerializable, IFormattable {
    #region Create Algorithm

    private static bool TryParseDecimal(string source, out BigRational result) {
      result = BigRational.NaN;

      if (string.IsNullOrWhiteSpace(source))
        return false;

      const string pattern =
        @"^\s*(?<sign>[+-])?\s*(?<int>[0-9]+)?(\.(?<frac>[0-9]+)?(?<period>\([0-9]+\))?)?([eE](?<exp>[+-]?[0-9]+)?)?\s*$";

      Match match = Regex.Match(source, pattern);

      if (!match.Success)
        return false;

      string signPart = match.Groups["sign"].Value;
      string intPart = match.Groups["int"].Value;
      string fracPart = match.Groups["frac"].Value;
      string periodPart = match.Groups["period"].Value.Trim('(', ')');
      string expPart = match.Groups["exp"].Value;

      if (string.IsNullOrEmpty(intPart) &&
          string.IsNullOrEmpty(fracPart) &&
          string.IsNullOrEmpty(periodPart))
        return false;

      result = 0;

      if (!string.IsNullOrEmpty(intPart))
        result += BigInteger.Parse(intPart);

      if (!string.IsNullOrEmpty(fracPart))
        result += new BigRational(BigInteger.Parse(fracPart), BigInteger.Pow(10, fracPart.Length));

      if (!string.IsNullOrEmpty(periodPart))
        result += new BigRational(BigInteger.Parse(periodPart), BigInteger.Pow(10, periodPart.Length) - 1) /
                  BigInteger.Pow(10, fracPart.Length);

      if (!string.IsNullOrEmpty(expPart)) {
        if (!int.TryParse(expPart, out int exp) && exp > 1_000_000_000) {
          result = BigRational.NaN;

          return false;
        }

        BigInteger factor = BigInteger.Pow(10, Math.Abs(exp));

        if (exp < 0)
          result /= factor;
        else
          result *= factor;
      }

      if (signPart == "-")
        result = -result;

      return true;
    }

    private static bool TryParseNatural(string value, out BigRational result) {
      result = NaN;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      value = value.Trim();

      if ("NaN".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = NaN;

        return true;
      }
      else if ("+Inf".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = PositiveInfinity;

        return true;
      }
      else if ("-Inf".Equals(value, StringComparison.OrdinalIgnoreCase)) {
        result = NegativeInfinity;

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

    #endregion Create Algorithm

    #region Create

    // Deserialization
    private BigRational(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      var numerator = BigInteger.Parse(info.GetString("Numerator"));
      var denominator = BigInteger.Parse(info.GetString("Denominator"));

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
        var gcd = BigInteger.GreatestCommonDivisor(denominator, numerator < 0 ? -numerator : numerator);

        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
      }

    }

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
        var gcd = BigInteger.GreatestCommonDivisor(denominator, numerator < 0 ? -numerator : numerator);

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
    /// Standard rational constuctor (from integer)
    /// </summary>
    /// <param name="value">integer value</param>
    public BigRational(int value) : this((BigInteger)value, 1) { }

    /// <summary>
    /// Standard rational constuctor (from integer)
    /// </summary>
    /// <param name="value">integer value</param>
    public BigRational(long value) : this((BigInteger)value, 1) { }

    /// <summary>
    /// From character numeric value ⅝ => 5 / 8
    /// </summary>
    public BigRational(char value) {
      if (char.GetNumericValue(value) < 0) {
        Numerator = 0;
        Denominator = 0;

        return;
      }

      long factor = 1_260_000;

      BigInteger numerator = (long)(char.GetNumericValue(value) * factor + 0.5);
      BigInteger denominator = factor;

      var gcd = BigInteger.GreatestCommonDivisor(denominator, numerator < 0 ? -numerator : numerator);

      Numerator = numerator / gcd;
      Denominator = denominator / gcd;
    }

    /// <summary>
    /// From Float value
    /// </summary>
    /// <param name="value">Float value</param>
    public BigRational(float value) {
      if (0 == value) {
        Numerator = 0;
        Denominator = 1;

        return;
      }

      if (float.IsPositiveInfinity(value)) {
        Numerator = 1;
        Denominator = 0;

        return;
      }

      if (float.IsNegativeInfinity(value)) {
        Numerator = -1;
        Denominator = 0;

        return;
      }

      if (float.IsNaN(value)) {
        Numerator = 0;
        Denominator = 0;

        return;
      }

      byte[] bits = BitConverter.GetBytes(value);

      int sign = 1 - ((bits[3] >> 7) & 1) * 2;

      bits[3] = (byte)((bits[3] | 0x80) ^ 0x80);

      int exp = ((((int)(bits[3])) << 1) | (bits[2] >> 7)) - 127;

      BigInteger mantissa = (bits[2] | 128);

      mantissa <<= 16;

      for (int i = 1; i >= 0; --i) {
        int item = bits[i];

        BigInteger term = ((BigInteger)item) << (i * 8);

        mantissa += term; //((BigInteger)item) << (i * 8);
      }

      mantissa *= sign;

      exp = 23 - exp;

      if (exp < 0) {
        Numerator = mantissa * BigInteger.Pow(2, -exp);
        Denominator = 1;
      }
      else {
        BigInteger factor = BigInteger.Pow(2, exp);

        var gcd = BigInteger.GreatestCommonDivisor(factor, mantissa < 0 ? -mantissa : mantissa);

        Numerator = mantissa / gcd;
        Denominator = factor / gcd;
      }
    }

    /// <summary>
    /// From Double value
    /// </summary>
    /// <param name="value">Double value</param>
    public BigRational(double value) {
      if (0 == value) {
        Numerator = 0;
        Denominator = 1;

        return;
      }

      if (double.IsPositiveInfinity(value)) {
        Numerator = 1;
        Denominator = 0;

        return;
      }

      if (double.IsNegativeInfinity(value)) {
        Numerator = -1;
        Denominator = 0;

        return;
      }

      if (double.IsNaN(value)) {
        Numerator = 0;
        Denominator = 0;

        return;
      }

      byte[] bits = BitConverter.GetBytes(value);

      int sign = 1 - ((bits[7] >> 7) & 1) * 2;

      bits[7] = (byte)((bits[7] | 0x80) ^ 0x80);

      int exp = ((((int)(bits[7])) << 4) | (bits[6] >> 4)) - 1023;

      BigInteger mantissa = (bits[6] & 0xF) + 16;

      mantissa <<= 48;

      for (int i = 5; i >= 0; --i) {
        int item = bits[i];

        mantissa += ((BigInteger)item) << (i * 8);
      }

      mantissa *= sign;

      exp = 52 - exp;

      if (exp < 0) {
        Numerator = mantissa * BigInteger.Pow(2, -exp);
        Denominator = 1;
      }
      else {
        BigInteger factor = BigInteger.Pow(2, exp);

        var gcd = BigInteger.GreatestCommonDivisor(factor, mantissa < 0 ? -mantissa : mantissa);

        Numerator = mantissa / gcd;
        Denominator = factor / gcd;
      }
    }

    /// <summary>
    /// Try Parse (either natural, like "23 / 97" or decimal like "-1.45(913)e-6")
    /// </summary>
    public static bool TryParse(string value, out BigRational result) {
      if (TryParseNatural(value, out result))
        return true;
      if (TryParseDecimal(value, out result))
        return true;

      result = NaN;

      return false;
    }

    /// <summary>
    /// From Continued Fraction
    /// </summary>
    /// <param name="terms"></param>
    /// <returns></returns>
    public static BigRational FromContinuedFraction(IEnumerable<BigInteger> terms) {
      if (terms is null)
        throw new ArgumentNullException(nameof(terms));

      BigRational result = BigRational.PositiveInfinity;

      foreach (BigInteger term in terms.Reverse())
        result = (term + 1 / result);

      return result;
    }

    /// <summary>
    /// From radix representation
    /// E.g. -3.26A0FF4 (Hex)
    /// </summary>
    /// <param name="value">Value in int.frac format</param>
    /// <param name="radix">Radix to use</param>
    public static BigRational FromRadix(IEnumerable<char> value, int radix) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (radix < 2 || radix > 36)
        throw new ArgumentOutOfRangeException(nameof(radix));

      static int DigitFromChar(char c) {
        if (c >= '0' && c <= '9')
          return c - '0';
        if (c >= 'a' && c <= 'z')
          return c - 'a' + 10;
        if (c >= 'A' && c <= 'Z')
          return c - 'A' + 10;

        return -1;
      }

      int sign = 1;

      bool first = true;

      BigInteger nom = 0;
      BigInteger den = 1;

      bool isFrac = false;

      foreach (char c in value) {
        if (char.IsWhiteSpace(c) || c == '_')
          continue;

        if (c == '-') {
          sign = -sign;

          if (!first)
            throw new FormatException("Invalid Rational Value Format");

          continue;
        }

        first = false;

        if (c == ',' || c == '.') {
          if (isFrac)
            throw new FormatException("Invalid Rational Value Format");

          isFrac = true;

          continue;
        }

        if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z') {
          int d = DigitFromChar(c);

          if (d < 0 || d >= radix)
            throw new FormatException("Invalid Rational Value Format");

          nom = nom * radix + d;

          if (isFrac)
            den *= radix;
        }
        else
          throw new FormatException("Invalid Rational Value Format");
      }

      return new BigRational(nom * sign, den);
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
    public static BigRational Zero => new(0, 1);

    /// <summary>
    /// One
    /// </summary>
    public static BigRational One => new(1, 1);

    /// <summary>
    /// NaN
    /// </summary>
    public static BigRational NaN => new(0, 0);

    /// <summary>
    /// Positive infinity
    /// </summary>
    public static BigRational PositiveInfinity => new(1, 0);

    /// <summary>
    /// Negative infinity
    /// </summary>
    public static BigRational NegativeInfinity => new(-1, 0);

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
    /// Min
    /// </summary>
    public static BigRational Min(BigRational left, BigRational right) =>
      left <= right ? left : right;

    /// <summary>
    /// Max
    /// </summary>
    public static BigRational Max(BigRational left, BigRational right) =>
      left >= right ? left : right;

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
    /// Is Finite (not NaN and not Infinity)
    /// </summary>
    public bool IsFinite => Denominator != 0;

    /// <summary>
    /// Is One (1)
    /// </summary>
    public bool IsOne => Numerator.IsOne && Denominator.IsOne;

    /// <summary>
    /// Is Zero (0)
    /// </summary>
    public bool IsZero => Numerator.IsZero && Denominator.IsOne;

    /// <summary>
    /// Is Power Of Two
    /// </summary>
    public bool IsPowerOfTwo => Numerator.IsOne && Denominator.IsPowerOfTwo ||
                                Numerator.IsPowerOfTwo && Denominator.IsOne;

    /// <summary>
    /// Is Integer (no fractional part)
    /// </summary>
    public bool IsInteger => Denominator == 1;

    /// <summary>
    /// Is Proper Fraction
    /// </summary>
    public bool IsProperFraction => BigInteger.Abs(Numerator) < BigInteger.Abs(Denominator);

    /// <summary>
    /// Absolute Value 
    /// </summary>
    public BigRational Abs() => Numerator < 0
      ? new BigRational(-Numerator, Denominator)
      : this;

    /// <summary>
    /// Sign (-1, +1, 0)
    /// </summary>
    public int Sign() => Numerator > 0 ? 1
                       : Numerator < 0 ? -1
                       : 0;

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
    /// Log10
    /// </summary>
    public double Log10() {
      if (Numerator < 0)
        return double.NaN;
      if (Denominator == 0) {
        if (Numerator > 0)
          return double.PositiveInfinity;

        return double.NaN;
      }
      if (Numerator == 0)
        return double.NegativeInfinity;

      return BigInteger.Log10(Numerator) - BigInteger.Log10(Denominator);
    }

    /// <summary>
    /// Log (natural)
    /// </summary>
    public double Log() {
      if (Numerator < 0)
        return double.NaN;
      if (Denominator == 0) {
        if (Numerator > 0)
          return double.PositiveInfinity;

        return double.NaN;
      }
      if (Numerator == 0)
        return double.NegativeInfinity;

      return BigInteger.Log(Numerator) - BigInteger.Log(Denominator);
    }

    /// <summary>
    /// Log 
    /// </summary>
    public double Log(double baseValue) {
      if (Numerator < 0)
        return double.NaN;
      if (Denominator == 0) {
        if (Numerator > 0)
          return double.PositiveInfinity;

        return double.NaN;
      }
      if (Numerator == 0)
        return double.NegativeInfinity;

      return BigInteger.Log(Numerator, baseValue) - BigInteger.Log(Denominator, baseValue);
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
      : new BigRational(Numerator % Denominator, Denominator);

    /// <summary>
    /// Floor
    /// </summary>
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
    /// Integer Division 
    /// </summary>
    public BigInteger Div(BigRational value) =>
      (Numerator * value.Denominator) / (value.Numerator * Denominator);

    /// <summary>
    /// Remainder
    /// </summary>
    public BigRational Rem(BigRational value) =>
      new((Numerator * value.Denominator) % (value.Numerator * Denominator),
            Denominator * value.Denominator);

    /// <summary>
    /// Round
    /// </summary>
    public BigRational Round() => Round(MidpointRounding.ToEven);

    /// <summary>
    /// Fractional digits
    /// e.g. 1/7 returns 1, 4, 2, 8, 5, 7, 1, 4, 2 ...
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
    /// To Continued Fraction 
    /// </summary>
    public IEnumerable<BigInteger> ToContinuedFraction() {
      BigInteger num = Numerator;
      BigInteger den = Denominator;

      while (den != 0) {
        yield return BigInteger.DivRem(num, den, out num);

        (num, den) = (den, num);
      }
    }

    /// <summary>
    /// To radix representation (eg. binary, hexadecimal etc.)
    /// </summary>
    /// <param name="radix">Radix in [2..36] range</param>
    /// <returns></returns>
    public IEnumerable<char> ToRadix(int radix) {
      if (radix < 2 || radix > 36)
        throw new ArgumentOutOfRangeException(nameof(radix));

      if (IsInfinity || IsNaN)
        foreach (char c in ToString())
          yield return c;

      static char DigitToChar(int v) => (char)(v < 10 ? '0' + v : 'a' + v - 10);

      BigRational value = this;

      if (value < 0) {
        yield return '-';

        value = -value;
      }

      BigInteger intPart = value.Numerator / value.Denominator;

      if (intPart == 0)
        yield return '0';
      else {
        Stack<char> digits = new();

        for (; intPart > 0; intPart /= radix)
          digits.Push(DigitToChar((int)(intPart % radix)));

        while (digits.Count > 0)
          yield return digits.Pop();
      }

      BigRational fracPart = value.Frac();

      if (fracPart == 0)
        yield break;

      yield return '.';

      while (fracPart > 0) {
        fracPart *= radix;

        yield return DigitToChar((int)(fracPart.Numerator / fracPart.Denominator));

        fracPart = fracPart.Frac();
      }
    }

    #endregion Public

    #region Operators

    #region Cast 

    /// <summary>
    /// From Character Numeric Value
    /// </summary>
    public static implicit operator BigRational(char value) => new(value);

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(BigInteger value) => new(value);

    /// <summary>
    /// To Integer
    /// </summary>
    public static explicit operator BigInteger(BigRational value) => value.IsNaN || value.IsInfinity
      ? throw new OverflowException()
      : value.Numerator / value.Denominator;

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(int value) => new(value, 1);

    /// <summary>
    /// To Integer
    /// </summary>
    public static explicit operator int(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < int.MinValue || result > int.MaxValue)
        throw new OverflowException();

      return (int)result;
    }

    /// <summary>
    /// From Signed Byte
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator BigRational(sbyte value) => new(value, 1);

    /// <summary>
    /// To Signed Byte
    /// </summary>
    [CLSCompliant(false)]
    public static explicit operator sbyte(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < sbyte.MinValue || result > sbyte.MaxValue)
        throw new OverflowException();

      return (sbyte)result;
    }

    /// <summary>
    /// From Byte
    /// </summary>
    public static implicit operator BigRational(byte value) => new(value, 1);

    /// <summary>
    /// To Byte
    /// </summary>
    public static explicit operator byte(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < byte.MinValue || result > byte.MaxValue)
        throw new OverflowException();

      return (byte)result;
    }

    /// <summary>
    /// From Short
    /// </summary>
    public static implicit operator BigRational(short value) => new(value, 1);

    /// <summary>
    /// To Short
    /// </summary>
    public static explicit operator short(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < short.MinValue || result > short.MaxValue)
        throw new OverflowException();

      return (short)result;
    }

    /// <summary>
    /// From UInt16
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator BigRational(ushort value) => new(value, 1);

    /// <summary>
    /// To UInt16
    /// </summary>
    [CLSCompliant(false)]
    public static explicit operator ushort(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < ushort.MinValue || result > ushort.MaxValue)
        throw new OverflowException();

      return (UInt16)result;
    }

    /// <summary>
    /// From UInt32
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator BigRational(uint value) => new(value, 1);

    /// <summary>
    /// To UInt32
    /// </summary>
    [CLSCompliant(false)]
    public static explicit operator uint(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < uint.MinValue || result > uint.MaxValue)
        throw new OverflowException();

      return (uint)result;
    }

    /// <summary>
    /// From UInt64
    /// </summary>
    [CLSCompliant(false)]
    public static implicit operator BigRational(ulong value) => new(value, 1);

    /// <summary>
    /// To UInt64
    /// </summary>
    [CLSCompliant(false)]
    public static explicit operator ulong(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < ulong.MinValue || result > ulong.MaxValue)
        throw new OverflowException();

      return (ulong)result;
    }

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator BigRational(long value) => new(value, 1);

    /// <summary>
    /// To Integer
    /// </summary>
    public static explicit operator long(BigRational value) {
      if (value.IsNaN || value.IsInfinity)
        throw new OverflowException();

      BigInteger result = value.Numerator / value.Denominator;

      if (result < long.MinValue || result > long.MaxValue)
        throw new OverflowException();

      return (long)result;
    }

    /// <summary>
    /// From Float (Single)
    /// </summary>
    public static implicit operator BigRational(float value) => new(value);

    /// <summary>
    /// From Double
    /// </summary>
    public static implicit operator BigRational(double value) => new(value);

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
    public static BigRational operator -(BigRational value) => new(-value.Numerator, value.Denominator);

    /// <summary>
    /// Binary +
    /// </summary>
    public static BigRational operator +(BigRational left, BigRational right) =>
      new(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary -
    /// </summary>
    public static BigRational operator -(BigRational left, BigRational right) =>
      new(left.Numerator * right.Denominator - right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary *
    /// </summary>
    public static BigRational operator *(BigRational left, BigRational right) =>
      new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    /// <summary>
    /// Binary /
    /// </summary>
    public static BigRational operator /(BigRational left, BigRational right) =>
      new(left.Numerator * right.Denominator, left.Denominator * right.Numerator);

    /// <summary>
    /// Remainder
    /// </summary>
    public static BigRational operator %(BigRational left, BigRational right) =>
      new((left.Numerator * right.Denominator) % (right.Numerator * left.Denominator),
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

    #region ISerializable

    /// <summary>
    /// Serialization
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      info.AddValue("Numerator", Numerator.ToString());
      info.AddValue("Denominator", Numerator.ToString());
    }

    #endregion ISerializable

    #region IFormattable

    /// <summary>
    /// To Natural representation (e.g. "1 / 6")
    /// </summary>
    public string ToStringNatural() {
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

    /// <summary>
    /// To Decimal representation (e.g. "0.1(6)")
    /// </summary>
    public string ToStringDecimal(IFormatProvider formatProvider = null) {
      if (Denominator == 0)
        if (Numerator == 0)
          return "NaN";
        else if (Numerator > 0)
          return "+Inf";
        else
          return "-Inf";
      else if (Denominator == 1)
        return Numerator.ToString();

      formatProvider ??= CultureInfo.CurrentCulture;

      var format = formatProvider.GetFormat(typeof(NumberFormatInfo));

      var numerator = Numerator;

      StringBuilder sb = new();

      if (numerator < 0) {
        sb.Append('-');

        numerator = BigInteger.Abs(numerator);
      }

      sb.Append(numerator / Denominator);

      if (format is NumberFormatInfo nfi)
        sb.Append(nfi.NumberDecimalSeparator);
      else
        sb.Append('.');

      numerator %= Denominator;

      List<int> digits = new();
      Dictionary<BigInteger, int> remainders = new();

      for (int index = 0; ; ++index) {
        numerator *= 10;

        BigInteger rem = numerator % Denominator;

        int digit = (int)(numerator / Denominator);
        numerator -= digit * Denominator;

        if (rem == 0) {
          digits.Add(digit);

          sb.Append(string.Concat(digits));

          break;
        }

        if (remainders.TryGetValue(rem, out int idx)) {
          if (digit != digits[idx]) {
            idx += 1;
            digits.Add(digit);
          }

          for (int i = 0; i < idx; ++i)
            sb.Append(digits[i]);

          sb.Append('(');

          for (int i = idx; i < digits.Count; ++i)
            sb.Append(digits[i]);

          sb.Append(')');

          break;
        }

        digits.Add(digit);

        remainders.Add(rem, index);
      }

      return sb.ToString();

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

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (string.IsNullOrEmpty(format) || format == "g" || format == "G")
        return ToStringNatural();
      if (format == "n" || format == "N")
        return ToStringNatural();
      if (format == "d" || format == "D")
        return ToStringDecimal(formatProvider);

      throw new FormatException("Invalid format");
    }

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format) =>
      ToString(format, CultureInfo.CurrentCulture);

    #endregion IFormattable
  }

}
