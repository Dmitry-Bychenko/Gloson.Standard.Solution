using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text.Parsing {
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Syntax Error
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Serializable]
  public class TokenSyntaxException : Exception {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenSyntaxException(string message, Exception innerCause, int line, int column)
      : base(message, innerCause) {

      Line = line;
      Column = column;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenSyntaxException(string message, Exception innerCause)
      : base(message, innerCause) {

    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenSyntaxException(string message, int line, int column)
      : base(message) {

      Line = line;
      Column = column;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Line (0-based)
    /// </summary>
    public int Line {
      get;
      private set;
    }

    /// <summary>
    /// Column (0-based)
    /// </summary>
    public int Column {
      get;
      private set;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Tokenizer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Tokenizer {
    #region Algorithm

    // Parsing
    private static IEnumerable<Token> CoreParse(IEnumerable<string> source, ITokenDescriptionRules rule) {
      Token current = null;
      string prefix = null;

      int line = -1;

      List<Token> context = new List<Token>();

      TokenDescription.TokenDescriptionMatch match;

      StringBuilder sb = new StringBuilder();

      foreach (var lineOfSource in source) {
        line += 1;
        int column = 0;

        while (column < lineOfSource.Length) {
          // Long lexeme
          if (current is not null) {
            if (current.Description.TryMatchStop(lineOfSource, column, context, out match, prefix)) {
              current.StopLine = line;
              current.StopColumn = match.To;

              sb.AppendLine();
              sb.Append(lineOfSource.Substring(0, match.To));
              //sb.Append(match.Extract(lineOfSource));

              current.Text = sb.ToString();

              yield return current;

              current = null;
              sb.Clear();

              column = match.To;
              prefix = null;

              continue;
            }
            else {
              sb.AppendLine();
              sb.Append(lineOfSource);

              break;
            }
          }

          // white space to skip
          if (char.IsWhiteSpace(lineOfSource[column])) {
            column += 1;

            continue;
          }

          // Test for lexeme
          match = rule.Match(lineOfSource, column, context);

          if (!match.IsMatch)
            throw new TokenSyntaxException($"Syntax error at {line + 1:00000} : {column + 1:000}", line, column);

          current = new Token(match.Description, line, match.From);

          if (match.Kind == TokenMatchKind.Entire) {
            current.StopLine = line;
            current.StopColumn = match.To;
            current.Text = match.Extract(lineOfSource);

            context.Add(current);

            yield return current;

            current = null;
            sb.Clear();

            column = match.To;
            prefix = null;

            continue;
          }

          prefix = match.Extract(lineOfSource);
          sb.Append(lineOfSource[(match.From)..]);

          break;
        }
      }

      if (current is not null)
        throw new TokenSyntaxException($"Dangling token at {current.StartLine + 1:00000} : {current.StartColumn + 1:000}",
                                         current.StartLine,
                                         current.StartColumn);

      yield break;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Parse
    /// </summary>
    /// <param name="source">Lines to parse</param>
    /// <param name="rules">Rules to use</param>
    public static IEnumerable<Token> Parse(this IEnumerable<string> source,
                                                ITokenDescriptionRules rules) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      if (rules is null)
        rules = Dependencies.GetService<ITokenDescriptionRules>();

      if (rules is null)
        throw new ArgumentNullException(nameof(rules));

      return CoreParse(source, rules);
    }

    /// <summary>
    /// Parse with default (Dependencies) rules
    /// </summary>
    /// <param name="source">Lines to parse</param>
    public static IEnumerable<Token> Parse(this IEnumerable<string> source) =>
      Parse(source, null);

    #endregion Public
  }

}
