using System;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Int64 Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Int64Extensions {
    #region public

    /// <summary>
    /// Greatest Common Divisor
    /// </summary>
    /// <param name="left">Left value</param>
    /// <param name="right">Right value</param>
    /// <returns></returns>
    public static long Gcd(this long left, long right) {
      int shift;

      int sign = (left < 0 && right > 0 || left > 0 && right < 0)
        ? -1
        : +1;

      // GCD(0,v) == v; GCD(u,0) == u, GCD(0,0) == 0 
      if (left == 0)
        return right;

      if (right == 0)
        return left;

      left = Math.Abs(left);
      right = Math.Abs(right);

      // Let shift := lg K, where K is the greatest power of 2 dividing both u and v. 
      for (shift = 0; ((left | right) & 1) == 0; ++shift) {
        left >>= 1;
        right >>= 1;
      }

      while ((left & 1) == 0)
        left >>= 1;

      // From here on, u is always odd. 
      do {
        // remove all factors of 2 in v - they are not common 
        // note: v is not zero, so while will terminate
        while ((right & 1) == 0)  /* Loop X */
          right >>= 1;

        // Now u and v are both odd. Swap if necessary so u <= v,
        // then set v = v - u (which is even). For bignums, the
        // swapping is just pointer movement, and the subtraction
        // can be done in-place. 
        if (left > right) {
          long t = right;
          right = left;
          left = t;
        }

        // Here v >= u.
        right -= left;

      } while (right != 0);

      // restore common factors of 2 
      return sign * (left << shift);
    }

    /// <summary>
    /// Extended Euclid Algorithm
    /// </summary>
    public static (long LeftFactor,
                   long RightFactor,
                   long Gcd) Egcd(this long left, long right) {
      long leftFactor = 0;
      long rightFactor = 1;

      long u = 1;
      long v = 0;
      long gcd = 0;

      while (left != 0) {
        long q = right / left;
        long r = right % left;

        long m = leftFactor - u * q;
        long n = rightFactor - v * q;

        right = left;
        left = r;
        leftFactor = u;
        rightFactor = v;
        u = m;
        v = n;

        gcd = right;
      }

      return (LeftFactor: leftFactor,
              RightFactor: rightFactor,
              Gcd: gcd);
    }

    /// <summary>
    /// Mod Inversion
    /// </summary>
    public static long ModInversion(this long value, long modulo) {
      var egcd = Egcd(value, modulo);

      if (egcd.Gcd != 1)
        throw new ArgumentException("Invalid modulo", nameof(modulo));

      long result = egcd.LeftFactor;

      if (result < 0)
        result += modulo;

      return result % modulo;
    }

    /// <summary>
    /// Mod Division
    /// </summary>
    public static long ModDivision(this long left, long right, long modulo) =>
      (left * ModInversion(right, modulo)) % modulo;

    #endregion public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Int32 Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Int32Extensions {
    #region public

    /// <summary>
    /// Greatest Common Divisor
    /// </summary>
    /// <param name="left">Left value</param>
    /// <param name="right">Right value</param>
    /// <returns></returns>
    public static int Gcd(this int left, int right) {
      int shift;

      // GCD(0,v) == v; GCD(u,0) == u, GCD(0,0) == 0 
      if (left == 0)
        return right;

      if (right == 0)
        return left;

      // Let shift := lg K, where K is the greatest power of 2 dividing both u and v. 
      for (shift = 0; ((left | right) & 1) == 0; ++shift) {
        left >>= 1;
        right >>= 1;
      }

      while ((left & 1) == 0)
        left >>= 1;

      // From here on, u is always odd. 
      do {
        // remove all factors of 2 in v - they are not common 
        // note: v is not zero, so while will terminate
        while ((right & 1) == 0)  /* Loop X */
          right >>= 1;

        // Now u and v are both odd. Swap if necessary so u <= v,
        // then set v = v - u (which is even). For bignums, the
        // swapping is just pointer movement, and the subtraction
        // can be done in-place. 
        if (left > right) {
          int t = right;
          right = left;
          left = t;
        }

        // Here v >= u.
        right -= left;

      } while (right != 0);

      // restore common factors of 2 
      return left << shift;
    }

    /// <summary>
    /// Extended Euclid Algorithm
    /// </summary>
    public static (int LeftFactor,
                   int RightFactor,
                   int Gcd) Egcd(this int left, int right) {
      int leftFactor = 0;
      int rightFactor = 1;

      int u = 1;
      int v = 0;
      int gcd = 0;

      while (left != 0) {
        int q = right / left;
        int r = right % left;

        int m = leftFactor - u * q;
        int n = rightFactor - v * q;

        right = left;
        left = r;
        leftFactor = u;
        rightFactor = v;
        u = m;
        v = n;

        gcd = right;
      }

      return (LeftFactor: leftFactor,
              RightFactor: rightFactor,
              Gcd: gcd);
    }

    /// <summary>
    /// Mod Inversion
    /// </summary>
    public static int ModInversion(this int value, int modulo) {
      var egcd = Egcd(value, modulo);

      if (egcd.Gcd != 1)
        throw new ArgumentException("Invalid modulo", nameof(modulo));

      int result = egcd.LeftFactor;

      if (result < 0)
        result += modulo;

      return result % modulo;
    }

    /// <summary>
    /// Mod Division
    /// </summary>
    public static int ModDivision(this int left, int right, int modulo) =>
      (left * ModInversion(right, modulo)) % modulo;

    #endregion public
  }

}
