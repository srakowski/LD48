using LD48.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LD48.Systems
{
    struct Sprite
    {
        public Either<SpriteTexture, SpriteText> Content;
        public Transform Transform;
        public SpriteEtc Etc;
    }

    class Layer
    {
        public SpriteSortMode SpriteSortMode { get; set; } = SpriteSortMode.Deferred;
        public BlendState BlendState { get; set; } = null;
        public SamplerState SamplerState { get; set; } = null;
        public DepthStencilState DepthStencilState { get; set; } = null;
        public RasterizerState RasterizerState { get; set; } = null;
        public Effect Effect { get; set; } = null;
        public Matrix? TransformMatrix { get; set; } = null;

        public void Draw(
            SpriteBatch spriteBatch,
            Texture2D[] textures,
            SpriteFont[] spriteFonts,
            IEnumerable<Sprite> sprites)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode,
                blendState: BlendState,
                samplerState: SamplerState,
                depthStencilState: DepthStencilState,
                rasterizerState: RasterizerState,
                effect: Effect,
                transformMatrix: TransformMatrix
                );

            foreach (var sprite in sprites)
            {
                if (sprite.Content.IsLeft)
                {
                    var texture = textures[sprite.Content.Left.TextureId];
                    spriteBatch.Draw(
                        texture,
                        sprite.Transform.Position,
                        sprite.Content.Left.SourceRectangle.HasValue 
                            ? sprite.Content.Left.SourceRectangle.Value 
                            : (Rectangle?)null,
                        sprite.Etc.Color,
                        sprite.Transform.Rotation,
                        sprite.Etc.Origin,
                        sprite.Transform.Scale,
                        sprite.Etc.SpriteEffects,
                        sprite.Etc.LayerDepth
                    );
                }
                else
                {
                    var spriteFont = spriteFonts[sprite.Content.Right.SpriteFontId];
                    spriteBatch.DrawString(
                        spriteFont,
                        sprite.Content.Right.Text ?? "",
                        sprite.Transform.Position,
                        sprite.Etc.Color,
                        sprite.Transform.Rotation,
                        sprite.Etc.Origin,
                        sprite.Transform.Scale,
                        sprite.Etc.SpriteEffects,
                        sprite.Etc.LayerDepth
                    );
                }
            }

            spriteBatch.End();
        }
    }

    class RenderingSystem : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private Texture2D[] _textures;
        private SpriteFont[] _spriteFonts;

        public RenderingSystem(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        protected override void LoadContent()
        {
            _textures = GameContent.Texture2Ds.Load(Game.Content);
            _spriteFonts = GameContent.SpriteFonts.Load(Game.Content);
        }
    }
}
