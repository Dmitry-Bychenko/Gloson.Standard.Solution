using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gloson.Linq.Expressions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Kind
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum ExpressionTokenKind {
    None = 0,
    Delimiter = 1,
    BraceOpen = 2,
    BraceClose = 3,
    WhiteSpace = 4,

    Constant = 5,
    Variable = 6,
    Operator = 7,
    Function = 8,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token to operate with
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IExpressionToken {
    /// <summary>
    /// Priority 
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Kind
    /// </summary>
    ExpressionTokenKind Kind { get; }

    /// <summary>
    /// Name
    /// </summary>
    string Name { get; }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Expression Token
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class ExpressionToken : IExpressionToken, IEquatable<IExpressionToken> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ExpressionToken(string name, int priority, ExpressionTokenKind kind) {
      Name = name;
      Priority = priority;
      Kind = kind;
    }

    /// <summary>
    /// Standard Constructor (all types except operators)
    /// </summary>
    public ExpressionToken(string name, ExpressionTokenKind kind) {
      if (kind == ExpressionTokenKind.Operator)
        throw new InvalidOperationException("Priority must be specified for an operator.");

      Name = name;
      Priority = 0;
      Kind = kind;
    }

    /// <summary>
    /// Standard constructor (operator)
    /// </summary>
    public ExpressionToken(string name, int priority) {
      Name = name;
      Priority = priority;
      Kind = ExpressionTokenKind.Operator;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ExpressionToken(string name) {
      if (string.IsNullOrEmpty(name))
        throw new InvalidOperationException("name should be specified here.");

      var category = char.GetUnicodeCategory(name[0]);

      if (category == UnicodeCategory.OpenPunctuation) {
        Name = name;
        Priority = 0;
        Kind = ExpressionTokenKind.BraceOpen;
      }
      else if (category == UnicodeCategory.ClosePunctuation) {
        Name = name;
        Priority = 0;
        Kind = ExpressionTokenKind.BraceClose;
      }
      else if (name.Trim().Length == 1 && category == UnicodeCategory.OtherPunctuation) {
        Name = name;
        Priority = 0;
        Kind = ExpressionTokenKind.Delimiter;
      }
      else {
        Name = name;
        Priority = 0;
        Kind = ExpressionTokenKind.Constant;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// ToString (Name)
    /// </summary>
    public override string ToString() => Name;

    #endregion Public

    #region IExpressionToken

    /// <summary>
    /// Priority 
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Kind
    /// </summary>
    public ExpressionTokenKind Kind { get; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    #endregion IExpressionToken

    #region IEquatable<ExpressionToken>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IExpressionToken other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return string.Equals(Name, other.Name) &&
             Kind == other.Kind &&
             Priority == other.Priority;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as IExpressionToken);

    /// <summary>
    /// HashCode
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return (null == Name ? 0 : Name.GetHashCode()) ^
               ((int)Kind << 4) ^
                Priority;
      }
    }

    #endregion IEquatable<ExpressionToken>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Shunting Yard
  /// </summary>
  /// https://ru.wikipedia.org/wiki/%D0%90%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC_%D1%81%D0%BE%D1%80%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%BE%D1%87%D0%BD%D0%BE%D0%B9_%D1%81%D1%82%D0%B0%D0%BD%D1%86%D0%B8%D0%B8
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ShuntingYard<T> where T : IExpressionToken {
    #region public

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="source">Original parsed text</param>
    /// <returns>Reversed Polish Order</returns>
    public static IEnumerable<T> Process(IEnumerable<T> source) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      Stack<T> funcs = new Stack<T>();

      foreach (T token in source) {
        if (token.Kind == ExpressionTokenKind.Constant || token.Kind == ExpressionTokenKind.Variable)
          yield return token;
        else if (token.Kind == ExpressionTokenKind.Function || token.Kind == ExpressionTokenKind.BraceOpen)
          funcs.Push(token);
        else if (token.Kind == ExpressionTokenKind.Delimiter) {
          while (true) {
            if (funcs.Count <= 0)
              throw new ArgumentException($"Either delimiter or opening parenthesis missed.", nameof(source));

            if (funcs.Peek().Kind == ExpressionTokenKind.BraceOpen)
              break;

            yield return funcs.Pop();
          }
        }
        else if (token.Kind == ExpressionTokenKind.BraceClose) {
          while (true) {
            if (funcs.Count <= 0)
              throw new ArgumentException($"Opening parenthesis missed.", nameof(source));

            if (funcs.Peek().Kind == ExpressionTokenKind.BraceOpen) {
              if (!string.Equals(token.Name, funcs.Peek().Name.ParenthesesReversed()))
                throw new ArgumentException($"Mismatch parentheses for {token.Name}", nameof(source));

              funcs.Pop();

              if (funcs.Count > 0 && funcs.Peek().Kind == ExpressionTokenKind.Function)
                yield return funcs.Pop();

              break;
            }

            yield return funcs.Pop();
          }
        }
        else if (token.Kind == ExpressionTokenKind.Operator) {
          while (funcs.Count > 0)
            if (funcs.Peek().Kind == ExpressionTokenKind.Operator && funcs.Peek().Priority >= token.Priority)
              yield return funcs.Pop();
            else
              break;

          funcs.Push(token);
        }
        else if (token.Kind == ExpressionTokenKind.None || token.Kind == ExpressionTokenKind.WhiteSpace)
          continue;
        else
          throw new ArgumentException($"Unknown token {token.Name} with {token.Kind} kind", nameof(source));
      }

      // Tail
      while (funcs.Count > 0) {
        if (funcs.Peek().Kind == ExpressionTokenKind.BraceOpen)
          throw new ArgumentException($"Closing parenthesis missed.", nameof(source));
        else if (funcs.Peek().Kind == ExpressionTokenKind.BraceClose)
          throw new ArgumentException($"Opening parenthesis missed.", nameof(source));

        yield return funcs.Pop();
      }
    }

    #endregion public
  }

}
