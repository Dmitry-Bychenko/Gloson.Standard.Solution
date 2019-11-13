using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Gloson.Diagnostics;

namespace Gloson.Services.Git {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Controller
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitController {
    #region Private Data

    private static readonly Lazy<GitController> s_Default = new Lazy<GitController>(
      () => new GitController());

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="gitPath"></param>
    public GitController(string gitPath) {
      if (string.IsNullOrWhiteSpace(gitPath))
        gitPath = "git";

      Location = gitPath;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public GitController() 
      : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Default Git Controller
    /// </summary>
    public static GitController Default => s_Default.Value;

    /// <summary>
    /// Location
    /// </summary>
    public string Location { get; }

    /// <summary>
    /// Version
    /// </summary>
    public Version Version {
      get {
        GitResult result = TryExecute("version");

        if (!result)
          return new Version(0, 0);

        string match = Regex.Match(result.Out, @"[0-9]+(\.[0-9]+)+").Value;

        if (string.IsNullOrEmpty(match))
          return new Version(0, 0);
        else
          return new Version(match);
      }
    }

    /// <summary>
    /// Try Execute a command
    /// </summary>
    public GitResult TryExecute(string command) {
      try {
        var result = ProcessExecutor.Execute(Location, command, Encoding.UTF8);

        return new GitResult(
          result.Out,
          result.Error,
          result.ExitCode);
      }
      catch (IOException) {
        return GitResult.GitNotFound;
      }
    }

    /// <summary>
    /// Try Execute (async version)
    /// </summary>
    public async Task<GitResult> TryExecuteAsync(string command, CancellationToken token) {
      var task = ProcessExecutor.ExecuteAsync(Location, token, command, Encoding.UTF8);

      try {
        var result = await task;

        return new GitResult(
          result.Out,
          result.Error,
          result.ExitCode);
      }
      catch (IOException) {
        return GitResult.GitNotFound;
      }
    }

    /// <summary>
    /// Try Execute (async version)
    /// </summary>
    public async Task<GitResult> TryExecuteAsync(string command) 
      => await TryExecuteAsync(command, CancellationToken.None);

    /// <summary>
    /// Execute
    /// </summary>
    public GitResult Execute(string command) {
      GitResult result = TryExecute(command);

      result.ThrowIfError();

      return result;
    }

    /// <summary>
    /// Execute (async version)
    /// </summary>
    public async Task<GitResult> ExecuteAsync(string command, CancellationToken token) {
      GitResult result = await TryExecuteAsync(command, token);

      result.ThrowIfError();

      return result;
    }

    /// <summary>
    /// Execute (async version)
    /// </summary>
    public async Task<GitResult> ExecuteAsync(string command) =>
      await ExecuteAsync(command, CancellationToken.None);

    #endregion Public
  }

}
