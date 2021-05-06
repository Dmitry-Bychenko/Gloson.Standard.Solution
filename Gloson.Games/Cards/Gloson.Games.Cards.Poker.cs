using Gloson.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Poker Combinations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class PokerCombinationExtensions {
    /// <summary>
    /// Name
    /// </summary>
    public static String Name(this PokerCombination value) => value switch {
      PokerCombination.Flush => "Flush",
      PokerCombination.Four => "Four",
      PokerCombination.FullHouse => "Full House",
      PokerCombination.None => "Single",
      PokerCombination.Pair => "Pair",
      PokerCombination.RoyalFlush => "Flush Royal",
      PokerCombination.Straight => "Straight",
      PokerCombination.StraitFlush => "Straight Flush",
      PokerCombination.Three => "Three",
      PokerCombination.TwoPairs => "Two Pairs",
      _ => "???",
    };
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Poker Best Score
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PokerBestScore
    : IComparable<PokerBestScore>,
      IEquatable<PokerBestScore> {

    #region Private Data

    private readonly List<Card> m_CombinationSet = new();

    #endregion Private Data

    #region Algorithm

    private bool HasStraitFlush() {
      if (Hand.Count < 5)
        return false;

      Card[] jokers = Hand.Where(card => card.IsJoker).ToArray();

      int jokersCount = jokers.Length;

      foreach (var suit in CardSuit.Suits) {
        for (int startAt = 14; startAt >= 6; --startAt) {
          List<Card> list = new();
          int jokerIndex = 0;

          for (int v = 0; v < 5; ++v) {
            Card cardToAdd = Hand.FirstOrDefault(card => card.Value == startAt - v && card.Suit == suit);

            if (cardToAdd is null && jokerIndex < jokers.Length) {
              cardToAdd = jokers[jokerIndex];

              jokerIndex += 1;
            }

            if (cardToAdd is null)
              break;

            list.Add(cardToAdd);
          }

          if (list.Count == 5) {
            m_CombinationSet.Clear();
            m_CombinationSet.AddRange(list);

            MajorValue = 14;

            for (int i = 0; i < list.Count; ++i)
              if (!list[i].IsJoker) {
                MajorValue = i + list[i].Value.Value;

                break;
              }

            if (MajorValue == 14)
              Combination = PokerCombination.RoyalFlush;
            else
              Combination = PokerCombination.StraitFlush;

            return true;
          }
        }
      }

      return false;
    }

    private bool HasStrait() {
      if (Hand.Count < 5)
        return false;

      Card[] jokers = Hand.Where(card => card.IsJoker).ToArray();

      int jokersCount = jokers.Length;

      for (int startAt = 14; startAt >= 6; --startAt) {
        List<Card> list = new();
        int jokerIndex = 0;

        for (int v = 0; v < 5; ++v) {
          Card cardToAdd = Hand.FirstOrDefault(card => card.Value == startAt - v);

          if (cardToAdd is null && jokerIndex < jokers.Length) {
            cardToAdd = jokers[jokerIndex];

            jokerIndex += 1;
          }

          if (cardToAdd is null)
            break;

          list.Add(cardToAdd);
        }

        if (list.Count == 5) {
          Combination = PokerCombination.Straight;

          m_CombinationSet.Clear();
          m_CombinationSet.AddRange(list);

          MajorValue = 14;

          for (int i = 0; i < list.Count; ++i)
            if (!list[i].IsJoker) {
              MajorValue = i + list[i].Value.Value;

              break;
            }

          return true;
        }
      }

      return false;
    }

    private bool HasFlush() {
      if (Hand.Count < 5)
        return false;

      int jokersCount = Hand.Count(card => card.IsJoker);

      var flushes = Hand
        .Where(card => !card.IsJoker)
        .GroupBy(card => card.Suit)
        .Where(group => group.Count() + jokersCount >= 5)
        .Select(group => group.OrderByDescending(card => card.Value).Take(5 - jokersCount))
        .OrderByDescending(group => group.Aggregate(0, (s, a) => s * 20 + a.Value.Value))
        .Select(group => group.ToArray())
        .ToArray();

      if (flushes.Length <= 0)
        return false;

      Combination = PokerCombination.Flush;

      m_CombinationSet.Clear();
      m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker).Take(5));
      m_CombinationSet.AddRange(flushes[0].Take(5 - m_CombinationSet.Count));

      MajorValue = m_CombinationSet.Aggregate(0, (s, a) => s * 20 + (a.IsJoker ? 14 : a.Value.Value));
      MinorValue = 0;

      return true;
    }

    private void CoreUpdate() {
      m_CombinationSet.Clear();
      MajorValue = 0;
      MinorValue = 0;

      if (Hand.Count <= 0)
        return;

      if (Hand.Count == 1) {
        m_CombinationSet.Add(Hand[0]);

        MajorValue = Hand[0].IsJoker ? 14 : Hand[0].Value.Value;

        return;
      }

      if (Hand.All(card => card.IsJoker)) {
        MajorValue = 14;

        m_CombinationSet.AddRange(Hand.Take(5));

        if (Hand.Count >= 5)
          Combination = PokerCombination.Poker;
        else if (Hand.Count >= 4)
          Combination = PokerCombination.Four;
        else if (Hand.Count >= 3)
          Combination = PokerCombination.Three;
        else if (Hand.Count >= 2)
          Combination = PokerCombination.Pair;
        else
          Combination = PokerCombination.None;

        return;
      }

      int jokers = Hand.Count(card => card.IsJoker);

      var bestSeq = Hand
        .Where(card => !card.IsJoker)
        .GroupBy(card => card.Value)
        .Where(group => group.Count() > 1)
        .OrderByDescending(group => group.Count())
        .ThenByDescending(group => group.Key.Value)
        .Take(2)
        .Select(group => group.ToArray())
        .ToArray();

      if (bestSeq.Length > 0) {
        if (bestSeq[0].Length + jokers >= 5) {
          Combination = PokerCombination.Poker;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);

          MajorValue = bestSeq[0][0].Value.Value;

          return;
        }

        if (HasStraitFlush())
          return;

        if (bestSeq[0].Length + jokers >= 4) {
          Combination = PokerCombination.Four;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);

          MajorValue = bestSeq[0][0].Value.Value;

          return;
        }

        if (bestSeq[0].Length + jokers >= 3 && bestSeq.Length > 1 && bestSeq[1].Length == 2) {
          Combination = PokerCombination.FullHouse;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);
          m_CombinationSet.AddRange(bestSeq[1]);

          MajorValue = bestSeq[0][0].Value.Value;
          MinorValue = bestSeq[1][0].Value.Value;

          return;
        }

        if (HasFlush())
          return;

        if (HasStrait())
          return;

        if (bestSeq[0].Length + jokers >= 3) {
          Combination = PokerCombination.Three;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);

          MajorValue = bestSeq[0][0].Value.Value;

          return;
        }

        if (bestSeq[0].Length + jokers >= 2 && bestSeq.Length > 1 && bestSeq[1].Length == 2) {
          Combination = PokerCombination.TwoPairs;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);
          m_CombinationSet.AddRange(bestSeq[1]);

          MajorValue = bestSeq[0][0].Value.Value;
          MinorValue = bestSeq[1][0].Value.Value;

          return;
        }

        if (bestSeq[0].Length + jokers >= 2) {
          Combination = PokerCombination.Pair;

          m_CombinationSet.AddRange(Hand.Where(card => card.IsJoker));
          m_CombinationSet.AddRange(bestSeq[0]);

          MajorValue = bestSeq[0][0].Value.Value;

          return;
        }
      }

      // Nothing
      if (HasStraitFlush())
        return;

      if (HasFlush())
        return;

      if (HasStrait())
        return;

      Combination = PokerCombination.None;

      var maxCard = Hand.OrderByDescending(item => item.Value).First();

      m_CombinationSet.Add(maxCard);
      MajorValue = maxCard.Value.Value;
    }

    #endregion Algorithm

    #region Constructor

    /// <summary>
    /// Poker Best Score
    /// </summary>
    public PokerBestScore(IEnumerable<Card> hand) {
      if (hand is null)
        throw new ArgumentNullException(nameof(hand));

      Hand = hand.ToList();

      CoreUpdate();
    }

    #endregion Constructor

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(PokerBestScore left, PokerBestScore right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      int result = left.Combination.CompareTo(right.Combination);

      if (result != 0)
        return result;

      result = left.MajorValue.CompareTo(right.MajorValue);

      if (result != 0)
        return result;

      return left.MinorValue.CompareTo(right.MinorValue);
    }

    /// <summary>
    /// Hand
    /// </summary>
    public IReadOnlyList<Card> Hand { get; }

    /// <summary>
    /// Combination Set
    /// </summary>
    public IReadOnlyList<Card> CombinationSet => m_CombinationSet;

    /// <summary>
    /// Combination
    /// </summary>
    public PokerCombination Combination { get; private set; }

    /// <summary>
    /// Major Value
    /// </summary>
    public int MajorValue { get; private set; }

    /// <summary>
    /// Major Value
    /// </summary>
    public int MinorValue { get; private set; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return $"{Combination.Name()} : {string.Join(" ", CombinationSet.Select(c => c.IsJoker ? "Joker" : c.ToString()))}";
    }

    #endregion Public

    #region IComparable<PokerBestScore>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(PokerBestScore other) => Compare(this, other);

    #endregion IComparable<PokerBestScore>

    #region IEquatable<PokerBestScore>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(PokerBestScore other) => Compare(this, other) == 0;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object o) => Equals(o as PokerBestScore);

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return (((int)Combination) << 24) ^ (MajorValue << 8) ^ MinorValue;
      }
    }

    #endregion IEquatable<PokerBestScore>
  }
}
