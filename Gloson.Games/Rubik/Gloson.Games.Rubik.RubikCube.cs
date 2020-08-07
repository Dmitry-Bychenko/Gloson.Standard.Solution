using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Games.Rubik {
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Rubik Cube
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class RubikCube<T> : IEquatable<RubikCube<T>> {
    #region Private Data

    private static readonly List<char> s_Faces = new List<char>() { 'U', 'F', 'R', 'B', 'L', 'D' };

    private readonly Dictionary<char, T[]> m_Items = new Dictionary<char, T[]>(CharacterComparer.OrdinalIgnoreCase) {
      { 'B', new T[9] },
      { 'D', new T[9] },
      { 'F', new T[9] },
      { 'L', new T[9] },
      { 'R', new T[9] },
      { 'U', new T[9] },
    };

    #endregion Private Data

    #region Algorithm

    private void RotateTriple(string command) {
      command = string.Concat(command.Where(c => char.IsLetterOrDigit(c)));

      for (int r = 0; r < 3; ++r) {
        string[] loop = Enumerable
          .Range(0, 4)
          .Select(i => command.Substring(r * 2 + i * 6, 2))
          .ToArray();

        T h = this[loop[loop.Length - 1]];

        for (int i = loop.Length - 1; i >= 1; --i)
          this[loop[i]] = this[loop[i - 1]];

        this[loop[0]] = h;
      }
    }

    private void CoreRotate(char face) {
      face = char.ToUpper(face);

      if (!m_Items.TryGetValue(face, out T[] items))
        throw new ArgumentOutOfRangeException(nameof(face));

      // Face itself Rotation
      T h = items[0];
      items[0] = items[6];
      items[6] = items[8];
      items[8] = items[2];
      items[2] = h;

      h = items[1];
      items[1] = items[3];
      items[3] = items[7];
      items[7] = items[5];
      items[5] = h;

      if (face == 'U')
        RotateTriple("b2b1b0-r2r1r0-f2f1f0-l2l1l0");
      else if (face == 'D')
        RotateTriple("b6b7b8-l6l7l8-f6f7f8-r6r7r8");
      else if (face == 'F')
        RotateTriple("u6u7u8-r0r3r6-d2d1d0-l8l5l2");
      else if (face == 'B')
        RotateTriple("u2u1u0-l0l3l6-d6d7d8-r8r5r2");
      else if (face == 'R')
        RotateTriple("u8u5u2-b0b3b6-d8d5d2-f8f5f2");
      else if (face == 'L')
        RotateTriple("u0u3u6-f0f3f6-d0d3d6-b8b5b2");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RubikCube() { }

    /// <summary>
    /// Clone (shallow copy)
    /// </summary>
    public RubikCube<T> Clone() {
      RubikCube<T> result = new RubikCube<T>();

      foreach (var pair in m_Items)
        for (int i = 0; i < pair.Value.Length; ++i)
          result.m_Items[pair.Key][i] = pair.Value[i];

      return result;
    }

    /// <summary>
    /// Debug cube
    /// </summary>
    public static RubikCube<string> DebugCube() {
      RubikCube<string> result = new RubikCube<string>();

      foreach (char face in result.Faces)
        for (int i = 0; i < 9; ++i)
          result[face, i] = $"{face}{i}";

      return result;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Moves
    /// </summary>
    public int Moves { get; set; }

    /// <summary>
    /// Faces
    /// </summary>
    public IReadOnlyList<char> Faces => s_Faces;

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyDictionary<char, T[]> Items => m_Items;

    /// <summary>
    /// Indexer
    /// </summary>
    public T this[char face, int index] {
      get {
        if (m_Items.TryGetValue(face, out var line))
          if (index >= 0 && index < line.Length)
            return line[index];
          else
            throw new ArgumentOutOfRangeException(nameof(index));
        else
          throw new ArgumentOutOfRangeException(nameof(face));
      }
      set {
        if (m_Items.TryGetValue(face, out var line))
          if (index >= 0 && index < line.Length)
            line[index] = value;
          else
            throw new ArgumentOutOfRangeException(nameof(index));
        else
          throw new ArgumentOutOfRangeException(nameof(face));
      }
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public T this[(char face, int index) item] {
      get {
        if (m_Items.TryGetValue(item.face, out var line))
          if (item.index >= 0 && item.index < line.Length)
            return line[item.index];
          else
            throw new ArgumentOutOfRangeException(nameof(item), "index is out of range");
        else
          throw new ArgumentOutOfRangeException(nameof(item), "face is out of range");
      }
      set {
        if (m_Items.TryGetValue(item.face, out var line))
          if (item.index >= 0 && item.index < line.Length)
            line[item.index] = value;
          else
            throw new ArgumentOutOfRangeException(nameof(item), "index is out of range");
        else
          throw new ArgumentOutOfRangeException(nameof(item), "face is out of range");
      }
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public T this[string address] {
      get {
        if (null == address)
          throw new ArgumentNullException(nameof(address));

        if (address.Length != 2 || !m_Items.ContainsKey(address[0]) || address[1] < '0' || address[1] > '8')
          throw new ArgumentException("Invalid address format", nameof(address));

        return m_Items[address[0]][address[1] - '0'];
      }
      set {
        if (null == address)
          throw new ArgumentNullException(nameof(address));

        if (address.Length != 2 || !m_Items.ContainsKey(address[0]) || address[1] < '0' || address[1] > '8')
          throw new ArgumentException("Invalid address format", nameof(address));

        m_Items[address[0]][address[1] - '0'] = value;
      }
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public T[] this[char face] {
      get => m_Items.TryGetValue(face, out T[] result)
        ? result
        : throw new ArgumentOutOfRangeException(nameof(face));
    }

    /// <summary>
    /// Rotate clockwise
    /// </summary>
    public void Rotate(char face, int steps = 1) {
      if (!m_Items.ContainsKey(face))
        throw new ArgumentOutOfRangeException(nameof(face));

      steps = (steps % 4 + 4) % 4;

      for (int i = 0; i < steps; ++i)
        CoreRotate(face);

      if (steps != 0)
        Moves += 1;
    }

    /// <summary>
    /// Perform Command 
    /// like "R2,D-1,FF"
    /// </summary>
    public bool TryPerform(string command) {
      if (null == command)
        return false;

      var comms = Regex
        .Matches(command, @"(?<face>[A-Za-z])(?<count>\-?[0-9]*)?")
        .Cast<Match>()
        .Select(m => (face: m.Groups["face"].Value, count: m.Groups["count"].Value));

      List<(char face, int count)> rotates = new List<(char face, int count)>();

      foreach (var comm in comms) {
        char face = char.ToUpperInvariant(comm.face[0]);

        if (face != 'B' && face != 'L' && face != 'U' && face != 'R' && face != 'F' && face != 'D')
          return false;

        if (!int.TryParse(comm.count, out int value)) {
          if (comm.count.StartsWith("-"))
            value = -1;
          else
            value = 1;
        }

        rotates.Add((face, value));
      }

      foreach (var (face, count) in rotates)
        Rotate(face, count);

      return true;
    }

    /// <summary>
    /// Perform command
    /// like "R2,D-1,FF"
    /// </summary>
    public void Perform(string command) {
      if (null == command)
        throw new ArgumentNullException(nameof(command));

      if (!TryPerform(command))
        throw new FormatException("Invalid command format");
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      Dictionary<char, string> faces = new Dictionary<char, string>() {
        { 'U', "Up" },
        { 'F', "Facade" },
        { 'R', "Right" },
        { 'B', "Back" },
        { 'L', "Left" },
        { 'D', "Down" }
      };

      StringBuilder sb = new StringBuilder();

      foreach (var pair in faces) {
        if (sb.Length > 0) {
          sb.AppendLine();
          sb.AppendLine();
        }

        sb.Append($"{pair.Value} ({pair.Key}): ");

        T[] line = this[pair.Key];

        for (int i = 0; i < 3; ++i) {
          sb.AppendLine();
          sb.Append($"    {string.Join(" ", line.Skip(i * 3).Take(3))}");
        }

      }

      return sb.ToString();
    }

    /// <summary>
    /// Cube Net
    /// </summary>
    public string NetView() {
      string Line(string command) {
        return string.Join(" ", command
          .Where(c => char.IsLetterOrDigit(c))
          .Select((c, i) => new { c, i })
          .GroupBy(item => item.i / 2, item => item.c)
          .Select(group => string.Concat(group))
          .Select(c => this[c]));
      }

      int shift = Line("l6l3l0").Length;
      shift = Math.Max(shift, Line("l7l4l1").Length);
      shift = Math.Max(shift, Line("l8l5l2").Length);

      string pad = new string(' ', shift + 1);

      return string.Join(Environment.NewLine,
        pad + Line("b8b7b6"),
        pad + Line("b5b4b3"),
        pad + Line("b2b1b0"),

        Line("l6l3l0-u0u1u2-r2r5r8"),
        Line("l7l4l1-u3u4u5-r1r4r7"),
        Line("l8l5l2-u6u7u8-r0r3r6"),

        pad + Line("f0f1f2"),
        pad + Line("f3f4f5"),
        pad + Line("f6f7f8"),

        pad + Line("d0d1d2"),
        pad + Line("d3d4d5"),
        pad + Line("d6d7d8")
      );
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(RubikCube<T> left, RubikCube<T> right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(RubikCube<T> left, RubikCube<T> right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<RubikCube<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(RubikCube<T> other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return s_Faces.All(face => other.m_Items[face].SequenceEqual(m_Items[face]));
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as RubikCube<T>);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (m_Items['F'][0]?.GetHashCode() ?? 0) ^
             (m_Items['B'][1]?.GetHashCode() ?? 0) ^
             (m_Items['D'][0]?.GetHashCode() ?? 4) ^
             (m_Items['R'][8]?.GetHashCode() ?? 0);
    }

    #endregion IEquatable<RubikCube<T>>
  }

}
