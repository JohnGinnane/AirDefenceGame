using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public class end_scene : scene {
        private label lblScore;
        private button btnExit;
        public end_scene(RenderWindow window) {
            float halfScreenWidth = Globals.ScreenSize.X / 2f;
            float division = Globals.ScreenSize.Y / 3f;

            lblScore = new label();
            lblScore.Size = new Vector2f(500, 50);
            lblScore.Text = "Score: ";
            lblScore.CharacterSize = 24;
            lblScore.Position = new Vector2f(halfScreenWidth + lblScore.Size.X / 2f, division * 1f);
            controls.Add(lblScore);

            btnExit = new button();
            btnExit.Size = new Vector2f(100, 50);
            btnExit.Position = new Vector2f(halfScreenWidth + btnExit.Size.X / 2f, division * 2f);
            btnExit.Text = "Exit";
            btnExit.Click += btnExit_Click;
            controls.Add(btnExit);
        }

        public void btnExit_Click(object? sender, EventArgs? e) {
            onSceneRequested(this, new SceneRequestEventArgs(typeof(menu_scene)));
        }

        public override void update(float delta)
        {
            
        }

        public override void draw(RenderWindow window)
        {
            window.SetMouseCursorVisible(true);
            window.SetView(sceneView);
            
            foreach (control c in controls ){
                c.draw(window);
            }
        }
    }
}