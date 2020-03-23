using System;
using System.Text;

namespace Gloson.Consoles {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Console Reader
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ConsoleReader {
    #region Public

    /// <summary>
    /// Read string as password (masked)
    /// </summary>
    /// <param name="mask">mask to use</param>
    public static string ReadPasswordLine(char mask) {
      if (mask == '\0')
        return Console.ReadLine();

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
    public static string ReadPasswordLine() => ReadPasswordLine('*');

    #endregion Public
  }

}
