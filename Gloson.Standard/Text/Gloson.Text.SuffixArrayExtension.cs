using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Linear Time Suffix Array
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class StringSuffixArrayExtension {
    #region Internal Classes

    internal interface IBaseArray {
      int this[int i] {
        set;
        get;
      }
    }

    internal class IntArray : IBaseArray {
      private readonly int[] m_array;
      private readonly int m_pos;

      public IntArray(int[] array, int pos) {
        m_pos = pos;
        m_array = array;
      }

      public int this[int i] {
        get { return m_array[i + m_pos]; }
        set { m_array[i + m_pos] = value; }
      }
    }

    internal class StringArray : IBaseArray {
      private readonly string m_array;
      private readonly int m_pos;

      public StringArray(string array, int pos) {
        m_pos = pos;
        m_array = array;
      }

      public int this[int i] {
        get { return m_array[i + m_pos]; }
        set {; }
      }
    }

    #endregion Internal Classes

    #region Algorithm

    private const int MINBUCKETSIZE = 256;

    private static void GetCounts(IBaseArray T, IBaseArray C, int n, int k) {
      for (int i = 0; i < k; ++i)
        C[i] = 0;

      for (int i = 0; i < n; ++i)
        C[T[i]] = C[T[i]] + 1;
    }

    private static void GetBuckets(IBaseArray C, IBaseArray B, int k, bool end) {
      int sum = 0;

      if (end)
        for (int i = 0; i < k; ++i) {
          sum += C[i];
          B[i] = sum;
        }
      else
        for (int i = 0; i < k; ++i) {
          sum += C[i];
          B[i] = sum - C[i];
        }
    }

    /* sort all type LMS suffixes */
    private static void LMSsort(IBaseArray T, int[] SA, IBaseArray C, IBaseArray B, int n, int k) {
      int i, j, b;
      int c0, c1;

      /* compute SAl */
      if (C == B)
        GetCounts(T, C, n, k);

      GetBuckets(C, B, k, false); /* find starts of buckets */

      j = n - 1;
      b = B[c1 = T[j]];
      --j;
      SA[b++] = (T[j] < c1) ? ~j : j;

      for (i = 0; i < n; ++i) {
        if (0 < (j = SA[i])) {
          if ((c0 = T[j]) != c1) {
            B[c1] = b;
            b = B[c1 = c0];
          }

          --j;

          SA[b++] = (T[j] < c1) ? ~j : j;
          SA[i] = 0;
        }
        else if (j < 0)
          SA[i] = ~j;
      }

      /* compute SAs */
      if (C == B)
        GetCounts(T, C, n, k);

      GetBuckets(C, B, k, true);

      for (i = n - 1, b = B[c1 = 0]; 0 <= i; --i) {
        if (0 < (j = SA[i])) {
          if ((c0 = T[j]) != c1) {
            B[c1] = b;
            b = B[c1 = c0];
          }

          --j;

          SA[--b] = (T[j] > c1) ? ~(j + 1) : j;
          SA[i] = 0;
        }
      }
    }

    private static int LMSpostproc(IBaseArray T, int[] SA, int n, int m) {
      int i, j, p, q, plen, qlen, name;
      int c0, c1;
      bool diff;

      /* compact all the sorted substrings into the first m items of SA
          2*m must be not larger than n (proveable) */
      for (i = 0; (p = SA[i]) < 0; ++i)
        SA[i] = ~p;

      if (i < m)
        for (j = i, ++i; ; ++i) {
          if ((p = SA[i]) < 0) {
            SA[j++] = ~p;
            SA[i] = 0;

            if (j == m)
              break;
          }
        }

      /* store the length of all substrings */
      i = n - 1; j = n - 1; c0 = T[n - 1];

      do {
        c1 = c0;
      }
      while ((0 <= --i) && ((c0 = T[i]) >= c1));

      for (; 0 <= i;) {
        do {
          c1 = c0;
        }
        while ((0 <= --i) && ((c0 = T[i]) <= c1));

        if (0 <= i) {
          SA[m + ((i + 1) >> 1)] = j - i; j = i + 1;

          do {
            c1 = c0;
          }
          while ((0 <= --i) && ((c0 = T[i]) >= c1));
        }
      }

      /* find the lexicographic names of all substrings */
      for (i = 0, name = 0, q = n, qlen = 0; i < m; ++i) {
        p = SA[i];
        plen = SA[m + (p >> 1)];
        diff = true;

        if ((plen == qlen) && ((q + plen) < n)) {
          for (j = 0; (j < plen) && (T[p + j] == T[q + j]); ++j)
            ;

          if (j == plen)
            diff = false;
        }

        if (diff != false) {
          ++name;
          q = p;
          qlen = plen;
        }

        SA[m + (p >> 1)] = name;
      }

      return name;
    }

    /* compute SA and BWT */
    private static void InduceSA(IBaseArray T, int[] SA, IBaseArray C, IBaseArray B, int n, int k) {
      int b, i, j;
      int c0, c1;

      /* compute SAl */
      if (C == B)
        GetCounts(T, C, n, k);

      GetBuckets(C, B, k, false); /* find starts of buckets */

      j = n - 1;
      b = B[c1 = T[j]];
      SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;

      for (i = 0; i < n; ++i) {
        j = SA[i];
        SA[i] = ~j;

        if (0 < j) {
          if ((c0 = T[--j]) != c1) {
            B[c1] = b;
            b = B[c1 = c0];
          }

          SA[b++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
        }
      }

      /* compute SAs */
      if (C == B)
        GetCounts(T, C, n, k);

      GetBuckets(C, B, k, true); /* find ends of buckets */

      for (i = n - 1, b = B[c1 = 0]; 0 <= i; --i) {
        if (0 < (j = SA[i])) {
          if ((c0 = T[--j]) != c1) {
            B[c1] = b;
            b = B[c1 = c0];
          }

          SA[--b] = ((j == 0) || (T[j - 1] > c1)) ? ~j : j;
        }
        else
          SA[i] = ~j;
      }
    }

    /* find the suffix array SA of T[0..n-1] in {0..k-1}^n
       use a working space (excluding T and SA) of at most 2n+O(1) for a constant alphabet */
    private static int SaisMain(IBaseArray T, int[] SA, int fs, int n, int k) {
      IBaseArray C, B, RA;
      int i, j, b, m, p, q, name, pidx = 0, newfs;
      int c0, c1;
      uint flags;

      if (k <= MINBUCKETSIZE) {
        C = new IntArray(new int[k], 0);

        if (k <= fs) {
          B = new IntArray(SA, n + fs - k);
          flags = 1;
        }
        else {
          B = new IntArray(new int[k], 0);
          flags = 3;
        }
      }
      else if (k <= fs) {
        C = new IntArray(SA, n + fs - k);

        if (k <= (fs - k)) {
          B = new IntArray(SA, n + fs - k * 2);
          flags = 0;
        }
        else if (k <= (MINBUCKETSIZE * 4)) {
          B = new IntArray(new int[k], 0);
          flags = 2;
        }
        else {
          B = C;
          flags = 8;
        }
      }
      else {
        C = B = new IntArray(new int[k], 0);
        flags = 4 | 8;
      }

      /* stage 1: reduce the problem by at least 1/2
         sort all the LMS-substrings */
      GetCounts(T, C, n, k);
      GetBuckets(C, B, k, true); /* find ends of buckets */

      for (i = 0; i < n; ++i)
        SA[i] = 0;

      b = -1;
      i = n - 1;
      j = n; m = 0;
      c0 = T[n - 1];

      do {
        c1 = c0;
      } while ((0 <= --i) && ((c0 = T[i]) >= c1));

      for (; 0 <= i;) {
        do {
          c1 = c0;
        } while ((0 <= --i) && ((c0 = T[i]) <= c1));

        if (0 <= i) {
          if (0 <= b) {
            SA[b] = j;
          }

          b = --B[c1];
          j = i;
          ++m;

          do {
            c1 = c0;
          } while ((0 <= --i) && ((c0 = T[i]) >= c1));
        }
      }

      if (1 < m) {
        LMSsort(T, SA, C, B, n, k);
        name = LMSpostproc(T, SA, n, m);
      }
      else if (m == 1) {
        SA[b] = j + 1;
        name = 1;
      }
      else {
        name = 0;
      }

      /* stage 2: solve the reduced problem
         recurse if names are not yet unique */
      if (name < m) {
        if ((flags & 4) != 0) {
          C = null;
          B = null;
        }

        if ((flags & 2) != 0)
          B = null;

        newfs = (n + fs) - (m * 2);

        if ((flags & (1 | 4 | 8)) == 0)
          if ((k + name) <= newfs)
            newfs -= k;
          else
            flags |= 8;

        for (i = m + (n >> 1) - 1, j = m * 2 + newfs - 1; m <= i; --i)
          if (SA[i] != 0)
            SA[j--] = SA[i] - 1;


        RA = new IntArray(SA, m + newfs);
        SaisMain(RA, SA, newfs, m, name);
        //RA = null;

        i = n - 1;
        j = m * 2 - 1;
        c0 = T[n - 1];

        do {
          c1 = c0;
        }
        while ((0 <= --i) && ((c0 = T[i]) >= c1));

        for (; 0 <= i;) {
          do {
            c1 = c0;
          }
          while ((0 <= --i) && ((c0 = T[i]) <= c1));

          if (0 <= i) {
            SA[j--] = i + 1;

            do {
              c1 = c0;
            }
            while ((0 <= --i) && ((c0 = T[i]) >= c1));
          }
        }

        for (i = 0; i < m; ++i)
          SA[i] = SA[m + SA[i]];

        if ((flags & 4) != 0)
          C = B = new IntArray(new int[k], 0);

        if ((flags & 2) != 0)
          B = new IntArray(new int[k], 0);
      }

      /* stage 3: induce the result for the original problem */
      if ((flags & 8) != 0)
        GetCounts(T, C, n, k);

      /* put all left-most S characters into their buckets */
      if (1 < m) {
        GetBuckets(C, B, k, true); /* find ends of buckets */

        i = m - 1;
        j = n;
        p = SA[m - 1];
        c1 = T[p];

        do {
          q = B[c0 = c1];

          while (q < j)
            SA[--j] = 0;

          do {
            SA[--j] = p;

            if (--i < 0)
              break;

            p = SA[i];
          }
          while ((c1 = T[p]) == c0);
        }
        while (0 <= i);

        while (0 < j)
          SA[--j] = 0;
      }

      InduceSA(T, SA, C, B, n, k);

      return pidx;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Linear Time Suffix Array
    /// </summary>
    public static int[] SuffixArray(this string value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));
      else if (value.Length <= 1)
        return new int[value.Length];

      int[] result = new int[value.Length];

      SaisMain(new StringArray(value, 0), result, 0, result.Length, 65536);

      return result;
    }

    #endregion Public
  }

}
