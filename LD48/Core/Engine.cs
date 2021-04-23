namespace LD48.Core
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    class Engine : GameComponent
    {
        private readonly SceneManager _sceneManager;

        public Engine(Game game, Dictionary<string, Func<Scene>> sceneFactories) : base(game)
        {
            _sceneManager = new SceneManager(game, sceneFactories);
        }

        public void Start(string sceneName)
        {
            _sceneManager.LoadScene(sceneName);
        }
    }
}
