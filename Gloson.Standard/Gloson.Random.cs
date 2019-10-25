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

        using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider()) {
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
        if (m_Random != null)
          m_Random.Dispose();

        m_Random = null;
      }
    }

    #endregion IDisposable
  }

}
