using Gloson.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gloson.IO {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Path Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class PathHelper {
    #region Public

    /// <summary>
    /// Expand And Quote
    ///   - Expand All environment variables
    ///   - Add quotation
    /// </summary>
    public static string ExpandAndQuote(string path) {
      if (string.IsNullOrEmpty(path))
        return path;

      return Environment
        .ExpandEnvironmentVariables(path)
        .QuotationAdd();
    }

    /// <summary>
    /// Subtract
    /// </summary>
    public static string Subtract(string path, string root, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
      if (path is null || root is null)
        return path;

      int index = -1;
      string[] ri = Split(root).ToArray();

      IEnumerable<string> di = Split(path);

      bool remove = true;
      List<string> list = new();

      foreach (string dir in di) {
        if (remove) {
          index += 1;

          if (index >= ri.Length)
            remove = false;
          else if (!string.Equals(dir, ri[index], comparison))
            remove = false;
        }

        if (!remove)
          list.Add(dir);
      }

      return string.Join(Path.DirectorySeparatorChar.ToString(), list);
    }

    /// <summary>
    /// Split path into chunks
    /// </summary>
    public static IEnumerable<string> Split(string path) {
      if (string.IsNullOrEmpty(path))
        yield break;

      for (DirectoryInfo di = new(path); di is not null; di = di.Parent)
        yield return di.Name;
    }

    /// <summary>
    /// Split path into Subdirectories
    /// </summary>
    public static IEnumerable<string> Subdirectories(string path) {
      if (string.IsNullOrEmpty(path))
        yield break;

      for (DirectoryInfo di = new(path); di is not null; di = di.Parent)
        yield return di.FullName;
    }

    #endregion Public
  }
}
