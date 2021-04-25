namespace LD48
{
    using System;
    using System.Linq;

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var g = new DungeonDiceGame();
            g.Play();
            while (true)
            {
                if (g.Mode == LevelMode.Dungeon)
                {
                    Console.WriteLine($"{g.Mode} LEVEL {g.Level}");
                    Console.WriteLine($"LOOT: {g.LootCount}, XP: {g.XP}, HORDE: {g.HordeCount}");
                    Console.WriteLine($"LOOT DICE IN PLAY: {string.Join(", ", g.ActiveLootDice.Select(c => $"{c.FaceUp}-{c.Color}"))}");
                    Console.WriteLine($"GOBLIN DICE IN PLAY: {string.Join(", ", g.ActiveGoblinDice.Select(c => $"{c.FaceUp}-{c.Color}"))}");
                    Console.WriteLine($"CURRENT ROLL: {string.Join(", ", g.CurrentRoll.Select(c => $"{c.FaceUp}-{c.Color}"))}");
                }
                else if (g.Mode == LevelMode.AddToTheGoblinKingsHorde)
                {
                    Console.WriteLine($"GOBLIN KING RECRUITS: {string.Join(", ", g.GoblinKingRoll.Select(c => $"{c.FaceUp}-{c.Color}"))}");
                }
                Console.WriteLine($"MESSAGE: {g.Message}");
                var i = 0;
                foreach (var option in g.PlayerOptions)
                {
                    Console.WriteLine($"{i}: {option.name}");
                    i++;
                }
                var sel = int.Parse(Console.ReadLine());
                g.PlayerOptions.ElementAt(sel).action();
                Console.WriteLine("----------------- NEXT -------------------");
                Console.WriteLine();
            }


            using (var game = new LD48Game())
                game.Run();
        }
    }
}
