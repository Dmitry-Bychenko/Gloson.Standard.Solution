using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Services.Git {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Command Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GitCommandBuilder {
    #region Public

    /// <summary>
    /// Build Abstract Command
    /// </summary>
    /// <param name="items">Items</param>
    /// <returns></returns>
    public static string Build(IEnumerable<string> items) {
      if (items is null)
        throw new ArgumentNullException(nameof(items));

      return string.Join(" ", items
        .Select(item => item?.Trim())
        .Where(item => !string.IsNullOrWhiteSpace(item)));
    }

    /// <summary>
    /// Build Command (same as Build)
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static string BuildCommand(params string[] items) => Build(items);

    /// <summary>
    /// Build Clone command
    /// </summary>
    /// <param name="ssh">SSH to clone</param>
    /// <param name="branch">Branch to clone</param>
    /// <param name="targetPath">Target Path</param>
    /// <param name="cloneSubModules">If submodules should be cloned</param>
    public static string Clone(Uri ssh, string branch, string targetPath, bool cloneSubModules = true) {
      if (ssh is null)
        throw new ArgumentNullException(nameof(ssh));

      return BuildCommand(
        "clone -v",
         cloneSubModules ? "--recurse-submodules" : "",
       $"--branch {branch}",
       $"\"{ssh}\"",
       $"\"{targetPath}\"");
    }

    /// <summary>
    /// Format command
    /// </summary>
    /// <see cref="https://git-scm.com/docs/pretty-formats"/>
    /// <param name="targetPath">Path</param>
    /// <param name="format">Format</param>
    public static string Format(string targetPath, string format) {
      if (string.IsNullOrWhiteSpace(targetPath))
        targetPath = Environment.CurrentDirectory;

      if (format is null)
        throw new ArgumentNullException(nameof(format));

      return BuildCommand(
       $"-C \"{targetPath}\"",
       $"--no-pager show -s",
       $"--format=\"{format}\"");
    }

    /// <summary>
    /// With directory mentioned
    /// </summary>
    /// <param name="command">Command</param>
    /// <param name="directory">Directory to add</param>
    /// <returns>Command with directory mentioned</returns>
    public static string WithDirectory(string command, string directory) {
      string dir = $"-C \"{directory}\"";

      if (string.IsNullOrWhiteSpace(directory))
        return command;
      else if (string.IsNullOrWhiteSpace(command))
        return dir;

      StringComparison comparison = Environment.OSVersion.Platform == PlatformID.Win32NT
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

      if (command.Contains(dir, comparison))
        return command;

      return string.Join(" ", command.TrimEnd(), $"-C \"{directory}\"");
    }

    /// <summary>
    /// Add file(s) to the source control
    /// </summary>
    /// <param name="filesToAdd">Files to be added</param>
    /// <returns></returns>
    public static string AddFiles(params string[] filesToAdd) {
      if (filesToAdd is null)
        throw new ArgumentNullException(nameof(filesToAdd));
      else if (filesToAdd.Length <= 0)
        return "";

      string files = string.Join(" ", filesToAdd.Select(file => Environment.ExpandEnvironmentVariables(file).QuotationAdd('\"')));

      return string.Join(
        " ",
        "add -f",
        "--",
         files);
    }

    /// <summary>
    /// Add file(s) to the source control
    /// </summary>
    /// <param name="filesToAdd">Files to be removed</param>
    /// <returns></returns>
    public static string RemoveFiles(params string[] filesToAdd) {
      if (filesToAdd is null)
        throw new ArgumentNullException(nameof(filesToAdd));
      else if (filesToAdd.Length <= 0)
        return "";

      string files = string.Join(" ", filesToAdd.Select(file => Environment.ExpandEnvironmentVariables(file).QuotationAdd('\"')));

      return string.Join(
        " ",
        "rm -f",
        "--",
         files);
    }

    #endregion Public
  }
}
