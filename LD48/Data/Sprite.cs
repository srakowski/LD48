using LD48.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LD48.Data
{
    struct SpriteTexture : IEntityData
    {
        public int TextureId;
        public Optional<Rectangle> SourceRectangle;

        public SpriteTexture(int textureId)
        {
            TextureId = textureId;
            SourceRectangle = default;
        }

        public static SpriteTexture New(int textureId) => new SpriteTexture(textureId);
    }

    struct SpriteText : IEntityData
    {
        public int SpriteFontId;
        public string Text;

        public SpriteText(int spriteFontId, string text)
        {
            SpriteFontId = spriteFontId;
            Text = text;
        }
    }

    struct SpriteEtc : IEntityData
    {
        public Color Color;
        public Vector2 Origin;
        public SpriteEffects SpriteEffects;
        public float LayerDepth;
    }
}
