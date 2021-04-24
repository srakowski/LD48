using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static LD48.GameConfig;

namespace LD48
{
    static class GameConfig
    {
        public const int MAP_ROWS = 3;
        public const int MAP_COLS = 3;
        public const int MAP_SLOTS = MAP_ROWS * MAP_COLS;
        public const int STAIR_CARDS = 7;
        public const int ROOM_CARDS = MAP_SLOTS;
        public const int CORRIDOR_CARDS = MAP_SLOTS / 3;
        public const int CLEARED_ROOM_CARDS = (MAP_SLOTS - 1) * STAIR_CARDS;
        public const int GOBLIN_CARDS = 100;
        public const int ORC_CARDS = 100;
        public const int DEMON_CARDS = 100;

        public static readonly Random Rand = new();
    }

    static class Box
    {
        public static GameBoard GetGameBoard() => new GameBoard();

        public static PlayerToken GetPlayerToken() => new PlayerToken();

        public static Deck<StairCard> GetStairDeck() =>
            CreateDeck(STAIR_CARDS, i => new StairCard(i + 1));

        public static Deck<MapCard> GetMapDeck() =>
            GetRoomDeck().Combine(GetCorridorDeck());

        private static Deck<MapCard> GetRoomDeck() =>
            CreateDeck(ROOM_CARDS, i => new RoomCard() as MapCard);

        private static Deck<MapCard> GetCorridorDeck() =>
            CreateDeck(CORRIDOR_CARDS, i => new CorridorCard() as MapCard);

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
                    _map[key] = new MapSlot(key);
                }
        }

        public IEnumerable<MapSlot> Map => _map.Values;
        public Deck<StairCard> StairsDeck { get; set; }
        public Deck<MapCard> MapDeck { get; set; }
        public Deck<ClearedRoomCard> ClearedRoomDeck { get; set; }
        public Deck<MonsterCard> MonsterDeck { get; set; }
        public Deck<LootCard> LootDeck { get; set; }
        public Deck<ActionCard> ActionsDeck { get; set; }

        public MapCard ReplaceCardInMap(int col, int row, MapCard cardToReplaceWith)
        {
            var key = new Point(col, row);
            var replacedCard = _map[key].Card;
            _map[key].Card = cardToReplaceWith;
            return replacedCard;
        }

        public void PlaceToken(int col, int row, PlayerToken playerToken)
        {
            _map[new Point(col, row)].Token = playerToken;
        }
    }

    class PlayerToken { }

    class MapSlot
    {
        public MapSlot(Point pos)
        {
            Position = pos;
        }

        public Point Position { get; }
        public bool IsTravelTarget = false;
        public MapCard Card;
        public PlayerToken Token;

        public bool IsAdjacentTo(MapSlot tokenSlot)
        {
            return Position + new Point(-1, 0) == tokenSlot.Position ||
                Position + new Point(1, 0) == tokenSlot.Position ||
                Position + new Point(0, 1) == tokenSlot.Position ||
                Position + new Point(0, -1) == tokenSlot.Position;
        }
    }

    class GameState
    {
        public void Reset()
        {
            GameBoard = Box.GetGameBoard();
            GameBoard.ClearedRoomDeck = Box.GetClearedRoomDeck();
            
            GameBoard.StairsDeck = Box.GetStairDeck();
            GameBoard.StairsDeck.SortAsc(c => c.Level);

            GameBoard.MapDeck = Box.GetMapDeck();
            GameBoard.MapDeck.Shuffle();

            GameBoard.MonsterDeck = Box.GetMonsterDeck();
            GameBoard.LootDeck = Box.GetLootDeck();
            GameBoard.ActionsDeck = Box.GetActionsDeck();

            var map = GameBoard.StairsDeck.Draw(1)
                .Select(s => s.FlipUp())
                .Concat(GameBoard.StairsDeck.Draw(1))
                .Concat(GameBoard.MapDeck.Draw(MAP_SLOTS - 2))
                .OrderBy(_ => Rand.Next(1000))
                .Cast<MapCard>()
                .ToArray();

            var i = 0;
            for (var r = 0; r < MAP_ROWS; r++)
                for (var c = 0; c < MAP_COLS; c++)
                {
                    var card = map[i];
                    var _ = GameBoard.ReplaceCardInMap(c, r, card);
                    if (_ is not null) throw new Exception();

                    if (card is StairCard sc && sc.Level == 1)
                    {
                        GameBoard.PlaceToken(c, r, new PlayerToken());
                    }

                    i++;
                }

            UpdateTravelTargets();
        }

        public void UpdateTravelTargets()
        { 
            foreach (var slot in GameBoard.Map)
                slot.IsTravelTarget = false;

            var tokenSlot = GameBoard.Map.Single(c => c.Token != null);

            foreach (var slot in GameBoard.Map.Where(s => s.IsAdjacentTo(tokenSlot)))
                slot.IsTravelTarget = true;
        }


        public GameBoard GameBoard { get; private set; }
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
            return $"Stair {Level}";
        }
    }

    class RoomCard : MapCard { }

    class CorridorCard : MapCard { }

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
