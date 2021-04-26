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
			public const int blankleft = 0; // "Texture2D/blankleft"
			public const int blankright = 1; // "Texture2D/blankright"
			public const int card = 2; // "Texture2D/card"
			public const int dead = 3; // "Texture2D/dead"
			public const int deeper = 4; // "Texture2D/deeper"
			public const int door = 5; // "Texture2D/door"
			public const int drill = 6; // "Texture2D/drill"
			public const int dummy = 7; // "Texture2D/dummy"
			public const int gameboard = 8; // "Texture2D/gameboard"
			public const int goblin = 9; // "Texture2D/goblin"
			public const int leave = 10; // "Texture2D/leave"
			public const int loot = 11; // "Texture2D/loot"
			public const int placeholder = 12; // "Texture2D/placeholder"
			public const int roll = 13; // "Texture2D/roll"
			public const int run = 14; // "Texture2D/run"
			public const int tiles = 15; // "Texture2D/tiles"
			public static Texture2D[] Load(ContentManager content)
			{
				return new Texture2D[]
				{
					content.Load<Texture2D>("Texture2D/blankleft"),
					content.Load<Texture2D>("Texture2D/blankright"),
					content.Load<Texture2D>("Texture2D/card"),
					content.Load<Texture2D>("Texture2D/dead"),
					content.Load<Texture2D>("Texture2D/deeper"),
					content.Load<Texture2D>("Texture2D/door"),
					content.Load<Texture2D>("Texture2D/drill"),
					content.Load<Texture2D>("Texture2D/dummy"),
					content.Load<Texture2D>("Texture2D/gameboard"),
					content.Load<Texture2D>("Texture2D/goblin"),
					content.Load<Texture2D>("Texture2D/leave"),
					content.Load<Texture2D>("Texture2D/loot"),
					content.Load<Texture2D>("Texture2D/placeholder"),
					content.Load<Texture2D>("Texture2D/roll"),
					content.Load<Texture2D>("Texture2D/run"),
					content.Load<Texture2D>("Texture2D/tiles"),
				};
			}
		}
	}
}
