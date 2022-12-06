using Global;
using SFML.Audio;
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

        int wave = 0;
        int enemiesToSpawn = 0;
        DateTime nextEnemySpawn = DateTime.Now;
        float enemiesSpawnRate = 0.5f; // per second
        Text textWave;

        private Sprite sprCrosshair;
        private Texture textureSpritesheet;

        private Sprite sprTurretBase;
        private Sprite sprTurretGun;
        private Sprite sprSmallBlimp;

        private CircleShape csFlakZone;
        private static float flakZoneThickness = 40f;

        private static float minFlakTime = 50f;
        private static float maxFlakTime = 500f;
        private static float flakScrollSpeed = 10f;

        private float flakTime = minFlakTime;

        private List<shell> shells;

        private List<enemy> enemies;
        
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

            sprSmallBlimp = new Sprite(textureSpritesheet, new IntRect(128, 0, 128, 32*3));
            sprSmallBlimp.Origin = new Vector2f(128/2, (32*3)/2);

            csFlakZone = new CircleShape(0, 100);
            csFlakZone.Position = sprTurretBase.Position;
            csFlakZone.OutlineThickness = flakZoneThickness;
            csFlakZone.OutlineColor = new Color(240, 50, 40, 80);
            csFlakZone.FillColor = Color.Transparent;

            textWave = new Text("", Global.Fonts.Arial);
            textWave.CharacterSize = 30;
            textWave.Position = new Vector2f(Globals.ScreenSize.X / 2f, Globals.ScreenSize.Y / 2f);

            flak.lastFire = DateTime.Now;
            bullet.lastFire = DateTime.Now;

            shells = new List<shell>();
            enemies = new List<enemy>();
            
            // Load sound buffers
            Globals.Buffers.Add("sound/machine_gun1.wav", new SoundBuffer("sound/machine_gun1.wav"));
            Globals.Buffers.Add("sound/machine_gun2.wav", new SoundBuffer("sound/machine_gun2.wav"));
            Globals.Buffers.Add("sound/machine_gun3.wav", new SoundBuffer("sound/machine_gun3.wav"));
            Globals.Buffers.Add(flak.FireSFX, new SoundBuffer(flak.FireSFX));
            Globals.Buffers.Add(flak.ExplodeSFX, new SoundBuffer(flak.ExplodeSFX));

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

