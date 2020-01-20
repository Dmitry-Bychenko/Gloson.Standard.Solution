using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Gloson.IO;
using Gloson.Text;

namespace Gloson.Services.Git.Repository {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Grit Repository Branch
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitRepoBranch 
    : IEquatable<GitRepoBranch>,
      IComparable<GitRepoBranch> {

    #region Create

    internal GitRepoBranch(string value, GitRepo repo) {
      Repo = repo;
      IsCurrent = false;

      if (value.StartsWith("*")) {
        IsCurrent = true;
        value = value.TrimStart('*');
      }

      Name = value.Trim();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(GitRepoBranch left, GitRepoBranch right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, null))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;

      int result = GitRepo.Compare(left.Repo, right.Repo);

      if (result != 0)
        return result;

      return StringComparers.StandardOrdinalComparer.Compare(left.Name, right.Name);
    }

    /// <summary>
    /// Repository
    /// </summary>
    public GitRepo Repo { get; }

    /// <summary>
    /// Is Current
    /// </summary>
    public bool IsCurrent { get; }

    /// <summary>
    /// Branch Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// To String (Name)
    /// </summary>
    public override string ToString() => Name;

    #endregion Public

    #region IEquatable<GitRepoBranch>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(GitRepoBranch other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      if (!GitRepo.Equals(Repo, other.Repo))
        return false;

      return string.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as GitRepoBranch);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (Repo == null ? 0 : Repo.GetHashCode()) ^
             (Name == null ? 0 : Name.GetHashCode());
    }

    #endregion IEquatable<GitRepoBranch> 

    #region IComparable<GitRepoBranch>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(GitRepoBranch other) => Compare(this, other);

    #endregion IComparable<GitRepoBranch>
  }
}
