namespace LD48
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    static class SpriteBatchEx
    {
        public static void DrawSprite(this SpriteBatch self, LoadedGameContent loadedGameContent, Sprite sprite)
        {
            if (sprite.Content is SpriteTexture st)
            {
                var texture = loadedGameContent.Textures[st.TextureId];
                self.Draw(
                    texture,
                    sprite.Transform.Position,
                    st.SourceRectangle,
                    sprite.Etc.Color ?? Color.White,
                    sprite.Transform.Rotation,
                    sprite.Etc.Origin,
                    sprite.Transform.Scale ?? 1f,
                    sprite.Etc.SpriteEffects,
                    sprite.Etc.LayerDepth
                );

                return;
            }
            else if (sprite.Content is SpriteText t)
            {
                var spriteFont = loadedGameContent.SpriteFonts[t.SpriteFontId];
                self.DrawString(
                    spriteFont,
                    t.Text ?? "",
                    sprite.Transform.Position,
                    sprite.Etc.Color ?? Color.White,
                    sprite.Transform.Rotation,
                    sprite.Etc.Origin,
                    sprite.Transform.Scale ?? 1f,
                    sprite.Etc.SpriteEffects,
                    sprite.Etc.LayerDepth
                );
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
