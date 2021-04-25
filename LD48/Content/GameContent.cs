using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace LD48
{
	public static class GameContent
	{
		public static class Effects
		{
			public const int dummy = 0; // "Effect/dummy"
			public static Effect[] Load(ContentManager content)
			{
				return new Effect[]
				{
					content.Load<Effect>("Effect/dummy"),
				};
			}
		}
		public static class Songs
		{
			public const int dummy = 0; // "Song/dummy"
			public static Song[] Load(ContentManager content)
			{
				return new Song[]
				{
					content.Load<Song>("Song/dummy"),
				};
			}
		}
		public static class SoundEffects
		{
			public const int dummy = 0; // "SoundEffect/dummy"
			public static SoundEffect[] Load(ContentManager content)
			{
				return new SoundEffect[]
				{
					content.Load<SoundEffect>("SoundEffect/dummy"),
				};
			}
		}
		public static class SpriteFonts
		{
			public const int dummy = 0; // "SpriteFont/dummy"
			public static SpriteFont[] Load(ContentManager content)
			{
				return new SpriteFont[]
				{
					content.Load<SpriteFont>("SpriteFont/dummy"),
				};
			}
		}
		public static class Texture2Ds
		{
			public const int card = 0; // "Texture2D/card"
			public const int drill = 1; // "Texture2D/drill"
			public const int dummy = 2; // "Texture2D/dummy"
			public const int placeholder = 3; // "Texture2D/placeholder"
			public const int tiles = 4; // "Texture2D/tiles"
			public static Texture2D[] Load(ContentManager content)
			{
				return new Texture2D[]
				{
					content.Load<Texture2D>("Texture2D/card"),
					content.Load<Texture2D>("Texture2D/drill"),
					content.Load<Texture2D>("Texture2D/dummy"),
					content.Load<Texture2D>("Texture2D/placeholder"),
					content.Load<Texture2D>("Texture2D/tiles"),
				};
			}
		}
	}
}
