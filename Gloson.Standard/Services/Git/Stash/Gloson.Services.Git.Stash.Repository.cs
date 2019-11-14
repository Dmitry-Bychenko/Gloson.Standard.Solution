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

    private List<StashBranch> m_Branches;

    #endregion Private Data

    #region Algorithm

    private void CoreCreateBranches() {
      if (null != m_Branches)
        return;

      m_Branches = new List<StashBranch>();

      foreach (var json in Storage.Query($"projects/{Project.Key}/repos/{Slug}/branches"))
        m_Branches.Add(new StashBranch(this, json));
    }

    #endregion Algorithm

    #region Create

    internal StashRepository(StashProject project, JsonValue json) {
      Project = project;

      Slug       = json.Value("slug");
      Id         = json.Value("id");
      Key        = json.Value("name");
      IsForkable = json.Value("forkable");

      JsonArray array = json.Value("links")?.Value("clone") as JsonArray;

      if (array != null) {
        foreach (JsonValue item in array) {
          string hRef = item.Value("href");

          if (hRef != null && hRef.Trim().StartsWith("ssh://", StringComparison.OrdinalIgnoreCase)) {
            Ssh = new Uri(hRef.Trim());

            break;
          }
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
    public IReadOnlyList<StashBranch> Branches {
      get {
        CoreCreateBranches();

        return m_Branches;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Project?.Key}.{Id} ({Ssh})";

    #endregion Public
  }

}
