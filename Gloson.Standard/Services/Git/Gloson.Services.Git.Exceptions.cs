using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gloson.Services.Git {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Git Exception
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class GitException : Exception {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected GitException() : base() { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected GitException(string message) : base(message) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected GitException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Serialization constructor
    /// </summary>
    protected GitException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Not Found Exception
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitNotFoundException : GitException {
    #region Algorithm

    private static bool IsWindows { 
      get {
        return Environment.OSVersion.Platform == PlatformID.Win32NT;
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public GitNotFoundException() 
      : base(IsWindows 
          ? @"Git not found. Add path in registry [HKEY_LOCAL_MACHINE\Software\GitForWindows] [InstallPath] to %PATH%"
          : @"Git not found") { }

    /// <summary>
    /// Serialization constructor
    /// </summary>
    internal GitNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Failure
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class GitFailureException : GitException, ISerializable {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public GitFailureException(string message, int exitCode) : base(message) {
      ExitCode = exitCode;
    }

    /// <summary>
    /// Serialization constructor
    /// </summary>
    internal GitFailureException(SerializationInfo info, StreamingContext context)
      : base(info, context) {

      ExitCode = info.GetInt32("ExitCode");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Exit Code
    /// </summary>
    public int ExitCode {
      get;
      private set;
    }

    #endregion Public
  }
}
