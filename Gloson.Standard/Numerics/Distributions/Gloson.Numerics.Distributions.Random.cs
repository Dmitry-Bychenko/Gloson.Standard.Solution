﻿using System;
using System.Security.Cryptography;
using System.Threading;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Continuous Random 
  /// Thread Safe (when static)
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ContinuousRandom : IDisposable {
    #region Private Data

    // Per Thread Storage
    private ThreadLocal<Random> m_Random;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="distribution">Distribution to use</param>
    /// <param name="seed">Seed</param>
    public ContinuousRandom(IContinuousProbabilityDistribution distribution, int seed) {
      m_Random = new ThreadLocal<Random>(() => new Random(seed));
      Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="distribution">Distribution to use</param>
    public ContinuousRandom(IContinuousProbabilityDistribution distribution) {
      m_Random = new ThreadLocal<Random>(() => {
        int seed;

        using (RNGCryptoServiceProvider provider = new()) {
          byte[] seedData = new byte[sizeof(int)];

          provider.GetBytes(seedData);

          seed = BitConverter.ToInt32(seedData, 0);
        }

        return new Random(seed);
      });

      Distribution = distribution ?? throw new ArgumentNullException(nameof(distribution));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Next Random Value
    /// </summary>
    public double Next() {
      return Distribution.Qdf(m_Random.Value.NextDouble());
    }

    /// <summary>
    /// Distribution to use
    /// </summary>
    public IContinuousProbabilityDistribution Distribution { get; }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() {
      return $"Continuous Random for {Distribution}";
    }

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed => m_Random is null;

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() {
      Dispose(true);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    private void Dispose(bool disposing) {
      if (disposing) {
        if (m_Random is not null)
          m_Random.Dispose();

        m_Random = null;
      }
    }

    #endregion IDisposable
  }

}
