using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Console Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ConsoleExtensions {
    #region Public

    /// <summary>
    /// Read string as password (masked)
    /// </summary>
    /// <param name="mask">mask to use</param>
    public static string ReadPassword(char mask) {
      StringBuilder sb = new StringBuilder();

      int position = Console.CursorLeft;
      int was = 0;

      while (true) {
        var key = Console.ReadKey(true);

        if (key.Key == ConsoleKey.Enter)
          return sb.ToString();
        else if (key.Key == ConsoleKey.Escape) {
          Console.CursorLeft = position;
          Console.Write(new string(' ', was));
          Console.CursorLeft = position;

          return "";
        }
        else if (key.Key == ConsoleKey.Backspace) {
          if (sb.Length > 0)
            sb.Length -= 1;
        }
        else if (key.KeyChar >= ' ')
          sb.Append(key.KeyChar);

        Console.CursorLeft = position;

        Console.Write(new string(' ', was));
        Console.CursorLeft = position;
        Console.Write(new string(mask, sb.Length));

        was = sb.Length;
      }
    }

    /// <summary>
    /// Read string as password (masked)
    /// </summary>
    public static string ReadPassword() {
      return ReadPassword('*');
    }

    #endregion Public
  }

}
