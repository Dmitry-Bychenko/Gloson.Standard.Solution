using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Services.Git {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Person who commits to Git repository
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GitPerson {
    #region Private Data
    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    internal GitPerson(string name, string email, string date) : base() {
      Name = name ?? "";
      EMail = email ?? "";

      if (string.IsNullOrWhiteSpace(date))
        At = DateTime.MinValue;
      else
        At = DateTime.ParseExact(
               date,
               new string[] { "yyyy-M-d'T'H:m:szzz", "yyyy-M-d'T'H:m:s'Z'", "yyyy-M-d'T'H:m:s" },
               CultureInfo.InvariantCulture,
               DateTimeStyles.AssumeUniversal);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Name
    /// </summary>
    public string Name {
      get;
    }

    /// <summary>
    /// EMail
    /// </summary>
    public string EMail {
      get;
    }

    /// <summary>
    /// At 
    /// </summary>
    public DateTime At {
      get;
    }

    /// <summary>
    /// Name
    /// </summary>
    public override string ToString() {
      return Name;
    }

    #endregion Public
  }

}
