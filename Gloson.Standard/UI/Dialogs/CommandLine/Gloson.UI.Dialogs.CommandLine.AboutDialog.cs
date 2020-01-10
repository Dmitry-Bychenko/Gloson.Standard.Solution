using System;
using System.Collections.Generic;
using System.Text;

using Gloson.Diagnostics;
using Gloson.UI.CommandLine;

namespace Gloson.UI.Dialogs.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// About Dialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class AboutDialog : IAboutDialog {
    #region Algorithm

    private static string BasicInfo() => AssemblyInfo.Default.ToString();

    private static string CommandLineInfo() => CommandLineArgumentDescriptions.Default.ToString();

    #endregion Algorithm

    #region IAboutDialog 

    /// <summary>
    /// Show
    /// </summary>
    public void Show(int level) {
      if (level < 0)
        level = 0;

      Console.WriteLine(BasicInfo());

      string st = CommandLineInfo();

      if (!string.IsNullOrWhiteSpace(st)) {
        Console.WriteLine();
        Console.WriteLine("Syntax:");
        Console.WriteLine();
        Console.WriteLine(CommandLineInfo());
      }
    }

    #endregion IAboutDialog
  }
}
