using SFML.Graphics;
using Global;
using SFML.System;

namespace ww1defence {
    public class pause_scene : scene {
        public pause_scene(RenderWindow window, scene? requestedBy = null) {
            setControls();
        }

        public override void draw(RenderWindow window)
        {
            window.SetMouseCursorVisible(true);
            window.SetView(sceneView);

            foreach (control c in controls) {
                c.draw(window);
            }
        }

        public override void setControls()
        {
            controls = new List<control>();
            float buttonWidth = 100;
            float buttonHeight = 50;
            Vector2f buttonSize = new Vector2f(buttonWidth, buttonHeight);
            float buttonPadding = 10;
            int index = 0;

            button btnResume = new button();
            btnResume.Click += btnResume_Click;
            btnResume.Text = "Resume";
            btnResume.Size = buttonSize;
            btnResume.Position = new Vector2f(buttonPadding / 2f + (buttonWidth / 2f),
                                              buttonPadding / 2f + (buttonHeight / 2f) + ((buttonHeight + buttonPadding) * index));
            controls.Add(btnResume);

            index++;
            button btnRestart = new button();
            btnRestart.Click = btnRestart_Click;
            btnRestart.Text = "Restart";
            btnRestart.Size = buttonSize;
            btnRestart.Position = new Vector2f(buttonPadding / 2f + (buttonWidth / 2f),
                                               buttonPadding / 2f + (buttonHeight / 2f) + ((buttonHeight + buttonPadding) * index));
            controls.Add(btnRestart);

            index++;
            button btnSettings = new button();
            btnSettings.Click = btnSettings_Click;
            btnSettings.Text = "Settings";
            btnSettings.Size = buttonSize;
            btnSettings.Position = new Vector2f(buttonPadding / 2f + (buttonWidth / 2f),
                                                buttonPadding / 2f + (buttonHeight / 2f) + ((buttonHeight + buttonPadding) * index));
            controls.Add(btnSettings);

            index++;
            button btnExitToMenu = new button();
            btnExitToMenu.Click = btnExitToMenu_Click;
            btnExitToMenu.Text = "Exit to Menu";
            btnExitToMenu.Size = buttonSize;
            btnExitToMenu.Position = new Vector2f(buttonPadding / 2f + (buttonWidth / 2f),
                                                  buttonPadding / 2f + (buttonHeight / 2f) + ((buttonHeight + buttonPadding) * index));
            controls.Add(btnExitToMenu);

            index++;
            button btnExitToDesktop = new button();
            btnExitToDesktop.Click = btnExitToDesktop_Click;
            btnExitToDesktop.Text = "Exit to Desktop";
            btnExitToDesktop.Size = buttonSize;
            btnExitToDesktop.Position = new Vector2f(buttonPadding / 2f + (buttonWidth / 2f),
                                                     buttonPadding / 2f + (buttonHeight / 2f) + ((buttonHeight + buttonPadding) * index));
            controls.Add(btnExitToDesktop);

            // resume
            // restart
            // settings
            // exit to menu / desktop
        }

#region "Events"
    public void btnResume_Click(object? sender, EventArgs? e) {
        scene? newScene = onSceneRequested(this, new SceneRequestEventArgs(typeof(game_scene)));

        if (newScene != null) {
            if (util.IsSameOrSubclass(typeof(game_scene), newScene.GetType())) {
                game_scene gameScene = (game_scene)newScene;
                gameScene.start();
            } else {
                Exception ex = new Exception("Requested scene was not a game scene!");
                throw ex;
            }
        } else {
            Exception ex = new Exception("Request game scene not returned!");
            throw ex;
        }
    }

    public void btnRestart_Click(object? sender, EventArgs? e) {
        onSceneRequested(this, new SceneRequestEventArgs(typeof(game_scene)));
    }

    public void btnSettings_Click(object? sender, EventArgs? e) {
        onSceneRequested(this, new SceneRequestEventArgs(typeof(settings_scene), false));
    }

    public void btnExitToMenu_Click(object? sender, EventArgs? e) {
        onSceneRequested(this, new SceneRequestEventArgs(typeof(menu_scene)));
    }

    public void btnExitToDesktop_Click(object? sender, EventArgs? e) {
        onSceneRequested(this, new SceneRequestEventArgs(null));
    }
#endregion
    }
}