using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD48
{
    struct InputState
    {
        public InputState(KeyboardState ks, MouseState ms, GamePadState gp)
        {
            Keyboard = ks;
            Mouse = ms;
            GamePad = gp;
        }

        public KeyboardState Keyboard { get; }

        public MouseState Mouse { get; }

        public GamePadState GamePad { get; }
    }

    static class Input
    {
        public static InputState Prev { get; private set; }

        public static InputState Curr { get; private set; }

        public static void Update()
        {
            Prev = Curr;
            Curr = new InputState(
                Keyboard.GetState(),
                Mouse.GetState(),
                GamePad.GetState(PlayerIndex.One)
            );
        }
    }
}
