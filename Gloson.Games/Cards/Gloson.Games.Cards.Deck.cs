using Gloson.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Games.Cards {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Suit
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CardSuit : IEquatable<CardSuit>, IComparable<CardSuit> {
    #region Internal Classes

    /// <summary>
    /// Standard Suit Comparer
    /// </summary>
    public class SuitComparer : IComparer<CardSuit> {
      /// <summary>
      /// Standard Construtor
      /// </summary>
      /// <param name="order">Order</param>
      public SuitComparer(string order) {
        if (null == order)
          throw new ArgumentNullException(nameof(order));

        Order = order
          .Trim()
          .ToUpper()
          .Replace('S', '♠')
          .Replace('H', '♥')
          .Replace('D', '♦')
          .Replace('C', '♣')
          .Replace('1', '♠')
          .Replace('2', '♥')
          .Replace('3', '♦')
          .Replace('4', '♣')
          .Replace('N', '-');
      }

      /// <summary>
      /// Order
      /// </summary>
      public string Order { get; }

      /// <summary>
      /// Compare
      /// </summary>
      public int Compare(CardSuit x, CardSuit y) {
        if (ReferenceEquals(x, y))
          return 0;
        else if (null == x)
          return -1;
        else if (null == y)
          return 1;

        int pX = Order.IndexOf(x.Symbol);
        int pY = Order.IndexOf(y.Symbol);

        return pX - pY;
      }
    }

    #endregion Internal Classes

    #region Private Data

    private const string s_StandardOrder = "-♣♦♥♠";

    #endregion Private Data

    #region Create

    // Standard Constructor
    private CardSuit(string name, char symbol) {
      Name = name;
      Symbol = symbol;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out CardSuit result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      return TryParse(char.ToUpper(value.TrimStart()[0]), out result);
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static CardSuit Parse(string value) {
      return TryParse(value, out CardSuit result)
        ? result
        : throw new FormatException($"{value} is not a valid Suit.");
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(char value, out CardSuit result) {
      result = null;

      char code = char.ToUpper(value);

      if (code == 'N' || code == '-' || code == '0') {
        result = None;

        return true;
      }
      else if (code == 'S' || code == '♠' || code == '1' || code == '♤') {
        result = Spades;

        return true;
      }
      else if (code == 'H' || code == '♥' || code == '2' || code == '♡') {
        result = Hearts;

        return true;
      }
      else if (code == 'D' || code == '♦' || code == '3' || code == '♢') {
        result = Diamonds;

        return true;
      }
      else if (code == 'C' || code == '♣' || code == '4' || code == '♧') {
        result = Clubs;

        return true;
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static CardSuit Parse(char value) {
      return TryParse(value, out CardSuit result)
        ? result
        : throw new FormatException($"{value} is not a valid Suit.");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Standard Comparer
    /// </summary>
    public IComparer<CardSuit> StandardComparer { get; } = new SuitComparer("CDHS");

    /// <summary>
    /// Preference Comparer
    /// </summary>
    public IComparer<CardSuit> PreferenceComparer { get; } = new SuitComparer("SCDH");

    /// <summary>
    /// None
    /// </summary>
    public static CardSuit None { get; } = new CardSuit("None", '-');

    /// <summary>
    /// Spades
    /// </summary>
    public static CardSuit Spades { get; } = new CardSuit("Spades", '♠');

    /// <summary>
    /// Hearts
    /// </summary>
    public static CardSuit Hearts { get; } = new CardSuit("Hearts", '♥');

    /// <summary>
    /// Diamonds
    /// </summary>
    public static CardSuit Diamonds { get; } = new CardSuit("Diamonds", '♦');

    /// <summary>
    /// Clubs
    /// </summary>
    public static CardSuit Clubs { get; } = new CardSuit("Clubs", '♣');

    /// <summary>
    /// Suits
    /// </summary>
    public static IReadOnlyList<CardSuit> Suits { get; } = new List<CardSuit>() {
      Spades, Hearts, Diamonds, Clubs,
    };

    /// <summary>
    /// Compare (Standard Bridge Order)
    /// </summary>
    public static int Compare(CardSuit left, CardSuit right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;

      int pLeft = s_StandardOrder.IndexOf(left.Symbol);
      int pRight = s_StandardOrder.IndexOf(right.Symbol);

      return pLeft - pRight;
    }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Symbol
    /// </summary>
    public char Symbol { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Symbol.ToString();

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(CardSuit left, CardSuit right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == right || left == null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(CardSuit left, CardSuit right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == right || null == left)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<Suit>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(CardSuit other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return string.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as CardSuit);

    /// <summary>
    /// Hash Code
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => Name == null ? 0 : Name.GetHashCode();

    #endregion IEquatable<Suit>

    #region IComparable<Suit>

    /// <summary>
    /// Compare (Standard Bridge Order)
    /// </summary>
    public int CompareTo(CardSuit other) => Compare(this, other);

    #endregion IComparable<Suit>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Card Value
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CardValue : IEquatable<CardValue>, IComparable<CardValue> {
    #region Private Data

    private static readonly List<CardValue> s_Items;

    #endregion Private Data

    #region Create

    // Static Constructor
    static CardValue() {
      s_Items = Enumerable
        .Range(0, 15)
        .Select(index => new CardValue(index))
        .ToList();
    }

    // Standard Constructor
    private CardValue(int value) {
      Value = value;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out CardValue result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value)) {
        result = s_Items[0];

        return true;
      }

      if (int.TryParse(value, out int v)) {
        result = (v >= 0 && v < s_Items.Count)
          ? s_Items[v]
          : s_Items[0];

        return true;
      }

      value = value.Trim();

      if (string.Equals("A", value, StringComparison.OrdinalIgnoreCase) || string.Equals("Ace", value, StringComparison.OrdinalIgnoreCase))
        result = s_Items[14];
      else if (string.Equals("K", value, StringComparison.OrdinalIgnoreCase) || string.Equals("King", value, StringComparison.OrdinalIgnoreCase))
        result = s_Items[13];
      else if (string.Equals("Q", value, StringComparison.OrdinalIgnoreCase) || string.Equals("Queen", value, StringComparison.OrdinalIgnoreCase))
        result = s_Items[12];
      else if (string.Equals("J", value, StringComparison.OrdinalIgnoreCase) || string.Equals("Jack", value, StringComparison.OrdinalIgnoreCase))
        result = s_Items[11];
      else if (string.Equals("X", value, StringComparison.OrdinalIgnoreCase))
        result = s_Items[10];
      else
        return false;

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static CardValue Parse(string value) {
      return TryParse(value, out var result)
        ? result
        : throw new FormatException($"{value} is not a valid CardValue");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(CardValue left, CardValue right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;

      return left.Value - right.Value;
    }

    /// <summary>
    /// Value
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name {
      get {
        if (Value == 0)
          return "Joker";
        else if (Value == 1 || Value == 14)
          return "Ace";
        else if (Value >= 2 && Value <= 10)
          return Value.ToString();
        else if (Value == 11)
          return "Jack";
        else if (Value == 12)
          return "Queen";
        else if (Value == 13)
          return "King";
        else
          return "?";
      }
    }

    /// <summary>
    /// Symbol
    /// </summary>
    public string Symbol {
      get {
        if (Value == 0)
          return "?";
        else if (Value == 1 || Value == 14)
          return "A";
        else if (Value >= 2 && Value <= 10)
          return Value.ToString();
        else if (Value == 11)
          return "J";
        else if (Value == 12)
          return "Q";
        else if (Value == 13)
          return "K";
        else
          return "?";
      }
    }

    /// <summary>
    /// ToString (Name)
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Name;

    #endregion Create

    #region Operators

    #region Cast

    /// <summary>
    /// To Integer
    /// </summary>
    public static implicit operator int(CardValue card) {
      return card == null ? 0 : card.Value;
    }

    /// <summary>
    /// From Integer
    /// </summary>
    public static implicit operator CardValue(int value) {
      if (value >= 0 && value < s_Items.Count)
        return s_Items[value];
      else
        return s_Items[0];
    }

    /// <summary>
    /// From Integer
    /// </summary>
    public static CardValue FromInteger(int value) => (CardValue)value;

    #endregion Cast

    #region Comparison

    public static bool operator ==(CardValue left, CardValue right) => Compare(left, right) == 0;

    public static bool operator !=(CardValue left, CardValue right) => Compare(left, right) != 0;

    public static bool operator >(CardValue left, CardValue right) => Compare(left, right) > 0;

    public static bool operator <(CardValue left, CardValue right) => Compare(left, right) < 0;

    public static bool operator >=(CardValue left, CardValue right) => Compare(left, right) >= 0;

    public static bool operator <=(CardValue left, CardValue right) => Compare(left, right) <= 0;

    #endregion Comparison

    #endregion Operators

    #region IEquatable<CardValue>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(CardValue other) {
      if (ReferenceEquals(other, this))
        return true;
      else if (null == other)
        return false;

      return other.Value == Value;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as CardValue);

    /// <summary>
    /// Equals
    /// </summary>
    public override int GetHashCode() => Value;

    #endregion IEquatable<CardValue>

    #region IComparable<CardValue>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(CardValue other) => Compare(this, other);

    #endregion IComparable<CardValue>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Card
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Card : IEquatable<Card>, IComparable<Card> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public Card(CardValue value, CardSuit suit) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));
      else if (null == suit)
        throw new ArgumentNullException(nameof(suit));

      if (Value == 0)
        suit = CardSuit.None;

      Value = value;
      Suit = suit;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out Card result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      if (value.Equals("joker", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("?", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("*", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("any", StringComparison.OrdinalIgnoreCase)) {
        result = new Card(0, CardSuit.None);

        return true;
      }

      HashSet<char> suitsMarks = new HashSet<char>() {
        'S', 'C', 'D', 'H',
        's', 'c', 'd', 'h',
        '♣', '♦', '♥', '♠',
        '♤', '♡', '♢', '♧'
      };

      var suits = value
        .Where(c => suitsMarks.Contains(c))
        .Select(c => CardSuit.Parse(c))
        .Distinct()
        .ToList();

      if (suits.Count != 1)
        return false;

      CardSuit suit = suits[0];

      string valueData = string.Concat(value
        .Where(c => char.IsLetterOrDigit(c))
        .Where(c => !suitsMarks.Contains(c)));

      if (!CardValue.TryParse(valueData, out CardValue cardValue))
        return false;

      result = new Card(cardValue, suit);

      return true;
    }

    /// <summary>
    /// Parse into Card
    /// </summary>
    public static Card Parse(string value) {
      return TryParse(value, out Card result)
        ? result
        : throw new FormatException($"{value} is not a valid Card");
    }

    /// <summary>
    /// Create Joker
    /// </summary>
    public static Card CreateJoker() => new Card(0, CardSuit.None);

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(Card left, Card right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;

      int result = left.Suit.CompareTo(right.Suit);

      if (result != 0)
        return result;

      return left.Value.CompareTo(right.Value);
    }

    /// <summary>
    /// Value
    /// </summary>
    public CardValue Value { get; }

    /// <summary>
    /// Suit
    /// </summary>
    public CardSuit Suit { get; }

    /// <summary>
    /// Is Joker
    /// </summary>
    public bool IsJoker => Suit == CardSuit.None;

    /// <summary>
    /// ToString
    /// </summary>
    public override string ToString() {
      return IsJoker ? "Joker" : $"{Suit.Symbol}{Value.Symbol}";
    }

    #endregion Public

    #region IEquatable<Card>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Card other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Value == other.Value && Suit == other.Suit;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Card);

    /// <summary>
    /// GetHash Code
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return ((Suit.GetHashCode() << 5) ^ Value);
      }
    }

    #endregion IEquatable<Card>

    #region IComparable<Card>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(Card other) => Compare(this, other);

    #endregion IComparable<Card>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Hand
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CardHand : IList<Card>, IEquatable<CardHand> {
    #region Private Data

    private readonly List<Card> m_Items = new List<Card>();

    #endregion Private Data

    #region Algorithm

    private int ReferenceIndex(Card value) {
      if (null == value)
        return -1;

      for (int i = 0; i < m_Items.Count; ++i)
        if (ReferenceEquals(value, m_Items[i]))
          return i;

      return -1;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="isOrdered">Is Ordered (deck) or not (hand)</param>
    public CardHand(bool isOrdered) {
      IsOrdered = isOrdered;
    }

    /// <summary>
    /// Hand
    /// </summary>
    public CardHand() :
      this(false) {
    }

    /// <summary>
    /// Hand
    /// </summary>
    public CardHand(IEnumerable<Card> cards, bool isOrdered)
      : this(isOrdered) {

      if (null == cards)
        throw new ArgumentNullException(nameof(cards));

      foreach (var card in cards) {
        if (null == card)
          throw new ArgumentException("Null cards aren't allowed!", nameof(cards));

        Add(card);
      }
    }

    /// <summary>
    /// Create hand from cards
    /// </summary>
    public CardHand(IEnumerable<Card> cards) : this(cards, false) { }

    /// <summary>
    /// Build Pack
    /// </summary>
    /// <param name="from">From value (included)</param>
    /// <param name="to">To value (included)</param>
    /// <param name="jokers">jokers</param>
    /// <returns></returns>
    public static CardHand BuildPack(int from, int to, int jokers) {
      if (from <= 0)
        throw new ArgumentOutOfRangeException(nameof(from));
      else if (to >= 15)
        throw new ArgumentOutOfRangeException(nameof(to));
      else if (from > to)
        throw new ArgumentOutOfRangeException(nameof(to));
      else if (jokers < 0)
        throw new ArgumentOutOfRangeException(nameof(jokers));

      CardSuit[] suits = new CardSuit[] {
        CardSuit.Spades, CardSuit.Hearts, CardSuit.Diamonds, CardSuit.Clubs
      };

      var cards = Enumerable
        .Range(from, to - from + 1)
        .SelectMany(index => suits.Select(suit => new Card(index, suit)))
        .Concat(Enumerable
          .Range(0, jokers)
          .Select(index => new Card(0, CardSuit.None)));

      CardHand result = new CardHand(true);

      result.m_Items.AddRange(cards);

      return result;
    }

    /// <summary>
    /// Build Pack
    /// </summary>
    /// <param name="from">From value (included)</param>
    /// <param name="to">To value (included)</param>
    /// <returns></returns>
    public static CardHand BuildPack(int from, int to) => BuildPack(from, to, 0);

    /// <summary>
    /// Build pack 52
    /// </summary>
    public static CardHand BuildPack52(int jokers) => BuildPack(2, 14, jokers);

    /// <summary>
    /// Build pack 52
    /// </summary>
    public static CardHand BuildPack52() => BuildPack(2, 14, 0);

    /// <summary>
    /// Build pack 36
    /// </summary>
    public static CardHand BuildPack36(int jokers) => BuildPack(6, 14, jokers);

    /// <summary>
    /// Build pack 36
    /// </summary>
    public static CardHand BuildPack36() => BuildPack(6, 14, 0);

    /// <summary>
    /// Build pack 32
    /// </summary>
    public static CardHand BuildPack32(int jokers) => BuildPack(7, 14, jokers);

    /// <summary>
    /// Build pack 32
    /// </summary>
    public static CardHand BuildPack32() => BuildPack(7, 14, 0);

    /// <summary>
    /// Build pack 24
    /// </summary>
    public static CardHand BuildPack24(int jokers) => BuildPack(9, 14, jokers);

    /// <summary>
    /// Build pack 24
    /// </summary>
    public static CardHand BuildPack24() => BuildPack(9, 14, 0);

    #endregion Create

    #region Public

    /// <summary>
    /// Is Ordered (false for hand, true for deck)
    /// </summary>
    public bool IsOrdered { get; }

    /// <summary>
    /// To Report
    /// </summary>
    public string ToReport() {
      if (IsOrdered)
        return string.Join(" ", m_Items);

      CardSuit[] suits = new CardSuit[] {
        CardSuit.Spades, CardSuit.Hearts, CardSuit.Diamonds, CardSuit.Clubs
      };

      StringBuilder sb = new StringBuilder();

      foreach (CardSuit suit in suits) {
        if (sb.Length > 0)
          sb.AppendLine();

        var cards = string.Join(" ", m_Items
          .Where(card => card.Suit == suit)
          .OrderByDescending(card => card.Value)
          .Select(card => card.Value.Symbol));

        sb.Append($"{suit.Symbol} : {cards}");
      }

      int jokers = m_Items.Count(item => item.IsJoker);

      if (jokers > 0) {
        if (sb.Length > 0)
          sb.AppendLine();

        if (jokers == 1)
          sb.Append($"1 Joker");
        else
          sb.Append($"{jokers} Jokers");
      }

      return sb.ToString();
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      if (IsOrdered)
        return string.Join(" ", m_Items);
      else
        return string.Join(" ", m_Items.OrderBy(item => item.Suit).ThenByDescending(item => item.Value));
    }

    /// <summary>
    /// Jokers
    /// </summary>
    public int Jokers => m_Items.Count(card => card.IsJoker);

    /// <summary>
    /// Peek Card
    /// </summary>
    public Card Peek(int index) {
      if (index < 0 || index >= m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(index));

      Card result = m_Items[index];

      m_Items.RemoveAt(index);

      return result;
    }

    /// <summary>
    /// Peek Top Card 
    /// </summary>
    public Card Peek() => Peek(0);

    /// <summary>
    /// Peek Card
    /// </summary>
    public Card Peek(Card card) {
      int index = IndexOf(card);

      if (index < 0)
        throw new ArgumentException("Card has not been found", nameof(card));

      return Peek(index);
    }

    /// <summary>
    /// Peek hand
    /// </summary>
    public CardHand PeekHand(int length) {
      if (length < 0 || length >= m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(length));

      CardHand result = new CardHand(false);

      for (int i = 0; i < length; ++i)
        result.m_Items.Add(m_Items[i]);

      m_Items.RemoveRange(0, length);

      return result;
    }

    /// <summary>
    /// Shuffle
    /// </summary>
    public void Shuffle() {
      var list = m_Items.Shuffle().ToList();

      m_Items.Clear();
      m_Items.AddRange(list);
    }

    /// <summary>
    /// Shuffle
    /// </summary>
    public void Shuffle(Random random) {
      var list = (null == random)
        ? m_Items.Shuffle().ToList()
        : m_Items.Shuffle(random).ToList();

      m_Items.Clear();
      m_Items.AddRange(list);
    }

    /// <summary>
    /// Shuffle
    /// </summary>
    public void Shuffle(int seed) {
      var list = m_Items.Shuffle(seed).ToList();

      m_Items.Clear();
      m_Items.AddRange(list);
    }

    #endregion Public

    #region IList<Card>

    /// <summary>
    /// Indexer
    /// </summary>
    public Card this[int index] {
      get => m_Items[index];
      set {
        if (null == value) {
          RemoveAt(index);

          return;
        }

        if (index < 0 || index > m_Items.Count)
          throw new ArgumentOutOfRangeException(nameof(index));

        int old;

        if (index == m_Items.Count) {
          old = ReferenceIndex(value);

          if (old >= 0)
            m_Items.RemoveAt(old);

          m_Items.Add(value);

          return;
        }

        if (m_Items[index] == value)
          return;

        old = ReferenceIndex(value);

        if (old < 0) {
          m_Items[index] = value;

          return;
        }

        if (old < index)
          index -= 1;

        m_Items.RemoveAt(old);

        m_Items[index] = value;
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(Card item) {
      if (null == item)
        throw new ArgumentNullException(nameof(item));

      int old = ReferenceIndex(item);

      if (old >= 0)
        m_Items.RemoveAt(old);

      m_Items.Add(item);
    }

    /// <summary>
    /// Insert
    /// </summary>
    public void Insert(int index, Card item) {
      if (index < 0 || index > m_Items.Count)
        throw new ArgumentOutOfRangeException(nameof(index));
      else if (null == item)
        throw new ArgumentNullException(nameof(item));

      int old = ReferenceIndex(item);

      if (old < 0)
        m_Items.Insert(index, item);
      else {
        if (old == index)
          return;

        if (old < index)
          index -= 1;

        m_Items.RemoveAt(old);

        m_Items.Insert(index, item);
      }
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Is ReadOnly
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() => m_Items.Clear();

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(Card item) => m_Items.Contains(item);

    /// <summary>
    /// CopyTo
    /// </summary>
    public void CopyTo(Card[] array, int arrayIndex) => m_Items.CopyTo(array, arrayIndex);

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<Card> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Index Of
    /// </summary>
    public int IndexOf(Card item) => m_Items.IndexOf(item);

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(Card item) => m_Items.Remove(item);

    /// <summary>
    /// Remove At
    /// </summary>
    public void RemoveAt(int index) => m_Items.RemoveAt(index);

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Items.GetEnumerator();

    #endregion IList<Card>

    #region IEquatable<Card>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(CardHand other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      if (IsOrdered != other.IsOrdered)
        return false;
      else if (Count != other.Count)
        return false;

      if (IsOrdered)
        return m_Items.SequenceEqual(other.m_Items);
      else
        return m_Items.OrderBy(x => x).SequenceEqual(other.m_Items.OrderBy(x => x));
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as CardHand);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      if (m_Items.Count <= 0)
        return 0;

      int result = (IsOrdered ? 65536 : 0) ^ m_Items.Count;

      return IsOrdered
        ? result ^ m_Items[0].GetHashCode()
        : result ^ m_Items.Max().GetHashCode();
    }

    #endregion IEquatable<Card>
  }

}
