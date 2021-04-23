namespace LD48
{
    using LD48.Core;
    using LD48.Scenes;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    public class LD48Game : Game
    {
        private GraphicsDeviceManager _graphics;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            var engine = new Engine(this, new Dictionary<string, Func<Scene>>
            {
                { nameof(MainMenu), MainMenu.Create },
                { nameof(Gameplay), Gameplay.Create },
            });

            engine.Start(nameof(MainMenu));
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.ApplyChanges();
        }

        [STAThread]
        static void Main()
        {
            using var game = new LD48Game();
            game.Run();
        }
    }
}
