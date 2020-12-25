using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Execution result
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ProcessExecutionResult
    : IEquatable<ProcessExecutionResult>,
      IComparable<ProcessExecutionResult>,
      ISerializable {

    #region Create

    private ProcessExecutionResult(int exitCode, string error) {
      AtUtc = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      ProcessPath = "";
      ProcessParameters = "";

      Out = "";
      Error = string.IsNullOrEmpty(error) ? "" : error;
      ExitCode = exitCode;
      Encoding = Encoding.UTF8;
    }

    // Standard constructor
    internal ProcessExecutionResult(
      string processPath,
      string processCommand,
      string stdOut,
      string stdErr,
      int code,
      Encoding encoding) {

      AtUtc = DateTime.UtcNow;

      ProcessPath = string.IsNullOrEmpty(processPath) ? "" : processPath;
      ProcessParameters = string.IsNullOrEmpty(processCommand) ? "" : processCommand;

      Out = string.IsNullOrEmpty(stdOut) ? "" : stdOut;
      Error = string.IsNullOrEmpty(stdErr) ? "" : stdErr;
      ExitCode = code;
      Encoding = encoding;
    }

    /// <summary>
    /// Serialization constructor
    /// </summary>
    internal ProcessExecutionResult(SerializationInfo info, StreamingContext context) {
      AtUtc = info.GetDateTime("AtUtc");
      ProcessPath = info.GetString("Path");
      ProcessParameters = info.GetString("Command");

      Out = info.GetString("Error");
      Error = info.GetString("Out");
      ExitCode = info.GetInt32("ExitCode");

      Encoding = Encoding.GetEncoding(info.GetString("Encoding"));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// None (Stub)
    /// </summary>
    public static ProcessExecutionResult None {
      get;
    } = new ProcessExecutionResult(0, "");

    /// <summary>
    /// Failed to Run (Stub)
    /// </summary>
    public static ProcessExecutionResult FailedToRun {
      get;
    } = new ProcessExecutionResult(int.MinValue, "Failed to run.");

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(ProcessExecutionResult left, ProcessExecutionResult right) {
      if (object.ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      int result = left.AtUtc.CompareTo(right.AtUtc);

      if (result != 0)
        return result;

      result = string.Compare(left.ProcessPath, right.ProcessPath, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.ProcessPath, right.ProcessPath, StringComparison.Ordinal);

      if (result != 0)
        return result;

      result = string.Compare(left.ProcessParameters, right.ProcessParameters, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.ProcessParameters, right.ProcessParameters, StringComparison.Ordinal);

      if (result != 0)
        return result;

      result = left.ExitCode.CompareTo(right.ExitCode);

      if (result != 0)
        return result;

      result = string.Compare(left.Out, right.Out, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.Out, right.Out, StringComparison.Ordinal);

      if (result != 0)
        return result;

      result = string.Compare(left.Error, right.Error, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.Error, right.Error, StringComparison.Ordinal);

      if (result != 0)
        return result;

      return 0;
    }

    /// <summary>
    /// Process Path
    /// </summary>
    public string ProcessPath {
      get;
    }

    /// <summary>
    /// Process Parameters
    /// </summary>
    public string ProcessParameters {
      get;
    }

    /// <summary>
    /// Standard Out
    /// </summary>
    public string Out {
      get;
    }

    /// <summary>
    /// Stamdard Error
    /// </summary>
    public string Error {
      get;
    }

    /// <summary>
    /// Exit Code
    /// </summary>
    public int ExitCode {
      get;
    }

    /// <summary>
    /// Encoding
    /// </summary>
    public Encoding Encoding {
      get;
    }

    /// <summary>
    /// Is Valid
    /// </summary>
    public bool IsValid {
      get {
        return ExitCode == 0;
      }
    }

    /// <summary>
    /// Throw on Error
    /// </summary>
    public void ThrowOnError(bool throwOnErrorMessage = false) {
      if (!((ExitCode != 0) || (throwOnErrorMessage && string.IsNullOrWhiteSpace(Error))))
        return;

      string message;

      if (!string.IsNullOrWhiteSpace(Error))
        message = Error;
      else if (ExitCode == 0)
        message = $"{ProcessPath} failure";
      else
        message = $"{ExitCode} : {ProcessPath} failure";

      throw new IOException(message, ExitCode);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return $"At {AtUtc:yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'} Code {ExitCode} : {ProcessPath} {ProcessParameters}";
    }

    /// <summary>
    /// To Report
    /// </summary>
    public string ToReport() {
      StringBuilder sb = new StringBuilder();

      sb.Append($"Run:       {ProcessPath}");

      sb.AppendLine();
      sb.Append($"Params:    {ProcessParameters}");

      sb.AppendLine();
      sb.Append($"At (UTC):  {AtUtc:yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'}");

      sb.AppendLine();
      sb.Append($"Exit code: {ExitCode} ({(ExitCode == 0 ? "Success" : "Failure")})");

      sb.AppendLine();
      sb.AppendLine();
      sb.Append("Standard Out:");
      sb.AppendLine();
      sb.AppendLine();
      sb.Append(Out);

      sb.AppendLine();
      sb.AppendLine();
      sb.Append("Standard Error:");
      sb.AppendLine();
      sb.AppendLine();
      sb.Append(Error);

      return sb.ToString();
    }

    /// <summary>
    /// Execute Again
    /// </summary>
    public ProcessExecutionResult ExecuteAgain() {
      return ProcessExecutor.Execute(ProcessPath, ProcessParameters, Encoding);
    }

    /// <summary>
    /// Execute Again
    /// </summary>
    public Task<ProcessExecutionResult> ExecuteAgainAsync(CancellationToken token) {
      return ProcessExecutor.ExecuteAsync(ProcessPath, token, ProcessParameters, Encoding);
    }

    /// <summary>
    /// Execute Again
    /// </summary>
    public Task<ProcessExecutionResult> ExecuteAgainAsync() {
      return ProcessExecutor.ExecuteAsync(ProcessPath, ProcessParameters, Encoding);
    }

    #endregion Public

    #region Operators

    #region Cast

    /// <summary>
    /// To Integer (exit code)
    /// </summary>
    public static implicit operator int(ProcessExecutionResult value) {
      return (value is null) ? int.MinValue : value.ExitCode;
    }

    /// <summary>
    /// To Boolean (is valid)
    /// </summary>
    public static implicit operator bool(ProcessExecutionResult value) {
      return value?.IsValid ?? false;
    }

    #endregion Cast

    #region Comparison

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) == 0;
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) != 0;
    }

    /// <summary>
    /// More Or Equal
    /// </summary>
    public static bool operator >=(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) >= 0;
    }

    /// <summary>
    /// Less Or Equal
    /// </summary>
    public static bool operator <=(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) <= 0;
    }

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) > 0;
    }

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(ProcessExecutionResult left, ProcessExecutionResult right) {
      return Compare(left, right) < 0;
    }

    #endregion Comparison

    #endregion Operators

    #region IEquatable<GitResult>

    /// <summary>
    /// At (Utc)
    /// </summary>
    public DateTime AtUtc {
      get;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ProcessExecutionResult other) {
      if (object.ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return ExitCode == other.ExitCode &&
             AtUtc == other.AtUtc &&
             ProcessPath == other.ProcessPath &&
             ProcessParameters == other.ProcessParameters &&
             string.Equals(Out, other.Out) &&
             string.Equals(Error, other.Error);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as ProcessExecutionResult);
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

    #region IComparable<ProcessExecutionResult>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(ProcessExecutionResult other) {
      return Compare(this, other);
    }

    #endregion IComparable<ProcessExecutionResult>

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

      info.AddValue("AtUtc", AtUtc);
      info.AddValue("Path", ProcessPath);
      info.AddValue("Command", ProcessParameters);

      info.AddValue("Encoding", Encoding.EncodingName);
    }

    #endregion ISerializable
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Process Executor
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ProcessExecutor {
    #region Public

    /// <summary>
    /// Execute
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding (Utf8)</param>
    /// <returns></returns>
    public static ProcessExecutionResult Execute(string processPath,
                                                 string processParameters = "",
                                                 Encoding encoding = null) {
      if (processPath is null)
        throw new ArgumentNullException(nameof(processPath));

      encoding ??= Encoding.UTF8;

      ProcessStartInfo info = new ProcessStartInfo() {
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        Arguments = processParameters,
        FileName = processPath,
        StandardErrorEncoding = encoding,
        StandardOutputEncoding = encoding,
      };

      using Process process = new Process() {
        StartInfo = info,
      };

      process.Start();

      StringBuilder sbOut = new StringBuilder();
      StringBuilder sbErr = new StringBuilder();

      process.OutputDataReceived += (sender, e) => {
        if (e.Data is not null) {
          sbOut.AppendLine(e.Data);
        }
      };

      process.ErrorDataReceived += (sender, e) => {
        if (e.Data is not null)
          sbErr.AppendLine(e.Data);
      };

      process.BeginErrorReadLine();
      process.BeginOutputReadLine();

      process.WaitForExit();

      return new ProcessExecutionResult(processPath,
                                        processParameters,
                                        sbOut.ToString(),
                                        sbErr.ToString(),
                                        process.ExitCode,
                                        encoding);
    }

    /// <summary>
    /// Async Process Execution
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding (Utf-8)</param>
    /// <returns></returns>
    public static Task<ProcessExecutionResult> ExecuteAsync(string processPath,
                                                            CancellationToken token,
                                                            string processParameters = "",
                                                            Encoding encoding = null) {
      if (processPath is null)
        throw new ArgumentNullException(nameof(processPath));

      encoding ??= Encoding.UTF8;

      TaskCompletionSource<ProcessExecutionResult> cs = new TaskCompletionSource<ProcessExecutionResult>();

      ProcessStartInfo info = new ProcessStartInfo() {
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        Arguments = processParameters,
        FileName = processPath,
        StandardErrorEncoding = encoding,
        StandardOutputEncoding = encoding,
      };

      StringBuilder sbOut = new StringBuilder();
      StringBuilder sbErr = new StringBuilder();

      token.ThrowIfCancellationRequested();

      Process process = new Process() {
        StartInfo = info,
        EnableRaisingEvents = true
      };

      process.Exited += (s, e) => {
        try {
          if (token.IsCancellationRequested)
            cs.TrySetCanceled(token);
          else
            cs.SetResult(new ProcessExecutionResult(processPath,
                                                    processParameters,
                                                    sbOut.ToString(),
                                                    sbErr.ToString(),
                                                    process.ExitCode,
                                                    encoding));
        }
        finally {
          process.Dispose();
        }
      };

      process.OutputDataReceived += (sender, e) => {
        if (e.Data is not null)
          lock (sbOut) {
            if (sbOut.Length > 0)
              sbOut.AppendLine();

            sbOut.Append(e.Data);
          }
      };

      process.ErrorDataReceived += (sender, e) => {
        if (e.Data is not null)
          lock (sbErr) {
            if (sbErr.Length > 0)
              sbErr.AppendLine();

            sbErr.Append(e.Data);
          }
      };

      process.Start();

      process.BeginErrorReadLine();
      process.BeginOutputReadLine();

      if (token != CancellationToken.None)
        token.Register(() => {
          if (!process.HasExited)
            process.Kill();
        });

      return cs.Task;
    }

    /// <summary>
    /// Async Process Execution
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding (Utf-8)</param>
    /// <returns></returns>
    public static Task<ProcessExecutionResult> ExecuteAsync(string processPath,
                                                            string processParameters = "",
                                                            Encoding encoding = null) {
      return ExecuteAsync(processPath, CancellationToken.None, processParameters, encoding);
    }

    /// <summary>
    /// Fire And Forget
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding</param>
    /// <returns></returns>
    public static int FireAndForget(string processPath,
                                    string processParameters = "") {
      if (processPath is null)
        throw new ArgumentNullException(nameof(processPath));

      ProcessStartInfo info = new ProcessStartInfo() {
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        Arguments = processParameters,
        FileName = processPath,
      };

      using Process process = new Process() {
        StartInfo = info
      };

      process.Start();

      process.WaitForExit();

      return process.ExitCode;
    }

    /// <summary>
    /// Fire And Forget
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding</param>
    /// <returns></returns>
    public static Task<int> FireAndForgetAsync(string processPath,
                                               CancellationToken token,
                                               string processParameters = "") {
      if (processPath is null)
        throw new ArgumentNullException(nameof(processPath));

      ProcessStartInfo info = new ProcessStartInfo() {
        UseShellExecute = false,
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        Arguments = processParameters,
        FileName = processPath,
      };

      TaskCompletionSource<int> cs = new TaskCompletionSource<int>();

      Process process = new Process();

      process.Exited += (s, e) => {
        try {
          if (token.IsCancellationRequested)
            cs.TrySetCanceled(token);
          else
            cs.TrySetResult(process.ExitCode);
        }
        finally {
          process.Dispose();
        }
      };

      if (token != CancellationToken.None)
        token.Register(() => {
          if (!process.HasExited)
            process.Kill();
        });

      process.Start();

      return cs.Task;
    }

    /// <summary>
    /// Fire And Forget
    /// </summary>
    /// <param name="processPath">Process Path</param>
    /// <param name="token">Cancellation token</param>
    /// <param name="processParameters">Process Parameters</param>
    /// <param name="encoding">Encoding</param>
    /// <returns></returns>
    public static Task<int> FireAndForgetAsync(string processPath,
                                               string processParameters = "") {
      return FireAndForgetAsync(processPath, CancellationToken.None, processParameters);
    }

    #endregion Public
  }

}
