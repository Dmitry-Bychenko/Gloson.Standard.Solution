using Gloson.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gloson.Data.Oracle.Client {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// TnsNames
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TnsNames {
    #region Private Data

    private string m_Name;

    // Items
    private readonly List<TnsNames> m_Items = new();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    public TnsNames(string name, TnsNames parent) {
      Name = name;
      Parent = parent;

      if (parent is not null)
        parent.m_Items.Add(this);
    }

    /// <summary>
    /// Load
    /// </summary>
    public static TnsNames Load(IEnumerable<string> lines) {
      if (lines is null)
        throw new ArgumentNullException(nameof(lines));

      TnsNames root = new("", null);

      Stack<TnsNames> current = new();

      current.Push(root);

      StringBuilder sb = new();

      int bracketCount = 0;
      bool inQuot = false;
      bool inValue = false;
      bool inName = false;

      foreach (string line in lines) {
        foreach (char c in line) {
          if (inQuot) {
            sb.Append(c);

            if (c == '"')
              inQuot = !inQuot;
          }
          else if (c == '#')
            break;
          else if (c == '"') {
            sb.Append(c);

            inQuot = true;
          }
          else if (c == '(') {
            bracketCount += 1;

            inValue = false;
            inName = true;

            TnsNames child = new("", current.Peek());

            current.Push(child);
          }
          else if (c == '=') {
            if (inName)
              current.Peek().Name = sb.ToString().Trim();
            else {
              if (bracketCount <= 0) {
                bracketCount = 0;

                if (current.Count > 1)
                  current.Pop();
              }

              TnsNames child = new(sb.ToString(), current.Peek());
              current.Push(child);
            }

            sb.Clear();

            inName = false;
            inValue = true;
          }
          else if (c == ')') {
            if (inValue)
#pragma warning disable CA1806 // Do not ignore method results
              new TnsNames(sb.ToString(), current.Peek());
#pragma warning restore CA1806 // Do not ignore method results

            bracketCount -= 1;

            sb.Clear();

            inValue = false;
            inName = false;

            current.Pop();
          }
          else
            sb.Append(c);
        }
      }

      return root;
    }

    /// <summary>
    /// Load
    /// </summary>
    public static TnsNames Load(string fileName, Encoding encoding) {
      return Load(File.ReadLines(fileName, encoding));
    }

    /// <summary>
    /// Load
    /// </summary>
    public static TnsNames Load(string fileName) {
      return Load(File.ReadLines(fileName));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Root
    /// </summary>
    public TnsNames Root {
      get {
        TnsNames result = this;

        while (true) {
          if (result.Parent is null)
            return result;

          result = result.Parent;
        }
      }
    }

    /// <summary>
    /// Parent
    /// </summary>
    public TnsNames Parent { get; private set; }

    /// <summary>
    /// Name
    /// </summary>
    public String Name {
      get {
        return m_Name;
      }
      set {
        value = (value ?? "").Trim();

        if (value.StartsWith("\""))
          value = value.QuotationRemove('"');

        m_Name = value;
      }
    }

    /// <summary>
    /// Value (null if not exists)
    /// </summary>
    public String Value {
      get {
        if (Items.Count == 1 && Items[0].Items.Count <= 0)
          return Items[0].Name;
        else
          return null;
      }
    }

    /// <summary>
    /// Level
    /// </summary>
    public int Level {
      get {
        int result = 0;

        for (TnsNames parent = Parent; parent is not null; parent = parent.Parent)
          result += 1;

        return result;
      }
    }

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<TnsNames> Items => m_Items;

    /// <summary>
    /// To String (Name)
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Lines
    /// </summary>
    public IEnumerable<string> Lines {
      get {
        int level = Level;

        if (Items.Count <= 0)
          yield return $"{Name}";
        else if (Items.Count == 1 && Items[0].Items.Count == 0) {
          int shift = level * 2 - 2;
          string pad = new(' ', shift);

          yield return $"{pad}({Name} = {Items[0].Name})";
        }
        else if (level == 0) {
          foreach (var item in Items)
            foreach (string line in item.Lines)
              yield return line;
        }
        else {
          int shift = level * 2 - 2;
          string pad = new(' ', shift);

          if (level <= 1)
            yield return $"{pad}{Name} = ";
          else
            yield return $"{pad}({Name} = ";

          foreach (var item in Items) {
            foreach (string line in item.Lines) {
              yield return line;
            }
          }

          if (level <= 1)
            yield return $"{pad}";
          else
            yield return $"{pad})";
        }
      }
    }

    #endregion Public
  }

}
