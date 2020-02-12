using System;
using System.Collections.Generic;
using System.Text;

using Gloson.Text.NaturalLanguages;

using NUnit.Framework;

namespace Gloson.Standard.Test.Text.NaturalLanguages {

  public class SoundexTest {
    #region Tests

    public static IEnumerable<TestCaseData> TestCases() {
      return new TestCaseData[] {
        new TestCaseData("ammonium").Returns("A555"),
        new TestCaseData("implementation").Returns("I514"),
        new TestCaseData("Robert").Returns("R163"),
        new TestCaseData("Rupert").Returns("R163"),
        new TestCaseData("Rubin").Returns("R150"),

        new TestCaseData("Ashcraft").Returns("A261"),
        new TestCaseData("Ashcroft").Returns("A261"),
        new TestCaseData("Tymczak").Returns("T522"),
      };
    }

    [TestCaseSource("TestCases")]
    //https://ru.wikipedia.org/wiki/Soundex
    public string TestKnownValues(string value) {
      string actual = Soundex.Encode(value);

      return actual;
    }

    #endregion Tests
  }
}
