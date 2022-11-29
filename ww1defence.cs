using Global;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ww1defence {
    public class ww1defence {
#region "Properties"
        private RenderWindow window;
        private View sceneView;

        DateTime lastUpdate;
        DateTime lastRender;
        float timeStep = 1f / 60f;
        float frameRate = 1f / 60f;

        private Sprite sprCrosshair;
        private Texture textureSpritesheet;

        private Sprite sprTurretBase;
        private Sprite sprTurretGun;

        private CircleShape csInnerFlakZone;
        private CircleShape csOuterFlakZone;

        private float flakTime = 0f;

        private DateTime lastFire;
        private float fireRate = 1000f / 4f;
        private List<shell> shells;
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

            sceneView = new View(new FloatRect(0, 0, (float)Globals.ScreenSize.X, (float)Globals.ScreenSize.Y));

            sprCrosshair = new Sprite(new Texture("images/crosshair.png"));
            sprCrosshair.Origin = new Vector2f(16, 16);

            textureSpritesheet = new Texture("images/spritesheet.png");
            sprTurretBase = new Sprite(textureSpritesheet, new IntRect(0, 0, 64, 64));
            sprTurretBase.Origin = new Vector2f(32, 64);
            sprTurretBase.Position = new Vector2f(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y);

            sprTurretGun = new Sprite(textureSpritesheet, new IntRect(64, 0, 64, 64));
            sprTurretGun.Origin = new Vector2f(32, 32);
            sprTurretGun.Position = sprTurretBase.Position - new Vector2f(0, 32);

            csInnerFlakZone = new CircleShape();
            csInnerFlakZone.Position = sprTurretBase.Position;
            csInnerFlakZone.OutlineThickness = 3f;
            csInnerFlakZone.OutlineColor = Color.Red;
            csInnerFlakZone.FillColor = Color.Transparent;

            csOuterFlakZone = new CircleShape();
            csOuterFlakZone.Position = sprTurretBase.Position;
            csOuterFlakZone.OutlineThickness = 3f;
            csOuterFlakZone.OutlineColor = Color.Red;
            csOuterFlakZone.FillColor = Color.Transparent;

            lastFire = DateTime.Now;
            fireRate = 1000 / 10; 
            shells = new List<shell>();

            lastUpdate = DateTime.Now;
            lastRender = DateTime.Now;
        }

#region "Events"
        public void window_CloseWindow(object? sender, EventArgs e) {
            window.Close();
        }

        public void window_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e) {
            if (e == null) { return; }

            float scrollSpeed = 2.5f;

            if (e.Delta > 0 && flakTime < 100.0f) {
                flakTime += e.Delta * scrollSpeed;
            } else if (e.Delta < 0 && flakTime > 0.0f) {
                flakTime += e.Delta * scrollSpeed;
            }

            if (flakTime < 0) { flakTime = 0; }
            if (flakTime > 100) { flakTime = 100; }
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

            if (Input.Mouse["left"].isPressed) {
                if ((DateTime.Now - lastFire).Milliseconds >= bullet.fireRate) {
                    lastFire = DateTime.Now;
                    shell? fireThis = shells.Find((x) => x.isAlive == false && x.GetType() == typeof(bullet));

                    if (fireThis == null) {
                        fireThis = new bullet();
                        shells.Add(fireThis);
                    }
                    
                    double radians = (-90 + sprTurretGun.Rotation) * Math.PI / 180;
                    Vector2f newVel = new Vector2f((float)Math.Cos(radians), (float)Math.Sin(radians));

                    fireThis.fire(sprTurretGun.Position, newVel * bullet.speed);
                }
            }

            if (Input.Mouse["right"].isPressed) {
                if ((DateTime.Now - lastFire).Milliseconds >= flak.fireRate) {
                    lastFire = DateTime.Now;
                    shell? fireThis = shells.Find((x) => x.isAlive == false && x.GetType() == typeof(flak));
                    float timer = 600 + flakTime * 50;

                    if (fireThis == null) {
                        fireThis = new flak(DateTime.Now.AddMilliseconds(timer));
                        shells.Add(fireThis);
                    }
                    
                    double radians = (-90 + sprTurretGun.Rotation) * Math.PI / 180;
                    Vector2f newVel = new Vector2f((float)Math.Cos(radians), (float)Math.Sin(radians));
                    
                    flak flakThis = (flak)fireThis;
                    flakThis.fire(sprTurretGun.Position, newVel * flak.speed, DateTime.Now.AddMilliseconds(timer));
                }
            }

            int k = 1;
            foreach (shell s in shells) {
                k++;
                s.update(delta);
            }
        }

        public void draw() {
            window.Clear(Colour.LightBlue);
            window.SetView(sceneView);

            sprCrosshair.Position = (Vector2f)Mouse.GetPosition(window);
            window.Draw(sprCrosshair);

            // rotate the gun to point at crosshair
            Vector2f curRelPos = sprCrosshair.Position - sprTurretGun.Position;
            double idk = Math.Atan2((double)curRelPos.Y, (double)curRelPos.X);
            sprTurretGun.Rotation = 90 + (float)(idk * 180 / Math.PI);

            window.Draw(sprTurretGun);
            window.Draw(sprTurretBase);

            foreach(shell s in shells) {
                s.draw(window);
            }

            // Draw the flak time zone
            csInnerFlakZone.Radius = (40 + flakTime - 10) * 4f;
            csInnerFlakZone.Origin = new Vector2f(csInnerFlakZone.Radius, csInnerFlakZone.Radius);
            window.Draw(csInnerFlakZone);

            csOuterFlakZone.Radius = (40 + flakTime + 10) * 4f;
            csOuterFlakZone.Origin = new Vector2f(csOuterFlakZone.Radius, csOuterFlakZone.Radius);
            window.Draw(csOuterFlakZone);

            window.Display();
        }
#endregion
    }
}