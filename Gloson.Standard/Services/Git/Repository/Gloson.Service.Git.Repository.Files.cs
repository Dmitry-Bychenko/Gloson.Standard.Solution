using Gloson.Text;
using System;
using System.IO;

namespace Gloson.Services.Git.Repository {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Repository File
  /// </summary>
  //
  // https://github.com/joshnh/Git-Commands 
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitRepoFile
    : IEquatable<GitRepoFile>,
      IComparable<GitRepoFile> {

    #region Create

    internal GitRepoFile(string record, GitRepo repo) {
      Repo = repo;
      Tag = "";
      Object = "";

      int p = record.IndexOf('\t');

      if (p < 0) {
        FileName = Path.Combine(Repo.Location, record);

        return;
      }

      FileName = Path.Combine(Repo.Location, record[(p + 1)..]);

      string[] items = record.Substring(0, p).Split(' ');

      if (items.Length >= 4) {
        Tag = items[0];
        Mode = int.Parse(items[1]);
        Object = items[2];
        Stage = int.Parse(items[3]);
      }
      else {
        Mode = int.Parse(items[0]);
        Object = items[1];
        Stage = int.Parse(items[2]);
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(GitRepoFile left, GitRepoFile right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      return StringComparers.StandardOrdinalComparer.Compare(left.FileName, right.FileName);
    }

    /// <summary>
    /// Repository
    /// </summary>
    public GitRepo Repo { get; }

    /// <summary>
    /// File Name
    /// </summary>
    public String FileName { get; }

    /// <summary>
    /// Tag
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// Mode
    /// </summary>
    public int Mode { get; }

    /// <summary>
    /// Object
    /// </summary>
    public string Object { get; }

    /// <summary>
    /// Stage
    /// </summary>
    public int Stage { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => FileName;

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(GitRepoFile left, GitRepoFile right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(GitRepoFile left, GitRepoFile right) => Compare(left, right) != 0;

    /// <summary>
    /// More Or Equals
    /// </summary>
    public static bool operator >=(GitRepoFile left, GitRepoFile right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less Or Equals
    /// </summary>
    public static bool operator <=(GitRepoFile left, GitRepoFile right) => Compare(left, right) <= 0;

    /// <summary>
    /// Nore
    /// </summary>
    public static bool operator >(GitRepoFile left, GitRepoFile right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(GitRepoFile left, GitRepoFile right) => Compare(left, right) < 0;

    #endregion Operators

    #region IEquatable<GitRepoFile>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(GitRepoFile other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return string.Equals(FileName, other.FileName, StringComparison.Ordinal);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(Object o) => Equals(o as GitRepoFile);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return FileName is null ? 0 : FileName.GetHashCode();
    }

    #endregion IEquatable<GitRepoFile>

    #region IComparable<GitRepoFile>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(GitRepoFile other) => Compare(this, other);

    #endregion IComparable<GitRepoFile>
  }

}
