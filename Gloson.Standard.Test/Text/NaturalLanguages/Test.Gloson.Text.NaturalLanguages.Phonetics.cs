using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Gloson.Text.NaturalLanguages;

namespace Gloson.Standard.Test.Text.NaturalLanguages {
  public class TestPhonetics {
    [TestCase('Y', true)]
    [TestCase('A', true)]
    [TestCase('B', false)]
    [TestCase('w', false)]
    [TestCase('e', true)]
    public void TestVowels(char value, bool expected) {
      Assert.AreEqual(Phonetics.IsVowel(value), expected);
    }
  }
}
