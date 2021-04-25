using Microsoft.Xna.Framework;

namespace LD48.Gameplay
{
    class Cell
    {
        public Cell(Vector3 location, Matter matter)
        {
            Location = location;
            Matter = matter;
        }

        public Vector3 Location { get; }

        public Matter Matter { get; private set; }
    }

    abstract class Matter { }

    enum MineralType
    {
        None = 0,
        Stone
    }

    class MineralMatter : Matter
    {
        public MineralMatter(MineralType mineralType)
        {
            mineralType = MineralType;
        }

        public MineralType MineralType { get; }
    }
}
