namespace LD48.Scenes
{
    using LD48.Core;
    using LD48.Data;
    using LD48.Systems;
    using static GameContent;

    static class MainMenu
    {
        public static Scene Create()
        {
            var scene = new Scene();

            scene
                .AddSystem<RenderingSystem>();

            scene +=
                Entity.New
                    + SpriteTexture.New(Texture2Ds.dummy)
                    + Transform.New;

            return scene;
        }
    }
}
