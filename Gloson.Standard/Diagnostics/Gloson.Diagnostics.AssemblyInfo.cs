using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Assembly Info
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class AssemblyInfo : IEquatable<AssemblyInfo> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="master">Assembly to get info about</param>
    public AssemblyInfo(Assembly master) {
      Master = master ?? Assembly.GetEntryAssembly();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public AssemblyInfo() : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static AssemblyInfo Default { get; } = new AssemblyInfo();

    /// <summary>
    /// Master Assembly
    /// </summary>
    public Assembly Master { get; }

    /// <summary>
    /// Title
    /// </summary>
    public string Title => Master.GetCustomAttribute<AssemblyTitleAttribute>()?.Title?.Trim() ?? "";

    /// <summary>
    /// Description
    /// </summary>
    public string Description => Master.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description?.Trim() ?? "";

    /// <summary>
    /// Company
    /// </summary>
    public string Company => Master.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company?.Trim() ?? "";

    /// <summary>
    /// Product
    /// </summary>
    public string Product => Master.GetCustomAttribute<AssemblyProductAttribute>()?.Product?.Trim() ?? "";

    /// <summary>
    /// Copyright
    /// </summary>
    public string Copyright => Master.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright?.Trim() ?? "";

    /// <summary>
    /// Trademark
    /// </summary>
    public string Trademark => Master.GetCustomAttribute<AssemblyTrademarkAttribute>()?.Trademark?.Trim() ?? "";

    /// <summary>
    /// Build Configuration
    /// </summary>
    public string BuildConfiguration => Master.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration?.Trim() ?? "";

    /// <summary>
    /// Com Visible
    /// </summary>
    public bool ComVisible {
      get {
        var attr = Master.GetCustomAttribute<ComVisibleAttribute>();

        return attr == null ? false : attr.Value;
      }
    }

    /// <summary>
    /// Com Guid
    /// </summary>
    public Guid ComGuid {
      get {
        var attr = Master.GetCustomAttribute<GuidAttribute>();

        return (attr != null && Guid.TryParse(attr.Value, out Guid result))
          ? result
          : Guid.Empty;
      }
    }

    /// <summary>
    /// Assembly Version
    /// </summary>
    public Version AssemblyVersion {
      get {
        var attr = Master.GetCustomAttribute<AssemblyVersionAttribute>();

        return attr != null && Version.TryParse(attr.Version, out var result)
          ? result
          : new Version(0, 0);
      }
    }

    /// <summary>
    /// File Version
    /// </summary>
    public Version FileVersion {
      get {
        var attr = Master.GetCustomAttribute<AssemblyFileVersionAttribute>();

        return attr != null && Version.TryParse(attr.Version, out var result)
          ? result
          : new Version(0, 0);
      }
    }

    /// <summary>
    /// Location
    /// </summary>
    public String Location => Master.Location;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return string.Join(" ", new string[] { 
           Company,
           Title,
         $"Version {FileVersion}",
           Copyright
        }
        .Where(item => !string.IsNullOrWhiteSpace(item)));
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator == (AssemblyInfo left, AssemblyInfo right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (ReferenceEquals(null, right))
        return false;
      else if (ReferenceEquals(left, null))
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(AssemblyInfo left, AssemblyInfo right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (ReferenceEquals(null, right))
        return true;
      else if (ReferenceEquals(left, null))
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<AssemblyInfo>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(AssemblyInfo other) {
      return ReferenceEquals(Master, other?.Master);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(this, obj as AssemblyInfo);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Master == null
      ? 0
      : Master.GetHashCode();

    #endregion IEquatable<AssemblyInfo>
  }
}
