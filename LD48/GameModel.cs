using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static LD48.GameConfig;

namespace LD48
{
    static class GameConfig
    {
        public const int LEVELS = 7;

        public const int MAP_ROWS = 3;
        public const int MAP_COLS = 3;
        public const int MAP_SLOT_COUNT = MAP_ROWS * MAP_COLS;
        public const int STAIR_CARD_COUNT = LEVELS;
        public const int ROOMS_PER_LEVEL = MAP_SLOT_COUNT - 2;
        public const int ROOM_CARD_COUNT = ROOMS_PER_LEVEL * LEVELS;
        public const int CLEARED_ROOM_CARDS = ROOMS_PER_LEVEL * LEVELS;
        public const int GOBLIN_CARDS = 100;
        public const int ORC_CARDS = 100;
        public const int DEMON_CARDS = 100;

        public static readonly Random Rand = new();
    }

    static class Box
    {
        public static PlayerToken GetPlayerToken() => new PlayerToken();

        public static Deck<StairCard> GetStairDeck() =>
            CreateDeck(STAIR_CARD_COUNT, i => new StairCard(i + 1));

        public static Deck<RoomCard> GetRoomDeck() =>
            CreateDeck(ROOM_CARD_COUNT, i => new RoomCard());

        public static Deck<ClearedRoomCard> GetClearedRoomDeck() =>
            CreateDeck(CLEARED_ROOM_CARDS, _ => new ClearedRoomCard());

        public static Deck<MonsterCard> GetMonsterDeck() =>
            CreateDeck(GOBLIN_CARDS, _ => (MonsterCard)new GoblinCard())
                .Combine(CreateDeck(ORC_CARDS, _ => (MonsterCard)new OrcCard()))
                .Combine(CreateDeck(DEMON_CARDS, _ => (MonsterCard)new DemonCard()));

        public static Deck<LootCard> GetLootDeck() =>
            CreateDeck(100, _ => (LootCard)new DummyLootCard());

        public static Deck<ActionCard> GetActionsDeck() =>
            CreateDeck(100, _ => (ActionCard)new DummyActionCard());

        private static Deck<TCard> CreateDeck<TCard>(int count, Func<int, TCard> constructor) where TCard : Card =>
            new Deck<TCard>(Enumerable.Range(0, count).Select(i => constructor(i)));
    }

    class GameBoard
    {
        private Dictionary<Point, MapSlot> _map;

        public GameBoard()
        {
            _map = new();
            for (var r = 0; r < MAP_ROWS; r++)
                for (var c = 0; c < MAP_COLS; c++)
                {
                    var key = new Point(c, r);
                    _map[key] = new MapSlot(this, key);
                }
            Level = 1;
        }

        public int Level { get; private set; }
        public Point? PlayerTokenPos { get; private set; }
        public IEnumerable<MapSlot> Map => _map.Values;
        public Deck<StairCard> StairsDeck { get; set; }
        public Deck<RoomCard> RoomDeck { get; set; }
        public Deck<ClearedRoomCard> ClearedRoomDeck { get; set; }
        public Deck<MonsterCard> MonsterDeck { get; set; }
        public Deck<LootCard> LootDeck { get; set; }
        public Deck<ActionCard> ActionsDeck { get; set; }

        public bool CurrentRoomIsResolved => PlayerTokenPos.HasValue &&
            ((_map[PlayerTokenPos.Value].Card is StairCard) ||
            (_map[PlayerTokenPos.Value].Card is ClearedRoomCard));

        public void Reset()
        {
            Level = 1;

            ClearedRoomDeck = Box.GetClearedRoomDeck();

            StairsDeck = Box.GetStairDeck();
            StairsDeck.SortAsc(c => c.Level);
            RoomDeck = Box.GetRoomDeck();
            MonsterDeck = Box.GetMonsterDeck();
            LootDeck = Box.GetLootDeck();
            ActionsDeck = Box.GetActionsDeck();

            var map = StairsDeck.Draw(1)
                .Select(d => d.FlipUp())
                .Concat(StairsDeck.Draw(1))
                .Concat(RoomDeck.Draw(MAP_SLOT_COUNT - 2))
                .OrderBy(_ => Rand.Next(1000))
                .Cast<MapCard>()
                .ToArray();

            var i = 0;
            for (var r = 0; r < MAP_ROWS; r++)
                for (var c = 0; c < MAP_COLS; c++)
                {
                    var card = map[i];
                    var _ = ReplaceCardInMap(c, r, card);
                    if (_ is not null) throw new Exception();

                    if (card is StairCard sc && sc.Level == 1)
                    {
                        PlacePlayerToken(c, r, new PlayerToken());
                    }

                    i++;
                }

            UpdateTravelTargets();
        }

        public void UpdateTravelTargets()
        {
            foreach (var slot in Map)
                slot.IsTravelTarget = false;

            var tokenSlot = Map.Single(c => c.Token != null);

            foreach (var slot in Map.Where(s => s.IsAdjacentTo(tokenSlot)))
                slot.IsTravelTarget = true;
        }


        public MapCard ReplaceCardInMap(int col, int row, MapCard cardToReplaceWith)
        {
            var key = new Point(col, row);
            var replacedCard = _map[key].Card;
            _map[key].Card = cardToReplaceWith;
            return replacedCard;
        }

