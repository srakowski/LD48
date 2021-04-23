namespace LD48.Data
{
    using Microsoft.Xna.Framework;

    struct Transform
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
