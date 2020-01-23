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
  /// Git Repository
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitRepo 
    : IEquatable<GitRepo>,
      IComparable<GitRepo> {

    #region Create

    // Standard constructor
    private GitRepo() {
    }

    /// <summary>
    /// Clone
    /// </summary>
    /// <param name="directory">Directory</param>
    /// <param name="ssh">SSH</param>
    /// <param name="branch">Branch</param>
    /// <param name="cloneSubModules">Clone Submodules</param>
    /// <returns></returns>
    public static GitRepo Clone(string directory, Uri ssh, string branch, bool cloneSubModules = true) {
      if (string.IsNullOrEmpty(branch))
        branch = "master";

      string command = GitCommandBuilder.Clone(ssh, branch, directory, cloneSubModules);

      GitController.Default.Execute(command);

      GitRepo result = new GitRepo();

      result.Location = directory;
      result.Ssh = ssh;

      return result;
    }

    /// <summary>
    /// Open Existing Repository
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static GitRepo Open(string directory) {
      if (string.IsNullOrEmpty(directory))
        directory = Environment.CurrentDirectory;

      string gitDir = Path.Combine(directory, ".git");

      if (!Directory.Exists(gitDir))
        throw new ArgumentException("Not a repositary Directory", nameof(directory));

      GitRepo result = new GitRepo();

      result.Location = directory;

      //result.Ssh = ssh;

      return result;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(GitRepo left, GitRepo right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, null))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;

      return StringComparers.StandardOrdinalComparer.Compare(left.Location, right.Location);
    }

    /// <summary>
    /// Directory
    /// </summary>
    public string Location { get; private set; } 

    /// <summary>
    /// Add File
    /// </summary>
    public void AddFile(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (!File.Exists(fileName))
        throw new ArgumentException($"File {fileName} doesn't exist.", nameof(fileName));

      Perform($"add -f -- {Environment.ExpandEnvironmentVariables(fileName).QuotationAdd('\"')}");
    }

    /// <summary>
    /// Add File
    /// </summary>
    public void RemoveFile(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (!File.Exists(fileName))
        throw new ArgumentException($"File {fileName} doesn't exist.", nameof(fileName));

      Perform($"reset -- {Environment.ExpandEnvironmentVariables(fileName).QuotationAdd('\"')}");
    }

    /// <summary>
    /// Files
    /// </summary>
    public IEnumerable<GitRepoFile> Files {
      get {
        return Perform($"ls-files -c -s -d -o -m -k")
          .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(item => new GitRepoFile(item, this));
      }
    }

    /// <summary>
    /// SSH
    /// </summary>
    public Uri Ssh { get; private set; }

    /// <summary>
    /// Branch
    /// </summary>
    public string Branch => Perform("rev-parse --abbrev-ref HEAD");

    /// <summary>
    /// Branches
    /// </summary>
    public IEnumerable<GitRepoBranch> Branches { 
      get {
        return Perform("branch -a")
          .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
          .Select(item => new GitRepoBranch(item, this));
      }
    }

    /// <summary>
    /// Commit
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public string Commit(string message) {
      if (null == message)
        message = "";

      message = string.Concat(message.Where(c => !char.IsControl(c)));

      return Perform("commit -m " + message.QuotationAdd('"'));
    }

    /// <summary>
    /// Push
    /// </summary>
    public void Push(string origin = null) {
      if (string.IsNullOrWhiteSpace(origin))
        origin = "origin";

      origin = string.Concat(origin.Where(c => !char.IsControl(c)));

      Perform($"push --force {origin}");
    }

    /// <summary>
    /// Commit And Push
    /// </summary>
    public void CommitAndPush(string message, string origin = null) {
      message = (null == message) 
        ? ""
        : string.Concat(message.Where(c => !char.IsControl(c)));

      Perform("commit -m " + message.QuotationAdd('"'));

      origin = string.IsNullOrWhiteSpace(origin)
        ? "origin"
        : string.Concat(origin.Where(c => !char.IsControl(c)));

      Perform($"push --force {origin}");
    }

    /// <summary>
    /// Pull
    /// </summary>
    public void Pull(string origin = null) {
      if (string.IsNullOrEmpty(origin))
        origin = "origin";

      Perform($"pull {origin.QuotationAdd()}");
    }

    /// <summary>
    /// Perform a command
    /// </summary>
    /// <param name="command">Command to perform</param>
    public string Perform(string command) {
      if (null == command)
        throw new ArgumentNullException(nameof(command));

      using (new CurrentDirectory(Location)) {
        return GitController.Default.Execute(command).Value;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"{Ssh} (branch: {Branch})";

    #endregion Public

    #region IEquatable<GitRepo>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(GitRepo other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return String.Equals(Location, other.Location);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as GitRepo);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Location == null ? 0 : Location.GetHashCode();

    #endregion IEquatable<GitRepo>

    #region IComparable<GitRepo>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(GitRepo other) => Compare(this, other);

    #endregion IComparable<GitRepo>
  }
}