        public void PlacePlayerToken(int col, int row, PlayerToken playerToken) => PlacePlayerToken(new Point(col, row), playerToken);

        public void PlacePlayerToken(Point pos, PlayerToken playerToken)
        {
            if (PlayerTokenPos.HasValue) throw new Exception();
            _map[pos].Token = playerToken;
            PlayerTokenPos = pos;
        }

        public PlayerToken PickupToken()
        {
            if (!PlayerTokenPos.HasValue) throw new Exception();
            var token = _map[PlayerTokenPos.Value].Token;
            _map[PlayerTokenPos.Value].Token = null;
            PlayerTokenPos = null;
            return token;
        }

        public void MovePlayerHere(MapSlot to)
        {
            if (!to.IsTravelTarget) throw new Exception();
            to.Card.FlipUp();
            var playerToken = PickupToken();
            PlacePlayerToken(to.Position, playerToken);
            UpdateTravelTargets();
        }

        public void Descend(MapSlot from)
        {
            if (!from.IsDescentTarget) throw new Exception();

            Level++;

            var map = new Queue<MapCard>(StairsDeck.Draw(1)
                .Concat(RoomDeck.Draw(ROOMS_PER_LEVEL))
                .OrderBy(_ => Rand.Next(1000))
                .Cast<MapCard>());
                
            foreach (var slot in Map)
            {
                if (slot == from)
                    continue;

                var nextCard = map.Dequeue();
                slot.PushCard(nextCard);
            }

            UpdateTravelTargets();
        }
    }

    class PlayerToken { }

    class MapSlot
    {
        private bool _isTravelTarget = false;
        private readonly Stack<MapCard> _cardLevelStack;

        public MapSlot(GameBoard gameBoard, Point pos)
        {
            _cardLevelStack = new();
            GameBoard = gameBoard;
            Position = pos;
        }

        public GameBoard GameBoard { get; }
        public Point Position { get; }
        public bool IsTravelTarget
        {
            get => _isTravelTarget && GameBoard.CurrentRoomIsResolved;
            set => _isTravelTarget = value;
        }
        
        public bool IsDescentTarget =>
            Card is StairCard sc &&
            sc.Level != GameBoard.Level &&
            !sc.IsFaceDown &&
            Token != null;

        public MapCard Card;
        public PlayerToken Token;

        public bool IsAdjacentTo(MapSlot tokenSlot)
        {
            return Position + new Point(-1, 0) == tokenSlot.Position ||
                Position + new Point(1, 0) == tokenSlot.Position ||
                Position + new Point(0, 1) == tokenSlot.Position ||
                Position + new Point(0, -1) == tokenSlot.Position;
        }

        public void PlayerActionHere()
        {
            if (IsDescentTarget)
            {
                GameBoard.Descend(this);
                return;
            }

            if (!IsTravelTarget) return;
            GameBoard.MovePlayerHere(this);
        }

        public void PushCard(MapCard nextCard)
        {
            _cardLevelStack.Push(Card);
            Card = nextCard;
        }
    }

    abstract class Card
    {
        public bool IsFaceDown { get; private set; } = true;

        public Card FlipUp()
        {
            IsFaceDown = false;
            return this;
        }

        public override string ToString()
        {
            return this.GetType().Name.Replace("Card", "");
        }
    }

    class Deck<TCard> where TCard : Card
    {
        private List<TCard> _cards;

        public Deck(IEnumerable<TCard> startingCards)
        {
            _cards = startingCards.ToList();
        }

        public Deck<TCard> Combine(Deck<TCard> deck)
        {
            _cards.AddRange(deck._cards);
            deck._cards.Clear();
            return this;
        }

        public bool IsEmpty => !_cards.Any();

        public IEnumerable<Card> Draw(int count)
        {
            var drawn = _cards.Take(count).ToArray();
            _cards.RemoveRange(0, count);
            return drawn;
        }

        public void Shuffle()
        {
            _cards = _cards.OrderBy(_ => Rand.Next(1000)).ToList();
        }

        public void SortAsc<TKey>(Func<TCard, TKey> key)
        {
            _cards = _cards.OrderBy(key).ToList();
        }
    }

    class MapCard : Card { }

    class ClearedRoomCard : MapCard { }

    class StairCard : MapCard
    {
        public StairCard(int level)
        {
            Level = level;
        }

        public int Level { get; }

        public override string ToString()
        {
            return Level == 1 ? "Entrance" :
                Level == LEVELS ? "Yendor" :
                $"Stair {Level}";
        }
    }

    class RoomCard : MapCard { }

    abstract class EncounterCard : Card { }

    abstract class MonsterCard : EncounterCard
    {
        protected MonsterCard(int difficulty)
        {
            Difficulty = difficulty;
        }

        public int Difficulty { get; }
    }

    class GoblinCard : MonsterCard { public GoblinCard() : base(1) { } }
    class OrcCard : MonsterCard { public OrcCard() : base(2) { } }
    class DemonCard : MonsterCard { public DemonCard() : base(3) { } }

    abstract class LootCard : EncounterCard { }

    class DummyLootCard : LootCard { }

    abstract class ActionCard : EncounterCard { }

    class DummyActionCard : ActionCard { }
}
