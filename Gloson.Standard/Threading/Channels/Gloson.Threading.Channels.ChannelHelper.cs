using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Gloson.Threading.Channels {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Channel Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ChannelHelper {
    #region Public

    /// <summary>
    /// Generate ChannelReader from source
    /// </summary>
    public static ChannelReader<T> Generate<T>(IEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      Channel<T> channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        foreach (T item in source)
          await channel.Writer.WriteAsync(item);

        channel.Writer.Complete();
      });

      return channel;
    }

    /// <summary>
    /// Generate ChannelReader from many sources
    /// </summary>
    public static ChannelReader<T> GenerateMany<T>(IEnumerable<IEnumerable<T>> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var data = source
        .Where(sequence => sequence is not null)
        .Distinct()
        .ToArray();

      Channel<T> channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        foreach (IEnumerable<T> sequence in data)
          foreach (T item in sequence)
            await channel.Writer.WriteAsync(item);

        channel.Writer.Complete();
      });

      return channel;
    }

    /// <summary>
    /// Generate ChannelReader from source
    /// </summary>
    public static ChannelReader<T> Generate<T>(IAsyncEnumerable<T> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      Channel<T> channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        await foreach (T item in source)
          await channel.Writer.WriteAsync(item);

        channel.Writer.Complete();
      });

      return channel;
    }

    /// <summary>
    /// Generate ChannelReader from sources
    /// </summary>
    public static ChannelReader<T> GenerateMany<T>(IEnumerable<IAsyncEnumerable<T>> source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));

      var data = source
        .Where(sequence => sequence is not null)
        .Distinct()
        .ToArray();

      Channel<T> channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        foreach (IAsyncEnumerable<T> sequence in data)
          await foreach (T item in sequence)
            await channel.Writer.WriteAsync(item);

        channel.Writer.Complete();
      });

      return channel;
    }

    /// <summary>
    /// Combine Channels into one
    /// </summary>
    public static ChannelReader<T> Combine<T>(IEnumerable<ChannelReader<T>> readers) {
      if (readers is null)
        throw new ArgumentNullException(nameof(readers));

      ChannelReader<T>[] source = readers
        .Where(reader => reader is not null)
        .Distinct()
        .ToArray();

      var channel = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        async Task Redirect(ChannelReader<T> reader) {
          await foreach (var item in reader.ReadAllAsync())
            await channel.Writer.WriteAsync(item);
        }

        await Task.WhenAll(source
          .Select(reader => Redirect(reader))
          .ToArray());

        channel.Writer.Complete();
      });

      return channel;
    }

    /// <summary>
    /// Split Channel into count ones
    /// </summary>
    public static ChannelReader<T>[] SplitRoundRobin<T>(ChannelReader<T> reader, int count) {
      if (reader is null)
        throw new ArgumentNullException(nameof(reader));
      if (count <= 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      if (1 == count)
        return new ChannelReader<T>[] { reader };

      var result = new Channel<T>[count];

      for (int i = 0; i < count; i++)
        result[i] = Channel.CreateUnbounded<T>();

      Task.Run(async () => {
        var index = 0;

        await foreach (var item in reader.ReadAllAsync()) {
          await result[index].Writer.WriteAsync(item);

          index = (index + 1) % count;
        }

        foreach (var channel in result)
          channel.Writer.Complete();
      });

      return result
        .Select(ch => ch.Reader)
        .ToArray();
    }

    /// <summary>
    /// Split Channel into count ones
    /// </summary>
    public static ChannelReader<T>[] SplitRound<T>(ChannelReader<T> reader, int count) {
      if (reader is null)
        throw new ArgumentNullException(nameof(reader));
      if (count <= 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      if (1 == count)
        return new ChannelReader<T>[] { reader };

      var result = new Channel<T>[count];

      Task[] tasks = Enumerable
        .Range(1, count)
        .Select(index => Task.Run(async () => {
          await foreach (T item in reader.ReadAllAsync())
            await result[index].Writer.WriteAsync(item);

          result[index].Writer.Complete();
        }))
        .ToArray();

      return result
        .Select(channel => channel.Reader)
        .ToArray();
    }

    /// <summary>
    /// Into Async Enumerable
    /// </summary>
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(ChannelReader<T> reader) {
      if (reader is null)
        throw new ArgumentNullException(nameof(reader));

      return reader.ReadAllAsync();
    }

    #endregion Public
  }

}
