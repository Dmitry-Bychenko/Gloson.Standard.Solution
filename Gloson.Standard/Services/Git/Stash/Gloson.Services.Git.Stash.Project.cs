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

    private List<StashRepository> m_Items;

    #endregion Private Data

    #region Algorithm

    private void CoreLoadRepositories() {
      if (null != m_Items)
        return;

      m_Items = new List<StashRepository>();

      foreach (var json in Storage.Query($"projects/{Key}/repos"))
        m_Items.Add(new StashRepository(this, json));
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
    public IReadOnlyList<StashRepository> Repositories {
      get {
        CoreLoadRepositories();

        return m_Items;
      }
    }



    #endregion Public
  }

}
