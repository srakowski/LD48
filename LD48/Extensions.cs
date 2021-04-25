using Microsoft.Xna.Framework;

namespace LD48
{
    static class Extensions
    {
        public static Point TopDownToXY(this Vector3 self) => new Point((int)self.X, (int)self.Z);
        public static Point SideToXY(this Vector3 self) => new Point((int)self.Z, (int)self.Y);
        public static Point FrontToXY(this Vector3 self) => new Point((int)self.X, (int)self.Y);
    }
}
