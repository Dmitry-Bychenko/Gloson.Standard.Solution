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
  public enum LoginPasswordServerPolicy {
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

    private static StringComparison ComparisonFor(LoginPasswordServerPolicy left, LoginPasswordServerPolicy right) =>
      ((left & right) == LoginPasswordServerPolicy.None)
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
    public LoginPasswordServer(string login, string password, string server, LoginPasswordServerPolicy policy) {
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
      : this(login, password, server, LoginPasswordServerPolicy.Standard) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="server">Server</param>
    public LoginPasswordServer(string login, string server)
      : this(login, null, server, LoginPasswordServerPolicy.Standard) { }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out LoginPasswordServer result, LoginPasswordServerPolicy policy) {
      result = null;
      
      if (string.IsNullOrWhiteSpace(value))
        return false;

      int p = value.LastIndexOf('@');

      string server = "";

      if (p >= 0) {
        server = value.Substring(p + 1);
        value = value.Substring(0, p);
      }

      string login = value;
      string password = "";

      p = value.IndexOfAny(new char[] { '\\', '/' });

      if (p >= 0) {
        login = value.Substring(0, p);
        password = value.Substring(p + 1);
      }

      login = login.TrimStart();

      if (string.IsNullOrEmpty(login))
        return false;

      result = new LoginPasswordServer(login, password, server, policy);

      return true;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out LoginPasswordServer result) =>
      TryParse(value, out result, LoginPasswordServerPolicy.Standard);

    /// <summary>
    /// Parse
    /// </summary>
    public static LoginPasswordServer Parse(string value, LoginPasswordServerPolicy policy) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      if (TryParse(value, out var result, policy))
        return result;

      throw new FormatException($"Invalid format: string provided can't be parsed as {typeof(LoginPasswordServer).Name}");
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static LoginPasswordServer Parse(string value) => Parse(value, LoginPasswordServerPolicy.Standard);

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
    public LoginPasswordServerPolicy Policy { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (string.IsNullOrEmpty(Password))
        return $"{Login}@{Server}";
      else if (string.IsNullOrEmpty(Server))
        return $"{Login}/{Password}";
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
        string.Equals(Login, other.Login, ComparisonFor(Policy & LoginPasswordServerPolicy.LoginCaseInsensitive, other.Policy)) &&
        string.Equals(Password, other.Password, ComparisonFor(Policy & LoginPasswordServerPolicy.PasswordCaseInsensitive, other.Policy)) &&
        string.Equals(Server, other.Server, ComparisonFor(Policy & LoginPasswordServerPolicy.ServerCaseInsensitive, other.Policy));
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

      if (Policy.HasFlag(LoginPasswordServerPolicy.LoginCaseInsensitive))
        result ^= Login.ToUpperInvariant().GetHashCode();
      else
        result ^= Login.GetHashCode();

      if (Policy.HasFlag(LoginPasswordServerPolicy.ServerCaseInsensitive))
        result ^= Server.ToUpperInvariant().GetHashCode();
      else
        result ^= Server.GetHashCode();

      return result;
    }

    #endregion IEquatable<LoginPasswordServer>
  }
}
