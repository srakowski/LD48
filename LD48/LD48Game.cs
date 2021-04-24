namespace LD48
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using static GameContent;

    public class LD48Game : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private LoadedGameContent _loadedContent;
        private GameState _gameState;
        private List<MapSlotWidget> _cardWidgets;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _gameState = new GameState();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameState.Reset();

            _cardWidgets = new List<MapSlotWidget>();
            foreach (var slot in _gameState.GameBoard.Map)
            { 
                var pos = new Vector2(20, 20) + (slot.Position.ToVector2() * new Vector2(250, 346));
                var size = new Vector2(240, 336);
                _cardWidgets.Add(new MapSlotWidget(pos, size, slot));
            }
        }

        protected override void LoadContent()
        {
            _loadedContent = new LoadedGameContent(
                GameContent.Texture2Ds.Load(Content),
                GameContent.SpriteFonts.Load(Content)
            );
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var gb = _gameState.GameBoard;

            _spriteBatch.Begin();

            foreach (var widget in _cardWidgets)
            {
                widget.Draw(_spriteBatch, _loadedContent);
            }

            _spriteBatch.End();
        }

        [STAThread]
        static void Main()
        {
            using var game = new LD48Game();
            game.Run();
        }
    }
}
