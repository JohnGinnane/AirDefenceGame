using System.Collections;
using System.Collections.Generic;
using Global;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ww1defence {
    public class ww1defence {
#region "Properties"
        private RenderWindow window;

        DateTime lastUpdate;
        DateTime lastRender;
        float timeStep = 1f / 60f;
        float frameRate = 1f / 60f;


        // scene management
        private scene curScene;
        private List<scene> activeScenes;
#endregion

        public void SceneRequestHandler(object? sender, SceneRequestEventArgs? e) {
            // perhaps the scene request could specify a parent scene
            // this way the "Settings" scene can point BACK to the menu scene?
            if (e != null) {
                // Not specifying a target scene will close the game
                // Perhaps we should have a "Exit Game" scene instead?
                if (e.targetScene == null) {
                    window.Close();
                } else {
                    // Check if the scene is already loaded
                    scene? newScene = activeScenes.Find((x) => x.GetType() == e.targetScene);

                    if (newScene == null) {
                        newScene = (scene?)Activator.CreateInstance(e.targetScene, window);

                        if (newScene != null) {
                            newScene.sceneRequestEvent += SceneRequestHandler;
                        }
                    } else {
                        // remove from stack
                        activeScenes.Remove(newScene);
                    }

                    if (newScene != null) {
                        if (!e.unloadMe) {
                            activeScenes.Insert(0, curScene);
                        }

                        curScene = (scene)newScene;
                    }
                }
            }
        }

        public ww1defence() {
            window = new RenderWindow(new SFML.Window.VideoMode((uint)Globals.ScreenSize.X, 
                                                                (uint)Globals.ScreenSize.Y),
                                      "World War One Defence");
            // TODO: Replace "software cursor" with actual cursor later
            // Requires converting image to byte[]
            window.Closed += window_CloseWindow;
            window.MouseWheelScrolled += window_MouseWheelScrolled;
            window.MouseMoved += window_MouseMoved;
            window.MouseButtonPressed += window_MouseButtonPressed;
            window.MouseButtonReleased += window_MouseButtonReleased;

            activeScenes = new List<scene>();

            // Load the menu scene as the first scene
            curScene = new menu_scene(window);
            curScene.sceneRequestEvent += SceneRequestHandler;

            // Load sound buffers
            Globals.Buffers.Add("sound/machine_gun1.wav", new SoundBuffer("sound/machine_gun1.wav"));
            Globals.Buffers.Add("sound/machine_gun2.wav", new SoundBuffer("sound/machine_gun2.wav"));
            Globals.Buffers.Add("sound/machine_gun3.wav", new SoundBuffer("sound/machine_gun3.wav"));
            Globals.Buffers.Add(explosion.ExplodeSFX, new SoundBuffer(explosion.ExplodeSFX));
            Globals.Buffers.Add(flak.FireSFX, new SoundBuffer(flak.FireSFX));

            // Put test enemy in middle of screen
            // smallBlimp publicEnemyNumberOne = new smallBlimp(copySprite(sprSmallBlimp), Globals.ScreenSize / 2, new Vector2f());
            // publicEnemyNumberOne.initialHealth = 10000f;
            // publicEnemyNumberOne.health = publicEnemyNumberOne.initialHealth;
            // enemies.Add(publicEnemyNumberOne);

            lastUpdate = DateTime.Now;
            lastRender = DateTime.Now;
        }

#region "Events"
        public void window_CloseWindow(object? sender, EventArgs e) {
            window.Close();
        }

        public void window_MouseMoved(object? sender, MouseMoveEventArgs? e) {
            curScene.MouseMoved(sender, e);
        }

        public void window_MouseButtonPressed(object? sender, MouseButtonEventArgs? e) {
            curScene.MouseButtonPressed(sender, e);
        }

        public void window_MouseButtonReleased(object? sender, MouseButtonEventArgs? e) {
            curScene.MouseButtonReleased(sender, e);
        }

        public void window_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e) {
            curScene.MouseWheelScrolled(sender, e);
        }
#endregion

#region "Main"
        public void run() {
            while (window.IsOpen) {
                if (DateTime.Now > lastUpdate.AddSeconds(timeStep)) {
                    float delta = timeStep;
                    window.DispatchEvents();
                    update(delta);
                    lastUpdate = DateTime.Now;
                }

                if (DateTime.Now > lastRender.AddSeconds(frameRate)) {
                    draw();
                    lastRender = DateTime.Now;
                }
            }
        }

        public void update(float delta) {
            if (window.HasFocus()) {
                Input.Keyboard.update();
                Input.Mouse.update(window);
            }

            // if (Input.Keyboard["escape"].isPressed) {
            //     window.Close();
            // }

            curScene.update(delta);
        }

        public void draw() {
            window.Clear(Colour.LightBlue);

            curScene.draw(window);

            window.Display();
        }
#endregion
    }
}