using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Gloson.Linq;

namespace Gloson.Games.Cards {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Poker Combinations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum PokerCombination {
    None = 0,
    Pair = 1,
    TwoPairs = 2,
    Three = 3,
    Straight = 4,
    Flush = 5,
    FullHouse = 6,
    Four = 7,
    StraitFlush = 8,
    RoyalFlush = 9,
    Poker = 10
  }
}
