using System;
using System.Globalization;
using System.Numerics;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Decimal Builder (see decimal.GetBits)
  /// </summary>
  /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.decimal.getbits?view=net-5.0"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class DecimalBuilder : IEquatable<DecimalBuilder> {
    #region Private Data

    private readonly int[] m_Bits;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DecimalBuilder(int[] bits) {
      if (bits is null)
        throw new ArgumentNullException(nameof(bits));
      if (bits.Length != 4)
        throw new ArgumentOutOfRangeException(nameof(bits), $"{nameof(bits)} must have 4 items.");

      int value = m_Bits[3];
      int scale = (value >> 16) & 0xFF;

      if (scale > 28)
        throw new ArgumentException("Incorrect scale", nameof(bits));

      int mask = unchecked((1 << 31) | (0xFF << 16));

      value = (value | mask) ^ mask;

      if (value != 0)
        throw new ArgumentException($"Incorrect {nameof(bits)}[3] item.", nameof(bits));

      m_Bits = new int[4];

      Array.Copy(bits, m_Bits, 4);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="low">Low part of 96-bit mantissa</param>
    /// <param name="middle">Middle part of 96-bit mantissa</param>
    /// <param name="high">High part of 96-bit mantissa</param>
    /// <param name="scale">Scale [0..28]</param>
    /// <param name="isNegative">Is Negative</param>
    public DecimalBuilder(int low, int middle, int high, int scale, bool isNegative) {
      if (scale < 0 || scale > 28)
        throw new ArgumentNullException(nameof(scale));

      m_Bits = new int[4];

      m_Bits[0] = low;
      m_Bits[1] = middle;
      m_Bits[2] = high;
      m_Bits[3] = scale << 16;

      if (isNegative)
        m_Bits[3] = unchecked(m_Bits[3] | (1 << 31));
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DecimalBuilder(decimal value) {
      m_Bits = decimal.GetBits(value);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public DecimalBuilder() : this(0m) { }

    /// <summary>
    /// Clone
    /// </summary>
    public DecimalBuilder Clone() => new(Build());

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, NumberStyles styles, IFormatProvider provider, out decimal result) {
      if (decimal.TryParse(value, styles, provider, out decimal d)) {
        result = new DecimalBuilder(d);

        return true;
      }

      result = default;

      return false;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, IFormatProvider provider, out decimal result) {
      if (decimal.TryParse(value, NumberStyles.Any, provider, out decimal d)) {
        result = new DecimalBuilder(d);

        return true;
      }

      result = default;

      return false;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out decimal result) {
      if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out decimal d)) {
        result = new DecimalBuilder(d);

        return true;
      }

      result = default;

      return false;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Build
    /// </summary>
    public decimal Build() => new(m_Bits);

    /// <summary>
    /// Bits
    /// </summary>
    public int[] Bits {
      get {
        int[] result = new int[4];

        Array.Copy(m_Bits, result, m_Bits.Length);

        return result;
      }
    }

    /// <summary>
    /// Is Zero (either positive or negative)
    /// </summary>
    public bool IsZero => (m_Bits[0] == 0) && (m_Bits[1] == 0) && (m_Bits[2] == 0);

    /// <summary>
    /// Low part of 96-bit number
    /// </summary>
    public int Low {
      get => m_Bits[0];
      set => m_Bits[0] = value;
    }

    /// <summary>
    /// Middle part of 96-bit number
    /// </summary>
    public int Middle {
      get => m_Bits[1];
      set => m_Bits[1] = value;
    }

    /// <summary>
    /// High part of 96-bit number
    /// </summary>
    public int High {
      get => m_Bits[2];
      set => m_Bits[2] = value;
    }

    /// <summary>
    /// Is Negative
    /// </summary>
    public bool IsNegative {
      get => (m_Bits[3] & (1 << 31)) != 0;
      set {
        if (value)
          m_Bits[3] = m_Bits[3] | (1 << 31);
        else
          m_Bits[3] = (m_Bits[3] | (1 << 31)) ^ (1 << 31);
      }
    }

    /// <summary>
    /// Sign
    /// </summary>
    public int Sign {
      get {
        if (m_Bits[0] == 0 && m_Bits[1] == 0 && m_Bits[2] == 0)
          return 0;
        if (IsNegative)
          return -1;

        return 1;
      }
    }

    /// <summary>
    /// Scale [0..28]
    /// </summary>
    public int Scale {
      get {
        return (m_Bits[3] >> 16) & 0xFF;
      }
      set {
        if (value < 0 || value > 28)
          throw new ArgumentOutOfRangeException(nameof(value));

        int mask = 255 << 16;
        int scale = value << 16;

        m_Bits[3] = ((m_Bits[3] | mask) ^ mask) | scale;
      }
    }

    /// <summary>
    /// Scale factor [1..1**28]
    /// </summary>
    public BigInteger ScaleFactor => BigInteger.Pow(10, Scale);

    /// <summary>
    /// If Integer (4, 4.0, 4.00 etc.)
    /// </summary>
    public bool IsInteger => Mantissa % ScaleFactor == 0;

    /// <summary>
    /// Ratio
    /// </summary>
    public BigRational Ratio => new(Sign * Mantissa, ScaleFactor);

    /// <summary>
    /// Mantissa
    /// </summary>
    public BigInteger Mantissa {
      get {
        unchecked {
          BigInteger result = new((long)((uint)High));

          result = (result << 32) + (long)((uint)Middle);
          result = (result << 32) + (long)((uint)Low);

          return result;
        }
      }
      set {
        BigInteger factor = BigInteger.One << 32;

        unchecked {
          m_Bits[0] = (int)(value % factor);

          value /= factor;

          m_Bits[1] = (int)(value % factor);
          m_Bits[2] = (int)(value / factor);
        }
      }
    }

    /// <summary>
    /// To Report 
    /// </summary>
    public string ToReport() {
      return string.Join(Environment.NewLine,
        $"Value:   {(Build() >= 0 ? " " : "")}{Build()}",
        $"Sign:    {(Sign > 0 ? "+1" : Sign < 0 ? "-1" : " 0")}",
        $"Mantissa: {Mantissa}",
        $"Scale:    {Scale}"
      );
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"{Build()} = {(IsNegative ? "-" : "")}{Mantissa} (scale : {Scale})";

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(DecimalBuilder left, DecimalBuilder right) {
      if (ReferenceEquals(left, right))
        return true;
      if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(DecimalBuilder left, DecimalBuilder right) {
      if (ReferenceEquals(left, right))
        return false;
      if (left is null || right is null)
        return true;

      return !left.Equals(right);
    }

    /// <summary>
    /// To Decimal
    /// </summary>
    public static implicit operator decimal(DecimalBuilder builder) {
      return builder is not null
        ? builder.Build()
        : throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// From Decimal
    /// </summary>
    public static implicit operator DecimalBuilder(decimal value) => new(value);

    #endregion Operators

    #region IEquatable<DecimalBuilder>

    /// <summary>
    /// Equals
    /// We assume 1 != 1.0 != 1.00 != 1.000 != ...
    /// </summary>
    public bool Equals(DecimalBuilder other) {
      if (ReferenceEquals(other, this))
        return true;
      if (other is null)
        return false;

      return Build() == other.Build();
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as DecimalBuilder);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => m_Bits[0];

    #endregion IEquatable<DecimalBuilder>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Decimal Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class DecimalExtensions {
    #region Public

    /// <summary>
    /// Get Builder
    /// </summary>
    public static DecimalBuilder GetBuilder(this decimal value) => new(value);

    #endregion Public
  }

}
