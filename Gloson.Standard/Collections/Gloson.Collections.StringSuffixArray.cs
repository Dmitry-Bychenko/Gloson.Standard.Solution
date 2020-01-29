using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Collections {

  //---------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard compressed suffix array (Invariant culture, ordinal comparison)
  /// </summary>
  //
  //---------------------------------------------------------------------------

  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  public sealed class StringSuffixArray : IReadOnlyList<String> {
    #region Private Data

    // Value
    private String m_Value = "";
    // Suffix array
    private int[] m_Array;

    #endregion Private Data

    #region Algorithm

    // Update array
    private void CoreUpdate() {
      if (!Object.ReferenceEquals(null, m_Array))
        return;

      m_Array = new int[m_Value.Length + 1];

      for (int i = m_Value.Length; i >= 0; --i)
        m_Array[i] = i;

      Array.Sort<int>(m_Array,
        (left, right) => {
          int n = left < right ? left : right;
          int length = m_Value.Length;

          for (int i = 0; i < n; ++i) {
            Char chLeft = m_Value[length - left + i];
            Char chRight = m_Value[length - right + i];

            if (chLeft < chRight)
              return -1;
            else if (chLeft > chRight)
              return 1;
          }

          if (left < right)
            return -1;
          else if (left > right)
            return 1;

          return 0;
        });
    }

    // Split to seeds
    private static List<String> SplitToSeeds(String value, int tolerance) {
      List<String> result = new List<String>();

      int size = value.Length / (tolerance + 1);

      for (int i = 0; i <= tolerance; ++i)
        if (i < tolerance)
          result.Add(value.Substring(i * size, size));
        else
          result.Add(value.Substring(i * size));

      return result;
    }

    /// <summary>
    /// Compare value with suffix
    /// </summary>
    /// <param name="value">value to compare</param>
    /// <param name="index">Suffix index</param>
    /// <returns>-1, 0, 1</returns>
    private int Compare(String value, int index) {
      CoreUpdate();

      if ((index < 0) || (index >= m_Array.Length)) {
        if (Object.ReferenceEquals(null, value))
          return 0;
        else
          return 1;
      }

      if (Object.ReferenceEquals(null, value))
        return -1;

      int length = m_Array[index];
      int n = value.Length < length ? n = value.Length : length;

      for (int i = 0; i < n; ++i) {
        Char ch1 = value[i];
        Char ch2 = m_Value[m_Value.Length - length + i];

        if (ch1 > ch2)
          return 1;
        else if (ch1 < ch2)
          return -1;
      }

      if (value.Length > length)
        return 1;
      else if (value.Length < length)
        return -1;
      else
        return 0;
    }

    /// <summary>
    /// Compare value with suffix
    /// </summary>
    /// <param name="value">value to compare</param>
    /// <param name="index">Suffix index</param>
    /// <returns>int.MinValue, -1, 0, 1, int.MaxValue</returns>
    private int CompareExtended(String value, int index) {
      CoreUpdate();

      if ((index < 0) || (index >= m_Array.Length)) {
        if (Object.ReferenceEquals(null, value))
          return 0;
        else
          return int.MaxValue;
      }

      if (Object.ReferenceEquals(null, value))
        return int.MinValue;

      int length = m_Array[index];
      int n = value.Length < length ? n = value.Length : length;

      for (int i = 0; i < n; ++i) {
        Char ch1 = value[i];
        Char ch2 = m_Value[m_Value.Length - length + i];

        if (ch1 > ch2)
          return int.MaxValue;
        else if (ch1 < ch2)
          return int.MinValue;
      }

      if (value.Length > length)
        return 1;
      else if (value.Length < length)
        return -1;
      else
        return 0;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructror
    /// </summary>
    public StringSuffixArray() {
    }

    /// <summary>
    /// Standard constructror
    /// </summary>
    public StringSuffixArray(String value)
      : this() {

      Value = value;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public String Value {
      get {
        return m_Value;
      }
      set {
        if (String.IsNullOrEmpty(value))
          value = "";

        if (String.Equals(value, m_Value, StringComparison.Ordinal))
          return;

        m_Value = value;
        m_Array = null;
      }
    }

    /// <summary>
    /// Dichotomy search: index to start checking suffixes 
    /// </summary>
    public int StartIndexOf(String value) {
      CoreUpdate();

      if (m_Array.Length <= 0)
        return 0;

      int low = 0;
      int high = m_Array.Length - 1;

      if (Compare(value, 0) <= 0)
        return 0;
      else if (Compare(value, high) > 0)
        return high;

      while (true) {
        int middle = (low + high) / 2;

        int compare = CompareExtended(value, middle);

        if (compare == int.MaxValue)
          low = middle;
        else if (compare == int.MinValue)
          high = middle;
        else if (compare == 0)
          return middle;
        else if (compare == 1)
          low = middle;
        else if (compare == -1)
          high = middle;

        if (high == low)
          return high;
        else if ((high - low) == 1) {
          if (CompareExtended(value, low) >= 0)
            return high;
          else
            return low;
        }
      }
    }

    /// <summary>
    /// Indexes of substring (in Value)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    public List<int> IndexesOf(String value) {
      List<int> result = new List<int>();

      if (String.IsNullOrEmpty(value))
        return result;
      else if (String.IsNullOrEmpty(m_Value))
        return result;
      else if (value.Length > m_Value.Length)
        return result;

      CoreUpdate();

      int start = StartIndexOf(value);

      for (int i = start; i < m_Array.Length; ++i) {
        if (!this[i].StartsWith(value, StringComparison.Ordinal))
          break;

        result.Add(m_Value.Length - SuffixLength(i));
      }

      result.Sort();

      return result;
    }

    /// <summary>
    /// Indexes of unexact substrings (in Value)
    /// </summary>
    /// <param name="value">Substring to find</param>
    /// <param name="tolerance">Errors number tolerated in the substring</param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    public List<int> IndexesOf(String value, int tolerance) {
      if (tolerance <= 0)
        return IndexesOf(value);

      List<int> result = new List<int>();

      if (String.IsNullOrEmpty(value))
        return result;
      else if (String.IsNullOrEmpty(m_Value))
        return result;
      else if (value.Length > m_Value.Length)
        return result;

      if (tolerance >= value.Length) {
        for (int i = 0; i < m_Value.Length - value.Length; ++i)
          result.Add(i);

        return result;
      }

      CoreUpdate();

      int size = value.Length / (tolerance + 1);

      List<String> seeds = SplitToSeeds(value, tolerance);
      HashSet<int> hs = new HashSet<int>();

      for (int j = 0; j < seeds.Count; ++j) {
        int start = StartIndexOf(seeds[j]);
        String seed = seeds[j];

        for (int k = start; k < m_Array.Length; ++k) {
          if (!this[k].StartsWith(seed, StringComparison.Ordinal))
            break;

          int index = m_Value.Length - SuffixLength(k) - j * size;

          if (index < 0)
            continue;
          else if (index + value.Length > m_Value.Length)
            continue;

          int errors = 0;

          for (int p = 0; p < value.Length; ++p)
            if (value[p] != m_Value[index + p])
              errors += 1;

          if (errors <= tolerance)
            hs.Add(index);
        }
      }

      foreach (int v in hs)
        result.Add(v);

      result.Sort();

      return result;
    }

    /// <summary>
    /// Indexes of substrings (in Value)
    /// </summary>
    /// <param name="values">Substrings</param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    public List<int> IndexesOf(IEnumerable<String> values) {
      List<int> result = new List<int>();

      if (Object.ReferenceEquals(null, values))
        return result;
      else if (String.IsNullOrEmpty(m_Value))
        return result;

      CoreUpdate();

      foreach (String value in values) {
        if (String.IsNullOrEmpty(value))
          continue;
        else if (value.Length > m_Value.Length)
          continue;

        int start = StartIndexOf(value);

        for (int i = start; i < m_Array.Length; ++i) {
          if (!this[i].StartsWith(value, StringComparison.Ordinal))
            break;

          result.Add(m_Value.Length - SuffixLength(i));
        }
      }

      result.Sort();

      return result;
    }

    /// <summary>
    /// Indexes of inexact substrings (in Value)
    /// </summary>
    /// <param name="values">Substrings to find</param>
    /// <param name="tolerance">Errors number tolerated in the substring</param>
    /// <returns></returns>
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
    public List<int> IndexesOf(IEnumerable<String> values, int tolerance) {
      List<int> result = new List<int>();

      if (Object.ReferenceEquals(null, values))
        return result;
      else if (String.IsNullOrEmpty(m_Value))
        return result;

      CoreUpdate();

      foreach (String value in values) {
        if (String.IsNullOrEmpty(value))
          continue;
        else if (String.IsNullOrEmpty(m_Value))
          continue;
        else if (value.Length > m_Value.Length)
          return result;

        if (tolerance >= value.Length) {
          for (int i = 0; i < m_Value.Length - value.Length; ++i)
            result.Add(i);

          continue;
        }

        int size = value.Length / (tolerance + 1);

        List<String> seeds = SplitToSeeds(value, tolerance);
        HashSet<int> hs = new HashSet<int>();

        for (int j = 0; j < seeds.Count; ++j) {
          int start = StartIndexOf(seeds[j]);
          String seed = seeds[j];

          for (int k = start; k < m_Array.Length; ++k) {
            if (!this[k].StartsWith(seed, StringComparison.Ordinal))
              break;

            int index = m_Value.Length - SuffixLength(k) - j * size;

            if (index < 0)
              continue;
            else if (index + value.Length > m_Value.Length)
              continue;

            int errors = 0;

            for (int p = 0; p < value.Length; ++p)
              if (value[p] != m_Value[index + p])
                errors += 1;

            if (errors <= tolerance) {
              hs.Add(index);

              //result.Add(index);
            }
          }
        }

        foreach (int v in hs)
          result.Add(v);
      }

      result.Sort();

      return result;
    }

    /// <summary>
    /// Suffix size at position
    /// </summary>
    public int SuffixLength(int index) {
      CoreUpdate();

      return m_Array[index];
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode() {
      return m_Value.GetHashCode();
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override Boolean Equals(Object obj) {
      if (Object.ReferenceEquals(this, obj))
        return true;

      StringSuffixArray other = obj as StringSuffixArray;

      if (Object.ReferenceEquals(null, other))
        return false;

      return String.Equals(m_Value, other.m_Value, StringComparison.Ordinal);
    }

    /// <summary>
    /// To string (value)
    /// </summary>
    public override String ToString() {
      return m_Value;
    }

    /// <summary>
    /// To debug report
    /// </summary>
    public String ToDebugReport(int size) {
      CoreUpdate();

      if (size <= 0)
        size = m_Array.Length;

      int n = size < m_Array.Length ? size : m_Array.Length;

      StringBuilder Sb = new StringBuilder();

      for (int i = 0; i < n; ++i) {
        if (i > 0)
          Sb.AppendLine();

        Sb.Append(this[i]);
      }

      return Sb.ToString();
    }

    /// <summary>
    /// To debug report
    /// </summary>
    public String ToDebugReport() {
      return ToDebugReport(-1);
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static Boolean operator ==(StringSuffixArray left, StringSuffixArray right) {
      if (Object.ReferenceEquals(left, right))
        return true;
      else if (Object.ReferenceEquals(left, null))
        return false;
      else if (Object.ReferenceEquals(right, null))
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public static Boolean operator !=(StringSuffixArray left, StringSuffixArray right) {
      if (Object.ReferenceEquals(left, right))
        return false;
      else if (Object.ReferenceEquals(left, null))
        return true;
      else if (Object.ReferenceEquals(right, null))
        return true;

      return left.Equals(right);
    }

    /// <summary>
    /// From string
    /// </summary>
    public static StringSuffixArray FromString(String value) {
      return new StringSuffixArray(value);
    }

    /// <summary>
    /// From string
    /// </summary>
    public static implicit operator StringSuffixArray(String value) {
      return FromString(value);
    }

    #endregion Operators

    #region IReadOnlyList<string> Members

    /// <summary>
    /// Items
    /// </summary>
    public String this[int index] {
      get {
        CoreUpdate();

        int length = m_Array[index];

        return m_Value.Substring(m_Value.Length - length, length);
      }
    }

    #endregion IReadOnlyList<string> Members

    #region IReadOnlyCollection<string> Members

    /// <summary>
    /// Count
    /// </summary>
    public int Count {
      get {
        CoreUpdate();

        return m_Array.Length;
      }
    }

    #endregion IReadOnlyCollection<string> Members

    #region IEnumerable<string> Members

    /// <summary>
    /// Enumerate
    /// </summary>
    public IEnumerator<String> GetEnumerator() {
      CoreUpdate();

      for (int i = 0; i < m_Array.Length; ++i) {
        int length = m_Array[i];

        yield return m_Value.Substring(m_Value.Length - length, length);
      }

    }

    #endregion IEnumerable<string> Members

    #region IEnumerable Members

    /// <summary>
    /// Enumerate
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }

    #endregion IEnumerable Members
  }

}
