namespace LD48.Editor
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Text;

    class SceneViewer : Game
    {
        public int b;

        private GraphicsDeviceManager _graphics;
        public SceneViewer()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(255, 255, b, 255));
        }
    }
}
