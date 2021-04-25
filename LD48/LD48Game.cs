namespace LD48
{
    using LD48.Gameplay;
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
        private MiningGame _miningGame;

        private Viewport[,] _viewports;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _miningGame = new MiningGame();
            _viewports = new Viewport[2, 2];
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            for (var x = 0; x < 2; x++)
                for (var y = 0; y < 2; y++)
                {
                    var vp = new Viewport();
                    vp.X = x * 400;
                    vp.Y = y * 400;
                    vp.Width = 400;
                    vp.Height = 400;
                    vp.MinDepth = 0;
                    vp.MaxDepth = 1;
                    _viewports[x, y] = vp;
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
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            var original = _graphics.GraphicsDevice.Viewport;

            // topdown  side
            // front    interface

            var topDown = _miningGame.GetCellsForTopDownView();
            DrawPlane(_viewports[0, 0], topDown, static l => l.TopDownToXY().ToVector2());

            var side = _miningGame.GetCellsForSideView();
            DrawPlane(_viewports[1, 0], side, static l => l.SideToXY().ToVector2());

            var front = _miningGame.GetCellsForFrontView();
            DrawPlane(_viewports[0, 1], front, static l => l.FrontToXY().ToVector2());

            GraphicsDevice.Viewport = original;
        }

        private void DrawPlane(Viewport viewport, IEnumerable<Cell> cells, Func<Vector3, Vector2> getPos)
        {
            _graphics.GraphicsDevice.Viewport = viewport;

            var playerPos = getPos(_miningGame.Player.Location);

            var tmat = Matrix.Identity *
                //Matrix.CreateRotationZ(Entity.GlobalRotation) *
                //Matrix.CreateScale(Entity.GlobalScale) *
                Matrix.CreateTranslation(-playerPos.X, -playerPos.Y, 0f) *
                Matrix.CreateTranslation(
                    (viewport.Width * 0.5f),
                    (viewport.Height * 0.5f),
                    0f);

            _spriteBatch.Begin(transformMatrix: tmat);
            foreach (var cell in cells)
            {
                if (cell.Matter == null) continue;
                _spriteBatch.DrawSprite(
                    _loadedContent,
                    new Sprite
                    {
                        Content = new SpriteTexture(Texture2Ds.placeholder),
                        Transform = new Transform
                        {
                            Position = getPos(cell.Location) * new Vector2(32, 32),
                        }
                    });
            }

            _spriteBatch.DrawSprite(
                _loadedContent,
                new Sprite
                {
                    Content = new SpriteTexture(Texture2Ds.drill),
                    Transform = new Transform
                    {
                        Position = playerPos * new Vector2(32, 32),
                    }

                });

            _spriteBatch.End();
        }

        enum PlanePerspective
        {
            TopDown,
            Side,
            Front
        }


        [STAThread]
        static void Main()
        {
            using var game = new LD48Game();
            game.Run();
        }
    }
}
