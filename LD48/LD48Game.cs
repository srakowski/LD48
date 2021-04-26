namespace LD48
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using static LD48.GameContent;

    public class LD48Game : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private LoadedGameContent _content;
        private DungeonDiceGame _game;
        private LeftButton left;
        private RightButton right;

        public LD48Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _game = new DungeonDiceGame();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            left = new LeftButton
            {
                Game = _game,
                Position = new Vector2(300, 530)
            };

            right = new RightButton
            {
                Game = _game,
                Position = new Vector2(680, 530)
            };

            _game.Play();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _content = new LoadedGameContent(
                Texture2Ds.Load(Content),
                SpriteFonts.Load(Content)
            );
        }

        private MouseState currms;
        private MouseState prevms;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevms = currms;
            currms = Mouse.GetState();

            if (prevms.LeftButton == ButtonState.Pressed &&
                currms.LeftButton == ButtonState.Released)
            {
                var pos = currms.Position;
                left.TryClick(pos);
                right.TryClick(pos);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                _content.Textures[Texture2Ds.gameboard],
                Vector2.Zero,
                Color.White
            );

            left.Draw(_spriteBatch, _content);
            right.Draw(_spriteBatch, _content);

            var i = 0;
            foreach (var dice in _game.CurrentRoll)
            {
                var t = dice.FaceUp == DieFace.Goblin ? Texture2Ds.goblin :
                    dice.FaceUp == DieFace.Loot ? Texture2Ds.loot :
                    dice.FaceUp == DieFace.Door ? Texture2Ds.door
                    : throw new System.Exception();
                var c = dice.Color == DieColor.Green ? Color.DarkGreen :
                    dice.Color == DieColor.Yellow ? Color.Gold :
                    dice.Color == DieColor.Red ? Color.DarkRed :
                    Color.White;

                _spriteBatch.Draw(
                    _content.Textures[t],
                    new Rectangle((new Vector2(80, 60) + (new Vector2(i, 0) * new Vector2(140, 0))).ToPoint(), new Point(120, 120)),
                    null,
                    c);

                i++;
            }

            i = 0;
            foreach (var dice in _game.ActiveLootDice)
            {
                var t = Texture2Ds.loot;
                var c = dice.Color == DieColor.Green ? Color.DarkGreen :
                    dice.Color == DieColor.Yellow ? Color.Gold :
                    dice.Color == DieColor.Red ? Color.DarkRed :
                    Color.White;

                _spriteBatch.Draw(
                    _content.Textures[t],
                    new Rectangle((new Vector2(80, 260) + (new Vector2(i, 0) * new Vector2(70, 0))).ToPoint(), new Point(60, 60)),
                    null,
                    c);

                i++;
            }

            i = 0;
            foreach (var dice in _game.ActiveGoblinDice)
            {
                var t = Texture2Ds.goblin;
                var c = dice.Color == DieColor.Green ? Color.DarkGreen :
                    dice.Color == DieColor.Yellow ? Color.Gold :
                    dice.Color == DieColor.Red ? Color.DarkRed :
                    Color.White;

                _spriteBatch.Draw(
                    _content.Textures[t],
                    new Rectangle((new Vector2(80, 380) + (new Vector2(i, 0) * new Vector2(70, 0))).ToPoint(), new Point(60, 60)),
                    null,
                    c);

                i++;
            }

            _spriteBatch.DrawString(
                _content.SpriteFonts[GameContent.SpriteFonts.dummy],
                $"{_game.LootCount}",
                new Vector2(130, 510),
                Color.WhiteSmoke);

            _spriteBatch.DrawString(
                _content.SpriteFonts[GameContent.SpriteFonts.dummy],
                $"{_game.HordeCount}",
                new Vector2(1060, 510),
                Color.WhiteSmoke);

            _spriteBatch.DrawString(
                _content.SpriteFonts[GameContent.SpriteFonts.dummy],
                $"{_game.XP}",
                new Vector2(130, 620),
                Color.WhiteSmoke);

            _spriteBatch.End();

        }
    }

    abstract class Button
    {
        protected abstract Texture2D GetTexture2D(LoadedGameContent loadedGameContent);

        public Vector2 Position { get; set; }

        public void Draw(SpriteBatch sb, LoadedGameContent loadedGameContent)
        {
           var t = GetTexture2D(loadedGameContent);
            sb.Draw(t, Position, Color.White);
        }
    }

    class RightButton : Button
    {
        public DungeonDiceGame Game { get; set; }

        public bool TryClick(Point pos)
        {
            var bounds = new Rectangle((int)Position.X, (int)Position.Y, 300, 118);
            if (!bounds.Contains(pos)) return false;
            Game.RightButtonAction?.Invoke();
            return true;
        }

        protected override Texture2D GetTexture2D(LoadedGameContent loadedGameContent)
        {
            if (Game.RightButton.HasValue)
            {
                switch (Game.RightButton.Value)
                {
                    case Buttons.Deeper: return loadedGameContent.Textures[GameContent.Texture2Ds.deeper];
                    case Buttons.Roll: return loadedGameContent.Textures[GameContent.Texture2Ds.roll];
                    case Buttons.Run: return loadedGameContent.Textures[GameContent.Texture2Ds.run];
                    case Buttons.Leave: return loadedGameContent.Textures[GameContent.Texture2Ds.leave];
                    case Buttons.Respawn: return loadedGameContent.Textures[GameContent.Texture2Ds.dead];
                }
            }
            return loadedGameContent.Textures[GameContent.Texture2Ds.blankright];
        }
    }

    class LeftButton : Button
    {
        public DungeonDiceGame Game { get; set; }

        protected override Texture2D GetTexture2D(LoadedGameContent loadedGameContent)
        {
            if (Game.LeftButton.HasValue)
            {
                switch (Game.LeftButton.Value)
                {
                    case Buttons.Deeper: return loadedGameContent.Textures[GameContent.Texture2Ds.deeper];
                    case Buttons.Roll: return loadedGameContent.Textures[GameContent.Texture2Ds.roll];
                    case Buttons.Run: return loadedGameContent.Textures[GameContent.Texture2Ds.run];
                    case Buttons.Leave: return loadedGameContent.Textures[GameContent.Texture2Ds.leave];
                    case Buttons.Respawn: return loadedGameContent.Textures[GameContent.Texture2Ds.dead];
                }
            }
            return loadedGameContent.Textures[GameContent.Texture2Ds.blankleft];
        }

        public bool TryClick(Point pos)
        {
            var bounds = new Rectangle((int)Position.X, (int)Position.Y, 300, 118);
            if (!bounds.Contains(pos)) return false;
            Game.LeftButtonAction?.Invoke();
            return true;
        }
    }
}
