using System;
using System.Collections.Generic;
using System.Text;

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
          long t = right;
          right = left;
          left = t;
        }

        // Here v >= u.
        right = right - left;

      } while (right != 0);

      // restore common factors of 2 
      return left << shift;
    }

    #endregion public
  }

}
