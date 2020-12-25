using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Text.Parsing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TokensExtensions {
    #region Public

    /// <summary>
    /// Filter out all White Space (comments) tokens
    /// </summary>
    public static IEnumerable<T> MeanfulOnly<T>(this IEnumerable<T> source)
      where T : Token {

      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (T item in source)
        if (!item.IsWhiteSpace)
          yield return item;
    }

    /// <summary>
    /// Filter out all Non essential (White Space - comments and syntax only) tokens
    /// </summary>
    public static IEnumerable<T> EssentialOnly<T>(this IEnumerable<T> source)
      where T : Token {

      if (source is null)
        throw new ArgumentNullException(nameof(source));

      foreach (T item in source)
        if (!item.IsWhiteSpace && item.Classification != TokenClassification.SyntaxElement)
          yield return item;
    }

    /// <summary>
    /// Code Patterns
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// var result = source
    ///   .MeanfulOnly()
    ///   .Pattern(token => token.Description == KeyWord && token.Text == "begin",
    ///            token => token.Description == Indetifier,
    ///            token => token.Description == Syntax && token.Text == ":=")
    ///   .Select(group => $"{group[i].StartLine} : incorrect assignment");         
    /// </code>
    /// </example>
    public static IEnumerable<T[]> Patterns<T>(this IEnumerable<T> source,
                                               params Func<T, bool>[] pattern)
      where T : Token {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (pattern is null)
        throw new ArgumentNullException(nameof(pattern));
      else if (pattern.Length <= 0)
        throw new ArgumentOutOfRangeException(nameof(pattern), "pattern must not be empty.");
      else if (pattern.Any(func => func is null))
        throw new ArgumentException("null is not allowed as a pattern's item.", nameof(pattern));

      Queue<T> queue = new Queue<T>(pattern.Length + 1);

      foreach (T item in source) {
        queue.Enqueue(item);

        if (queue.Count > pattern.Length)
          queue.Dequeue();

        if (queue.Count == pattern.Length) {
          int index = 0;
          bool matched = true;

          foreach (var q in queue) {
            if (!pattern[index++](q)) {
              matched = false;

              break;
            }
          }

          if (matched)
            yield return queue.ToArray();
        }
      }
    }

    /// <summary>
    /// Build
    /// </summary>
    /// <param name="source">Tokens</param>
    /// <returns>Source text</returns>
    public static IEnumerable<String> Build(this IEnumerable<Token> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var tokens = source
        .Where(token => token is not null)
        .OrderBy(token => token.StartLine)
        .ThenBy(token => token.StartColumn)
        .ThenBy(token => token.StopLine)
        .ThenBy(token => token.StopColumn)
        .GroupBy(token => token.StartLine);

      StringBuilder sb = new StringBuilder();
      int currentLine = 0;
      bool first = true;

      foreach (var group in tokens) {
        if (first) {
          first = false;

          if (group.Key < currentLine)
            currentLine = group.Key;
        }

        while (currentLine < group.Key) {
          if (sb.Length > 0)
            yield return sb.ToString();
          else
            yield return "";

          sb.Clear();

          currentLine += 1;
        }

        foreach (var token in group) {
          if (token.StartColumn > sb.Length)
            sb.Append(new string(' ', token.StartColumn - sb.Length));

          if (token.StartLine == token.StopLine) {
            currentLine = token.StopLine;
            sb.Append(token.Text);
          }
          else {
            var lines = token.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Length > 0) {
              sb.Append(lines[0]);

              yield return sb.ToString();
            }

            for (int i = 1; i < lines.Length - 1; ++i)
              yield return lines[i];

            sb.Clear();

            if (lines.Length > 1)
              sb.Append(lines[^1]);

            if (lines.Length > 1)
              currentLine = token.StopLine - 1;
            else
              currentLine = token.StopLine;
          }
        }

        currentLine += 1;

        if (group.Last().StopLine == group.First().StartLine) {
          if (sb.Length > 0)
            yield return sb.ToString();

          sb.Clear();
        }
      }

      if (sb.Length > 0)
        yield return sb.ToString();
    }

    #endregion Public
  }

}
