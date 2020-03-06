using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Gloson.IO {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Byte Order Mark
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ByteOrderMark : IEquatable<ByteOrderMark> {
    #region Create

    private static readonly List<ByteOrderMark> s_Items = new List<ByteOrderMark>();

    private static readonly Dictionary<string, ByteOrderMark> s_ByName =
      new Dictionary<string, ByteOrderMark>(StringComparer.OrdinalIgnoreCase);

    #endregion Create

    #region Algorithm

    private static int MaxSequenceLength() {
      return s_Items
        .Select(item => item.Sequences.Max(seq => seq.Count))
        .Max();
    }

    private static bool StartsWith(byte[] actual, IReadOnlyList<byte> pattern) {
      if (pattern.Count > actual.Length)
        return false;

      for (int i = 0; i < pattern.Count; ++i)
        if (actual[i] != pattern[i])
          return false;

      return true;
    }

    #endregion Algorithm

    #region Create

    static ByteOrderMark() {
      new ByteOrderMark("None", new byte[] { }, Encoding.ASCII);

      new ByteOrderMark("UTF-8", new byte[] { 0xEF, 0xBB, 0xBF }, Encoding.UTF8);
      new ByteOrderMark("UTF-16 Big Endian", new byte[] { 0xFE, 0xFF }, Encoding.BigEndianUnicode);
      new ByteOrderMark("UTF-16 Little Endian", new byte[] { 0xFF, 0xFE }, Encoding.Unicode);

      new ByteOrderMark("UTF-32 Big Endian", new byte[] { 0x00, 0x00, 0xFF, 0xFE }, Encoding.UTF32);
      new ByteOrderMark("UTF-32 Little Endian", new byte[] { 0xFF, 0xFE, 0x00, 0x00 }, Encoding.UTF32);

      new ByteOrderMark("UTF-7", new byte[][] {
        new byte[]{ 0x2B, 0x2F, 0x76, 0x38 },
        new byte[]{ 0x2B, 0x2F, 0x76, 0x39 },
        new byte[]{ 0x2B, 0x2F, 0x76, 0x2B },
        new byte[]{ 0x2B, 0x2F, 0x76, 0x2F },
      }, Encoding.UTF7);

      new ByteOrderMark("UTF-1", new byte[] { 0xF7, 0x64, 0x4C }, Encoding.ASCII);

      new ByteOrderMark("UTF-EBCDIC", new byte[] { 0xDD, 0x73, 0x66, 0x73 }, Encoding.ASCII);
      new ByteOrderMark("SCSU", new byte[] { 0x0E, 0xFE, 0xFF }, Encoding.ASCII);
      new ByteOrderMark("BOCU-1", new byte[] { 0xFB, 0xEE, 0x28 }, Encoding.ASCII);
      new ByteOrderMark("GB-18030", new byte[] { 0x84, 0x31, 0x95, 0x33 }, Encoding.ASCII);
    }

    private ByteOrderMark(string name, IEnumerable<IEnumerable<byte>> sequences, Encoding encoding) {
      if (null == name)
        throw new ArgumentNullException(nameof(name));
      else if (null == sequences)
        throw new ArgumentNullException(nameof(sequences));

      Encoding = encoding;
      Name = name;

      Sequences = sequences
        .Where(seq => seq != null)
        .Select(seq => seq.ToList())
        .ToList();

      s_Items.Add(this);

      s_ByName.Add(Name, this);

      if (Sequences.Count <= 0 || Sequences.Count == 1 && Sequences[0].Count <= 0)
        None = this;
    }

    private ByteOrderMark(string name, IEnumerable<byte> sequences, Encoding encoding)
      : this(name, new[] { sequences }, encoding) { }

    #endregion Create

    #region Public

    /// <summary>
    /// None
    /// </summary>
    public static ByteOrderMark None { get; private set; }

    /// <summary>
    /// All known byte order marks
    /// </summary>
    public static IReadOnlyList<ByteOrderMark> Items => s_Items;

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Encoding
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// Sequence
    /// </summary>
    public IReadOnlyList<IReadOnlyList<byte>> Sequences { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Name;

    #endregion Public

    #region Operators

    /// <summary>
    /// From String
    /// </summary>
    public static ByteOrderMark FromString(string name) {
      if (s_ByName.TryGetValue(name, out var result))
        return result;
      else
        return None;
    }

    /// <summary>
    /// From Sequence
    /// </summary>
    public static ByteOrderMark FromSequence(IEnumerable<byte> sequence) {
      if (null == sequence)
        throw new ArgumentNullException(nameof(sequence));

      byte[] chunk = sequence.Take(MaxSequenceLength()).ToArray();

      return s_Items
        .SelectMany(item => item
          .Sequences
          .Select(seq => new {
            item,
            seq
          }))
        .OrderByDescending(pair => pair.seq.Count)
        .FirstOrDefault(pair => StartsWith(chunk, pair.seq))
       ?.item ?? None;
    }

    /// <summary>
    /// From Stream
    /// </summary>
    public static ByteOrderMark FromStream(Stream stream) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));

      if (!stream.CanRead)
        throw new ArgumentException("Stream can't be read.", nameof(stream));
      else if (!stream.CanSeek)
        throw new ArgumentException("Stream can't be sought.", nameof(stream));

      long saved = stream.Position;

      try {
        stream.Position = 0;

        int size = MaxSequenceLength();

        List<byte> list = new List<byte>(size);

        while (list.Count < size) {
          int v = stream.ReadByte();

          if (v < 0)
            break;

          list.Add((byte) v);
        }

        return FromSequence(list);
      }
      finally {
        stream.Position = saved;
      }

    }

    #endregion Operators

    #region IEquatable<ByteOrderMark>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ByteOrderMark other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return string.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as ByteOrderMark);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (null == Name)
        ? 0
        : Name.GetHashCode();
    }

    #endregion IEquatable<ByteOrderMark>
  }
  
}
