using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD48.Data
{
    struct SpriteTexture
    {
        public int TextureId;
        public Optional<Rectangle> SourceRectangle;

        public SpriteTexture(int textureId)
        {
            TextureId = textureId;
            SourceRectangle = default;
        }
    }

    struct SpriteText
    {
        public int SpriteFontId;
        public string Text;

        public SpriteText(int spriteFontId, string text)
        {
            SpriteFontId = spriteFontId;
            Text = text;
        }
    }

    struct SpriteEtc
    {
        public Color Color;
        public Vector2 Origin;
        public SpriteEffects SpriteEffects;
        public float LayerDepth;
    }
}
