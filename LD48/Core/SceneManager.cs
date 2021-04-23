namespace LD48.Core
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    interface ISceneManager
    {
        Scene LoadScene(string sceneName);
        void UnloadScene(Scene scene);
    }

    class SceneManager : GameComponent, ISceneManager
    {
        private readonly Dictionary<string, Func<Scene>> _sceneFactories;
        private readonly List<Scene> _scenes;

        public SceneManager(Game game, Dictionary<string, Func<Scene>> sceneFactories) : base(game)
        {
            game.Services.AddService<ISceneManager>(this);
            game.Components.Add(this);
            _sceneFactories = sceneFactories;
            _scenes = new();
        }

        public Scene LoadScene(string sceneName)
        {
            var scene = _sceneFactories[sceneName]?.Invoke();
            _scenes.Add(scene);
            scene.BeginLoad();
            return scene;
        }

        public void UnloadScene(Scene scene)
        {
            scene.BeginUnload();
        }

        public override void Update(GameTime gameTime)
        {
            _scenes.RemoveAll(s => s.State == SceneState.Inactive);
        }
    }
}
