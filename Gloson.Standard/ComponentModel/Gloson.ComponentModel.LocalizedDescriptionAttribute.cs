using Gloson.Resources;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Gloson.ComponentModel {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Localized Description
  /// </summary>
  /// <example code ="c#">
  /// [LocalizedDescriptionAttribute("Description", "ResourceName@AssemblyName.Properties.Resources")]
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [AttributeUsage(AttributeTargets.All)]
  public sealed class LocalizedDescriptionAttribute : DescriptionAttribute {
    #region Create

    /// <summary>
    /// Description with resource address
    /// </summary>
    public LocalizedDescriptionAttribute(string description, string address)
      : base(description) {

      Address = address ?? "";
    }

    /// <summary>
    /// Description with resource address
    /// </summary>
    public LocalizedDescriptionAttribute(string description)
      : this(description, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Resource Address
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Resource Name
    /// </summary>
    public string ResourceName {
      get {
        if (string.IsNullOrWhiteSpace(Address))
          return "";

        int p = Address.IndexOf('@');

        if (p <= 0)
          return "";

        return Address.Substring(0, p);
      }
    }

    /// <summary>
    /// Resource File Name
    /// </summary>
    public string ResourceFileName {
      get {
        if (string.IsNullOrWhiteSpace(Address))
          return "";

        int p = Address.IndexOf('@');

        if (p <= 0)
          return "";

        return Address[(p + 1)..];
      }
    }

    /// <summary>
    /// Resource Assembly Name
    /// </summary>
    public string ResourceAssemblyName {
      get {
        if (string.IsNullOrWhiteSpace(Address))
          return "";

        int p = Address.IndexOf('@');

        if (p <= 0)
          return "";

        string st = Address[(p + 1)..];

        p = st.IndexOf('.');

        if (p <= 0)
          return "";

        return st.Substring(0, p);
      }
    }

    /// <summary>
    /// Localized Resource if any
    /// </summary>
    /// <param name="value">Localized resource or unlocalized one</param>
    /// <returns>If localized resource returned</returns>
    public bool TryGetLocalizedDescription(out string value) {
      value = DescriptionValue;

      string assemblyName = ResourceAssemblyName;
      string fileName = ResourceFileName;
      string name = ResourceName;

      Assembly asm = AppDomain
        .CurrentDomain
        .GetAssemblies()
        .FirstOrDefault(item => item.GetName().Name == assemblyName);

      if (asm is null)
        return false;

      var managers = asm
        .Resources()
        .Where(rs => rs.BaseName == fileName);

      foreach (var manager in managers) {
        using var rs = manager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

        foreach (DictionaryEntry entry in rs) {
          if (string.Equals(entry.Key as String, name)) {
            value = entry.Value as string;

            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Description 
    /// </summary>
    public override string Description {
      get {
        if (TryGetLocalizedDescription(out var value))
          return value;
        else
          return DescriptionValue;
      }
    }

    /// <summary>
    /// ToString (Description)
    /// </summary>
    public override string ToString() {
      return Description;
    }

    #endregion Public
  }

}
