using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gloson.Consoles;
using Gloson.Data;
using Gloson.Text;

namespace Gloson.UI.Dialogs.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Unhandled Exception Dialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class UnhandledExceptionDialog : IUnhandledExceptionDialog {
    #region Algorithm

    private static string ToTable(string value, int shift) {
      if (string.IsNullOrEmpty(value))
        return "";

      return string.Join(Environment.NewLine + new string(' ', shift), value.SplitToLines(NewLine.Smart));
    }

    private static string InnerMessage(Exception e) {
      if (null == e)
        return "";

      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"Type:    {e.GetType().Name}");
      sb.AppendLine($"Message: {ToTable(e.Message, 9)}");
      sb.AppendLine($"Trace:   {ToTable(e.StackTrace, 9)}");

      return sb.ToString();
    }

    #endregion Algorithm

    #region IUnhandledExceptionDialog

    /// <summary>
    /// Show
    /// </summary>
    public void Show(Exception error) {
      if (null == error)
        return;

      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"Unhandled    {error.GetType().Name} exception occurred");
      sb.AppendLine($"Message:     {ToTable(error.Message, 13)}");
      sb.AppendLine($"Stack trace: {ToTable(error.StackTrace, 13)}");

      for (Exception inner = error.InnerException; inner != null; inner = inner.InnerException) {
        sb.AppendLine();
        sb.AppendLine(InnerMessage(inner));
      }

      Console.WriteLine(sb.ToString());
    }

    #endregion IUnhandledExceptionDialog
  }
}
