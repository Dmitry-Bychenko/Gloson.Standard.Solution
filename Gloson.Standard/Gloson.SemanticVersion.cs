using System;
using System.Collections.Generic;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Semantic version 2.0.0.0
  /// </summary>
  /// <see cref="https://semver.org/"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SemanticVersion :
      IEquatable<SemanticVersion>,
      IComparable<SemanticVersion>,
      IFormattable {

    #region Private Data

    // Main Version: Major.Minor.Patch
    private int[] m_MainVersion;
    // Prerelease
    private string[] m_Prerelease;
    // Build meta
    private string[] m_BuildMetaData;

    #endregion Private Data

    #region Algorithm

    // Compare arrays
    private static int CompareArrays(string[] left, string[] right) {
      int result;

      for (int i = 0; i < Math.Min(left.Length, right.Length); ++i) {
        string vLeft = left[i];
        string vRight = right[i];

        if (int.TryParse(vLeft, out var v1) && int.TryParse(vRight, out var v2)) {
          if (v1 > v2)
            return 1;
          else if (v1 < v2)
            return -1;
        }
        else {
          result = string.Compare(vLeft, vRight, StringComparison.OrdinalIgnoreCase);

          if (result == 0)
            result = string.Compare(vLeft, vRight, StringComparison.Ordinal);

          if (result != 0)
            return result;
        }
      }

      if ((result = left.Length.CompareTo(right.Length)) != 0)
        return result;

      return 0;
    }

    #endregion Algorithm

    #region Create 

    private SemanticVersion()
      : base() {
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out SemanticVersion result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      string main;
      string pre = "";
      string build = "";

      int p = value.IndexOf('-');

      if (p >= 0) {
        main = value.Substring(0, p);

        value = value[(p + 1)..];

        p = value.IndexOf('+');

        if (p >= 0) {
          pre = value.Substring(0, p);
          build = value.Substring(0, p + 1);
        }
        else
          pre = value;

      }
      else
        main = value;

      string[] items = main.Split('.');

      if (items.Length < 1 && items.Length > 3)
        return false;

      List<int> list = new List<int>();

      foreach (var item in items)
        if (int.TryParse(item, out var v))
          if (v < 0)
            return false;
          else
            list.Add(v);

      while (list.Count < 3)
        list.Add(0);

      result = new SemanticVersion() {
        m_MainVersion = list.ToArray(),
        m_Prerelease = pre.Split('.'),
        m_BuildMetaData = build.Split('.'),
      };

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static SemanticVersion Parse(string value) {
      if (TryParse(value, out var result))
        return result;

      throw new FormatException("value provided can't be parsed to SemanticVersion");
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="major">Major</param>
    /// <param name="minor">Minor</param>
    /// <param name="build">Build</param>
    /// <param name="prerelease">PreRelease</param>
    /// <param name="metadata">MetaData</param>
    public SemanticVersion(int major, int minor, int build, string prerelease, string metadata) {
      if (major < 0)
        throw new ArgumentOutOfRangeException(nameof(major));
      else if (minor < 0)
        throw new ArgumentOutOfRangeException(nameof(minor));
      else if (build < 0)
        throw new ArgumentOutOfRangeException(nameof(build));

      m_MainVersion = new int[] { major, minor, build };

      if (!string.IsNullOrWhiteSpace(prerelease))
        m_Prerelease = prerelease.Trim().Split('.');
      else
        m_Prerelease = Array.Empty<string>();

      if (!string.IsNullOrWhiteSpace(metadata))
        m_BuildMetaData = metadata.Trim().Split('.');
      else
        m_BuildMetaData = Array.Empty<string>();
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="major">Major</param>
    /// <param name="minor">Minor</param>
    /// <param name="build">Build</param>
    /// <param name="prerelease">PreRelease</param>
    public SemanticVersion(int major, int minor, int build, string prerelease)
      : this(major, minor, build, prerelease, null) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="major">Major</param>
    /// <param name="minor">Minor</param>
    /// <param name="build">Build</param>
    public SemanticVersion(int major, int minor, int build)
      : this(major, minor, build, null, null) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="major">Major</param>
    /// <param name="minor">Minor</param>
    public SemanticVersion(int major, int minor)
      : this(major, minor, 0, null, null) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(SemanticVersion left, SemanticVersion right) {
      if (object.ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      int result;

      if ((result = left.Major.CompareTo(right.Major)) != 0)
        return result;
      else if ((result = left.Minor.CompareTo(right.Minor)) != 0)
        return result;
      else if ((result = left.Patch.CompareTo(right.Patch)) != 0)
        return result;

      result = CompareArrays(left.m_Prerelease, right.m_Prerelease);

      if (result != 0)
        return result;

      result = CompareArrays(left.m_BuildMetaData, right.m_BuildMetaData);

      if (result != 0)
        return result;

      return 0;
    }

    /// <summary>
    /// Major
    /// </summary>
    public int Major {
      get {
        return m_MainVersion[0];
      }
    }

    /// <summary>
    /// Minor
    /// </summary>
    public int Minor {
      get {
        return m_MainVersion[1];
      }
    }

    /// <summary>
    /// Patch
    /// </summary>
    public int Patch {
      get {
        return m_MainVersion[2];
      }
    }

    /// <summary>
    /// Version
    /// </summary>
    public IReadOnlyList<int> Version {
      get {
        return m_MainVersion;
      }
    }

    /// <summary>
    /// Prerelease
    /// </summary>
    public IReadOnlyList<string> PreRelease {
      get {
        return m_Prerelease;
      }
    }

    /// <summary>
    /// Build Meta Data
    /// </summary>
    public IReadOnlyList<string> BuildMetaData {
      get {
        return m_BuildMetaData;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      string main = string.Join(".", Version);
      string pre = string.Join(".", PreRelease);
      string build = string.Join(".", BuildMetaData);

      string result = main;

      if (!string.IsNullOrWhiteSpace(pre))
        result += "-" + pre;

      if (!string.IsNullOrWhiteSpace(build))
        result += "+" + build;

      return result;
    }

    #endregion Public

    #region Operators

    #region Comparisons

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) == 0;
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) != 0;
    }

    /// <summary>
    /// More or Equals
    /// </summary>
    public static bool operator >=(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) >= 0;
    }

    /// <summary>
    /// Less or Equals
    /// </summary>
    public static bool operator <=(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) <= 0;
    }

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) > 0;
    }

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(SemanticVersion left, SemanticVersion right) {
      return Compare(left, right) < 0;
    }

    #endregion Comparisons

    #region Cast

    /// <summary>
    /// String
    /// </summary>
    public static implicit operator string(SemanticVersion value) {
      if (value is null)
        return null;

      return value.ToString();
    }

    #endregion Cast

    #endregion Operators

    #region IEquatable<SemanticVersion>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(SemanticVersion other) {
      return Compare(this, other) == 0;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return base.Equals(obj as SemanticVersion);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (Major << 24) ^
             (Minor << 16) ^
                   (Patch) ^
             (m_Prerelease.Length > 0 ? m_Prerelease[0].GetHashCode() : 0) ^
             (m_BuildMetaData.Length > 0 ? m_BuildMetaData[0].GetHashCode() : 0);
    }

    #endregion IEquatable<SemanticVersion>

    #region IComparable<SemanticVersion>

    /// <summary>
    /// CompareTo
    /// </summary>
    public int CompareTo(SemanticVersion other) {
      return Compare(this, other);
    }

    #endregion IComparable<SemanticVersion>

    #region IFormattable

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format) => ToString(format, null);

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (string.IsNullOrEmpty(format))
        format = "g";

      if (string.Equals(format, "g", StringComparison.OrdinalIgnoreCase))
        return ToString();

      throw new FormatException($"Incorrect format \"{format}\" specifier");
    }

    #endregion IFormattable
  }

}
