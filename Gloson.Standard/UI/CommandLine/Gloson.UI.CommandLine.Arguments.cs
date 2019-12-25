using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.UI.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

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

    private List<string> m_CommandLineArgs;

    private List<CommandLineArgument> m_Items = new List<CommandLineArgument>();

    #endregion Private Data

    #region Algorithm

    private CommandLineDescriptor[] BestFits(string arg) {
      return Descriptors
        .Where(item => item.RegularExpression.IsMatch(arg))
        .OrderByDescending(item => item.MatchPriority)
        .ToArray();
    }

    private void CoreLoad() {
      CommandLineArgument prior = null;

      foreach (string arg in m_CommandLineArgs) {
        CommandLineDescriptor desc = BestFits(arg).FirstOrDefault();

        if (null != desc) {
          CommandLineArgument item = new CommandLineArgument(this, arg, desc);

          m_Items.Add(item);

          string value = arg.Substring(desc.RegularExpression.Match(arg).Length);

          prior = item;
        }
        else if (prior.Descriptor.ValueType != CommandLineType.None && prior.RawValue == null)
          prior.RawValue = arg;
      }
    }

    #endregion Algorithm 

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArguments(string commandLine, CommandLineDescriptors descriptors) {
      if (null == commandLine) {
        m_CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
        commandLine = Environment.CommandLine;
      }
      else {
        if (Gloson.Text.CommandLine.TrySplit(commandLine, out var array))
          m_CommandLineArgs = array.ToList();
        else
          m_CommandLineArgs = new List<string>();
      }

      if (null == descriptors)
        descriptors = CommandLineDescriptors.Default;

      
      CommandLine = commandLine;
      Descriptors = descriptors;

      CoreLoad();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArguments(IEnumerable<string> commandLineParameters, CommandLineDescriptors descriptors) {
      if (null == commandLineParameters)
        m_CommandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToList();
      else
        m_CommandLineArgs = commandLineParameters.ToList();

      if (null == descriptors)
        descriptors = CommandLineDescriptors.Default;

      CommandLine = string.Join(" ", m_CommandLineArgs);
      Descriptors = descriptors;

      CoreLoad();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArguments() 
      : this(commandLineParameters : null, null) { }

    #endregion Create 

    #region Public

    /// <summary>
    /// Command Line
    /// </summary>
    public string CommandLine { get; }

    /// <summary>
    /// Command Line Args
    /// </summary>
    public IReadOnlyList<String> CommandLineArgs => m_CommandLineArgs;

    /// <summary>
    /// Descriptors
    /// </summary>
    public CommandLineDescriptors Descriptors { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(" ", m_CommandLineArgs);
    }

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

    internal CommandLineArgument(CommandLineArguments owner, 
                                 string argument,
                                 CommandLineDescriptor descriptor) {
      Owner = owner;
      Argument = argument;
      Descriptor = descriptor;
    }

    #endregion Create 

    #region Public

    /// <summary>
    /// Owner
    /// </summary>
    public CommandLineArguments Owner { get; }

    /// <summary>
    /// Argument
    /// </summary>
    public string Argument { get; }

    /// <summary>
    /// Descriptor
    /// </summary>
    public CommandLineDescriptor Descriptor { get; }

    /// <summary>
    /// Raw Value
    /// </summary>
    public string RawValue { get; internal set; }

    #endregion Public
  }

}
