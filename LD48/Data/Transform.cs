namespace LD48.Data
{
    using LD48.Core;
    using Microsoft.Xna.Framework;

    struct Transform : IEntityData
    {
        public Vector2 Position;
        public float Rotation;
        public float Scale;

        public static Transform New => new Transform
        {
            Position = Vector2.Zero,
            Rotation = 0f,
            Scale = 1f
        };
    }
}
