//---------------------------------------------------------------------------------------------------------------------
//
// Brainfuck++ interpreter
//
//   https://en.wikipedia.org/wiki/Brainfuck
//   https://www.codeabbey.com/index/wiki/brainfuck
//
//---------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gloson.Language.Esoteric {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Brainfuck language interpreter
  /// </summary>
  /// <see cref="https://www.codeabbey.com/index/wiki/brainfuck"/>
  /// <example>
  /// <code>
  /// string code = ";>;<[->+<]:>:";
  /// string input = "3 5";
  ///
  /// var result = string.Join(" ", BrainfuckInterpreter.Run(
  ///  code,
  ///  input.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
  ///  
  /// Console.Write(result); // 0 8
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BrainfuckInterpreter : IDisposable {
    #region Inner classes

    /// <summary>
    /// Interpreter local memory 
    /// </summary>
    public class LocalMemory {
      #region Private Data

      private readonly Dictionary<long, long> m_Data = new Dictionary<long, long>();

      #endregion Private Data

      #region Algorithm

      // Clear (Reset to the initial state)
      internal void Reset() {
        m_Data.Clear();
        Pointer = 0;
      }

      #endregion Algorithm

      #region Public

      /// <summary>
      /// Current pointer
      /// </summary>
      public long Pointer { get; private set; }

      /// <summary>
      /// Next operation
      /// </summary>
      internal void Next() => ++Pointer;

      /// <summary>
      /// Prior operation
      /// </summary>
      internal void Prior() => --Pointer;

      /// <summary>
      /// Increment
      /// </summary>
      internal void Increment() {
        if (m_Data.TryGetValue(Pointer, out var v))
          m_Data[Pointer] = v + 1;
        else
          m_Data.Add(Pointer, 1);
      }

      /// <summary>
      /// Decrement
      /// </summary>
      internal void Decrement() {
        if (m_Data.TryGetValue(Pointer, out var v))
          m_Data[Pointer] = v - 1;
        else
          m_Data.Add(Pointer, -1);
      }

      /// <summary>
      /// Current Memory Cell
      /// </summary>
      public long Current {
        get {
          if (m_Data.TryGetValue(Pointer, out long value))
            return value;
          else
            return 0;
        }
        set {
          if (value == 0) {
            if (m_Data.ContainsKey(Pointer))
              m_Data.Remove(Pointer);
          }
          else if (m_Data.ContainsKey(Pointer))
            m_Data[Pointer] = value;
          else
            m_Data.Add(Pointer, value);
        }
      }

      /// <summary>
      /// Values
      /// </summary>
      public IReadOnlyDictionary<long, long> Values => m_Data;

      /// <summary>
      /// Dump
      /// </summary>
      public string Dump() {
        if (m_Data.Count <= 0)
          return "No local variables";

        static string ValueToString(long value) {
          StringBuilder sb = new StringBuilder();

          sb.Append($"{value,10}");

          if (value >= char.MinValue && value <= char.MaxValue) {
            sb.Append(" : ");

            if (value >= 32)
              sb.Append($"'{(char)value}' (\\u{value:x4})");
            else
              sb.Append($"\\u{value:x4}");
          }

          return sb.ToString();
        }

        var prefix = new string[] {
          $"Local variables:",
          $"  {m_Data.Count} variables in total",
          $"  Min value: {m_Data.Values.Min(),10}",
          $"  Max value: {m_Data.Values.Min(),10}",
          $"Values:",
        };

        var body = m_Data
          .Where(pair => pair.Value != 0)
          .OrderBy(pair => pair.Key)
          .Select(pair => $"  {pair.Key,10} : {ValueToString(pair.Value)}");

        return string.Join(Environment.NewLine, prefix.Concat(body));
      }

      #endregion Public
    }

    /// <summary>
    /// Brainfuck syntax
    /// </summary>
    [Flags]
    public enum BrainfuckSyntax {
      Standard = 0,
      PlusPlus = 1
    }

    /// <summary>
    /// Source Code Exception 
    /// </summary>
    public class ExecutionException : Exception {
      #region Create

      /// <summary>
      /// Standard Constructor
      /// </summary>
      /// <param name="message">Message</param>
      /// <param name="position">Position (zero based) in source code</param>
      public ExecutionException(string message, Exception innerException, BrainfuckInterpreter interpreter)
        : base(message, innerException) {

        Interpreter = interpreter;
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Interpreter
      /// </summary>
      public BrainfuckInterpreter Interpreter { get; }

      /// <summary>
      /// Position in source code
      /// </summary>
      public int Position => Interpreter?.Position ?? -1;

      #endregion Public
    }

    /// <summary>
    /// Output Event Arguments
    /// </summary>
    public class OutputEventArgs : EventArgs {
      #region Create

      public OutputEventArgs(BrainfuckInterpreter interpreter, char value) {
        Interpreter = interpreter;
        Value = value.ToString();
      }

      public OutputEventArgs(BrainfuckInterpreter interpreter, long value) {
        Interpreter = interpreter;
        Value = value.ToString(CultureInfo.InvariantCulture);
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Interpreter
      /// </summary>
      public BrainfuckInterpreter Interpreter { get; }

      /// <summary>
      /// Value to output
      /// </summary>
      public String Value { get; }

      /// <summary>
      /// Value to output
      /// </summary>
      /// <returns></returns>
      public override string ToString() => Value;

      #endregion Public
    }

    #endregion Inner classes

    #region Private Data

    // Inner memory to work with
    private readonly LocalMemory m_Memory = new LocalMemory();
    // Input Enumerator
    private IEnumerator<string> m_InputEnumerator;

    #endregion Private Data

    #region Algorithm

    private void Error(string message) {
      throw new ExecutionException(message, null, this);
    }

    private void WriteCharToOutput() {
      var output = Output;

      long item = m_Memory.Current;

      if (item < char.MinValue || item > char.MaxValue)
        Error($"Failed to output: {item} is out of character range.");

      CurrentOutput = ((char)item).ToString();

      if (output is not null)
        output.Invoke(this, new OutputEventArgs(this, (char)item));
    }

    private void WriteLongToOutput() {
      var output = Output;

      long item = m_Memory.Current;

      CurrentOutput = item.ToString(CultureInfo.InvariantCulture);

      if (output is not null)
        output.Invoke(this, new OutputEventArgs(this, item));
    }

    private long ReadLong() {
      if (!m_InputEnumerator.MoveNext())
        Error($"Can't read an item from input");

      string value = m_InputEnumerator.Current;

      if (long.TryParse(value, out long result))
        return result;

      Error($"Input value \"{value}\" is not a valid integer (long).");

      return 0;
    }

    private long ReadChar() {
      if (!m_InputEnumerator.MoveNext())
        Error($"Can't read an item from input");

      string value = m_InputEnumerator.Current;

      if (value.Length != 0)
        Error($"Input value \"{value}\" is not a valid character.");

      return (long)(value[0]);
    }

    // Next Step
    private bool CoreNext() {
      if (IsDisposed)
        throw new ObjectDisposedException("this");

      if (IsCompleted)
        return false;

      CurrentOutput = null;
      char command = SourceCode[Position];

      if (command == '>')
        m_Memory.Next();
      else if (command == '<')
        m_Memory.Prior();
      else if (command == '+')
        m_Memory.Increment();
      else if (command == '-')
        m_Memory.Decrement();
      else if (command == ',')
        m_Memory.Current = ReadChar();
      else if (command == '.')
        WriteCharToOutput();
      else if (command == ';' && Syntax.HasFlag(BrainfuckSyntax.PlusPlus))
        m_Memory.Current = ReadLong();
      else if (command == ':' && Syntax.HasFlag(BrainfuckSyntax.PlusPlus))
        WriteLongToOutput();
      else if (command == '[' && m_Memory.Current == 0) {
        int count = 1;

        for (int i = Position + 1; i < SourceCode.Length; ++i) {
          char c = SourceCode[i];

          if (c == '[')
            count += 1;
          else if (c == ']') {
            count -= 1;

            if (count <= 0) {
              Position = i;

              break;
            }
          }
        }

        if (count > 0)
          Error($"No match for [ keyword found.");
      }
      else if (command == ']') {
        int count = 1;

        for (int i = Position - 1; i >= 0; --i) {
          char c = SourceCode[i];

          if (c == ']')
            count += 1;
          else if (c == '[') {
            count -= 1;

            if (count == 0) {
              Position = i - 1;

              break;
            }
          }
        }

        if (count > 0)
          Error($"No match for ] keyword found.");
      }

      Position += 1;

      return true;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="sourceCode"></param>
    /// <param name="input"></param>
    public BrainfuckInterpreter(BrainfuckSyntax syntax,
                                string sourceCode,
                                IEnumerable<string> input) {
      Position = 0;

      Syntax = syntax;
      SourceCode = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));
      m_InputEnumerator = (input ?? throw new ArgumentNullException(nameof(input))).GetEnumerator();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// To String (Source Code)
    /// </summary>
    public override string ToString() => SourceCode;

    /// <summary>
    /// Syntax
    /// </summary>
    public BrainfuckSyntax Syntax { get; private set; }

    /// <summary>
    /// Source code
    /// </summary>
    public String SourceCode { get; private set; }

    /// <summary>
    /// Position
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    /// Inner Memory
    /// </summary>
    public LocalMemory Memory => m_Memory;

    /// <summary>
    /// Current Output
    /// </summary>
    public String CurrentOutput { get; private set; }

    /// <summary>
    /// Is Completed
    /// </summary>
    public bool IsCompleted => Position >= SourceCode.Length;

    /// <summary>
    /// Syntax check
    /// </summary>
    public IEnumerable<(int position, string message)> SyntaxCheck() {
      int count = 0;

      for (int i = 0; i < SourceCode.Length; ++i) {
        char c = SourceCode[i];

        if (c == '[')
          count = +1;
        else if (c == ']') {
          count -= 1;

          if (count < 0) {
            count = 0;

            yield return (i, $"Too many ']' operators");
          }
        }
      }

      if (count > 0)
        yield return (SourceCode.Length, $"Too many ({count}) '[' operators");
    }

    /// <summary>
    /// Next Step
    /// </summary>
    public bool Next() {
      if (Position == 0) {
        var list = SyntaxCheck().ToList();

        if (list.Any()) {
          Error(string.Join(Environment.NewLine, list
            .Select(item => $"{item.message} at {item.position}")));

          return false;
        }
      }

      return CoreNext();
    }

    /// <summary>
    /// Run to the completeion
    /// </summary>
    public void RunToCompletion() {
      while (Next())
        ;
    }

    /// <summary>
    /// Run To Point
    /// </summary>
    public int RunToPoint(params int[] breakPoints) {
      if (breakPoints is null)
        throw new ArgumentNullException(nameof(breakPoints));

      while (true) {
        if (breakPoints.Contains(Position))
          return Position;

        if (!Next())
          return -1;
      }
    }

    /// <summary>
    /// Run
    /// </summary>
    public static IEnumerable<string> Run(string sourceCode,
                                          IEnumerable<string> input,
                                          BrainfuckSyntax syntax = BrainfuckSyntax.PlusPlus) {
      if (sourceCode is null)
        throw new ArgumentNullException(nameof(sourceCode));
      else if (input is null)
        input = Array.Empty<string>();

      using BrainfuckInterpreter engine = new BrainfuckInterpreter(syntax, sourceCode, input);

      while (engine.Next())
        if (engine.CurrentOutput is not null)
          yield return engine.CurrentOutput;
    }

    /// <summary>
    /// Debug Run
    /// </summary>
    public static IEnumerable<string> DebugRun(string sourceCode,
                                               IEnumerable<string> input,
                                               BrainfuckSyntax syntax,
                                               Func<BrainfuckInterpreter, bool> breakFunction,
                                               params int[] breakPoints) {
      if (sourceCode is null)
        throw new ArgumentNullException(nameof(sourceCode));
      else if (input is null)
        input = Array.Empty<string>();

      using BrainfuckInterpreter engine = new BrainfuckInterpreter(syntax, sourceCode, input);

      while (true) {
        if (breakPoints.Contains(engine.Position))
          if (breakFunction is not null)
            if (breakFunction(engine))
              yield break;

        if (!engine.Next())
          yield break;

        if (engine.CurrentOutput is not null)
          yield return engine.CurrentOutput;
      }
    }

    /// <summary>
    /// Output event
    /// </summary>
    public event EventHandler<OutputEventArgs> Output;

    /// <summary>
    /// Remove all comments in a source code
    /// </summary>
    public static string RemoveAllComments(string sourceCode, BrainfuckSyntax syntax = BrainfuckSyntax.PlusPlus) {
      if (sourceCode is null)
        return null;

      HashSet<char> allowed = new HashSet<char>() {
        '>', '<', '+', '-', '.', ',', '[', ']',
      };

      if (syntax.HasFlag(BrainfuckSyntax.PlusPlus)) {
        allowed.Add(':');
        allowed.Add(';');
      }

      return string.Concat(sourceCode.Where(c => allowed.Contains(c)));
    }

    /// <summary>
    /// Is source code well formed
    /// </summary>
    public static bool IsWellFormed(string sourceCode) {
      if (sourceCode is null)
        return false;

      int count = 0;

      foreach (char c in sourceCode)
        if (c == '[')
          ++count;
        else if (c == ']')
          if (--count < 0)
            return false;

      return count == 0;
    }

    /// <summary>
    /// Dump
    /// </summary>
    public string Dump() {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine("Code:");
      sb.AppendLine("  " + SourceCode.Replace('\r', ' ').Replace('\n', ' '));
      sb.AppendLine(new string(' ', Position + 1) + $"^ ({Position})");

      if (CurrentOutput is not null) {
        sb.AppendLine();
        sb.AppendLine($"Current output value: {CurrentOutput.GetType().Name} {CurrentOutput}");
      }

      sb.AppendLine();
      sb.AppendLine(Memory.Dump());

      return sb.ToString();
    }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    // Dispose
    private void Dispose(bool disposing) {
      if (IsDisposed)
        return;

      if (disposing) {
        IsDisposed = true;

        if (m_InputEnumerator is not null)
          m_InputEnumerator.Dispose();

        m_InputEnumerator = null;
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    #endregion IDisposable
  }

}
