using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gloson.Drawing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Hue Luminosity Saturation Floating point color
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct ColorHlsF : IEquatable<ColorHlsF> {
    #region Algorithm
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ColorHlsF(float a, float h, float l, float s) {
      if (a < 0.0 || a > 1.0)
        throw new ArgumentOutOfRangeException(nameof(a));
      else if (h < 0.0 || h > 1.0)
        throw new ArgumentOutOfRangeException(nameof(h));
      else if (l < 0.0 || l > 1.0)
        throw new ArgumentOutOfRangeException(nameof(l));
      else if (s < 0.0 || s > 1.0)
        throw new ArgumentOutOfRangeException(nameof(s));

      A = a;
      H = h;
      L = l;
      S = s;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ColorHlsF(float h, float l, float s)
      : this(0.0f, h, l, s) { }

    #endregion Create

    #region Public

    /// <summary>
    /// A
    /// </summary>
    public float A { get; }

    /// <summary>
    /// Hue
    /// </summary>
    public float H { get; }

    /// <summary>
    /// Luminosity
    /// </summary>
    public float L { get; }

    /// <summary>
    /// Saturation
    /// </summary>
    public float S { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => string.Join("; ",
        $"A : {A:f3}",
        $"H : {H:f3}",
        $"L : {L:f3}",
        $"S : {S:f3}");

    #endregion Public

    #region IEquatable<ColorHlsF>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ColorHlsF other) {
      return A == other.A &&
             H == other.H &&
             L == other.L &&
             S == other.S;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) =>
      (obj is ColorHlsF other) ? Equals(other) : false;

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() =>
      A.GetHashCode() ^ H.GetHashCode() ^ L.GetHashCode() ^ S.GetHashCode();

    #endregion IEquatable<ColorHlsF>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Hue Luminosity Saturation Floating point color
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct ColorHls : IEquatable<ColorHls> {
    #region Algorithm 
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="a">A</param>
    /// <param name="h">Hue</param>
    /// <param name="l">Luminosity</param>
    /// <param name="s">Saturation</param>
    public ColorHls(int a, int h, int l, int s) {
      if (a < 0 || a > Byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(a));
      else if (h < 0 || h > Byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(h));
      else if (l < 0 || l > Byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(l));
      else if (s < 0 || s > Byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(s));

      unchecked {
        A = (byte)a;
        H = (byte)h;
        L = (byte)l;
        S = (byte)s;
      }
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="l">Luminosity</param>
    /// <param name="s">Saturation</param>
    public ColorHls(int h, int l, int s) :
      this(0, h, l, s) { }

    #endregion Create

    #region Public

    /// <summary>
    /// A
    /// </summary>
    public byte A { get; }

    /// <summary>
    /// H
    /// </summary>
    public byte H { get; }

    /// <summary>
    /// L
    /// </summary>
    public byte L { get; }

    /// <summary>
    /// S
    /// </summary>
    public byte S { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => string.Join("; ",
        $"A : {A,3}",
        $"H : {H,3}",
        $"L : {L,3}",
        $"S : {S,3}");

    #endregion Public

    #region IEquatable<ColorHls>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ColorHls other) {
      return A == other.A &&
             H == other.H &&
             L == other.L &&
             S == other.S;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) =>
      (obj is ColorHlsF other) ? Equals(other) : false;

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() =>
      unchecked((A << 24) | (H << 16) | (L << 8) | (S));

    #endregion IEquatable<ColorHls>
  }
}
