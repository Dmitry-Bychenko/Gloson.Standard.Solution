using System;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Bitwise operations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Bitwise {
    #region Public

    /// <summary>
    /// Number of bits sets
    /// </summary>
    [CLSCompliant(false)]
    public static int BitsSet(this sbyte value) {
      int result = 0;

      unchecked {
        for (uint n = (uint)value; n != 0; n >>= 1)
          result += (int)(n & 1);
      }

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this sbyte left, sbyte right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    public static int BitsSet(this byte value) {
      int result = 0;

      for (uint n = value; n != 0; n >>= 1)
        result += (int)(n & 1);

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this byte left, byte right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    public static int BitsSet(this Int16 value) {
      int result = 0;

      unchecked {
        for (uint n = (uint)value; n != 0; n >>= 1)
          result += (int)(n & 1);
      }

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this Int16 left, Int16 right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    [CLSCompliant(false)]
    public static int BitsSet(this UInt16 value) {
      int result = 0;

      for (uint n = value; n != 0; n >>= 1)
        result += (int)(n & 1);

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this UInt16 left, UInt16 right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    public static int BitsSet(this int value) {
      int result = 0;

      unchecked {
        for (uint n = (uint)value; n != 0; n >>= 1)
          result += (int)(n & 1);
      }

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this int left, int right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    [CLSCompliant(false)]
    public static int BitsSet(this uint value) {
      int result = 0;

      for (uint n = value; n != 0; n >>= 1)
        result += (int)(n & 1);

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this uint left, uint right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    public static int BitsSet(this long value) {
      int result = 0;

      unchecked {
        for (ulong n = (ulong)value; n != 0; n >>= 1)
          result += (int)(n & 1);
      }

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this long left, long right) => BitsSet(left ^ right);

    /// <summary>
    /// Number of bits sets
    /// </summary>
    [CLSCompliant(false)]
    public static int BitsSet(this ulong value) {
      int result = 0;

      for (ulong n = value; n != 0; n >>= 1)
        result += (int)(n & 1);

      return result;
    }

    /// <summary>
    /// Hamming Distance To 
    /// </summary>
    [CLSCompliant(false)]
    public static int HammingDistanceTo(this ulong left, ulong right) => BitsSet(left ^ right);

    #endregion Public
  }
}
