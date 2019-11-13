using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Services.Git {

  //---------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Local Repository
  /// </summary>
  //
  //---------------------------------------------------------------------------------------------------------

  public sealed class GitLocalRepository {
    #region Private Data

    private string m_Branch;

    private Uri m_Ssh;

    private GitPerson m_Author;

    private GitPerson m_Commiter;

    #endregion Private Data

    #region Algorithm

    private void CoreUpdateAuthorAndCommiter() {
      string format = "%an||%ae||%aI||%cn||%ce||%cI";

      var items = Execute($"--no-pager show -s --format=\"{format}\"")
        .Out.Split(new string[] { "||" }, StringSplitOptions.None);

      m_Author = new GitPerson(items[0], items[1], items[2]);
      m_Commiter = new GitPerson(items[3], items[4], items[5]);
    }

    #endregion Algorithm

    #region Create

    // Standard constructor
    private GitLocalRepository(string location) {
      Location = location;

      var result = GitController.Default.TryExecute(GitCommandBuilder.Format(Location, "%H"));
        
      Hash = result ? "" : result.Out.ToLowerInvariant();
    }

    /// <summary>
    /// Is valid repository
    /// </summary>
    public static bool IsRepository(string path) {
      if (string.IsNullOrWhiteSpace(path))
        path = Environment.CurrentDirectory;

      if (!Directory.Exists(path))
        return false;

      var result = GitController.Default.TryExecute(GitCommandBuilder.Format(path, "%H"));
        
      return !string.IsNullOrEmpty(result.Out) &&
              result.Out.All(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F');
    }

    /// <summary>
    /// Open Existing repository
    /// </summary>
    /// <param name="path">Path</param>
    /// <returns></returns>
    public static GitLocalRepository Open(string path) {
      if (!IsRepository(path))
        throw new ArgumentException($"Provided path {path} is not a valid Git Local Repository", nameof(path));

      GitLocalRepository repo = new GitLocalRepository(path);

      return repo;
    }

    /// <summary>
    /// Create a new repository by cloning
    /// </summary>
    /// <param name="ssh">SSH</param>
    /// <param name="branch">Branch</param>
    /// <param name="path">Path</param>
    /// <returns></returns>
    public static GitLocalRepository Clone(Uri ssh, string branch, string path) {
      if (null == ssh)
        throw new ArgumentNullException(nameof(ssh));

      if (string.IsNullOrEmpty(path))
        path = Environment.CurrentDirectory;

      if (Directory.Exists(path)) 
        if (Directory.EnumerateDirectories(path).Any() || Directory.EnumerateFiles(path).Any())
          throw new ArgumentException($"Directory {path} already exist.", nameof(path));
      
      Directory.CreateDirectory(path);

      var result = GitController.Default.TryExecute(GitCommandBuilder.Clone(ssh, branch, path, true));

      if (result) {
        GitLocalRepository repo = new GitLocalRepository(path);

        repo.m_Ssh = ssh;
        repo.m_Branch = branch;

        return repo;
      }
      else
        result.ThrowIfError();

      return null;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Location
    /// </summary>
    public String Location { get; }

    /// <summary>
    /// Hash
    /// </summary>
    public String Hash { get; }

    /// <summary>
    /// Branch
    /// </summary>
    public String Branch {
      get {
        if (!string.IsNullOrWhiteSpace(m_Branch))
          return m_Branch;

        m_Branch = Execute("branch --no-color").Out.TrimStart('*', ' ');

        return m_Branch;
      }
    }

    /// <summary>
    /// SSH
    /// </summary>
    public Uri Ssh { 
      get {
        if (null != m_Ssh)
          return m_Ssh;

        m_Ssh = new Uri(Execute("remote get-url origin").Out);

        return m_Ssh;
      }
    }

    /// <summary>
    /// Author
    /// </summary>
    public GitPerson Author { 
      get {
        if (null != m_Author)
          return m_Author;

        CoreUpdateAuthorAndCommiter();

        return m_Author;
      } 
    }
    
    /// <summary>
    /// Commiter
    /// </summary>
    public GitPerson Commiter {
      get {
        if (null != m_Commiter)
          return m_Commiter;

        CoreUpdateAuthorAndCommiter();

        return m_Commiter;
      }
    }

    /// <summary>
    /// Try Execute Async
    /// </summary>
    public Task<GitResult> TryExecuteAsync(string command, CancellationToken token) =>
      GitController.Default.TryExecuteAsync(GitCommandBuilder.WithDirectory(command, Location), token);
    
    /// <summary>
    /// Try Execute Async
    /// </summary>
    public Task<GitResult> TryExecuteAsync(string command) => 
      TryExecuteAsync(command, CancellationToken.None);

    /// <summary>
    /// Execute Async
    /// </summary>
    public Task<GitResult> ExecuteAsync(string command, CancellationToken token) =>
      GitController.Default.ExecuteAsync(GitCommandBuilder.WithDirectory(command, Location), token);

    /// <summary>
    /// Execute Async
    /// </summary>
    public Task<GitResult> ExecuteAsync(string command) =>
      ExecuteAsync(command, CancellationToken.None);

    /// <summary>
    /// Try Execute
    /// </summary>
    public GitResult TryExecute(string command) =>
      GitController.Default.TryExecute(GitCommandBuilder.WithDirectory(command, Location));

    /// <summary>
    /// Execute
    /// </summary>
    public GitResult Execute(string command) =>
      GitController.Default.Execute(GitCommandBuilder.WithDirectory(command, Location));

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"Git Local Repo {Hash} at {Location}";

    #endregion Public
  }
}
