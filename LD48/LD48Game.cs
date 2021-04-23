namespace LD48
{
    using LD48.Core;
    using LD48.Data;
    using LD48.Systems;
    using Microsoft.Xna.Framework;
    using System;

    public class LD48Game : Game
    {
        private GraphicsDeviceManager _graphics;
        private EntityDataManager _entityDataManager;
        private Entity _dummy;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _entityDataManager = new EntityDataManager();
            _entityDataManager.Initialize("LD48.Data");

            new RenderingSystem(this);
        }

        protected override void Initialize()
        {
            _dummy = new Entity(_entityDataManager);

            _dummy
                .SetData(Transform.New)
                .SetData(new SpriteTexture(GameContent.Texture2Ds.dummy));

            base.Initialize();
        }

        [STAThread]
        static void Main()
        {
            using var game = new LD48Game();
            game.Run();
        }
    }
}
