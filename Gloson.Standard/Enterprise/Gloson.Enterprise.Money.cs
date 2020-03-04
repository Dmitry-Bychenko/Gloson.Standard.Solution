using System;
using System.Collections.Generic;
using System.Text;

using Gloson.Globalization;
using Gloson.Numerics;

namespace Gloson.Enterprise {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Money
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public struct Money : IEquatable<Money> {
    #region Private Data
    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public Money(decimal value, CurrencyInfo info) {
      Currency = info ?? throw new ArgumentNullException(nameof(info));
      Value    = value + DecimalHelper.Zero(Currency.Decimals);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Value
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Currency
    /// </summary>
    public CurrencyInfo Currency { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return $"{Value} {Currency.Symbol}";
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Unary +
    /// </summary>
    public static Money operator +(Money value) => value;

    /// <summary>
    /// Unary -
    /// </summary>
    public static Money operator -(Money value) => new Money(-value.Value, value.Currency);

    /// <summary>
    /// Binary +
    /// </summary>
    public static Money operator +(Money left, Money right) {
      return left.Currency == right.Currency
        ? new Money(left.Value + right.Value, left.Currency)
        : throw new ArgumentException("Different currencies can be added", nameof(right));
    }

    /// <summary>
    /// Binary -
    /// </summary>
    public static Money operator -(Money left, Money right) {
      return left.Currency == right.Currency
        ? new Money(left.Value - right.Value, left.Currency)
        : throw new ArgumentException("Different currencies can be added", nameof(right));
    }

    #endregion Operators

    #region IEquatable<Money>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Money other) => other.Value == Value && other.Currency == Currency;

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode() ^ Currency.GetHashCode();

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => (obj is Money other) && Equals(this, other);

    #endregion IEquatable<Money>
  }
}
