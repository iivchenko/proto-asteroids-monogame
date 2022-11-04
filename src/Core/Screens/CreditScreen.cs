using Core.Screens.GamePlay;
using Engine.Content;
using Engine.Graphics;
using Engine;
using Engine.Screens;
using System.Numerics;

using XTime = Microsoft.Xna.Framework.GameTime;
using Microsoft.Extensions.DependencyInjection;
using Engine.Audio;
using Microsoft.Xna.Framework.Input;

namespace Core.Screens
{
    public sealed class CreditScreen : GameScreen
    {
        private IPainter _painter;
        private IFontService _fontService;
        private IViewport _view;

        private Font _headerFont;
        private Font _font;
        private Size _line;

        public CreditScreen()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            var container = ScreenManager.Container;
            var content = container.GetService<IContentProvider>();

            _painter = container.GetService<IPainter>();
            _fontService = container.GetService<IFontService>();
            _view = container.GetService<IViewport>();
            _headerFont = content.Load<Font>("Fonts/kenney-future.h4.font");
            _font = content.Load<Font>("Fonts/arial.h4.font");
            _line = _fontService.MeasureText("M", _font);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (input.IsNewKeyPress(Keys.Escape, null, out _) || input.IsNewButtonPress(Buttons.Start, null, out _))
            {
                LoadingScreen.Load(ScreenManager, false, null, new StarScreen(), new MainMenuScreen());
            }
        }

        public override void Draw(XTime gameTime)
        {
            base.Draw(gameTime);

            PrintHeader();
            PrintSlavasThanks();
            PrintKenneyThanks();
            PrintChiphead64Thanks();
        }

        private void Print(string text, int step)
        {
            var position = new Vector2( 200, _line.Height * step);

            _painter.DrawString(_font, text, position, Colors.White);
        }

        private void PrintHeader()
        {
            var header = "Credis";
            var size = _fontService.MeasureText(header, _headerFont);
            var position = new Vector2(_view.Width/2 - size.Width/2, _line.Height * 0);

            _painter.DrawString(_headerFont, header, position, Colors.White);
        }

        private void PrintSlavasThanks()
        {
            var thanksToMyWife =
                "Special thanks to\n" +
                "   My beloved wife Myroslava\n" +
                "   Who supported me for the whole journey\n" +
                "   And of course for creation of the great art!";

            Print(thanksToMyWife, 2);
        }

        private void PrintKenneyThanks()
        {
            var thanks =
                "Thanks to Kenney\n" +
                "   For the art that inspired me to build the game\n" +
                "   For the font I used in the game\n" +
                "   For the sfx I used in the game\n" +
                "   http://kenney.nl/";

            Print(thanks, 7);
        }

        private void PrintChiphead64Thanks()
        {
            var thanks =
                "Music\n" +
                "   Chiphead64 https://chiphead64.itch.io/dreamy-space-soundtrack\n" +
                "   McLean https://retroindiejosh.itch.io/free-music-pack-4\n" +
                "   Polarnyne https://polarnyne.itch.io/izakaya-funes-metal-pack\n" +
                "   David KBD https://davidkbd.itch.io/hair-and-kuckles-technometal-music-pack";

            Print(thanks, 13);
        }
    }
}
