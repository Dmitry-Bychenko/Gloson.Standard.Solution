using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
      if (null == items)
        throw new ArgumentNullException(nameof(items));

      return string.Join(" ", items
        .Select(item => item?.Trim())
        .Where(item => !string.IsNullOrWhiteSpace(item)));
    }

    /// <summary>
    /// Build Command
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
      if (null == ssh)
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

      if (null == format)
        throw new ArgumentNullException(nameof(format));

      return BuildCommand(
       $"-C \"{targetPath}\"",
       $"--no-pager show -s",
       $"--format=\"{format}\"");
    }

    #endregion Public
  }
}
