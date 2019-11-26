using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gloson.Drawing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// ColorF (floating point; all components are in [0..1] range)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct ColorF : IEquatable<ColorF> {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorF(float a, float r, float g, float b) {
      if (a < 0 || a > 1.0)
        throw new ArgumentOutOfRangeException(nameof(a));
      else if (r < 0 || r > 1.0)
        throw new ArgumentOutOfRangeException(nameof(r));
      else if (g < 0 || g > 1.0)
        throw new ArgumentOutOfRangeException(nameof(g));
      else if (b < 0 || b > 1.0)
        throw new ArgumentOutOfRangeException(nameof(b));

      A = a;
      R = r;
      G = g;
      B = b;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorF(float r, float g, float b)
      : this(0.0f, r, g, b) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ColorF(Color color)
      : this(color.A / 255.0f, color.R / 255.0f, color.G / 255.0f, color.B / 255.0f) { }

    #endregion Create

    #region Public

    /// <summary>
    /// A
    /// </summary>
    public float A { get; }

    /// <summary>
    /// Red
    /// </summary>
    public float R { get; }

    /// <summary>
    /// Green
    /// </summary>
    public float G { get; }

    /// <summary>
    /// Blue
    /// </summary>
    public float B { get; }

    /// <summary>
    /// Gray Scale
    /// </summary>
    public float GrayScale => 0.299f * R + 0.587f * G + 0.114f * B;

    /// <summary>
    /// To Gray Scale
    /// </summary>
    /// <returns></returns>
    public ColorF ToGrayScale() {
      float gs = GrayScale;

      return new ColorF(A, gs, gs, gs);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join("; ",
        $"A : {A:f3}",
        $"R : {R:f3}",
        $"G : {G:f3}",
        $"B : {B:f3}");
    }

    #endregion Public

    #region Operators

    #region Cast

    /// <summary>
    /// To Color
    /// </summary>
    public Color ToColor() => Color.FromArgb(
      (int) (A * 255 + 0.5),
      (int) (R * 255 + 0.5),
      (int) (G * 255 + 0.5),
      (int) (B * 255 + 0.5)
    );

    /// <summary>
    /// To Color
    /// </summary>
    public static implicit operator Color(ColorF value) => value.ToColor();

    /// <summary>
    /// From Color
    /// </summary>
    public static implicit operator ColorF(Color color) => new ColorF(color);

    #endregion Cast

    #endregion Operators

    #region IEquatable<ColorF>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ColorF other) {
      return A == other.A &&
             R == other.R &&
             G == other.G &&
             B == other.B;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      if (obj is ColorF other)
        return Equals(other);
      else
        return false;
    }

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      return A.GetHashCode() ^ 
             R.GetHashCode() ^ 
             G.GetHashCode() ^ 
             B.GetHashCode();
    }

    #endregion IEquatable<ColorF>
  }
}
