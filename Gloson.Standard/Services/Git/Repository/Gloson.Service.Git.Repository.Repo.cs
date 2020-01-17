using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Gloson.IO;

namespace Gloson.Services.Git.Repository {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitRepo {
    #region Algorithm
    #endregion Algorithm

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

      if (Directory.Exists(gitDir))
        throw new ArgumentException("Not a repositary Directory", nameof(directory));

      GitRepo result = new GitRepo();

      result.Location = directory;

      //result.Ssh = ssh;

      return result;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Directory
    /// </summary>
    public string Location { get; private set; } 

    /// <summary>
    /// SSH
    /// </summary>
    public Uri Ssh { get; private set; }

    /// <summary>
    /// Branch
    /// </summary>
    public string Branch => Perform("rev-parse --abbrev-ref HEAD");

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
  }
}
