using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.UI.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Argument
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class CommandLineArgument {
    #region Private Data

    private CommandLineArguments m_Owner;

    #endregion Private Data

    #region Algorithm

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArgument(CommandLineArguments owner,
                               CommandLineArgumentDescription description,
                               string rawValue) {
      Owner = owner ?? throw new ArgumentNullException(nameof(owner));
      Description = description;
      RawValue = rawValue ?? "";
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Owner
    /// </summary>
    public CommandLineArguments Owner {
      get => m_Owner;
      set {
        if (ReferenceEquals(m_Owner, value))
          return;

        if (null != m_Owner)
          m_Owner.CoreRemove(this);

        m_Owner = value;

        if (null != m_Owner)
          m_Owner.CoreAdd(this);
      }
    }

    /// <summary>
    /// Description
    /// </summary>
    public CommandLineArgumentDescription Description { get; }

    /// <summary>
    /// Raw Value
    /// </summary>
    public string RawValue { get; }

    /// <summary>
    /// Validation Errors
    /// </summary>
    public IEnumerable<string> ValidationErrors {
      get {
        if (null == Description)
          yield return $"{RawValue} : Description is not found";
      }
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

  public class CommandLineArguments : IReadOnlyList<CommandLineArgument> {
    #region Private Data

    private List<CommandLineArgument> m_Items = null;

    private readonly List<string> m_RawArguments;

    #endregion Private Data

    #region Algorithm

    internal void CoreAdd(CommandLineArgument item) {
      if (null == item)
        return;

      m_Items.Add(item);
    }

    internal void CoreRemove(CommandLineArgument item) {
      if (null == item)
        return;

      m_Items.Remove(item);
    }

    internal void CoreClearAll() {
      for (int i = m_Items.Count - 1; i >= 0; --i)
        m_Items[i].Owner = null;

      m_Items = null;
    }

    private void Descriptions_Updated(object sender, EventArgs e) {
      CoreClearAll();
    }

    private void CoreParse() {
      if (m_Items != null)
        return;

      bool parametersOnly = false;

      m_Items = new List<CommandLineArgument>();

      for (int i = 0; i < m_RawArguments.Count; ++i) {
        string arg = m_RawArguments[i];

        if ("--" == arg) {
          parametersOnly = true;

          continue;
        }

        if (parametersOnly) {
#pragma warning disable CA1806 // Do not ignore method results
          new CommandLineArgument(
            this,
            Descriptions.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.Name)),
            arg);
#pragma warning restore CA1806 // Do not ignore method results

          continue;
        }

        var best = Descriptions
          .Select(item => new {
            item,
            match = item.RegularExpression.Match(arg)
          })
          .Where(rec => rec.match.Success)
          .OrderByDescending(rec => rec.match.Length)
          .FirstOrDefault();

        if (null == best) {
#pragma warning disable CA1806 // Do not ignore method results
          new CommandLineArgument(this, null, null);
#pragma warning restore CA1806 // Do not ignore method results

          continue;
        }

        string val = arg[(best.match.Length)..];

        if (val == "" && best.item.ValueType.WantsValue()) {
          if (i < m_RawArguments.Count - 1) {
            i += 1;

            val = m_RawArguments[i];
          }
        }

#pragma warning disable CA1806 // Do not ignore method results
        new CommandLineArgument(this, best.item, val);
#pragma warning restore CA1806 // Do not ignore method results
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public CommandLineArguments(string[] arguments, CommandLineArgumentDescriptions descriptions) {
      m_RawArguments = (null == arguments)
        ? Environment.GetCommandLineArgs().Skip(1).ToList()
        : arguments.ToList();

      if (null == descriptions)
        descriptions = CommandLineArgumentDescriptions.Default;

      Descriptions = descriptions;

      Descriptions.Updated += Descriptions_Updated;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public CommandLineArguments(string[] arguments)
      : this(arguments, null) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public CommandLineArguments()
      : this((string[])null, null) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArguments(string arguments, CommandLineArgumentDescriptions descriptions)
      : this(Gloson.Text.CommandLine.Split(arguments), descriptions) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineArguments(string arguments)
      : this(arguments, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static CommandLineArguments Default { get; } = new CommandLineArguments();

    /// <summary>
    /// Descriptions
    /// </summary>
    public CommandLineArgumentDescriptions Descriptions { get; }

    /// <summary>
    /// Raw Arguments
    /// </summary>
    public IReadOnlyList<string> RawArguments => m_RawArguments;

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<CommandLineArgument> Items {
      get {
        CoreParse();

        return m_Items;
      }
    }

    /// <summary>
    /// Validation Errors
    /// </summary>
    public IEnumerable<string> ValidationErrors {
      get {
        foreach (var item in m_Items)
          foreach (var err in item.ValidationErrors)
            yield return err;
      }
    }

    #endregion Public

    #region IReadOnlyList<CommandLineArgument>

    /// <summary>
    /// Count
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Indexer
    /// </summary>
    public CommandLineArgument this[int index] => Items[index];

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<CommandLineArgument> GetEnumerator() => Items.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    #endregion IReadOnlyList<CommandLineArgument> 
  }

}
