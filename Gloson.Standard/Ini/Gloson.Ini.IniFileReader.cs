using System;
using System.IO;
using System.Text;

namespace Gloson.Ini {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini File Reader
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniFileReader : IDisposable {
    #region Private Data

    private TextReader m_Reader;

    #endregion Private Data

    #region Create

    // Standard constructor
    private IniFileReader(TextReader reader) {
      m_Reader = reader;
    }

    /// <summary>
    /// Create from Text Reader
    /// </summary>
    public static IniFileReader Create(TextReader reader) {
      if (null == reader)
        throw new ArgumentNullException(nameof(reader));

      return new IniFileReader(reader);
    }

    /// <summary>
    /// From Stream
    /// </summary>
    public static IniFileReader Create(Stream stream) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));

      TextReader reader = null;

      try {
        reader = new StreamReader(stream);

        return Create(reader);
      }
      catch {
        if (null != reader)
          reader.Dispose();

        throw;
      }
    }

    /// <summary>
    /// From Stream
    /// </summary>
    public static IniFileReader Create(Stream stream, Encoding encoding) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));

      TextReader reader = null;

      try {
        reader = new StreamReader(stream, encoding);

        return Create(reader);
      }
      catch {
        if (null != reader)
          reader.Dispose();

        throw;
      }
    }

    /// <summary>
    /// From File Name
    /// </summary>
    public static IniFileReader Create(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      TextReader reader = null;

      try {
        reader = new StreamReader(fileName);

        return Create(reader);
      }
      catch {
        if (null != reader)
          reader.Dispose();

        throw;
      }
    }

    /// <summary>
    /// From File Name
    /// </summary>
    public static IniFileReader Create(string fileName, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      TextReader reader = null;

      try {
        reader = new StreamReader(fileName, encoding);

        return Create(reader);
      }
      catch {
        if (null != reader)
          reader.Dispose();

        throw;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Current
    /// </summary>
    public IIniFileItem Current { get; private set; }

    /// <summary>
    /// Section
    /// </summary>
    public IniFileSection Section { get; private set; }

    /// <summary>
    /// Read
    /// </summary>
    public bool Read() {
      if (null == m_Reader)
        return false;

      int index = 0;

      for (string line = m_Reader.ReadLine(); line != null; line = m_Reader.ReadLine()) {
        index += 1;

        if (string.IsNullOrWhiteSpace(line))
          continue;

        Current = null;

        if (IniFileComment.TryParse(line, out var c))
          Current = c;
        else if (IniFileSection.TryParse(line, out var s)) {
          Section = s;
          Current = s;
        }
        else if (IniFileRecord.TryParse(line, out var r))
          Current = r;

        if (null != Current)
          return true;
        else
          throw new FormatException($"Syntax error at #{index} line");
      }

      return false;
    }

    #endregion Public

    #region IDisposable

    // Dispose
    private void Dispose(bool disposing) {
      if (disposing) {
        if (null != m_Reader)
          m_Reader.Dispose();

        m_Reader = null;
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    #endregion IDisposable
  }
}
