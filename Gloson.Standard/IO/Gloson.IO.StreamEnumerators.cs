using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gloson.IO {

  #region Enumerators 

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Byte Enumerator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StreamByteEnumerator : IEnumerator<byte> {
    #region Private Data

    private readonly long m_SavedPosition;

    private int m_Current = -1;

    private readonly bool m_DisposeStream;

    #endregion Private Data

    #region Algorithm

    private void ValidateIfNotDisposed() {
      if (IsDisposed)
        throw new ObjectDisposedException($"this {GetType().Name} instance has been disposed.");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="stream"></param>
    public StreamByteEnumerator(Stream stream, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;

      Stream = stream;
      m_SavedPosition = Stream.Position;

      Stream.Position = 0;
      m_Current = -1;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Stream
    /// </summary>
    public Stream Stream { get; private set; }

    /// <summary>
    /// Current Stream item
    /// </summary>
    public byte Current {
      get {
        ValidateIfNotDisposed();

        return m_Current >= 0
          ? (byte)m_Current
          : throw new InvalidOperationException("Current Item doesn't exist.");
      }
    }

    /// <summary>
    /// Current
    /// </summary>
    object IEnumerator.Current => this.Current;

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => Stream is null;

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      if (Stream is not null) {
        if (m_DisposeStream)
          Stream.Dispose();
        else
          Stream.Position = m_SavedPosition;

        Stream = null;
      }
    }

    /// <summary>
    /// Move Next
    /// </summary>
    /// <returns>If there are any items to read</returns>
    public bool MoveNext() {
      ValidateIfNotDisposed();

      m_Current = Stream.ReadByte();

      return m_Current >= 0;
    }

    /// <summary>
    /// Reset
    /// </summary>
    public void Reset() {
      ValidateIfNotDisposed();

      Stream.Position = 0;
      m_Current = -1;
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Character Enumerator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StreamCharEnumerator : IEnumerator<char> {
    #region Private Data

    private const int BufferSize = 8192;

    private readonly long m_SavedPosition;

    private StreamReader m_Reader;

    private readonly Encoding m_Encoding;

    private readonly bool m_UseBOM;

    private readonly bool m_DisposeStream;

    #endregion Private Data

    #region Algorithm

    private void ValidateIfNotDisposed() {
      if (IsDisposed)
        throw new ObjectDisposedException($"this {GetType().Name} instance has been disposed.");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="encoding">Encoding</param>
    public StreamCharEnumerator(Stream stream, Encoding encoding, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;

      Stream = stream;
      m_SavedPosition = Stream.Position;

      m_Encoding = encoding;
      m_UseBOM = false;

      m_Reader = new StreamReader(stream, m_Encoding, m_UseBOM, BufferSize, true);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="stream">Stream</param>
    public StreamCharEnumerator(Stream stream, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;
      Stream = stream;
      m_SavedPosition = Stream.Position;

      m_Encoding = Encoding.UTF8;
      m_UseBOM = true;

      m_Reader = new StreamReader(stream, m_Encoding, m_UseBOM, BufferSize, true);

      m_Encoding = m_Reader.CurrentEncoding;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Stream
    /// </summary>
    public Stream Stream { get; private set; }

    /// <summary>
    /// Current
    /// </summary>
    public char Current {
      get {
        ValidateIfNotDisposed();

        int value = m_Reader.Peek();

        return value >= 0
          ? (char)value
          : throw new InvalidOperationException("Current Item doesn't exist.");
      }
    }

    /// <summary>
    /// Encoding
    /// </summary>
    public Encoding Encoding {
      get {
        return m_Reader is null ? m_Reader.CurrentEncoding : m_Encoding;
      }
    }

    /// <summary>
    /// Current
    /// </summary>
    object IEnumerator.Current => this.Current;

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => Stream is null;

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      if (Stream is not null) {
        if (m_DisposeStream)
          Stream.Dispose();
        else
          Stream.Position = m_SavedPosition;

        Stream = null;

        m_Reader.Dispose();
        m_Reader = null;
      }
    }

    /// <summary>
    /// Move Next
    /// </summary>
    public bool MoveNext() {
      ValidateIfNotDisposed();

      return m_Reader.Read() >= 0;
    }

    /// <summary>
    /// Reset
    /// </summary>
    public void Reset() {
      ValidateIfNotDisposed();

      m_Reader.Dispose();
      m_Reader = new StreamReader(Stream, Encoding.UTF8, true, BufferSize, true);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Enumerator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StreamStringEnumerator : IEnumerator<string> {
    #region Private Data

    private const int BufferSize = 8192;

    private readonly long m_SavedPosition;

    private StreamReader m_Reader;

    private readonly Encoding m_Encoding;

    private readonly bool m_UseBOM;

    private string m_CurrentLine;

    private readonly bool m_DisposeStream;

    #endregion Private Data

    #region Algorithm

    private void ValidateIfNotDisposed() {
      if (IsDisposed)
        throw new ObjectDisposedException($"this {GetType().Name} instance has been disposed.");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="encoding">Encoding</param>
    public StreamStringEnumerator(Stream stream, Encoding encoding, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;
      m_CurrentLine = null;
      Stream = stream;
      m_SavedPosition = Stream.Position;

      m_Encoding = encoding;
      m_UseBOM = false;

      m_Reader = new StreamReader(stream, m_Encoding, m_UseBOM, BufferSize, true);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="stream">Stream</param>
    public StreamStringEnumerator(Stream stream, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;
      m_CurrentLine = null;
      Stream = stream;
      m_SavedPosition = Stream.Position;

      m_Encoding = Encoding.UTF8;
      m_UseBOM = true;

      m_Reader = new StreamReader(stream, m_Encoding, m_UseBOM, BufferSize, true);

      m_Encoding = m_Reader.CurrentEncoding;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Stream
    /// </summary>
    public Stream Stream { get; private set; }

    /// <summary>
    /// Current
    /// </summary>
    public string Current {
      get {
        ValidateIfNotDisposed();

        return m_CurrentLine ?? throw new InvalidOperationException("Current Item doesn't exist.");
      }
    }

    /// <summary>
    /// Encoding
    /// </summary>
    public Encoding Encoding {
      get {
        return m_Reader is null ? m_Reader.CurrentEncoding : m_Encoding;
      }
    }

    /// <summary>
    /// Current
    /// </summary>
    object IEnumerator.Current => this.Current;

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => Stream is null;

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      if (Stream is not null) {
        if (m_DisposeStream)
          Stream.Dispose();
        else
          Stream.Position = m_SavedPosition;

        Stream = null;

        m_Reader.Dispose();
        m_Reader = null;
      }
    }

    /// <summary>
    /// Move Next
    /// </summary>
    public bool MoveNext() {
      ValidateIfNotDisposed();

      m_CurrentLine = m_Reader.ReadLine();

      return m_CurrentLine is not null;
    }

    /// <summary>
    /// Reset
    /// </summary>
    public void Reset() {
      ValidateIfNotDisposed();

      m_Reader.Dispose();
      m_Reader = new StreamReader(Stream, Encoding.UTF8, true, BufferSize, true);

      m_CurrentLine = null;
    }

    #endregion Public
  }

  #endregion Enumerators

  #region Enumerables

  //--------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Stream Enumerable
  /// </summary>
  //
  //--------------------------------------------------------------------------------------------------------------------

  public sealed class StreamEnumerable
    : IEnumerable<byte>,
      IEnumerable<char>,
      IEnumerable<string> {

    #region Private Data

    private readonly Encoding m_Encoding;

    private readonly bool m_DetectEncoding;

    private readonly bool m_DisposeStream;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StreamEnumerable(Stream stream, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;
      m_Encoding = Encoding.UTF8;
      m_DetectEncoding = true;

      Stream = stream;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StreamEnumerable(Stream stream, Encoding encoding, bool disposeStream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new NotSupportedException("Stream must be readable");
      else if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be sought");

      m_DisposeStream = disposeStream;
      m_Encoding = encoding;
      m_DetectEncoding = false;

      Stream = stream;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Stream 
    /// </summary>
    public Stream Stream { get; }

    /// <summary>
    /// Stream Enumerator
    /// </summary>
    IEnumerator<byte> IEnumerable<byte>.GetEnumerator() =>
      new StreamByteEnumerator(Stream, m_DisposeStream);

    /// <summary>
    /// Stream Enumerator byte enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
      => new StreamByteEnumerator(Stream, m_DisposeStream);

    /// <summary>
    /// Character Enumerator
    /// </summary>
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => m_DetectEncoding
      ? new StreamCharEnumerator(Stream, m_DisposeStream)
      : new StreamCharEnumerator(Stream, m_Encoding, m_DisposeStream);

    /// <summary>
    /// String Enumerator
    /// </summary>
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => m_DetectEncoding
      ? new StreamStringEnumerator(Stream, m_DisposeStream)
      : new StreamStringEnumerator(Stream, m_Encoding, m_DisposeStream);

    #endregion Public
  }

  #endregion Enumerables

  #region Extensions

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Stream Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StreamExtensions {
    #region Public

    /// <summary>
    /// Cast Stream to IEnumerable<T>
    /// </summary>
    /// <typeparam name="T">byte, char or string</typeparam>
    /// <param name="stream">Stream to cast</param>
    /// <param name="encoding">Encoding</param>
    public static IEnumerable<T> Cast<T>(this Stream stream, Encoding encoding) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));

      if (typeof(T) == typeof(byte))
        return new StreamEnumerable(stream, true) as IEnumerable<byte> as IEnumerable<T>;
      else if (typeof(T) == typeof(char))
        return new StreamEnumerable(stream, encoding, true) as IEnumerable<char> as IEnumerable<T>;
      else if (typeof(T) == typeof(string))
        return new StreamEnumerable(stream, encoding, true) as IEnumerable<string> as IEnumerable<T>;
      else
        throw new InvalidCastException($"{typeof(T).Name} is not supported; put byte, char o string.");
    }

    /// <summary>
    /// Cast Stream to IEnumerable<T>
    /// </summary>
    /// <typeparam name="T">byte, char or string</typeparam>
    /// <param name="stream">Stream to cast</param>
    /// <param name="encoding">Encoding</param>
    public static IEnumerable<T> Cast<T>(this Stream stream) {
      if (stream is null)
        throw new ArgumentNullException(nameof(stream));

      if (typeof(T) == typeof(byte))
        return new StreamEnumerable(stream, true) as IEnumerable<byte> as IEnumerable<T>;
      else if (typeof(T) == typeof(char))
        return new StreamEnumerable(stream, true) as IEnumerable<char> as IEnumerable<T>;
      else if (typeof(T) == typeof(string))
        return new StreamEnumerable(stream, true) as IEnumerable<string> as IEnumerable<T>;
      else
        throw new InvalidCastException($"{typeof(T).Name} is not supported; put byte, char o string.");
    }

    /// <summary>
    /// Cast Stream to IEnumerable<T>
    /// </summary>
    /// <typeparam name="T">byte, char or string</typeparam>
    /// <param name="stream">Stream to cast</param>
    /// <param name="encoding">Encoding</param>
    public static IEnumerable<T> AsEnumerable<T>(this Stream stream, Encoding encoding) =>
      Cast<T>(stream, encoding);

    /// <summary>
    /// Cast Stream to IEnumerable<T>
    /// </summary>
    /// <typeparam name="T">byte, char or string</typeparam>
    /// <param name="stream">Stream to cast</param>
    public static IEnumerable<T> AsEnumerable<T>(this Stream stream) =>
      Cast<T>(stream);

    #endregion Public
  }

  #endregion Extensions
}
