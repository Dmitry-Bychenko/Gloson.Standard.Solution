using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Gloson.IO.Compression {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Zip File
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ZipFile {
    #region Private Data

    private const int BufferSize = 8192;

    private static readonly byte[] signature = new byte[] { 0x50, 0x4B, 0x03, 0x04 };

    #endregion Private Data

    #region Algorithm

    private static bool IsZipped(Stream value) {
      if (null == value)
        return false;

      if (!value.CanSeek || !value.CanRead)
        return false;

      long position = value.Position;

      try {
        value.Position = 0;

        byte[] buffer = new byte[signature.Length];

        if (value.Read(buffer, 0, buffer.Length) != buffer.Length)
          return false;

        return signature.SequenceEqual(buffer);
      }
      finally {
        value.Position = position;
      }
    }

    //private static IEnumerable<byte[]> ReadChunks(Stream stream) {
    //  if (null == stream)
    //    yield break;

    //  byte[] buffer = new byte[BufferSize];

    //  while (true) {
    //    int read = stream.Read(buffer, 0, buffer.Length);

    //    if (read <= 0)
    //      break;
    //    else if (read == buffer.Length)
    //      yield return buffer.ToArray();
    //    else {
    //      yield return buffer.Take(read).ToArray();

    //      break;
    //    }
    //  }
    //}

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Obtain Reading Stream either FileStream or GZipStream
    /// </summary>
    /// <param name="fileName">File Name</param>
    public static Stream ReadStream(string fileName, FileMode mode) {
      Stream stm = new FileStream(fileName, mode);

      try {
        if (IsZipped(stm))
          return new GZipStream(stm, CompressionLevel.Optimal);
        else
          return stm;
      }
      catch {
        if (stm != null)
          stm.Dispose();

        throw;
      }
    }

    /// <summary>
    /// Obtain writing Stream
    /// </summary>
    public static Stream WriteStream(string fileName, FileMode mode, CompressionLevel compression) {
      Stream stm = null;

      try {
        stm = new FileStream(fileName, mode, FileAccess.ReadWrite);

        if (mode == FileMode.Append) {
          if (IsZipped(stm))
            return new GZipStream(stm, compression);
          else if (stm.Position == 0 && compression != CompressionLevel.NoCompression)
            return new GZipStream(stm, compression);
          else
            return stm;
        }
        else if (compression != CompressionLevel.NoCompression)
          return new GZipStream(stm, compression);
        else
          return stm;
      }
      catch {
        if (stm != null)
          stm.Dispose();

        throw;
      }
    }

    /// <summary>
    /// Read Bytes
    /// </summary>
    public static IEnumerable<byte> ReadBytes(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using var stm = ReadStream(fileName, FileMode.Open);
        
      while (true) {
        int b = stm.ReadByte();

        if (b < 0)
          yield break;

        yield return (byte)b;
      }
    }

    /// <summary>
    /// Read All Bytes
    /// </summary>
    public static byte[] ReadAllBytes(string fileName) => ReadBytes(fileName).ToArray();

    /// <summary>
    /// Write All bytes
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="bytes">Bytes to write</param>
    /// <param name="compression">Compression Level</param>
    public static void WriteAllBytes(string fileName, IEnumerable<byte> bytes, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == bytes)
        throw new ArgumentNullException(nameof(bytes));

      using Stream stm = WriteStream(fileName, FileMode.Create, compression);

      byte[] buffer = new byte[BufferSize];
      int index = 0;

      foreach (byte b in bytes) {
        buffer[index] = b;
        index += 1;

        if (index >= buffer.Length) {
          stm.Write(buffer, 0, buffer.Length);

          index = 0;
        }
      }

      if (index > 0)
        stm.Write(buffer, 0, index);
    }

    /// <summary>
    /// Write All bytes
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="bytes">Bytes to write</param>
    /// <param name="compression">Compression Level</param>
    public static void AppendAllBytes(string fileName, IEnumerable<byte> bytes, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == bytes)
        throw new ArgumentNullException(nameof(bytes));

      using Stream stm = WriteStream(fileName, FileMode.Append, compression);

      byte[] buffer = new byte[BufferSize];
      int index = 0;

      foreach (byte b in bytes) {
        buffer[index] = b;
        index += 1;

        if (index >= buffer.Length) {
          stm.Write(buffer, 0, buffer.Length);

          index = 0;
        }
      }

      if (index > 0)
        stm.Write(buffer, 0, index);
    }

    /// <summary>
    /// Read All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="encoding">Encoding</param>
    public static String ReadAllText(string fileName, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using StreamReader reader = new StreamReader(ReadStream(fileName, FileMode.Open), encoding);
      
      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    public static String ReadAllText(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using StreamReader reader = new StreamReader(ReadStream(fileName, FileMode.Open), true);
      
      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="encoding">Encoding</param>
    public static IEnumerable<string> ReadLines(string fileName, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using StreamReader reader = new StreamReader(ReadStream(fileName, FileMode.Open), encoding);

      while (true) {
        string line = reader.ReadLine();

        if (null == line)
          yield break;
        else
          yield return line;
      }
    }

    /// <summary>
    /// Read Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    public static IEnumerable<string> ReadLines(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using StreamReader reader = new StreamReader(ReadStream(fileName, FileMode.Open), true);

      while (true) {
        string line = reader.ReadLine();

        if (null == line)
          yield break;
        else
          yield return line;
      }
    }

    /// <summary>
    /// Write All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="text">Text</param>
    /// <param name="compression">Compression</param>
    /// <param name="encoding">Encoding</param>
    public static void WriteAllText(string fileName, string text, CompressionLevel compression, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == text)
        throw new ArgumentNullException(text);

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Create, compression), encoding);

      writer.Write(text);
    }

    /// <summary>
    /// Write All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="text">Text</param>
    /// <param name="compression">Compression</param>
    public static void WriteAllText(string fileName, string text, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == text)
        throw new ArgumentNullException(text);

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Create, compression));
        
      writer.Write(text);
    }

    /// <summary>
    /// Write All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="text">Text</param>
    /// <param name="compression">Compression</param>
    /// <param name="encoding">Encoding</param>
    public static void AppendAllText(string fileName, string text, CompressionLevel compression, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == text)
        throw new ArgumentNullException(text);

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Append, compression), encoding);

      writer.Write(text);
    }

    /// <summary>
    /// Write All Text
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="text">Text</param>
    /// <param name="compression">Compression</param>
    public static void AppendAllText(string fileName, string text, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == text)
        throw new ArgumentNullException(text);

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Append, compression));

      writer.Write(text);
    }

    /// <summary>
    /// Write All Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="lines">Lines to write</param>
    /// <param name="compression">Compression</param>
    /// <param name="encoding">Encoding</param>
    public static void WriteAllLines(string fileName, IEnumerable<string> lines, CompressionLevel compression, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Create, compression), encoding); 

      foreach (var line in lines)
        writer.WriteLine(line);
    }

    /// <summary>
    /// Write All Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="lines">Lines to write</param>
    /// <param name="compression">Compression</param>
    public static void WriteAllLines(string fileName, IEnumerable<string> lines, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Create, compression));

      foreach (var line in lines)
        writer.WriteLine(line);
    }

    /// <summary>
    /// Append All Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="lines">Lines to write</param>
    /// <param name="compression">Compression</param>
    /// <param name="encoding">Encoding</param>
    public static void AppendAllLines(string fileName, IEnumerable<string> lines, CompressionLevel compression, Encoding encoding) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Append, compression), encoding);

      foreach (var line in lines)
        writer.WriteLine(line);
    }

    /// <summary>
    /// Append All Lines
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="lines">Lines to write</param>
    /// <param name="compression">Compression</param>
    public static void AppendAllLines(string fileName, IEnumerable<string> lines, CompressionLevel compression) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));
      else if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      using StreamWriter writer = new StreamWriter(WriteStream(fileName, FileMode.Append, compression));

      foreach (var line in lines)
        writer.WriteLine(line);
    }

    /// <summary>
    /// If file is zipped
    /// </summary>
    public static bool IsZipped(string fileName) {
      if (null == fileName)
        throw new ArgumentNullException(nameof(fileName));

      using Stream stm = ReadStream(fileName, FileMode.Open);
      
      return IsZipped(stm);
    }

    /// <summary>
    /// Copy 
    /// </summary>
    /// <param name="sourceFile">Source File</param>
    /// <param name="targetFile">Target File</param>
    /// <param name="targetFileCompression">Target File Compression</param>
    public static void Move(string sourceFile, string targetFile, CompressionLevel targetFileCompression) {
      if (string.Equals(sourceFile, targetFile, StringComparison.OrdinalIgnoreCase)) {
        byte[] data = ReadAllBytes(sourceFile);

        WriteAllBytes(targetFile, data, targetFileCompression);
      }
      else 
        WriteAllBytes(targetFile, ReadBytes(sourceFile), targetFileCompression);
    }

    #endregion Public
  }
}
