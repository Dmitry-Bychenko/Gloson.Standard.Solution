using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gloson.Services.Git {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Result Level
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum GitResultLevel {
    /// <summary>
    /// None (no errors, success)
    /// </summary>
    None = 0,
    /// <summary>
    /// Some warnings (partial success)
    /// </summary>
    Warning = 1,
    /// <summary>
    /// Error (failure)
    /// </summary>
    Error = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Git Result
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitResult : IEquatable<GitResult>, ISerializable {
    #region Private Data

    private readonly static GitResult s_GitNotFound = new GitResult(
      "",
      Environment.OSVersion.Platform == PlatformID.Win32NT 
        ? @"Git not found. Ensure path in the registry [HKEY_LOCAL_MACHINE\Software\GitForWindows] [InstallPath] is in %PATH%" :
          @"Git Not found.",
      int.MinValue);

    #endregion Private Data

    #region Algorithm

    // Exception To Throw
    internal GitException ExceptionToThrow() {
      if (ErrorLevel < GitResultLevel.Error)
        return null;

      if (ExitCode == int.MinValue)
        return new GitNotFoundException();
      else
        return new GitFailureException(Error, ExitCode);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="stdOut">Standard Out</param>
    /// <param name="stdErr">Standard Error</param>
    /// <param name="exitCode">Standard Exit Code</param>
    public GitResult(string stdOut, string stdErr, int exitCode)
      : base() {

      Out = string.IsNullOrWhiteSpace(stdOut) ? "" : stdOut;
      Error = string.IsNullOrWhiteSpace(stdErr) ? "" : stdErr;
      ExitCode = exitCode;
    }

    /// <summary>
    /// Serialization constructor
    /// </summary>
    internal GitResult(SerializationInfo info, StreamingContext context) {
      Out = info.GetString("Error");
      Error = info.GetString("Out");
      ExitCode = info.GetInt32("ExitCode");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Git Not Found Instance
    /// </summary>
    public static GitResult GitNotFound {
      get {
        return s_GitNotFound;
      }
    }

    /// <summary>
    /// Git None Instance
    /// </summary>
    public static GitResult None { get; } = new GitResult("", "", 0);

    /// <summary>
    /// Out
    /// </summary>
    public string Out {
      get;
      private set;
    }

    /// <summary>
    /// Error
    /// </summary>
    public string Error {
      get;
      private set;
    }

    /// <summary>
    /// Exit Code
    /// </summary>
    public int ExitCode {
      get;
      private set;
    }

    /// <summary>
    /// Value
    /// </summary>
    public string Value {
      get {
        if (ExitCode != 0)
          return string.Join(Environment.NewLine,
           $"Exit Code: {ExitCode}",
            "",
            "Standard Error:",
             Error,
            "Standard Out:",
             Out);
        else if (string.IsNullOrWhiteSpace(Error))
          return Out;
        else
          return string.Join(Environment.NewLine,
            "Standard Error:",
             Error,
            "Standard Out:",
             Out);
      }
    }

    /// <summary>
    /// Throw Exception If Error
    /// </summary>
    public void ThrowIfError() {
      if (ErrorLevel < GitResultLevel.Error)
        return;

      throw ExceptionToThrow();
    }

    /// <summary>
    /// Error Level
    /// </summary>
    public GitResultLevel ErrorLevel {
      get {
        if (ExitCode != 0)
          return GitResultLevel.Error;
        else if (!string.IsNullOrWhiteSpace(Error))
          return GitResultLevel.Warning;
        else
          return GitResultLevel.None;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return Value;
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// To Boolean (no failure)
    /// </summary>
    public static implicit operator bool(GitResult value) {
      return (null == value) ? false : value.ErrorLevel < GitResultLevel.Error;
    }

    /// <summary>
    /// To String
    /// </summary>
    public static implicit operator string(GitResult value) {
      return value?.Value;
    }

    /// <summary>
    /// Equals operator
    /// </summary>
    public static bool operator ==(GitResult left, GitResult right) {
      if (object.ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equals operator
    /// </summary>
    public static bool operator !=(GitResult left, GitResult right) {
      if (object.ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<GitResult>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(GitResult other) {
      if (object.ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return ExitCode == other.ExitCode &&
             string.Equals(Out, other.Out) &&
             string.Equals(Error, other.Error);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as GitResult);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return ExitCode ^
             Out.GetHashCode() ^
             Error.GetHashCode();
    }

    #endregion IEquatable<GitResult>

    #region ISerializable

    /// <summary>
    /// Get Object Data
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      info.AddValue("Error", Error);
      info.AddValue("Out", Out);
      info.AddValue("ExitCode", ExitCode);
    }

    #endregion ISerializable
  }

}
