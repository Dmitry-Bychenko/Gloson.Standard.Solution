using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Case policy for Login Password Server
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------
 
  [Flags]
  public enum LoginPasswordServerCases {
    /// <summary>
    /// All case sensitive
    /// </summary>
    None = 0,
    /// <summary>
    /// Login case insensitive
    /// </summary>
    LoginCaseInsensitive = 1,
    /// <summary>
    /// Password case insensitive
    /// </summary>
    PasswordCaseInsensitive = 2,
    /// <summary>
    /// Server case insensitive 
    /// </summary>
    ServerCaseInsensitive = 4,
    /// <summary>
    /// Standard : Login and Server are case insensitive
    /// </summary>
    Standard = LoginCaseInsensitive | ServerCaseInsensitive,
    /// <summary>
    /// All login, password and server are case sensitive
    /// </summary>
    Strict = None,
    /// <summary>
    /// All login, password and server are case insensitive
    /// </summary>
    Lenient = LoginCaseInsensitive | PasswordCaseInsensitive | ServerCaseInsensitive,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Login Password Server
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class LoginPasswordServer : IEquatable<LoginPasswordServer> {
    #region Internal Classes

    // Display Comparer
    private class LoginPasswordServerComparer : IComparer<LoginPasswordServer> {
      public int Compare(LoginPasswordServer x, LoginPasswordServer y) {
        if (ReferenceEquals(x, y))
          return 0;
        else if (ReferenceEquals(null, x))
          return -1;
        else if (ReferenceEquals(null, y))
          return 1;

        int result = string.Compare(x.Server, y.Server, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
          return result;

        result = string.Compare(x.Login, y.Login, StringComparison.OrdinalIgnoreCase);

        if (result != 0)
          return result;

        result = string.Compare(x.Server, y.Server, StringComparison.Ordinal);

        if (result != 0)
          return result;

        result = string.Compare(x.Login, y.Login, StringComparison.Ordinal);

        return result;
      }
    }

    #endregion Internal Classes

    #region Algorithm

    private static StringComparison ComparisonFor(LoginPasswordServerCases left, LoginPasswordServerCases right) =>
      ((left & right) == LoginPasswordServerCases.None)
        ? StringComparison.Ordinal
        : StringComparison.OrdinalIgnoreCase;

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    /// <param name="server">Server</param>
    /// <param name="policy">Policy</param>
    public LoginPasswordServer(string login, string password, string server, LoginPasswordServerCases policy) {
      Login = login ?? "";
      Password = password ?? "";
      Server = server ?? "";

      Policy = policy;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    /// <param name="server">Server</param>
    public LoginPasswordServer(string login, string password, string server)
      : this(login, password, server, LoginPasswordServerCases.Standard) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="server">Server</param>
    public LoginPasswordServer(string login, string server)
      : this(login, null, server, LoginPasswordServerCases.Standard) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer to sort items when displaying only
    /// </summary>
    public static IComparer<LoginPasswordServer> Comparer { get; } = new LoginPasswordServerComparer();

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Has Password
    /// </summary>
    public bool HasPassword => !string.IsNullOrEmpty(Password);

    /// <summary>
    /// Server
    /// </summary>
    public string Server { get; }

    /// <summary>
    /// Policy
    /// </summary>
    public LoginPasswordServerCases Policy { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (string.IsNullOrEmpty(Password))
        return $"{Login}@{Server}";
      else
        return $"{Login}/{Password}@{Server}";
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator == (LoginPasswordServer left, LoginPasswordServer right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (ReferenceEquals(null, left) || ReferenceEquals(null, right))
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator != (LoginPasswordServer left, LoginPasswordServer right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (ReferenceEquals(null, left) || ReferenceEquals(null, right))
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<LoginPasswordServer>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(LoginPasswordServer other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return 
        string.Equals(Login, other.Login, ComparisonFor(Policy & LoginPasswordServerCases.LoginCaseInsensitive, other.Policy)) &&
        string.Equals(Password, other.Password, ComparisonFor(Policy & LoginPasswordServerCases.PasswordCaseInsensitive, other.Policy)) &&
        string.Equals(Server, other.Server, ComparisonFor(Policy & LoginPasswordServerCases.ServerCaseInsensitive, other.Policy));
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as LoginPasswordServer);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      int result = 0;

      if (Policy.HasFlag(LoginPasswordServerCases.LoginCaseInsensitive))
        result ^= Login.ToUpperInvariant().GetHashCode();
      else
        result ^= Login.GetHashCode();

      if (Policy.HasFlag(LoginPasswordServerCases.ServerCaseInsensitive))
        result ^= Server.ToUpperInvariant().GetHashCode();
      else
        result ^= Server.GetHashCode();

      return result;
    }

    #endregion IEquatable<LoginPasswordServer>
  }
}
