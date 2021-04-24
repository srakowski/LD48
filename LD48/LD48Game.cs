namespace LD48
{
    using Microsoft.Xna.Framework;
    using System;

    public class LD48Game : Game
    {
        private GraphicsDeviceManager _graphics;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        [STAThread]
        static void Main()
        {
            using var game = new LD48Game();
            game.Run();
        }
    }
}
