using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Z-Algorithm
  /// </summary>
  /// https://www.geeksforgeeks.org/z-algorithm-linear-time-pattern-searching-algorithm/
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ZAlgorithm {
    #region Public

    /// <summary>
    /// ZArray from given string
    /// </summary>
    public static int[] ZArray(string value) {
      if (string.IsNullOrEmpty(value))
        return new int[0];

      int n = value.Length;
      int L = 0, R = 0;

      int[] result = new int[n];

      result[0] = n;

      for (int i = 1; i < n; ++i) {
        if (i > R) {
          L = R = i;

          while (R < n && value[R - L] == value[R])
            ++R;

          result[i] = R - L;
          --R;
        }
        else {
          int k = i - L;

          if (result[k] < R - i + 1)
            result[i] = result[k];
          else {
            L = i;

            while (R < n && value[R - L] == value[R])
              ++R;

            result[i] = R - L;
            --R;
          }
        }
      }

      return result;
    }

    #endregion Public
  }
}
