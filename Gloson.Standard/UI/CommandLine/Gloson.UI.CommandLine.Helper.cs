using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.UI.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Arguments Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class CommandLineArgumentHelper {
    #region Private Data

    private static readonly char[] s_Prefixes = new char[] { '/', '\\', '-', ':', '=', '@' };

    private static readonly char[] s_Suffixes = new char[] { ':', '=' };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Prefixes
    /// </summary>
    public static char[] Prefixes => s_Prefixes.ToArray();

    /// <summary>
    /// Suffixes
    /// </summary>
    public static char[] Suffixes => s_Suffixes.ToArray();

    /// <summary>
    /// Trim Name
    /// </summary>
    public static string TrimName(string name) {
      if (null == name)
        return "";

      return name.Trim().TrimStart(Prefixes).TrimEnd(Suffixes);
    }

    #endregion Public
  }
}
