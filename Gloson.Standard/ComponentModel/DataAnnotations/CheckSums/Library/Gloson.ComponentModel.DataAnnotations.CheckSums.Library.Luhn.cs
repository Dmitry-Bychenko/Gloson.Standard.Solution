using System.Collections.Generic;

namespace Gloson.ComponentModel.DataAnnotations.CheckSums.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Luhn Check Sum
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Luhn_algorithm"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CheckSumLuhn : CheckSumBase<int> {
    #region Algorithm

    /// <summary>
    /// Perform computations
    /// </summary>
    protected override void Perform() {
      ControlDigit = -1;
      IsValid = false;
      int oddSum = 0;
      int evenSum = 0;
      bool isEven = true;

      for (int i = 0; i < Sequence.Count; ++i) {
        int d = Sequence[i];

        if (d < 0 || d > 9)
          return;

        if (i == Sequence.Count - 1) {
          IsValid = d == (10 - (isEven ? oddSum : evenSum)) % 10;
        }

        int modified = d * 2 < 10 ? d * 2 : d * 2 - 9;

        if (isEven) {
          oddSum += d;
          evenSum += modified;
        }
        else {
          oddSum += modified;
          evenSum += d;
        }

        isEven = !isEven;
        oddSum %= 10;
        evenSum %= 10;
      }

      ControlDigit = (10 - (isEven ? oddSum : evenSum)) % 10;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public CheckSumLuhn(IEnumerable<int> sequence)
      : base(sequence) { }

    #endregion Create 
  }
}
