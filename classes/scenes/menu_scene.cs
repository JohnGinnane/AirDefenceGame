using SFML.Graphics;
using SFML.Window;
using Global;

namespace ww1defence {   

    public class menu_scene : scene
    {
        public menu_scene(RenderWindow window) {
            controls = new List<control>();

            float halfScreenWidth = Globals.ScreenSize.X / 2f;
            float division = Globals.ScreenSize.Y / 5f;

            button btnPlay = new button();
            btnPlay.Text = "Play";
            btnPlay.Size = new SFML.System.Vector2f(100, 50);
            btnPlay.Position = new SFML.System.Vector2f(halfScreenWidth - btnPlay.Size.X/2f, division * 1f);
            btnPlay.Click += btnPlay_Click;
            controls.Add(btnPlay);

            button btnSettings = new button();
            btnSettings.Text = "Settings";
            btnSettings.Size = new SFML.System.Vector2f(150, 50);
            btnSettings.Position = new SFML.System.Vector2f(halfScreenWidth - btnSettings.Size.X/2f, division * 2f);
            btnSettings.Click += btnSettings_Click;
            controls.Add(btnSettings);

            button btnQuit = new button();
            btnQuit.Text = "Quit";
            btnQuit.Size = new SFML.System.Vector2f(100, 50);
            btnQuit.Position = new SFML.System.Vector2f(halfScreenWidth - btnQuit.Size.X/2f, division * 3f);
            btnQuit.Click += btnQuit_Click;
            controls.Add(btnQuit);
        }

        public override void update(float delta) {
            
        }

        public override void draw(RenderWindow window) {
            window.SetMouseCursorVisible(true);
            window.SetView(sceneView);

            foreach (control c in controls) {
                c.draw(window);
            }
        }

#region "Events"
        public void btnPlay_Click(object? sender, EventArgs? e) {
            onSceneRequested(this, new SceneRequestEventArgs(typeof(game_scene)));
        }

        public void btnSettings_Click(object? sender, EventArgs? e) {
            onSceneRequested(this, new SceneRequestEventArgs(typeof(settings_scene)));
        }

        public void btnQuit_Click(object? sender, EventArgs? e) {
            onSceneRequested(this, new SceneRequestEventArgs(null));
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
                c.Control_MouseWheelScrolled(sender, e);
            }
        }
#endregion
    }
}