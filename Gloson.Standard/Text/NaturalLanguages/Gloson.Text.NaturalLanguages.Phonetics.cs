﻿using System.Collections.Generic;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Phonetics
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Phonetics {
    #region Private Data

    private static readonly HashSet<char> s_Vowels = new() {
      'a',
      'e',
      'i',
      'o',
      'u',
      'y',
      'A',
      'E',
      'I',
      'O',
      'U',
      'Y',
    };

    private static readonly HashSet<char> s_Consonants = new() {
      'B',
      'C',
      'D',
      'F',
      'G',
      'H',
      'J',
      'K',
      'L',
      'M',
      'N',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'V',
      'W',
      'X',
      'Z',
      'b',
      'c',
      'd',
      'f',
      'g',
      'h',
      'j',
      'k',
      'l',
      'm',
      'n',
      'p',
      'q',
      'r',
      's',
      't',
      'v',
      'w',
      'x',
      'z',
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Is Vowel
    /// </summary>
    public static bool IsVowel(char value) => s_Vowels.Contains(value);

    /// <summary>
    /// Is Consonant
    /// </summary>
    public static bool IsConsonant(char value) => s_Consonants.Contains(value);

    #endregion Public
  }
}
