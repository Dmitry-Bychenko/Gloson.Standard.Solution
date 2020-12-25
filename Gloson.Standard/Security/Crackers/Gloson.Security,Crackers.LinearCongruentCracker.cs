using Gloson.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Gloson.Security.Crackers {
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Linear Congruent Random Generator cracker
  /// </summary>
  /// <seealso cref="https://imgur.com/a/RrxEU"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LinearCongruentCracker {
    #region Private Data

    private bool? m_IsValid;

    private readonly List<BigInteger> m_X;

    #endregion Private Data

    #region Algorithm

    private void CrackM() {
      if (Modulo > 0)
        return;

      if (m_X.Count < 5)
        return;

      for (int i = 3; i < m_X.Count; ++i) {
        BigInteger Xn = m_X[i];
        BigInteger Xn1 = m_X[i - 1];
        BigInteger Xn2 = m_X[i - 2];
        BigInteger Xn3 = m_X[i - 3];

        BigInteger Z = (Xn - Xn1) * (Xn2 - Xn3) - (Xn1 - Xn2) * (Xn1 - Xn2);

        if (Z < 0)
          Z = -Z;

        if (Z == 0)
          continue;

        if (Modulo == 0)
          Modulo = Z;
        else
          Modulo = BigInteger.GreatestCommonDivisor(Modulo, Z);
      }
    }

    private void CrackAC() {
      if (0 == Modulo)
        return;

      if (m_X.Count < 3)
        return;

      BigInteger X1 = m_X[0];
      BigInteger X2 = m_X[1];
      BigInteger X3 = m_X[2];

      BigInteger Top = (X2 - X3);
      BigInteger Bottom = (X1 - X2);

      if (Top < 0)
        Top += Modulo;

      if (Bottom < 0)
        Bottom += Modulo;

      A = Top.ModDivision(Bottom, Modulo);
      C = X2 - A * X1;

      C = ((C % Modulo) + Modulo) % Modulo;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="modulo">Modulo, 0 if unknown and must be cracked</param>
    /// <param name="items">Consenquent random items</param>
    public LinearCongruentCracker(BigInteger modulo, IEnumerable<BigInteger> items) {
      if (modulo < 0)
        throw new ArgumentNullException(nameof(modulo));

      if (items is null)
        throw new ArgumentNullException(nameof(items));

      Modulo = modulo;

      m_X = items.ToList();

      CrackM();
      CrackAC();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="items">Consenquent random items</param>
    public LinearCongruentCracker(IEnumerable<BigInteger> items)
      : this(0, items) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Known values
    /// </summary>
    public IReadOnlyList<BigInteger> KnownItems => m_X;

    /// <summary>
    /// Success
    /// </summary>
    public bool Success => Modulo > 0 && A > 0;

    /// <summary>
    /// Is Crack valid one
    /// </summary>
    public bool IsValid {
      get {
        if (m_IsValid.HasValue)
          return m_IsValid.Value;

        if (!Success) {
          m_IsValid = false;

          return false;
        }

        for (int i = 0; i < m_X.Count; ++i)
          if (Next(m_X[i]) != m_X[i + 1]) {
            m_IsValid = false;

            return false;
          }

        m_IsValid = true;

        return true;
      }
    }

    /// <summary>
    /// Generate Next random value 
    /// </summary>
    public BigInteger Next(BigInteger prior) {
      if (!Success)
        throw new InvalidOperationException("Generator is not cracked");

      return (A * prior + C) % Modulo;
    }

    /// <summary>
    /// Modulo
    /// </summary>
    public BigInteger Modulo { get; private set; }

    /// <summary>
    /// A
    /// </summary>
    public BigInteger A { get; private set; }

    /// <summary>
    /// C
    /// </summary>
    public BigInteger C { get; private set; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (!Success)
        return "Not cracked.";

      if (C >= 0)
        return $"({A} * x + {C}) mod {Modulo}";
      else
        return $"({A} * x - {-C}) mod {Modulo}";
    }

    #endregion Public
  }

}
