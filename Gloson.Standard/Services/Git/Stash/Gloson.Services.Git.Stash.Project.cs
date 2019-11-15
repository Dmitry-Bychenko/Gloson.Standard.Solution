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
  /// Stash Project
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StashProject {
    #region Private Data

    private readonly Lazy<List<StashRepository>> m_Items = null;

    #endregion Private Data

    #region Algorithm

    private List<StashRepository> CoreLoadRepositories() {
      List<StashRepository> result  = new List<StashRepository>();

      foreach (var json in Storage.Query($"projects/{Key}/repos"))
        result.Add(new StashRepository(this, json));

      return result;
    }

    #endregion Algorithm

    #region Create

    internal StashProject(StashStorage controller, JsonValue json) {
      Storage = controller;

      Id          = json.Value("id");
      Key         = json.Value("key");
      Title       = json.Value("name");
      IsPublic    = json.Value("public");

      Description = json.Value("description") ?? "";

      m_Items = new Lazy<List<StashRepository>>(CoreLoadRepositories);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Storage
    /// </summary>
    public StashStorage Storage { get; }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Is Public
    /// </summary>
    public bool IsPublic { get; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Repositories
    /// </summary>
    public IReadOnlyList<StashRepository> Repositories => m_Items.Value;

    #endregion Public
  }

}
