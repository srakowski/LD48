namespace LD48
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using static GameContent;

    public class LD48Game : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private LoadedGameContent _loadedContent;
        private GameBoard _gameBoard;
        private List<Widget> _widgets;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            _widgets = new List<Widget>();
            _gameBoard = new GameBoard();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameBoard.Reset();
            foreach (var slot in _gameBoard.Map)
            {
                var size = _loadedContent.Textures[Texture2Ds.card].Bounds.Size.ToVector2();
                var pos = new Vector2(20, 20) + (slot.Position.ToVector2() * (size + new Vector2(10, 10)));
                _widgets.Add(new MapSlotWidget(pos, size, slot));
            }
        }

        protected override void LoadContent()
        {
            _loadedContent = new LoadedGameContent(
                Texture2Ds.Load(Content),
                SpriteFonts.Load(Content)
            );
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (Input.Curr.Mouse.LeftButton == ButtonState.Released &&
                Input.Prev.Mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var widget in _widgets)
                {
                    var mousePos = Input.Curr.Mouse.Position;
                    if (widget.ContainsPoint(mousePos))
                        if (widget.ProcessClick(mousePos))
                        {
                            break;
                        }
                }
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var gb = _gameBoard;

            _spriteBatch.Begin();

            foreach (var widget in _widgets)
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
