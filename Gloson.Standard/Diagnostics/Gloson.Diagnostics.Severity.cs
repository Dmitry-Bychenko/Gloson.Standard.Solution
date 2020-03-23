namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Error Severity
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum ErrorSeverity {
    /// <summary>
    /// Not an error
    /// </summary>
    None = 0,

    /// <summary>
    /// Remark (not an error)
    /// </summary>
    Remark = 1,

    /// <summary>
    /// Trivial
    /// </summary>
    Trivial = 2,

    /// <summary>
    /// Minor
    /// </summary>
    Minor = 3,

    /// <summary>
    /// Major
    /// </summary>
    Major = 4,

    /// <summary>
    /// Critical
    /// </summary>
    Critical = 5,

    /// <summary>
    /// Blocker
    /// </summary>
    Blocker = 6,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Error Priority
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum ErrorPriority {
    /// <summary>
    /// None
    /// </summary>
    None = 0,

    /// <summary>
    /// Low
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High
    /// </summary>
    High = 3,
  }
}
