using Global;

namespace ww1defence {
    public class Program {
        static private ww1defence? game;

        public static int Main(string[] args) {
            Globals.ScreenSize = new SFML.System.Vector2f(800, 600);
            game = new ww1defence();
            game.run();

            return 0;
        }
    }
}