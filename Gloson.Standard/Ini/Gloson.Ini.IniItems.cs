﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gloson.Text;

namespace Gloson.Ini {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Comments Kinds
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum IniFileCommentKind {
    /// <summary>
    /// Standard 
    /// </summary>
    Standard = 0,
    /// <summary>
    /// Windows
    /// </summary>
    Windows = Standard,
    /// <summary>
    /// Unix
    /// </summary>
    Unix = 1,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Comments Kind Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class IniFileCommentKindExtensions {
    #region Public

    /// <summary>
    /// To Prefix
    /// </summary>
    public static string ToPrefix(this IniFileCommentKind value) {
      switch (value) {
        case IniFileCommentKind.Windows:
          return ";";
        case IniFileCommentKind.Unix:
          return "#";
        default:
          return "";
      }
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Item
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IIniFileItem {
    /// <summary>
    /// Value
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Build
    /// </summary>
    string Build();
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// IniFile Comment
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniFileComment : IIniFileItem {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public IniFileComment(string value, IniFileCommentKind kind) {
      if (null == value)
        value = "";

      Value = string.Concat(value.Where(c => !char.IsControl(c)));
      Kind = kind;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public IniFileComment(string value) 
      : this(value, IniFileCommentKind.Standard) { }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out IniFileComment result) {
      result = null;

      if (null == value)
        return false;
      else if (value.StartsWith("#")) {
        result = new IniFileComment(value.Substring(1), IniFileCommentKind.Unix);

        return true;
      }
      else if (value.StartsWith(";")) {
        result = new IniFileComment(value.Substring(1), IniFileCommentKind.Windows);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static IniFileComment Parse(string value) {
      if (TryParse(value, out IniFileComment result))
        return result;

      throw new FormatException("Not valid Ini File comment");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Kind
    /// </summary>
    public IniFileCommentKind Kind { get; }

    /// <summary>
    /// Build
    /// </summary>
    public string Build() => $"{Kind.ToPrefix()}{Value}";

    /// <summary>
    /// Value
    /// </summary>
    public override string ToString() => Value;

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Section
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniFileSection : IIniFileItem {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="value"></param>
    public IniFileSection(string value) {
      if (null == value)
        value = "";

      Value = string.Concat(value.Where(c => !char.IsControl(c)));
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out IniFileSection result) {
      result = null;

      if (null == value)
        return false;
      else if (!value.StartsWith("[") || !value.EndsWith("]"))
        return false;

      result = new IniFileSection(value.Substring(1, value.Length - 2));

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static IniFileSection Parse(string value) {
      if (TryParse(value, out IniFileSection result))
        return result;

      throw new FormatException("Not valid Ini File section");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Build
    /// </summary>
    public string Build() => $"[{Value}]";

    /// <summary>
    /// Value
    /// </summary>
    public override string ToString() => Value;

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Record
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniFileRecord : IIniFileItem {
    #region Algorithm

    private static string EncodeName(string value) {
      if (string.IsNullOrEmpty(value))
        return "";

      if (value.Contains('"') || value.Contains('=') ||
          value.StartsWith("#") || value.StartsWith(";") || value.StartsWith(" ") ||
          value.EndsWith(" "))
        return value.QuotationAdd('"', '"');

      return value;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="value">Value</param>
    public IniFileRecord(string name, string value) {
      Name = name == null 
        ? "" 
        : string.Concat(name.Where(c => !char.IsControl(c)));

      Value = value == null
        ? ""
        : string.Concat(value.Where(c => !char.IsControl(c)));
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out IniFileRecord result) {
      result = null;

      if (string.IsNullOrEmpty(value))
        return false;

      if (!value.StartsWith("\"")) {
        int p = value.IndexOf('=');

        if (p < 0)
          return false;

        result = new IniFileRecord(value.Substring(0, p).Trim(), value.Substring(p + 1));
        return true;
      }

      bool inQuot = false;

      for (int i = 0; i < value.Length; ++i) {
        if (value[i] == '"')
          inQuot = !inQuot;
        
        if (!inQuot && value[i] == '=') {
          string name = value.Substring(0, i).Trim();

          if (name.StartsWith("\""))
            if (!name.TryQuotationRemove(out name))
              return false;

          result = new IniFileRecord(name, value.Substring(i + 1));
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static IniFileRecord Parse(string value) {
      if (TryParse(value, out IniFileRecord result))
        return result;

      throw new FormatException("Not valid Ini File record");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Build
    /// </summary>
    public string Build() => $"{EncodeName(Name)}={Value}";

    /// <summary>
    /// Value
    /// </summary>
    public override string ToString() => Value;

    #endregion Public
  }

}
