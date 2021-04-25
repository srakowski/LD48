namespace LD48.Gameplay
{
    using Microsoft.Xna.Framework;

    class Player
    {
        public Player(Vector3 location)
        {
            Location = location;
        }

        public Vector3 Location { get; private set; }

        public Vector3 CellLocation => Vector3.Round(Location);
    }
}
