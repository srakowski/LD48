using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LD48.Gameplay
{
    class MiningGame
    {
        public const int WORLD_DIM = 10;

        private readonly World _world;
        private readonly Player _player;

        public MiningGame()
        {
            _world = new World(WORLD_DIM, WORLD_DIM, WORLD_DIM);
            _player = new Player(new Vector3(WORLD_DIM / 2, 0, 0));
        }

        public Player Player => _player;

        public IEnumerable<Cell> GetCellsForTopDownView()
        {
            return _world.GetCellsOnYPlane((int)_player.CellLocation.Y);
        }

        public IEnumerable<Cell> GetCellsForSideView()
        {
            return _world.GetCellsOnXPlane((int)_player.CellLocation.X);
        }

        public IEnumerable<Cell> GetCellsForFrontView()
        {
            return _world.GetCellsOnZPlane((int)_player.CellLocation.Z);
        }
    }
}
