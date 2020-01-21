using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Gloson;
using Gloson.Text;

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
    /// Split path into chunks
    /// </summary>
    public static IEnumerable<string> Split(string path) {
      if (string.IsNullOrEmpty(path))
        yield break;

      for (DirectoryInfo di = new DirectoryInfo(path); di != null; di = di.Parent) 
        yield return di.Name;
    }

    /// <summary>
    /// Split path into Subdirectories
    /// </summary>
    public static IEnumerable<string> Subdirectories(string path) {
      if (string.IsNullOrEmpty(path))
        yield break;

      for (DirectoryInfo di = new DirectoryInfo(path); di != null; di = di.Parent)
        yield return di.FullName;
    }

    #endregion Public
  }
}
