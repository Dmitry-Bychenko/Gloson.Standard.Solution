using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gloson.IO {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// File Lock
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class FileLock : IDisposable {
    #region Private Data

    private FileStream m_Stream;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="fileName">File Name</param>
    public FileLock(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      m_Stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None); 

      FileName = fileName;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// File Name locked
    /// </summary>
    public string FileName { get; }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => m_Stream != null;

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing) {
      if (disposing) {
        if (IsDisposed) {
          m_Stream.Dispose();
        }
      }

      m_Stream = null;
    }

    /// <summary>
    /// Standard Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    #endregion IDisposable
  }

}
