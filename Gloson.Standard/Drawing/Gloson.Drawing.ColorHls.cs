using System;
using System.Drawing;

namespace Gloson.Drawing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Hue Luminosity Saturation Color
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct ColorHsl : IEquatable<ColorHsl> {
    #region Algorithm 

    // RGB to HLS
    private static void RgbToHls(int R, int G, int B,
                                 out int H, out int L, out int S,
                                 int scale) {

      // luminosity
      int cMax;
      int cMin;

      if (R > G) {
        if (G > B) {
          cMax = R;
          cMin = B;
        }
        else if (B > R) {
          cMax = B;
          cMin = G;
        }
        else {
          cMax = R;
          cMin = G;
        }
      }
      else if (R > B) {
        cMax = G;
        cMin = B;
      }
      else if (B > G) {
        cMax = B;
        cMin = R;
      }
      else {
        cMax = G;
        cMin = R;
      }

      L = (((cMax + cMin) * scale) + scale) / (2 * scale);

      if (cMax == cMin) {           // <- r=g=b --> achromatic case 
        S = 0;
        H = 0;
      }
      else {                        // <- chromatic case 
        if (L <= (scale / 2))
          S = (((cMax - cMin) * scale) + ((cMax + cMin) / 2)) / (cMax + cMin);
        else
          S = (((cMax - cMin) * scale) + ((2 * scale - cMax - cMin) / 2)) /
               (2 * scale - cMax - cMin);

        // Hue
        int Rdelta = (((cMax - R) * (scale / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
        int Gdelta = (((cMax - G) * (scale / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);
        int Bdelta = (((cMax - B) * (scale / 6)) + ((cMax - cMin) / 2)) / (cMax - cMin);

        if (R == cMax)
          H = Bdelta - Gdelta;
        else if (G == cMax)
          H = (scale / 3) + Rdelta - Bdelta;
        else /* B == cMax */
          H = ((2 * scale) / 3) + Gdelta - Rdelta;

        if (H < 0)
          H += scale;
        if (H > scale)
          H -= scale;
      }
    }

    // Hue to RGB
    private static int HueToRGB(int n1, int n2, int hue, int scale) {
      if (hue < 0)
        hue += scale;
      else if (hue > scale)
        hue -= scale;

      // return r,g, or b value from this tridrant 
      if (hue < (scale / 6))
        return (n1 + (((n2 - n1) * hue + (scale / 12)) / (scale / 6)));
      if (hue < (scale / 2))
        return (n2);
      if (hue < ((scale * 2) / 3))
        return (n1 + (((n2 - n1) * (((scale * 2) / 3) - hue) + (scale / 12)) / (scale / 6)));
      else
        return (n1);
    }

    // HLS to RGB
    private static void HlsToRgb(int H, int L, int S,
                                 out int R, out int G, out int B,
                                 int scale) {
      if (S == 0) { // <- achromatic case
        R = L;
        G = R;
        B = R;
      }
      else { // <- chromatic case 
        int magic1;
        int magic2;

        if (L <= (scale / 2))
          magic2 = (L * (scale + S) + (scale / 2)) / scale;
        else
          magic2 = L + S - ((L * S) + (scale / 2)) / scale;

        magic1 = 2 * L - magic2;

        R = (HueToRGB(magic1, magic2, H + (scale / 3), scale) * scale + (scale / 2)) / scale;
        G = (HueToRGB(magic1, magic2, H, scale) * scale + (scale / 2)) / scale;
        B = (HueToRGB(magic1, magic2, H - (scale / 3), scale) * scale + (scale / 2)) / scale;
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="a">A</param>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="l">Luminosity</param>
    public ColorHsl(int a, int h, int s, int l) {
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
    /// <param name="s">Saturation</param>
    /// <param name="l">Luminosity</param>
    public ColorHsl(int h, int s, int l) :
      this(0, h, l, s) { }

    /// <summary>
    /// From ARGB
    /// </summary>
    public static ColorHsl FromArgb(int a, int r, int g, int b) {
      if (a < 0 || a > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(a));
      else if (r < 0 || r > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(r));
      else if (g < 0 || g > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(g));
      else if (b < 0 || b > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(b));

      RgbToHls(r, g, b, out int h, out int l, out int s, byte.MaxValue);

      return new ColorHsl(a, h, l, s);
    }

    /// <summary>
    /// From RGB
    /// </summary>
    public static ColorHsl FromArgb(int r, int g, int b) {
      if (r < 0 || r > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(r));
      else if (g < 0 || g > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(g));
      else if (b < 0 || b > byte.MaxValue)
        throw new ArgumentOutOfRangeException(nameof(b));

      RgbToHls(r, g, b, out int h, out int s, out int l, byte.MaxValue);

      return new ColorHsl(0, h, l, s);
    }

    /// <summary>
    /// From Color
    /// </summary>
    public ColorHsl(Color color) {
      RgbToHls(color.R, color.G, color.B, out int h, out int l, out int s, byte.MaxValue);

      A = color.A;
      H = (byte)h;
      L = (byte)l;
      S = (byte)s;
    }

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
        $"S : {S,3}",
        $"L : {L,3}",
        $" ({A:x2}{H:x2}{S:x2}{L:x2})");

    #endregion Public

    #region Operators

    #region Comparison

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(ColorHsl left, ColorHsl right) => left.Equals(right);

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(ColorHsl left, ColorHsl right) => !left.Equals(right);

    #endregion Comparison

    #region Cast

    /// <summary>
    /// From Color
    /// </summary>
    public static implicit operator ColorHsl(Color value) => new ColorHsl(value);

    /// <summary>
    /// To Color
    /// </summary>
    public Color ToColor() {
      HlsToRgb(H, L, S, out int r, out int g, out int b, byte.MaxValue);

      return Color.FromArgb(A, r, g, b);
    }

    /// <summary>
    /// To Color
    /// </summary>
    public static implicit operator Color(ColorHsl value) => value.ToColor();

    #endregion Cast

    #endregion Operators

    #region IEquatable<ColorHls>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ColorHsl other) {
      return A == other.A &&
             H == other.H &&
             L == other.L &&
             S == other.S;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) =>
      (obj is ColorHsl other) ? Equals(other) : false;

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() =>
      unchecked((A << 24) | (H << 16) | (L << 8) | (S));

    #endregion IEquatable<ColorHls>
  }

}
