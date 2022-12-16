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

        public ww1defence() {
            window = new RenderWindow(new SFML.Window.VideoMode((uint)Globals.ScreenSize.X, 
                                                                (uint)Globals.ScreenSize.Y),
                                      "World War One Defence");
            // TODO: Replace "software cursor" with actual cursor later
            // Requires converting image to byte[]
            window.SetMouseCursorVisible(false);
            window.Closed += window_CloseWindow;
            window.MouseWheelScrolled += window_MouseWheelScrolled;

            activeScenes = new List<scene>();

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

        public void window_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e) {
            if (e == null) { return; }

            if (e.Delta > 0 && flakTime < maxFlakTime) {
                flakTime += e.Delta * flakScrollSpeed;;
            } else if (e.Delta < 0 && flakTime > minFlakTime) {
                flakTime += e.Delta * flakScrollSpeed;
            }

            if (flakTime < minFlakTime) { flakTime = minFlakTime; }
            if (flakTime > maxFlakTime) { flakTime = maxFlakTime; }
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
            Input.Keyboard.update();
            Input.Mouse.update(window);

            if (Input.Keyboard["escape"].isPressed) {
                window.Close();
            }
        }

        public void draw() {
            window.Clear(Colour.LightBlue);

            curScene.draw(window);
            // Draw HUD on top of scene

            window.Display();
        }
#endregion
    }
}