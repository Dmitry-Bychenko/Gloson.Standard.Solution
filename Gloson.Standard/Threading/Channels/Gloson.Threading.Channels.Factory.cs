﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gloson.Threading.Channels {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Channel Factory
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ChannelReaderFactory {
    #region Public

    /// <summary>
    /// Empty Reader
    /// </summary>
    public static ChannelReader<T> Empty<T>() {
      Channel<T> result = Channel.CreateBounded<T>(1);

      result.Writer.Complete();

      return result.Reader;
    }

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="bufferSize">Limit if channel must be bound</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IEnumerable<T> source,
                                                           int bufferSize,
                                                           CancellationToken token) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      token.ThrowIfCancellationRequested();

      Channel<T> channel;

      if (bufferSize > 0) {
        BoundedChannelOptions options = new(bufferSize) {
          FullMode = BoundedChannelFullMode.Wait
        };

        channel = Channel.CreateBounded<T>(options);
      }
      else
        channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        try {
          foreach (T item in source)
            await channel.Writer.WriteAsync(item, token).ConfigureAwait(false);

          channel.Writer.Complete();
        }
        catch (Exception e) {
          channel.Writer.Complete(e);

          throw;
        }
      }, token);

      return channel.Reader;
    }

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="bufferSize">Limit if channel must be bound</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IEnumerable<T> source, int bufferSize) =>
      ToChannelReader(source, bufferSize, default);

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IEnumerable<T> source, CancellationToken token) =>
      ToChannelReader(source, 0, token);

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IEnumerable<T> source) =>
      ToChannelReader(source, 0, default);

    /// <summary>
    /// Creates channel from IAsyncEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="bufferSize">Limit if channel must be bound</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IAsyncEnumerable<T> source,
                                                           int bufferSize,
                                                           CancellationToken token) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      token.ThrowIfCancellationRequested();

      Channel<T> channel;

      if (bufferSize > 0) {
        BoundedChannelOptions options = new(bufferSize) {
          FullMode = BoundedChannelFullMode.Wait
        };

        channel = Channel.CreateBounded<T>(options);
      }
      else
        channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        try {
          await foreach (T item in source.WithCancellation(token).ConfigureAwait(false))
            await channel.Writer.WriteAsync(item, token).ConfigureAwait(false);

          channel.Writer.Complete();
        }
        catch (Exception e) {
          channel.Writer.Complete(e);

          throw;
        }
      }, token);

      return channel.Reader;
    }

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="bufferSize">Limit if channel must be bound</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IAsyncEnumerable<T> source, int bufferSize) =>
      ToChannelReader(source, bufferSize, default);

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IAsyncEnumerable<T> source, CancellationToken token) =>
      ToChannelReader(source, 0, token);

    /// <summary>
    /// Creates channel from IEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <returns>Channel created</returns>
    public static ChannelReader<T> ToChannelReader<T>(this IAsyncEnumerable<T> source) =>
      ToChannelReader(source, 0, default);

    /// <summary>
    /// Creates channel from IAsyncEnumerable<T>
    /// </summary>
    /// <param name="source">Datasource to convert into channel</param>
    /// <param name="bufferSize">Limit if channel must be bound</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Channel created</returns>

    #endregion Public
  }

}
