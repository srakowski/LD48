namespace LD48
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static DieColor;
    using static DieFace;
    using static R;

    enum DieFace
    {
        None = 0,
        Goblin,
        Loot,
        Door
    }

    enum DieColor
    {
        None = 0,
        Green,
        Yellow,
        Red
    }

    class R
    {
        public static readonly Random Rand = new();
    }

    class Die
    {
        private readonly DieFace[] _faces;

        private Die(DieColor color, params DieFace[] faces)
        {
            Color = color;
            _faces = faces;
            if (_faces.Length != 6) throw new Exception();
        }

        public DieColor Color { get; }

        public DieFace FaceUp { get; private set; }

        public void Roll()
        {
            FaceUp = _faces[Rand.Next(_faces.Length)];
        }

        public static Die GreenDie => 
            new Die(Green, Loot, Loot, Loot, Goblin, Door, Door);

        public static Die YellowDie =>
            new Die(Yellow, Loot, Loot, Goblin, Goblin, Door, Door);

        public static Die RedDie =>
            new Die(Red, Loot, Goblin, Goblin, Goblin, Door, Door);
    }

    class Cup
    {
        public List<Die> _diceInCup = new();

        public void PlaceDiceInCup(IEnumerable<Die> dice)
        {
            _diceInCup.AddRange(dice);
        }

        public void Shake()
        {
            _diceInCup = _diceInCup.OrderBy(r => Rand.Next(1000)).ToList();
        }

        public IEnumerable<Die> DrawDice(int count)
        {
            var dice = _diceInCup.Take(count).ToArray();
            _diceInCup.RemoveRange(0, dice.Length);
            return dice;
        }

        internal IEnumerable<Die> DrawAllDice()
        {
            var dice = _diceInCup.ToArray();
            _diceInCup.Clear();
            return dice;
        }
    }

    static class Box
    {
        public static Die[] GetDice() => new[]
        {
            Die.GreenDie, Die.GreenDie, Die.GreenDie, Die.GreenDie, Die.GreenDie, Die.GreenDie,
            Die.YellowDie, Die.YellowDie, Die.YellowDie, Die.YellowDie,
            Die.RedDie, Die.RedDie, Die.RedDie
        };

        public static Cup GetCup() => new Cup();
    }

    enum PlayerChoice
    {
        None = 0,
        GoDeeper,
        EscapeWithTheLoot,
        RunAwayWithTheLoot
    }

    enum LevelMode
    {
        Dungeon,
        GoblinKing,
        AddToTheGoblinKingsHorde
    }

    class DungeonDiceGame
    {
        public const int TOTAL_LEVELS = 7;

        private Cup _cup;
        private IEnumerator _round;
        private PlayerChoice _choice;
        private List<(string name, Action action)> _playerOptions = new();
        private List<Die> _activeLootDice = new();
        private List<Die> _activeGoblinDice = new();

        public LevelMode Mode => Level == TOTAL_LEVELS
            ? LevelMode.GoblinKing 
            : _choice == PlayerChoice.RunAwayWithTheLoot 
                ? LevelMode.AddToTheGoblinKingsHorde 
                : LevelMode.Dungeon;

        public int Level { get; private set; } = 0;
        public int LootCount { get; private set; } = 1;
        public int HordeCount { get; private set; } = 1;
        public int XP { get; private set; } = 0;

        public IEnumerable<Die> GoblinKingRoll { get; private set; } = Enumerable.Empty<Die>();

        public IEnumerable<Die> CurrentRoll { get; private set; } = Enumerable.Empty<Die>();

        public IEnumerable<Die> ActiveLootDice => _activeLootDice;

        public IEnumerable<Die> ActiveGoblinDice => _activeGoblinDice;

        public IEnumerable<(string name, Action action)> PlayerOptions => _playerOptions;

        public string Message { get; private set; }

        public void Play()
        {
            if (_round != null) throw new Exception();
            Level = 1;
            LootCount = 1;
            HordeCount = 1;
            XP = 0;
            _round = PlayRound();
            _round.MoveNext();
        }

        private IEnumerator PlayRound()
        {
            if (Mode == LevelMode.GoblinKing)
            {
                ResetBoardState();
                var lootPlusXP = LootCount + XP;
                while (true)
                {                    
                    var diceToDraw = Math.Min(lootPlusXP, 3);
                    Message = $"You have {lootPlusXP} points available to attack the horde with.";
                    _playerOptions.Clear();
                    _playerOptions.Add(($"Buy and roll {diceToDraw} dice.", () => _round.MoveNext()));
                    yield return null;
                    ResetCup();
                    lootPlusXP -= diceToDraw;
                    var dice = _cup.DrawDice(diceToDraw).ToList();
                    dice.ForEach(d => d.Roll());
                    CurrentRoll = dice;
                    var kills = Math.Min(dice.Count(AreGoblinDice), HordeCount);
                    HordeCount -= kills;
                    Message = $"You killed {kills} goblins with that roll!";
                    _playerOptions.Clear();
                    _playerOptions.Add(($"Continue.", () => _round.MoveNext()));
                    yield return null;
                    if (HordeCount == 0 || lootPlusXP == 0)
                    {
                        break;
                    }
                }

                if (HordeCount == 0)
                {
                    Message = $"You killed the goblin horde, you win!";
                }
                else
                {
                    Message = $"You could not defeat the goblin horde, you lose!";
                }
                _playerOptions.Clear();
                _playerOptions.Add(($"Play again...", () => _round.MoveNext()));
                yield return null;
            }
            else
            {
                ResetBoardState();
                var dice = _cup.DrawDice(3).ToList();
                dice.ForEach(d => d.Roll());
                UpdateState(dice);
                yield return null;
                while (true)
                {
                    if (_choice == PlayerChoice.GoDeeper)
                    {
                        XP += _activeGoblinDice.Count();
                        var doorDice = CurrentRoll.Where(AreDoorDice);
                        _cup.Shake();
                        var newDice = doorDice.Concat(_cup.DrawDice(3 - doorDice.Count())).ToList();
                        newDice.ForEach(d => d.Roll());
                        UpdateState(dice);
                        yield return null;
                    }
                    else if (_choice == PlayerChoice.EscapeWithTheLoot)
                    {
                        LootCount += _activeLootDice.Count();
                        NextLevel();
                        break;
                    }
                    else if (Mode == LevelMode.AddToTheGoblinKingsHorde)
                    {
                        if (!CurrentRoll.Any(AreDoorDice)) throw new Exception();
                        var doorDice = CurrentRoll.Where(AreDoorDice);
                        var newDice = doorDice.Concat(_cup.DrawAllDice()).ToList();
                        Message = "You chose to run away with the loot. The goblin king will try to recruit any goblins left in this dungeon for the horde.";
                        _playerOptions.Clear();
                        _playerOptions.Add(("Roll to add to the Goblin King's horde", () => _round.MoveNext()));
                        yield return null;
                        newDice.ForEach(d => d.Roll());
                        GoblinKingRoll = newDice;
                        var goblinsToAdd = GoblinKingRoll.Count(AreGoblinDice);
                        HordeCount += goblinsToAdd;
                        Message = $"The goblin king recruited {goblinsToAdd} goblins for the horde!";
                        _playerOptions.Clear();
                        _playerOptions.Add(("Continue to the next level", () => _round.MoveNext()));
                        yield return null;
                        _choice = PlayerChoice.None;
                        NextLevel();
                        break;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }

        private void UpdateState(List<Die> dice)
        {
            UpdateBoardState(dice);
            UpdatePlayerOptions();
        }

        private static Func<Die, bool> AreDoorDice => r => r.FaceUp == DieFace.Door;
        private static Func<Die, bool> AreLootDice => r => r.FaceUp == DieFace.Loot;
        private static Func<Die, bool> AreGoblinDice => r => r.FaceUp == DieFace.Goblin;

        private void ResetBoardState()
        {
            ResetCup();
            _activeLootDice.Clear();
            _activeGoblinDice.Clear();
            _playerOptions.Clear();
            _choice = PlayerChoice.None;
        }

        private void ResetCup()
        {
            _cup = Box.GetCup();
            _cup.PlaceDiceInCup(Box.GetDice());
            _cup.Shake();
        }

        private void UpdateBoardState(List<Die> dice)
        {
            CurrentRoll = dice;
            _activeGoblinDice.AddRange(CurrentRoll.Where(m => m.FaceUp == Goblin));
            _activeLootDice.AddRange(CurrentRoll.Where(m => m.FaceUp == Loot));
        }

        private void UpdatePlayerOptions()
        {
            _choice = PlayerChoice.None;
            _playerOptions.Clear();

            if (_activeGoblinDice.Count >= 3)
            {
                Message = "You've been killed by goblins! These goblins have been added to the horde.";
                _playerOptions.Add(("Reincarnate in the next level", NextLevel));
                HordeCount += ActiveGoblinDice.Count();
                return;
            }

            if (!CurrentRoll.Any(AreDoorDice))
            {
                Message = "There aren't any doors remaining in this dugeon.";
                _playerOptions.Add(("Escape with the loot", EscapeWithTheLoot));
                return;
            }

            Message = "You have a choice!";
            _playerOptions.Add(("Go deeper and deeper", GoDeeper));
            _playerOptions.Add(("Run away with the loot", RunAwayWithTheLoot));
        }

        private void NextLevel()
        {
            if (_round == null) throw new Exception();
            Level++;
            _round = null;
            _round = PlayRound();
            _round.MoveNext();
        }

        private void EscapeWithTheLoot()
        {
            if (_round == null) throw new Exception();
            _playerOptions.Clear();
            _choice = PlayerChoice.EscapeWithTheLoot;
            _round.MoveNext();
        }

        private void RunAwayWithTheLoot()
        {
            if (_round == null) throw new Exception();
            _playerOptions.Clear();
            _choice = PlayerChoice.RunAwayWithTheLoot;
            _round.MoveNext();
        }

        private void GoDeeper()
        {
            if (_round == null) throw new Exception();
            _playerOptions.Clear();
            _choice = PlayerChoice.GoDeeper;
            _round.MoveNext();
        }
    }
}
