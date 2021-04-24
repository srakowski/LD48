using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static LD48.GameContent;

namespace LD48
{
    abstract class Widget
    {
        public Widget(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Vector2 Position { get; }

        public Vector2 Size { get; }
    }

    class MapSlotWidget : Widget
    {
        public MapSlotWidget(Vector2 position, Vector2 size, MapSlot mapSlot) : base(position, size)
        {
            MapSlot = mapSlot;
        }

        public MapSlot MapSlot { get; }

        public Card Card => MapSlot.Card;

        public PlayerToken Token => MapSlot.Token;

        public void Draw(SpriteBatch sb, LoadedGameContent content)
        {
            if (Card == null) return;

            sb.DrawSprite(content, new Sprite
            {
                Content = new SpriteTexture(Texture2Ds.card),
                Transform = new Transform
                {
                    Position = Position
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
    }
}
