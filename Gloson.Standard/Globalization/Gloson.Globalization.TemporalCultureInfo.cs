using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Temporary (within using) set provided culture (Invariant Culture by default)
  /// </summary>
  /// <example>
  /// <code language="C#">
  /// using (new TemporalCultureInfo()) {
  ///   ...
  /// }
  /// </code>
  /// </example>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TemporalCultureInfo 
    : IDisposable,
      IEquatable<TemporalCultureInfo> {

    #region Private Data

    // Saved Current Culture
    private CultureInfo m_CurrentCulture;
    // Culture to substitute
    private CultureInfo m_TemporalCulture;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TemporalCultureInfo(CultureInfo temporalCulture) {
      m_CurrentCulture = CultureInfo.CurrentCulture;

      m_TemporalCulture = ReferenceEquals(null, temporalCulture)
        ? CultureInfo.InvariantCulture
        : temporalCulture;

      CultureInfo.CurrentCulture = m_TemporalCulture;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TemporalCultureInfo(string cultureName) {
      m_CurrentCulture = string.IsNullOrEmpty(cultureName)
        ? CultureInfo.InvariantCulture
        : CultureInfo.GetCultureInfo(cultureName);

      CultureInfo.CurrentCulture = m_TemporalCulture;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public TemporalCultureInfo(int culture) {
      m_CurrentCulture = CultureInfo.GetCultureInfo(culture);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TemporalCultureInfo()
      : this(CultureInfo.InvariantCulture) {}

    /// <summary>
    /// Temporal Invariant Culture
    /// </summary>
    public static TemporalCultureInfo InvariantCulture() => new TemporalCultureInfo(CultureInfo.InvariantCulture);

    #endregion Create

    #region Public

    /// <summary>
    /// Culture In use
    /// </summary>
    public CultureInfo CultureInUse => 
      !IsDisposed ? m_TemporalCulture : throw new ObjectDisposedException("Instance has been disposed");

    /// <summary>
    /// Culture Masked
    /// </summary>
    public CultureInfo CultureMasked => 
      !IsDisposed ? m_CurrentCulture : throw new ObjectDisposedException("Instance has been disposed");

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return IsDisposed 
        ? $"Disposed instance of {GetType().Name}"
        : $"Temporary set \"{m_TemporalCulture}\" instead of current {m_CurrentCulture} culture.";
    }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Dispose
    /// </summary>
    private void Dispose(bool disposing) {
      if (disposing) {
        if (!ReferenceEquals(null, m_CurrentCulture)) {
          if (CultureInfo.CurrentCulture == m_TemporalCulture)
            CultureInfo.CurrentCulture = m_CurrentCulture;
          else
            throw new InvalidOperationException(
              $"Failed to restore {m_CurrentCulture.Name}; expected culture {m_TemporalCulture.Name}, actual {CultureInfo.CurrentCulture.Name}");

          m_CurrentCulture = null;
          m_TemporalCulture = null;
        }
      }
    }

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => ReferenceEquals(null, m_CurrentCulture);

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() => Dispose(true);

    #endregion IDisposable

    #region IEquatable<TemporalCulture>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TemporalCultureInfo other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return m_CurrentCulture == other.m_CurrentCulture &&
             m_TemporalCulture == other.m_TemporalCulture &&
             IsDisposed == other.IsDisposed;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as TemporalCultureInfo);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() =>
      null == m_TemporalCulture
        ? 0
        : m_TemporalCulture.GetHashCode();

    #endregion IEquatable<TemporalCulture>
  }
}
