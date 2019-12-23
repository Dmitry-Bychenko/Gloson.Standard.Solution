using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gloson.UI.CommandLine {

  public static class CommandLineHelper {
    #region Private Data

    private static List<char> s_CommandLinePrefixes = new List<char>() {
      '/', '\\', '-', ':'
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Command Line Prefixes
    /// </summary>
    public static IReadOnlyList<Char> CommandLinePrefixes => s_CommandLinePrefixes;

    /// <summary>
    /// Normalize
    /// </summary>
    public static string Normalize(string value) {
      if (string.IsNullOrWhiteSpace(value))
        return "";

      return value.Trim().Trim(s_CommandLinePrefixes.ToArray()).Trim();
    }

    #endregion Public

  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Arguments
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class CommandLineArguments {
    #region Private Data
    #endregion Private Data

    #region Algorithm
    #endregion Algorithm 

    #region Create
    #endregion Create 

    #region Public
    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Argument
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class CommandLineArgument {
    #region Private Data
    #endregion Private Data

    #region Algorithm
    #endregion Algorithm 

    #region Create
    #endregion Create 

    #region Public
    #endregion Public
  }

}
