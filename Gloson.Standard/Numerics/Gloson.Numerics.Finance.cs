using System;
using System.Collections.Generic;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  // 
  /// <summary>
  /// Finance 
  /// </summary>
  /// <see cref="https://fmhelp.filemaker.com/help/17/fmp/en/index.html#page/FMP_Help%2Fpmt.html%23"/>
  /// <see cref="https://corporatefinanceinstitute.com/resources/knowledge/finance/"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Finance {
    #region Public

    /// <summary>
    /// Future Value (FV)
    /// </summary>
    public static double FutureValue(double payment, double period, double interestRate) {
      if (interestRate < 0)
        throw new ArgumentOutOfRangeException(nameof(interestRate));

      if (interestRate == 0)
        return payment * period;

      return payment / interestRate * (Math.Pow(1 + interestRate, period) - 1);
    }

    /// <summary>
    /// Net Present Value (NPV)
    /// </summary>
    public static double NetPresentValue(double loanAmount, double interestRate, IEnumerable<double> payments) {
      if (interestRate <= -1)
        throw new ArgumentOutOfRangeException(nameof(interestRate));

      double factor = (1 + interestRate);
      double term = factor;

      double result = loanAmount / factor;

      foreach (double payment in payments) {
        factor *= term;

        result += payment / factor;
      }

      return result;
    }

    /// <summary>
    /// Payment (PMT) 
    /// </summary>
    public static double Payment(double payment, double periods, double interestRate) {
      if (interestRate < 0)
        throw new ArgumentOutOfRangeException(nameof(interestRate));

      if (interestRate == 0)
        return payment / periods;

      return payment / ((1 - Math.Pow(1 + interestRate, -periods)) / interestRate);
    }

    /// <summary>
    /// Present Value (PV)
    /// </summary>
    public static double PresentValue(double payment, double periods, double interestRate) {
      if (interestRate < 0)
        throw new ArgumentOutOfRangeException(nameof(interestRate));

      if (interestRate == 0)
        return payment * periods;

      return payment * (1 - Math.Pow(1 + interestRate, -periods)) / interestRate;
    }

    /// <summary>
    /// Internal Rate of Return (IRR)
    /// </summary>
    public static double InternalRateOfReturn(double interestRate, IEnumerable<double> cashFlow) {
      if (interestRate <= -1)
        throw new ArgumentOutOfRangeException(nameof(interestRate));

      if (cashFlow is null)
        throw new ArgumentNullException(nameof(cashFlow));

      double result = 0.0;
      double term = 1 + interestRate;
      double factor = 1;

      foreach (double cash in cashFlow) {
        result += cash / factor;

        factor *= term;
      }

      return result;
    }

    #endregion Public
  }
}
