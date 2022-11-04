using Comora;
using Engine.Audio;
using Engine.Content;
using Engine.Graphics;
using Engine.Screens;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using XTime = Microsoft.Xna.Framework.GameTime;
using XVector = Microsoft.Xna.Framework.Vector2;

namespace Core.Screens
{
    public sealed class BootstrapScreen<TStartScreen> : GameScreen
        where TStartScreen : GameScreen, new()
    {
        public override void Initialize()
        {
            base.Initialize();
            
            GameRoot.ScreenManager = ScreenManager;

            var camera = ScreenManager.Container.GetService<ICamera>();
            var view = ScreenManager.Container.GetService<IViewport>();
            var content = ScreenManager.Container.GetService<IContentProvider>();
            var nativeView = ScreenManager.GraphicsDevice.Viewport;
            
            camera.Position = new XVector(view.Width / 2.0f, view.Height / 2.0f);
            camera.Width = view.Width;
            camera.Height = view.Height;
            camera.Zoom = nativeView.Width / view.Width;

            foreach (var file in content.GetFiles("Music").Where(file => file.Contains("game")))
            {
                content.Load<Music>(file);
            }
        }

        public override void Update(XTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            LoadingScreen.Load(ScreenManager, false, null, new StarScreen(), new TStartScreen());
        }
    }
}
