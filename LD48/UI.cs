using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static LD48.GameContent;

namespace LD48
{
    delegate void ClickedEvent(object sender, Point clickedAt);

    abstract class Widget
    {
        private readonly Rectangle _bounds;

        public event ClickedEvent Clicked;

        public Widget(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
            _bounds = new Rectangle(position.ToPoint(), size.ToPoint());
        }

        public Vector2 Position { get; }

        public Vector2 Size { get; }

        public abstract void Draw(SpriteBatch sb, LoadedGameContent content);

        public bool ContainsPoint(Point mousePos) => _bounds.Contains(mousePos);

        public bool ProcessClick(Point clickedAt)
        {
            Clicked?.Invoke(this, clickedAt);
            return true;
        }
    }

    class MapSlotWidget : Widget
    {
        public MapSlotWidget(Vector2 position, Vector2 size, MapSlot mapSlot) : base(position, size)
        {
            MapSlot = mapSlot;
            Clicked += MapSlotWidget_Clicked;
        }

        public MapSlot MapSlot { get; }

        public Card Card => MapSlot.Card;

        public PlayerToken Token => MapSlot.Token;

        public override void Draw(SpriteBatch sb, LoadedGameContent content)
        {
            if (Card == null) return;

            sb.DrawSprite(content, new Sprite
            {
                Content = new SpriteTexture(Texture2Ds.card),
                Transform = new Transform
                {
                    Position = Position
                },
                Etc = new SpriteEtc
                {
                    Color = 
                        MapSlot.IsDescentTarget 
                            ? Color.Red
                            : MapSlot.IsTravelTarget
                                ? Color.LightPink 
                                : Color.White
                }
            });

            if (!Card.IsFaceDown)
            {
                sb.DrawSprite(content, new Sprite
                {
                    Content = new SpriteText(SpriteFonts.dummy, Card.ToString()),
                    Transform = new Transform
                    {
                        Position = Position + new Vector2(20, 20)
                    },
                    Etc = new SpriteEtc
                    {
                        Color = Color.Black
                    }
                });
            }

            if (Token == null) return;

            sb.DrawSprite(content, new Sprite
            {
                Content = new SpriteText(SpriteFonts.dummy, "@"),
                Transform = new Transform
                {
                    Position = Position + new Vector2(20, 60)
                },
                Etc = new SpriteEtc
                {
                    Color = Color.Black
                }
            });
        }

        private void MapSlotWidget_Clicked(object sender, Point clickedAt)
        {
            MapSlot.PlayerActionHere();
        }
    }

    class RoomResolverWidget : Widget
    {
        private readonly GameBoard _gameBoard;

        public RoomResolverWidget(Vector2 position, Vector2 size, GameBoard gameBoard) : base(position, size)
        {
            _gameBoard = gameBoard;
            Clicked += RoomResolverWidget_Clicked;
        }

        public override void Draw(SpriteBatch sb, LoadedGameContent content)
        {
            throw new NotImplementedException();
        }

        private void RoomResolverWidget_Clicked(object sender, Point clickedAt)
        {
            throw new NotImplementedException();
        }
    }
}
