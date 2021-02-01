namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Integer Codes
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class IntegerExtensions {
    #region public

    /// <summary>
    /// To Gray code
    /// </summary>
    public static int ToGrayCode(this int value) {
      return unchecked(value ^ (int)(((uint)value) >> 1));
    }

    /// <summary>
    /// To Gray code
    /// </summary>
    public static long ToGrayCode(this long value) {
      return unchecked(value ^ (long)(((ulong)value) >> 1));
    }

    /// <summary>
    /// From Gray code
    /// </summary>
    public static int FromGrayCode(this int value) {
      uint result = unchecked((uint)value);

      for (uint v = result >> 1; v > 0; v >>= 1)
        result ^= v;

      return unchecked((int)result);
    }

    /// <summary>
    /// From Gray code
    /// </summary>
    public static long FromGrayCode(this long value) {
      ulong result = unchecked((ulong)value);

      for (ulong v = result >> 1; v > 0; v >>= 1)
        result ^= v;

      return unchecked((long)result);
    }

    #endregion public
  }

}
