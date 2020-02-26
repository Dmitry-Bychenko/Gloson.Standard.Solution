//---------------------------------------------------------------------------------------------------------------------
// 
// https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection
// https://www.nuget.org/packages/Microsoft.Extensions.configuration 
//
// https://stackoverflow.com/questions/49572079/net-core-dependency-injection-backwards-compatibility-with-net-framework
//
// Install-Package Microsoft.Extensions.DependencyInjection -Version 3.0.0 -ProjectName Gloson.Standard
// Install-Package Microsoft.Extensions.DependencyInjection.Abstractions -Version 3.0.0 -ProjectName Gloson.Standard
//
// HttpClient
//
// 3. Command Line Descriptors
// 5. Trees (RB, B)
// 8. Uri ReadLines etc. like File
//
//
//---------------------------------------------------------------------------------------------------------------------

using System;

[assembly: CLSCompliant(true)]
namespace Gloson.Standard {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Gloson Entry Point
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GlosonEntryPoint {
    #region Create

    static GlosonEntryPoint() {
      Run();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Run
    /// </summary>
    public static void Run() {
      StartUp.Run();
    }

    #endregion Public
  }
 
}
