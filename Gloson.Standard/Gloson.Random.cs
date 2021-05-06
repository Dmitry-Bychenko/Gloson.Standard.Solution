using System;
using System.Security.Cryptography;
using System.Threading;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Random Thread Safe 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RandomThreadSafe : IDisposable {
    #region Private Data

    // Per Thread Storage
    private ThreadLocal<Random> m_Random;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Random with seed (which is equal for all threads)
    /// </summary>
    /// <param name="seed">Seed</param>
    public RandomThreadSafe(int seed) {
      m_Random = new ThreadLocal<Random>(() => new Random(seed));
    }

    /// <summary>
    ///  Random with no seed (each thread has its own seed)
    /// </summary>
    public RandomThreadSafe() {
      m_Random = new ThreadLocal<Random>(() => {
        int seed;

        using (RNGCryptoServiceProvider provider = new()) {
          byte[] seedData = new byte[sizeof(int)];

          provider.GetBytes(seedData);

          seed = BitConverter.ToInt32(seedData, 0);
        }

        return new Random(seed);
      });
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static RandomThreadSafe Default {
      get;
    } = new RandomThreadSafe();

    /// <summary>
    /// Next
    /// </summary>
    public int Next() {
      return m_Random.Value.Next();
    }

    /// <summary>
    /// Next
    /// </summary>
    public int Next(int upTo) {
      return m_Random.Value.Next(upTo);
    }

    /// <summary>
    /// Next
    /// </summary>
    public int Next(int from, int upTo) {
      return m_Random.Value.Next(from, upTo);
    }

    /// <summary>
    /// Bytes to Set
    /// </summary>
    public void NextBytes(byte[] bytesToSet) {
      m_Random.Value.NextBytes(bytesToSet);
    }

    /// <summary>
    /// Next Double
    /// </summary>
    public double NextDouble() {
      return m_Random.Value.NextDouble();
    }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    private void Dispose(bool disposing) {
      if (disposing) {
        if (m_Random is not null)
          m_Random.Dispose();

        m_Random = null;
      }
    }

    #endregion IDisposable
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Random Linear Congruent Generator 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RandomLinearCongruent : IEquatable<RandomLinearCongruent> {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="a">A parameter</param>
    /// <param name="c">C parameter</param>
    /// <param name="m">M parameter</param>
    /// <param name="seed">Seed</param>
    public RandomLinearCongruent(long a, long c, long m, long seed) {
      A = a >= 1 ? a : throw new ArgumentOutOfRangeException(nameof(a));
      C = c >= 0 ? c : throw new ArgumentOutOfRangeException(nameof(c));
      M = m >= 2 ? m : throw new ArgumentOutOfRangeException(nameof(m));
      Seed = seed >= 0 ? seed : throw new ArgumentOutOfRangeException(nameof(seed));
    }

    /// <summary>
    /// Standard construcor
    /// </summary>
    /// <param name="seed">Seed</param>
    public RandomLinearCongruent(long seed)
      : this(445, 700001, 2097152, seed) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RandomLinearCongruent()
      : this(DateTime.Now.Ticks) { }

    #endregion Create

    #region Public

    /// <summary>
    /// A parameter
    /// </summary>
    public long A { get; }

    /// <summary>
    /// C parameter
    /// </summary>
    public long C { get; }

    /// <summary>
    /// M parameter
    /// </summary>
    public long M { get; }

    /// <summary>
    /// Seed
    /// </summary>
    public long Seed { get; private set; }

    /// <summary>
    /// Next
    /// </summary>
    public long Next() {
      Seed = (A * Seed + C) % M;

      return Seed;
    }

    /// <summary>
    /// Next double
    /// </summary>
    public double NextDouble() => ((double)Next()) / M;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"({A} * {Seed} + {C}) mod {M}";

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(RandomLinearCongruent left, RandomLinearCongruent right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(RandomLinearCongruent left, RandomLinearCongruent right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (left is null || right is null)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<RandomLinearCongruent>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(RandomLinearCongruent other) {
      if (other is null)
        return false;

      return A == other.A &&
             C == other.C &&
             M == other.M &&
             Seed == other.Seed;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as RandomLinearCongruent);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return unchecked((int)(A ^ C ^ M ^ Seed));
    }

    #endregion IEquatable<RandomLinearCongruent>
  }
}
