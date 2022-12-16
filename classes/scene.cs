using SFML.Graphics;
using Global;

namespace ww1defence {
    public abstract class scene {
        public abstract void update(float delta);

        public abstract void draw(RenderWindow window);
    }

    public class menu_scene : scene
    {
        private List<control> controls;

        public menu_scene(RenderWindow window) {
            controls = new List<control>();

            float halfScreenWidth = Globals.ScreenSize.X / 2f;
            float division = Globals.ScreenSize.Y / 5f;

            button btnPlay = new button();
            btnPlay.Text = "Play";
            btnPlay.Position = new SFML.System.Vector2f(halfScreenWidth, division * 1f);
            btnPlay.Size = new SFML.System.Vector2f(100, 50);
            controls.Add(btnPlay);

            button btnSettings = new button();
            btnSettings.Text = "Settings";
            btnSettings.Position = new SFML.System.Vector2f(halfScreenWidth, division * 2f);
            btnSettings.Size = new SFML.System.Vector2f(150, 50);
            controls.Add(btnSettings);

            button btnQuit = new button();
            btnQuit.Text = "Quit";
            btnQuit.Position = new SFML.System.Vector2f(halfScreenWidth, division * 3f);
            btnQuit.Size = new SFML.System.Vector2f(100, 5);
            controls.Add(btnQuit);

            foreach (control c in controls) {
                window.MouseButtonPressed += c.Control_MouseButtonPressed;
                window.MouseButtonReleased += c.Control_MouseButtonReleased;
                window.MouseMoved += c.Control_MouseMoved;
            }
        }

        public override void update(float delta)
        {
            foreach (control c in controls) {
                
            }
        }

        public override void draw(RenderWindow window)
        {
            foreach (control c in controls) {
                c.draw(window);
            }
        }

    }
}