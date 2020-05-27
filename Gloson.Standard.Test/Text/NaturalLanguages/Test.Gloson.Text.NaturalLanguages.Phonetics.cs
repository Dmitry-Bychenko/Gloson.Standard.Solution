
using Gloson.Text.NaturalLanguages;
using NUnit.Framework;

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
