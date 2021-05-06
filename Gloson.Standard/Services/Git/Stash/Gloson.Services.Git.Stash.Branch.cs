using Gloson.Json;
using System;
using System.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gloson.Services.Git.Stash {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Stash Branch
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StashBranch {
    #region Private Data

    private static readonly Regex s_VersionRegex = new(@"[0-9]+(\.[0-9]+)+");

    #endregion Private Data

    #region Create

    internal StashBranch(StashRepository repository, JsonValue json) {
      Repository = repository;

      Id = json.Value("id");
      Name = json.Value("displayId");
      IsDefault = json.Value("isDefault");
      Kind = json.Value("type");

      LatestChangeId = json.Value("latestChangeset");
      LatestCommitId = json.Value("latestCommit");

      var match = s_VersionRegex.Match(Id);

      if (match.Success && Version.TryParse(match.Value, out Version ver))
        Version = ver;
      else
        Version = new Version(0, 0);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Repository
    /// </summary>
    public StashRepository Repository { get; }

    /// <summary>
    /// Project
    /// </summary>
    public StashProject Project => Repository?.Project;

    /// <summary>
    /// Storage
    /// </summary>
    public StashStorage Storage => Repository?.Project?.Storage;

    /// <summary>
    /// Ssh
    /// </summary>
    public Uri Ssh => Repository?.Ssh;

    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Is Default
    /// </summary>
    public bool IsDefault { get; }

    /// <summary>
    /// Kind
    /// </summary>
    public string Kind { get; }

    /// <summary>
    /// Latest change
    /// </summary>
    public string LatestChangeId { get; }

    /// <summary>
    /// Latest Commit
    /// </summary>
    public string LatestCommitId { get; }

    /// <summary>
    /// Version (if any)
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Clone To
    /// </summary>
    public GitResult CloneTo(string path) {
      return GitController
        .Default
        .Execute(GitCommandBuilder.Clone(Ssh, Name, path, true));
    }

    /// <summary>
    /// Clone To
    /// </summary>
    public Task<GitResult> CloneToAsync(string path) {
      return GitController
        .Default
        .ExecuteAsync(GitCommandBuilder.Clone(Ssh, Name, path, true));
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Name} at {Repository?.Slug} ({Ssh})";

    #endregion Public
  }
}
