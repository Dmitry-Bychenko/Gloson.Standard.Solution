﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Gloson.Resources {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Assembly Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class AssemblyExtensions {
    #region Private Data

    private static readonly Lazy<MethodInfo> s_GetNeutralResourcesLanguage = new(() =>
     typeof(ResourceManager).GetMethod("GetNeutralResourcesLanguage",
       BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));

    #endregion Private Data

    #region Public

    /// <summary>
    /// Resources for a given assembly
    /// </summary>
    public static IEnumerable<ResourceManager> Resources(this Assembly assembly) {
      if (assembly is null)
        throw new ArgumentNullException(nameof(assembly));

      foreach (var resourceFile in assembly.GetManifestResourceNames()) {
        string name = Path.GetFileNameWithoutExtension(resourceFile);

        yield return new ResourceManager(name, assembly);
      }
    }

    /// <summary>
    /// Neutral Resources Language
    /// </summary>
    /// <param name="assembly">Assembly with resources</param>
    public static CultureInfo NeutralResourcesLanguage(this Assembly assembly) {
      if (assembly is null)
        throw new ArgumentNullException(nameof(assembly));

      return s_GetNeutralResourcesLanguage.Value.Invoke(null, new object[] { assembly }) as CultureInfo;
    }

    /// <summary>
    /// Resource Values
    /// </summary>
    /// <param name="assembly">Assembly to query</param>
    /// <param name="address">Address in name@file or name format</param>
    /// <param name="culture"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<Object> ResourceValues(this Assembly assembly,
                                                     string address,
                                                     CultureInfo culture,
                                                     StringComparer comparer) {
      if (assembly is null)
        throw new ArgumentNullException(nameof(assembly));
      else if (string.IsNullOrWhiteSpace(address))
        yield break;

      culture ??= CultureInfo.CurrentUICulture;

      if (comparer is null)
        comparer = StringComparer.Ordinal;

      int index = address.IndexOf('@');

      string name = (index < 0 ? address : address[0..(index - 1)]).Trim();
      string baseName = (index < 0 ? "" : address[(index + 1)..]).Trim();

      foreach (ResourceManager manager in Resources(assembly)) {
        if (string.IsNullOrWhiteSpace(baseName) || comparer.Equals(baseName, manager.BaseName)) {
          foreach (var pair in manager.EnumerateResources(culture)) {
            if (comparer.Equals(name, pair.Key))
              yield return pair.Value;
          }
        }
      }
    }

    /// <summary>
    /// Resource Values
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="address"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static IEnumerable<Object> ResourceValues(this Assembly assembly,
                                                     string address,
                                                     CultureInfo culture) =>
      ResourceValues(assembly, address, culture, null);

    /// <summary>
    /// Resource Values
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="address"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static IEnumerable<Object> ResourceValues(this Assembly assembly,
                                                     string address,
                                                     StringComparer comparer) =>
      ResourceValues(assembly, address, null, comparer);

    /// <summary>
    /// Resource Values
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="address"></param>
    /// <returns></returns>
    public static IEnumerable<Object> ResourceValues(this Assembly assembly,
                                                     string address) =>
      ResourceValues(assembly, address, null, null);

    #endregion Public
  }

}