#region "Functions"
        public Sprite copySprite(Sprite spr) {
            Sprite sprReturn = new Sprite(spr.Texture, spr.TextureRect);
            sprReturn.Origin = spr.Origin;
            sprReturn.Position = spr.Position;
            sprReturn.Rotation = spr.Rotation;
            sprReturn.Scale = spr.Scale;
            sprReturn.Color = spr.Color;

            return sprReturn;
        }

        public void handleWave() {
            if (nextEnemySpawn < DateTime.Now && enemiesToSpawn > 0) {
                spawnEnemy();
                enemiesToSpawn--;
                nextEnemySpawn = DateTime.Now.AddSeconds(1f / enemiesSpawnRate);
            }
        }
        
        public void increaseWave() {
            Console.WriteLine("Increasing Wave");
            wave += 1;
            enemiesToSpawn = (int)(10 + (Math.Pow(wave, 1.3)));
            nextEnemySpawn = DateTime.Now;

            textWave.DisplayedString = "Wave " + wave.ToString();
            FloatRect textRect = textWave.GetLocalBounds();
            textWave.Origin = new Vector2f(textRect.Left + textRect.Width / 2f,
                                           textRect.Top + textRect.Height / 2f);
        }

        public void spawnEnemy() {
            // check for spare objects not in use
            enemy? newEnemy = enemies.Find((x) => x.isAlive == false );

            int dir = util.randsign();
            Vector2f pos = new Vector2f((dir < 0 ? Globals.ScreenSize.X - 1 : 1),
                                        util.randint(64, (int)(Globals.ScreenSize.Y / 2)));
            Vector2f vel = new Vector2f(dir * 30f * util.randfloat(0.9f, 1.1f), 0);

            if (newEnemy == null) {
                newEnemy = new smallBlimp(copySprite(sprSmallBlimp), pos, vel);
                // offset the position by the width of the blimp to avoid "pop in"
                newEnemy.position.X = newEnemy.position.X + sprSmallBlimp.TextureRect.Width * dir * -1;
                enemies.Add(newEnemy);
            } else {
                newEnemy.position = pos;
                newEnemy.position.X = newEnemy.position.X + sprSmallBlimp.TextureRect.Width * dir * -1;
                newEnemy.velocity = vel;
                newEnemy.health = newEnemy.initialHealth;
                newEnemy.isAlive = true;
            }
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
                if ((DateTime.Now - bullet.lastFire).Milliseconds >= bullet.fireRate) {
                    bullet.lastFire = DateTime.Now;
                    shell? fireThis = shells.Find((x) => x.isAlive == false && x.GetType() == typeof(bullet));

                    if (fireThis == null) {
                        fireThis = new bullet();
                        shells.Add(fireThis);
                    }
                    
                    double radians = (-90 + sprTurretGun.Rotation + util.randfloat(-2, 2)) * Math.PI / 180;
                    Vector2f newVel = new Vector2f((float)Math.Cos(radians), (float)Math.Sin(radians));

                    bullet bulletThis = (bullet)fireThis;
                    bulletThis.fire(sprTurretGun.Position, newVel * bullet.speed);
                }
            }

            if (Input.Mouse["right"].isPressed) {
                if ((DateTime.Now - flak.lastFire).Milliseconds >= flak.fireRate) {
                    flak.lastFire = DateTime.Now;
                    shell? fireThis = shells.Find((x) =>(
                        x.isAlive == false &&
                        x.GetType() == typeof(flak)
                    ));
                    
                    float timer = (flakTime * 1.1f - flakZoneThickness / 2f
                                   + util.randfloat(flakZoneThickness / -2, flakZoneThickness / 2)) / flak.speed * 1000;

                    if (fireThis == null) {
                        fireThis = new flak(DateTime.Now.AddMilliseconds(timer));
                        shells.Add(fireThis);
                    }
                    
                    double radians = (-90 + sprTurretGun.Rotation + util.randfloat(-2, 2)) * Math.PI / 180;
                    Vector2f newVel = new Vector2f((float)Math.Cos(radians), (float)Math.Sin(radians));
                    
                    flak flakThis = (flak)fireThis;
                    flakThis.fire(sprTurretGun.Position, newVel * flak.speed, DateTime.Now.AddMilliseconds(timer));
                }
            }

            if (Input.Keyboard["R"].justReleased) {
                spawnEnemy();
            }

            handleWave();

            foreach (shell s in shells) {
                s.update(delta);

                if (!s.isAlive) { continue; }

                foreach (enemy e in enemies.FindAll((x) => x.isAlive)) {                    
                    if (s.GetType() == typeof(bullet)) {
                        if (intersection.pointInsideRectangle(s.position, e.sprite.GetGlobalBounds())) {
                            s.applyDamage(delta, e);
                            break;
                        }
                    }

                    if (s.GetType() == typeof(flak)) {
                        flak f = (flak)s;

                        if (f.explosionLife > 0) {
                            if (intersection.circleInsideRectangle(s.position, flak.explosionSizeDefault, e.sprite.GetGlobalBounds())) {
                                s.applyDamage(delta, e);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (enemy e in enemies) {
                e.update(delta);
            }

            if (enemies.FindAll((x) => x.isAlive).Count == 0 && enemiesToSpawn == 0) {
                increaseWave(); 
            }
        }

        public void draw() {
            window.Clear(Colour.LightBlue);
            window.SetView(sceneView);

            window.Draw(textWave);

            sprCrosshair.Position = (Vector2f)Mouse.GetPosition(window);
            window.Draw(sprCrosshair);

            // rotate the gun to point at crosshair
            Vector2f curRelPos = sprCrosshair.Position - sprTurretGun.Position;
            double idk = Math.Atan2((double)curRelPos.Y, (double)curRelPos.X);
            sprTurretGun.Rotation = 90 + (float)(idk * 180 / Math.PI);

            window.Draw(sprTurretGun);
            window.Draw(sprTurretBase);

            foreach (enemy e in enemies) {
                e.draw(window);
            }

            foreach(shell s in shells) {
                s.draw(window);
            }

            csFlakZone.Radius = flakTime;
            csFlakZone.Origin = new Vector2f(csFlakZone.Radius, csFlakZone.Radius);
            window.Draw(csFlakZone);

            window.Display();
        }
#endregion
    }
}