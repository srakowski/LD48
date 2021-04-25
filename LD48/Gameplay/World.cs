namespace LD48.Gameplay
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    class World
    {
        public const int IGNORE_COORD = -1;

        private readonly Dictionary<Vector3, Cell> _cells = new();
        private readonly Dictionary<int, List<Cell>> _cellsOnXPlane = new();
        private readonly Dictionary<int, List<Cell>> _cellsOnYPlane = new();
        private readonly Dictionary<int, List<Cell>> _cellsOnZPlane = new();

        public World(int width, int height, int depth)
        {
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    for (var z = 0; z < depth; z++)
                    {
                        if (!_cellsOnXPlane.ContainsKey(x)) _cellsOnXPlane[x] = new();
                        if (!_cellsOnYPlane.ContainsKey(y)) _cellsOnYPlane[y] = new();
                        if (!_cellsOnZPlane.ContainsKey(z)) _cellsOnZPlane[z] = new();

                        var location = new Vector3(x, y, z);

                        Matter initialMatter = null;

                        //if (location.X == width / 2 &&
                        //    location.Y == height / 2 &&
                        //    location.Z == depth / 2)
                        //{
                            initialMatter = new MineralMatter(MineralType.Stone);
                        //}

                        var cell = new Cell(location, initialMatter);
                        _cells[location] = cell;
                        _cellsOnXPlane[x].Add(cell);
                        _cellsOnYPlane[y].Add(cell);
                        _cellsOnZPlane[z].Add(cell);
                    }
        }

        public IEnumerable<Cell> GetCellsOnXPlane(int x) => _cellsOnXPlane[x];
        public IEnumerable<Cell> GetCellsOnYPlane(int y) => _cellsOnYPlane[y];
        public IEnumerable<Cell> GetCellsOnZPlane(int z) => _cellsOnZPlane[z];
    }
}
