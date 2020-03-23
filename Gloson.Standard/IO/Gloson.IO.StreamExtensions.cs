using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.IO {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Stream Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StreamExtensions {
    #region Constants

    /// <summary>
    /// Default Chunk Size in bytes
    /// </summary>
    public const int DefaultChunkSize = 8192;

    #endregion Constants

    #region Public

    /// <summary>
    /// Read by chunks
    /// </summary>
    /// <param name="stream">Stream to read</param>
    /// <param name="chunkSize">Chunk Size, 0 fro default</param>
    /// <returns>chunks</returns>
    public static IEnumerable<byte[]> ReadChunks(this Stream stream, int chunkSize) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanRead)
        throw new ArgumentException("Stream can't be read", nameof(stream));
      else if (chunkSize < 0)
        throw new ArgumentOutOfRangeException(nameof(chunkSize));

      if (0 == chunkSize)
        chunkSize = DefaultChunkSize;

      byte[] buffer = new byte[chunkSize];

      while (true) {
        int read = stream.Read(buffer, 0, buffer.Length);

        if (read == buffer.Length)
          yield return buffer.ToArray();
        else {
          if (read > 0)
            yield return buffer.Take(read).ToArray();

          break;
        }
      }
    }

    /// <summary>
    /// Read by chunks
    /// </summary>
    /// <param name="stream">Stream to read</param>
    /// <returns>chunks</returns>
    public static IEnumerable<byte[]> ReadChunks(this Stream stream) => ReadChunks(stream, 0);

    /// <summary>
    /// Write All Bytes
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="bytes">Bytes</param>
    /// <param name="chunkSize">Chunk size to use</param>
    /// <returns>bytes written</returns>
    public static long WriteAllBytes(this Stream stream, IEnumerable<byte> bytes, int chunkSize) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanWrite)
        throw new ArgumentException("Stream can't be written", nameof(stream));
      if (null == bytes)
        throw new ArgumentNullException(nameof(bytes));
      else if (chunkSize < 0)
        throw new ArgumentOutOfRangeException(nameof(chunkSize));

      if (0 == chunkSize)
        chunkSize = DefaultChunkSize;

      long count = 0;
      int index = 0;
      long position = stream.CanSeek ? stream.Position : -1;

      byte[] buffer = new byte[chunkSize];

      try {
        foreach (byte b in bytes) {
          buffer[index] = b;

          index += 1;

          if (index >= buffer.Length) {
            index = 0;

            stream.Write(buffer, 0, buffer.Length);

            count += buffer.Length;
          }
        }

        if (index > 0) {
          stream.Write(buffer, 0, index);

          count += index;
        }
      }
      catch {
        if (stream.CanSeek)
          stream.Position = position;

        throw;
      }

      return count;
    }

    /// <summary>
    /// Write All Bytes
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="bytes">Bytes</param>
    /// <returns>bytes written</returns>
    public static long WriteAllBytes(this Stream stream, IEnumerable<byte> bytes) =>
      WriteAllBytes(stream, bytes, 0);

    /// <summary>
    /// Write All Bytes
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="bytes">Bytes</param>
    /// <param name="chunkSize">Chunk size to use</param>
    /// <returns>bytes written</returns>
    public static async Task<long> WriteAllBytesAsync(this Stream stream,
                                                      IEnumerable<byte> bytes,
                                                      CancellationToken token,
                                                      int chunkSize) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));
      else if (!stream.CanWrite)
        throw new ArgumentException("Stream can't be written", nameof(stream));
      if (null == bytes)
        throw new ArgumentNullException(nameof(bytes));
      else if (chunkSize < 0)
        throw new ArgumentOutOfRangeException(nameof(chunkSize));

      if (0 == chunkSize)
        chunkSize = DefaultChunkSize;

      long count = 0;
      int index = 0;
      long position = stream.CanSeek ? stream.Position : -1;

      byte[] buffer = new byte[chunkSize];

      try {
        foreach (byte b in bytes) {
          buffer[index] = b;

          index += 1;

          if (index >= buffer.Length) {
            index = 0;

            token.ThrowIfCancellationRequested();

            await stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);

            count += buffer.Length;
          }
        }

        if (index > 0) {
          token.ThrowIfCancellationRequested();

          await stream.WriteAsync(buffer, 0, index).ConfigureAwait(false);

          count += index;
        }
      }
      catch {
        if (stream.CanSeek)
          stream.Position = position;

        throw;
      }

      return count;
    }

    /// <summary>
    /// Write All Bytes
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="bytes">Bytes</param>
    /// <returns>bytes written</returns>
    public static Task<long> WriteAllBytesAsync(this Stream stream, IEnumerable<byte> bytes, CancellationToken token) =>
      WriteAllBytesAsync(stream, bytes, token, 0);

    /// <summary>
    /// Write All Bytes
    /// </summary>
    /// <param name="stream">Stream</param>
    /// <param name="bytes">Bytes</param>
    /// <returns>bytes written</returns>
    public static Task<long> WriteAllBytesAsync(this Stream stream, IEnumerable<byte> bytes) =>
      WriteAllBytesAsync(stream, bytes, CancellationToken.None, 0);

    #endregion Public
  }

}
