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
    #endregion Private Data

    #region Algorithm
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
    /// Controller
    /// </summary>
    public StashController Controller => Project?.Controller;

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

    #endregion Public
  }

}
