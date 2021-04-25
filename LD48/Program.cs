namespace LD48
{
    using System;

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LD48Game())
                game.Run();
        }
    }
}
