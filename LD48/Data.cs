namespace LD48
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    struct Transform
    {
        public Vector2 Position;
        public float Rotation;
        public float? Scale;

        public static Transform New => new Transform
        {
            Position = Vector2.Zero,
            Rotation = 0f,
            Scale = 1f
        };
    }

    interface ISpriteContent { }

    struct SpriteTexture : ISpriteContent
    {
        public int TextureId;
        public Rectangle? SourceRectangle;

        public SpriteTexture(int textureId)
        {
            TextureId = textureId;
            SourceRectangle = default;
        }

        public static SpriteTexture New(int textureId) => new SpriteTexture(textureId);
    }

    struct SpriteText : ISpriteContent
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
        public Color? Color;
        public Vector2 Origin;
        public SpriteEffects SpriteEffects;
        public float LayerDepth;
    }

    struct Sprite
    {
        public ISpriteContent Content;
        public Transform Transform;
        public SpriteEtc Etc;
    }
}
