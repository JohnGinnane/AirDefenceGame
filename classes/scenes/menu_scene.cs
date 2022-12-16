using SFML.Graphics;
using SFML.Window;
using Global;

namespace ww1defence {   

    public class menu_scene : scene
    {
        private List<control> controls;

        public menu_scene(RenderWindow window) {
            window.SetMouseCursorVisible(true);

            controls = new List<control>();

            float halfScreenWidth = Globals.ScreenSize.X / 2f;
            float division = Globals.ScreenSize.Y / 5f;

            button btnPlay = new button();
            btnPlay.Text = "Play";
            btnPlay.Position = new SFML.System.Vector2f(halfScreenWidth, division * 1f);
            btnPlay.Size = new SFML.System.Vector2f(100, 50);
            btnPlay.Click += btnPlay_Click;
            controls.Add(btnPlay);

            button btnSettings = new button();
            btnSettings.Text = "Settings";
            btnSettings.Position = new SFML.System.Vector2f(halfScreenWidth, division * 2f);
            btnSettings.Size = new SFML.System.Vector2f(150, 50);
            btnSettings.Click += btnSettings_Click;
            controls.Add(btnSettings);

            button btnQuit = new button();
            btnQuit.Text = "Quit";
            btnQuit.Position = new SFML.System.Vector2f(halfScreenWidth, division * 3f);
            btnQuit.Size = new SFML.System.Vector2f(100, 5);
            btnQuit.Click += btnQuit_Click;
            controls.Add(btnQuit);

            // foreach (control c in controls) {
            //     window.MouseButtonPressed += c.Control_MouseButtonPressed;
            //     window.MouseButtonReleased += c.Control_MouseButtonReleased;
            //     window.MouseMoved += c.Control_MouseMoved;
            // }
        }

        public override void update(float delta) {
            
        }

        public override void draw(RenderWindow window) {
            foreach (control c in controls) {
                c.draw(window);
            }
        }

#region "Events"
        public void btnPlay_Click(object? sender, EventArgs? e) {
            Console.WriteLine("Play button pressed!");
        }

        public void btnSettings_Click(object? sender, EventArgs? e) {
            //sceneRequestEvent = new SceneRequestEventArgs()
            Console.WriteLine("Settings button pressed");
        }

        public void btnQuit_Click(object? sender, EventArgs? e) {
            Console.WriteLine("Exit game pls");
            sceneRequestEvent(this, new SceneRequestEventArgs(null));            
        }

        public override void MouseMoved(object? sender, MouseMoveEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseMoved(sender, e);
            }
        }

        public override void MouseButtonPressed(object? sender, MouseButtonEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseButtonPressed(sender, e);
            }
        }

        public override void MouseButtonReleased(object? sender, MouseButtonEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseButtonReleased(sender, e);
            }
        }

        public override void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs? e) {
            foreach (control c in controls) {
                c.ControL_MouseWheelScrolled(sender, e);
            }
        }
#endregion
    }
}