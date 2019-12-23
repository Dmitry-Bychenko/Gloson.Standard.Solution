using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// StartUp marker (static constructor will be executed on load)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
  public class StartUpAttribute : Attribute {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public StartUpAttribute() { }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// StartUp
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class StartUp {
    #region Private Data

    // Assemblies initialized
    private static ConcurrentDictionary<Assembly, bool> s_Assemblies = 
      new ConcurrentDictionary<Assembly, bool>();

    #endregion Private Data

    #region Algorithm

    private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args) {
      RunStaticConstructors(args.LoadedAssembly);
    }

    #endregion Algorithm

    #region Create

    static StartUp() {
      AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        RunStaticConstructors(asm);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Explicitly Execute Static Constructors
    /// </summary>
    /// <param name="assembly"></param>
    public static void RunStaticConstructors(Assembly assembly = null) {
      if (null == assembly)
        assembly = Assembly.GetCallingAssembly();

      if (s_Assemblies.TryGetValue(assembly, out bool _value))
        return;

      if (!s_Assemblies.TryAdd(assembly, true))
        return;

      foreach (Type t in assembly.GetTypes()) {
        if (!t.CustomAttributes.OfType<StartUpAttribute>().Any())
          continue;

        RuntimeHelpers.RunClassConstructor(t.TypeHandle);
      }
    }

    /// <summary>
    /// Number of assemblies initialized
    /// </summary>
    public static int Count { 
      get {
        return s_Assemblies.Count;
      }
    }

    /// <summary>
    /// Run
    /// </summary>
    public static int Run() {
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        RunStaticConstructors(asm);

      return Count;
    }

    #endregion Public
  }
}
