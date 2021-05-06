using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Gloson.Resources {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Resource Manager Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ResourceManagerExtensions {
    #region Private Data

    private static readonly Lazy<FieldInfo> s_MainAssembly = new(() =>
     typeof(ResourceManager).GetField("MainAssembly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

    #endregion Private Data

    #region Public

    /// <summary>
    /// Resources
    /// </summary>
    public static IEnumerable<KeyValuePair<string, object>> EnumerateResources(this ResourceManager manager,
                                                                                    CultureInfo culture) {

      if (manager is null)
        throw new ArgumentNullException(nameof(manager));

      if (culture is null)
        culture = CultureInfo.CurrentUICulture;

      using ResourceSet rs = manager.GetResourceSet(culture, true, true);

      foreach (DictionaryEntry entry in rs)
        yield return new KeyValuePair<string, object>(entry.Key as String, entry.Value);
    }

    /// <summary>
    /// Resources
    /// </summary>
    public static IEnumerable<KeyValuePair<string, object>> EnumerateResources(this ResourceManager manager) =>
      EnumerateResources(manager, null);

    /// <summary>
    /// Resources Main Assembly
    /// </summary>
    public static Assembly ResourceMainAssembly(this ResourceManager manager) {
      if (manager is null)
        throw new ArgumentNullException(nameof(manager));

      return s_MainAssembly.Value.GetValue(manager) as Assembly;
    }

    /// <summary>
    /// Load Assembly
    /// </summary>
    public static Assembly LoadAssembly(this ResourceManager manager, string resourceName) {
      if (manager is null)
        throw new ArgumentNullException(nameof(manager));
      else if (resourceName is null)
        throw new ArgumentNullException(nameof(resourceName));

      //if (manager.GetObject(resourceName) is byte[] bytes)
      //  return Assembly.Load(bytes);

      return manager.GetObject(resourceName) is byte[] bytes
        ? Assembly.Load(bytes)
        : throw new ArgumentException($"Resource {resourceName} is not found.", nameof(resourceName));
    }

    #endregion Public
  }
}
