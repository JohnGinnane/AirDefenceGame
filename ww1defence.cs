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

        private Texture textureSpritesheet;
        private Sprite sprCrosshair;
        private Sprite sprFlakCrosshair;
        private Sprite sprTurretBase;
        private Sprite sprTurretGun;
        private Sprite sprSmallBlimp;
        private Sprite sprSmallBomb;

        private CircleShape csFlakZone;
        private static float flakZoneThickness = 40f;

        private float flakTurnSpeed = 50f;
        private static float minFlakTime = 50f;
        private static float maxFlakTime = 500f;
        private static float flakScrollSpeed = 10f;

        private float flakTime = minFlakTime + (maxFlakTime - minFlakTime) / 2;

        private List<shell> shells;

        private List<enemy> enemies;
        private List<explosion> explosions;

        public List<shell> ActiveShells {
            get { return shells.FindAll((x) => x.isActive); }
        }

        public List<enemy> ActiveEnemies {
            get { return enemies.FindAll((x) => x.isActive && x.lifeState == enemy.eLifeState.alive); }
        }

        public List<explosion> ActiveExplosions {
            get { return explosions.FindAll((x) => x.isActive); }
        }
        
        private DateTime flakLastFire;
        private DateTime mgLastFire;
        private float playerHealth = 100f;
        private RectangleShape rsHealthBackground;
        private RectangleShape rsHealthCurrent;
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

            textureSpritesheet = new Texture("images/spritesheet.png");

            sprCrosshair = new Sprite(textureSpritesheet, new IntRect(0, 64, 32, 32));
            sprCrosshair.Origin = new Vector2f(16, 16);

            sprFlakCrosshair = new Sprite(textureSpritesheet, new IntRect(32, 64, 32, 32));
            sprFlakCrosshair.Origin = new Vector2f(16, 16);

            sprTurretBase = new Sprite(textureSpritesheet, new IntRect(0, 0, 64, 64));
            sprTurretBase.Origin = new Vector2f(32, 64);
            sprTurretBase.Position = new Vector2f(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y);

            sprTurretGun = new Sprite(textureSpritesheet, new IntRect(64, 0, 64, 64));
            sprTurretGun.Origin = new Vector2f(32, 32);
            sprTurretGun.Position = sprTurretBase.Position - new Vector2f(0, 32);

            sprSmallBlimp = new Sprite(textureSpritesheet, new IntRect(128, 0, 128, 32*3));
            sprSmallBlimp.Origin = new Vector2f(128/2, (32*3)/2);

            sprSmallBomb = new Sprite(textureSpritesheet, new IntRect(320, 0, 32, 64));
            sprSmallBomb.Origin = new Vector2f(32 / 2, 64 / 2);

            csFlakZone = new CircleShape(0, 100);
            csFlakZone.Position = sprTurretBase.Position;
            csFlakZone.OutlineThickness = flakZoneThickness;
            csFlakZone.OutlineColor = new Color(240, 50, 40, 80);
            csFlakZone.FillColor = Color.Transparent;

            rsHealthBackground = new RectangleShape(new Vector2f(104, 14));
            rsHealthBackground.FillColor = Colour.Grey;

            rsHealthCurrent = new RectangleShape(new Vector2f(100, 10));
            rsHealthCurrent.FillColor = new Color(75, 230, 35);

            textWave = new Text("", Global.Fonts.Arial);
            textWave.CharacterSize = 30;
            textWave.Position = new Vector2f(Globals.ScreenSize.X / 2f, Globals.ScreenSize.Y / 2f);

            flakLastFire = DateTime.Now;
            mgLastFire = DateTime.Now;

            shells = new List<shell>();
            enemies = new List<enemy>();
            explosions = new List<explosion>();
            
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
            enemy? newEnemy = enemies.Find((x) => x.lifeState == enemy.eLifeState.dead);

            int dir = util.randsign();
            Vector2f pos = new Vector2f((dir < 0 ? Globals.ScreenSize.X - 1 : 1),
                                        util.randint(64, (int)(Globals.ScreenSize.Y / 2)));
            Vector2f vel = new Vector2f(dir * 30f * util.randfloat(0.9f, 1.1f), 0);

            if (newEnemy == null) {
                newEnemy = new smallBlimp(copySprite(sprSmallBlimp), pos, vel);
                enemies.Add(newEnemy);
            } else {
                newEnemy.Rotation = 0;
                newEnemy.Position = pos;
                newEnemy.Velocity = vel;
                newEnemy.health = newEnemy.initialHealth;
                newEnemy.lifeState = enemy.eLifeState.alive;
            }
            
            // offset the position by the width of the blimp to avoid "pop in"
            //newEnemy.Position.X = newEnemy.Position.X + sprSmallBlimp.TextureRect.Width * dir * -1;

            if (newEnemy.Sprite != null) {
                newEnemy.Sprite.Scale = new Vector2f(Math.Abs(newEnemy.Sprite.Scale.X) * dir * -1,
                                                        Math.Abs(newEnemy.Sprite.Scale.Y));
            }

            // set last fire to be in the future to delay the first bomb being dropped
            newEnemy.lastFire = DateTime.Now.AddSeconds(util.randfloat(4, 8));
            newEnemy.isActive = true;
        }

        public void explode(Vector2f pos, float radius, float duration, float damage, float soundPitch = 1f) {
            explosion? e = explosions.Find((x) => !x.isActive);

            if (e == null) {
                e = new explosion(pos, radius, duration, damage);
            } else {
                e.start(pos, radius, duration, damage, soundPitch);
            }

            explosions.Add(e);
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
                if ((DateTime.Now - mgLastFire).Milliseconds >= bullet.fireRate) {
                    mgLastFire = DateTime.Now;
                    bullet? fireThis = (bullet?)shells.Find((x) => (
                        !x.isActive && x.GetType() == typeof(bullet)
                    ));

                    if (fireThis == null) {
                        fireThis = new bullet();
                        shells.Add(fireThis);
                    }
                    
                    Vector2f newVel = util.normalise(sprCrosshair.Position - sprTurretGun.Position) * bullet.speed;
                    Vector2f newPos = sprTurretGun.toWorld(new Vector2f(0, 20f));

                    fireThis.fire(newPos, newVel);
                }
            }

            if (Input.Mouse["right"].isPressed) {
                if ((DateTime.Now - flakLastFire).Milliseconds >= flak.fireRate) {
                    flakLastFire = DateTime.Now;
                    flak? fireThis = (flak?)shells.Find((x) => (
                        !x.isActive && x.GetType() == typeof(flak)
                    ));
                    
                    float timer = (flakTime * 1.1f - flakZoneThickness / 2f
                                   + util.randfloat(flakZoneThickness / -2, flakZoneThickness / 2)) / flak.speed * 1000;

                    if (fireThis == null) {
                        fireThis = new flak(DateTime.Now.AddMilliseconds(timer));
                        shells.Add(fireThis);
                    }
                    
                    double radians = (-90 + sprTurretGun.Rotation + util.randfloat(-2, 2)) * Math.PI / 180;
                    Vector2f newVel = new Vector2f((float)Math.Cos(radians), (float)Math.Sin(radians)) * flak.speed;
                    Vector2f newPos = sprTurretGun.toWorld(new Vector2f(0, 20f));
                    
                    fireThis.fire(newPos, newVel, DateTime.Now.AddMilliseconds(timer));
                }
            }

            // Handle flak cross hair
            Vector2f curRelPos = sprCrosshair.Position - sprTurretGun.Position;
            float targetAngle = 90f + (float)(Math.Atan2((double)curRelPos.Y, (double)curRelPos.X) * 180 / Math.PI);
            float moveAngle = Math.Clamp((targetAngle - sprTurretGun.Rotation),
                                         -flakTurnSpeed * delta,
                                         flakTurnSpeed * delta);
            sprTurretGun.Rotation = sprTurretGun.Rotation + moveAngle;

            sprFlakCrosshair.Position = sprTurretGun.toWorld(new Vector2f(0, -util.distance(sprTurretGun.Position, sprCrosshair.Position)));

            handleWave();

            foreach (shell s in ActiveShells) {
                s.update(delta);
            }

            foreach (enemy e in ActiveEnemies) {
                e.update(delta);
            }

            foreach (explosion ex in ActiveExplosions) {
                ex.update(delta);
            }

            // Check if any BULLETS have intersected with BOMBS
            foreach (bullet b in ActiveShells.FindAll((x) => x.GetType() == typeof(bullet))) {
                foreach (smallBomb sb in ActiveShells.FindAll((x)=>x.GetType() == typeof(smallBomb))) {
                    if (entity.collision(b, sb)) {
                        explode(sb.Position,
                                smallBomb.explosionRadius,
                                smallBomb.explosionDuration,
                                sb.damage,
                                smallBomb.explosionPitch());
                        b.kill();
                        sb.kill();
                    }
                }
            }

            // Check if explosions intersect with enemies
            foreach (explosion ex in ActiveExplosions) {
                foreach (enemy e in ActiveEnemies) {
                    if (entity.collision(ex, e)) {
                        ex.applyDamage(delta, e);
                    }
                }
            }

            // Check if we need to do anything else with shells (collisions etc)
            foreach (shell s in ActiveShells) {
                if (s.GetType() == typeof(flak)) {
                    flak f = (flak)s;

                    if (DateTime.Now >= f.explodeTime) {
                        explode(f.Position,
                                flak.explosionRadius,
                                flak.explosionDuration,
                                f.damage,
                                flak.explosionPitch());
                        f.kill();
                        continue;
                    }
                }

                if (s.GetType() == typeof(bullet)) {
                    bullet b = (bullet)s;

                    foreach (enemy e in ActiveEnemies.FindAll((x) => x.lifeState == enemy.eLifeState.alive)) {
                        if (entity.collision(b, e)) {
                            b.applyDamage(delta, e);
                            b.kill();
                            continue;
                        }
                    }
                }

                if (s.GetType() == typeof(smallBomb)) {
                    smallBomb sb = (smallBomb)s;

                    if (sb.Position.Y >= Globals.ScreenSize.Y) {
                        explode(sb.Position,
                                smallBomb.explosionRadius,
                                smallBomb.explosionDuration,
                                sb.damage,
                                smallBomb.explosionPitch());
                        sb.kill();
                        continue;
                    }
                }
            }

            foreach (enemy e in ActiveEnemies) {
                if (e.lifeState == enemy.eLifeState.alive && (DateTime.Now - e.lastFire).TotalMilliseconds >= smallBomb.fireRate) {
                    e.lastFire = DateTime.Now;

                    smallBomb? fireThis = (smallBomb?)shells.Find((x) => !x.isActive && x.GetType() == typeof(smallBomb));

                    if (fireThis == null) {
                        fireThis = new smallBomb(copySprite(sprSmallBomb));
                        shells.Add(fireThis);
                    } else {
                        if (fireThis.Sprite != null) {
                            fireThis.Sprite.Rotation = 0;
                        }
                    }
                     
                    Vector2f newPos = e.Position + new Vector2f(0, 0);
                    Vector2f newVel = e.Velocity;
                    fireThis.fire(newPos, newVel);
                }
            }

            if (enemies.FindAll((x) => x.lifeState == enemy.eLifeState.alive).Count == 0 && enemiesToSpawn == 0) {
                increaseWave(); 
            }
        }

        public void draw() {
            window.Clear(Colour.LightBlue);
            window.SetView(sceneView);

            window.Draw(textWave);

            foreach (enemy e in ActiveEnemies) {
                e.draw(window);
            }

            foreach (explosion ex in ActiveExplosions) {
                ex.draw(window);
            }

            foreach(shell s in ActiveShells) {
                s.draw(window);
            }

            window.Draw(sprTurretGun);
            window.Draw(sprTurretBase);

            rsHealthBackground.Position = sprTurretBase.Position + new Vector2f(-sprTurretBase.Origin.X,
                                                                                sprTurretBase.TextureRect.Height);
            window.Draw(rsHealthBackground);
            
            if (playerHealth > 0) {
                rsHealthCurrent.Position = sprTurretBase.Position + new Vector2f(-sprTurretBase.Origin.X + 2,
                                                                                 -sprTurretBase.TextureRect.Height + 12);
                rsHealthCurrent.Size = new Vector2f(playerHealth, 10);
                window.Draw(rsHealthCurrent);
            }
            
            csFlakZone.Radius = flakTime;
            csFlakZone.Origin = new Vector2f(csFlakZone.Radius, csFlakZone.Radius);
            window.Draw(csFlakZone);

            // Always display crosshair on top of background
            window.Draw(sprFlakCrosshair);
            sprCrosshair.Position = (Vector2f)Mouse.GetPosition(window);
            window.Draw(sprCrosshair);

            // Draw HUD on top of scene

            window.Display();
        }
#endregion
    }
}