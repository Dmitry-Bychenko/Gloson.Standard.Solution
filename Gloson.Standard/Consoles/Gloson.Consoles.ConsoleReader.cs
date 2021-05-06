using System;
using System.Collections.Generic;
using System.IO;
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

      StringBuilder sb = new();

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

    /// <summary>
    /// Read StdIn
    /// </summary>
    public static string StandardInputText() {
      using var reader = new StreamReader(Console.OpenStandardInput());

      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read StdIn
    /// </summary>
    public static IEnumerable<string> StandardInputLines() {
      using var reader = new StreamReader(Console.OpenStandardInput());

      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        yield return line;
    }

    /// <summary>
    /// Read StdOut
    /// </summary>
    public static string StandardOutputText() {
      using var reader = new StreamReader(Console.OpenStandardOutput());

      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read StdOut
    /// </summary>
    public static IEnumerable<string> StandardOutputLines() {
      using var reader = new StreamReader(Console.OpenStandardOutput());

      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        yield return line;
    }

    /// <summary>
    /// Read StdErr
    /// </summary>
    public static string StandardErrorText() {
      using var reader = new StreamReader(Console.OpenStandardError());

      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read StdErr
    /// </summary>
    public static IEnumerable<string> StandardErrorLines() {
      using var reader = new StreamReader(Console.OpenStandardError());

      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        yield return line;
    }

    /// <summary>
    /// To Console Standard Out
    /// </summary>
    public static void ToStandardOut<T>(this IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      bool first = true;

      foreach (T item in source) {
        if (first)
          first = false;
        else
          Console.Out.WriteLine();

        Console.Out.Write(item);
      }
    }

    /// <summary>
    /// To Console Standard Error
    /// </summary>
    public static void ToStandardError<T>(this IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      bool first = true;

      foreach (T item in source) {
        if (first)
          first = false;
        else
          Console.Error.WriteLine();

        Console.Error.Write(item);
      }
    }

    #endregion Public
  }

}
