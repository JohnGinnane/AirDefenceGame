using SFML.Graphics;
using SFML.Window;
using SFML.System;
using Global;

namespace ww1defence {   

    public class settings_scene : scene
    {
        private List<control> controls;
        private slider sldVolume;

        public settings_scene(RenderWindow window) {
            window.SetMouseCursorVisible(true);

            controls = new List<control>();

            float halfScreenWidth = Globals.ScreenSize.X / 2f;
            float division = Globals.ScreenSize.Y / 5f;

            // Volume slider
            sldVolume = new slider();
            sldVolume.Size = new Vector2f(300, 50);
            sldVolume.Position = new Vector2f(halfScreenWidth - sldVolume.Size.X/2f, division * 1f);
            sldVolume.MinimumValue = 0f;
            sldVolume.MaximumValue = 1f;
            sldVolume.Value = Globals.masterVolumeMulti;
            controls.Add(sldVolume);

            button btnBack = new button();
            btnBack.Text = "Back";
            btnBack.Size = new Vector2f(100, 50);
            btnBack.Position = new Vector2f(halfScreenWidth - btnBack.Size.X/2f, division * 3f);
            btnBack.Click += btnBack_Click;
            controls.Add(btnBack);
        }

        public override void update(float delta) {
            if (sldVolume != null) {
                Globals.masterVolumeMulti = sldVolume.Value;
            }
        }

        public override void draw(RenderWindow window) {
            foreach (control c in controls) {
                c.draw(window);
            }
        }

#region "Events"
        public void btnBack_Click(object? sender, EventArgs? e) {
            onSceneRequested(this, new SceneRequestEventArgs(typeof(menu_scene)));
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