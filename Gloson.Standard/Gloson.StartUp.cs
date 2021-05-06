using System;
using System.Collections.Concurrent;
using System.Linq;
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
    private static readonly ConcurrentDictionary<Assembly, bool> s_Assemblies = new();

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
      if (assembly is null)
        assembly = Assembly.GetCallingAssembly();

      if (s_Assemblies.TryGetValue(assembly, out bool _))
        return;

      if (!s_Assemblies.TryAdd(assembly, true))
        return;

      try {
        foreach (Type t in assembly.GetTypes()) {
          if (!t.GetCustomAttributes<StartUpAttribute>().Any())
            continue;

          try {
            RuntimeHelpers.RunClassConstructor(t.TypeHandle);
          }
          catch (Exception e) {
            throw new InvalidProgramException($"{t.FullName} at {t.Assembly.FullName}", e);
          }
        }
      }
      catch (Exception) {
        //throw new InvalidProgramException($"Assembly {assembly.GetName().Name} failed", ee);
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
