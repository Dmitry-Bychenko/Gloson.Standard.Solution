using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;

using Gloson;
using Gloson.Json;

namespace Gloson.Services.Git.Stash {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Stash Repository
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StashRepository {
    #region Private Data

    private readonly Lazy<List<StashBranch>> m_Branches;

    #endregion Private Data

    #region Algorithm

    private List<StashBranch> CoreCreateBranches() {
      List<StashBranch> result = new List<StashBranch>();

      foreach (var json in Storage.Query($"projects/{Project.Key}/repos/{Slug}/branches"))
        result.Add(new StashBranch(this, json));

      return result;
    }

    #endregion Algorithm

    #region Create

    internal StashRepository(StashProject project, JsonValue json) {
      Project = project;

      Slug       = json.Value("slug");
      Id         = json.Value("id");
      Key        = json.Value("name");
      IsForkable = json.Value("forkable");

      m_Branches = new Lazy<List<StashBranch>>(CoreCreateBranches);

      if (json.Value("links")?.Value("clone") is JsonArray array && array != null) 
        foreach (JsonValue item in array) {
          string hRef = item.Value("href");

          if (hRef != null && hRef.Trim().StartsWith("ssh://", StringComparison.OrdinalIgnoreCase)) {
            Ssh = new Uri(hRef.Trim());

            break;
          }
        }

    }

    #endregion Create

    #region Public

    /// <summary>
    /// Project
    /// </summary>
    public StashProject Project { get; }

    /// <summary>
    /// Storage
    /// </summary>
    public StashStorage Storage => Project?.Storage;

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Slug
    /// </summary>
    public string Slug { get; }

    /// <summary>
    /// Is forkable
    /// </summary>
    public bool IsForkable { get; }

    /// <summary>
    /// SSH
    /// </summary>
    public Uri Ssh { get; }

    /// <summary>
    /// Branches
    /// </summary>
    public IReadOnlyList<StashBranch> Branches => m_Branches.Value;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Project?.Key}.{Id} ({Ssh})";

    #endregion Public
  }

}
