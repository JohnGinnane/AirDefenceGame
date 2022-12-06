using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using Global;

namespace ww1defence {
    public abstract class shell {
        public Vector2f position;
        public Vector2f velocity;
        public bool isAlive = false;
        public float damage = 25f;

        public shell() {
            this.position = new Vector2f();
            this.velocity = new Vector2f();
            isAlive = false;
        }

        public void fire(Vector2f position, Vector2f velocity) {            
            isAlive = true;
            this.position = position;
            this.velocity = velocity;
        }

        public abstract void update(float delta);
        public abstract void draw(RenderWindow window);

        public abstract void applyDamage(float delta, enemy e);
    }

    public class flak : shell
    {
        public static float fireRate = 1000f / 4f; // 4 shots per second
        public static float speed = 300f;
        public static float explosionLifeDefault = 1f; // 1 second long explosion
        public static float explosionSizeDefault = 20f;

        private RectangleShape rsFlak;
        private CircleShape csExplosion;

        public static string FireSFX {
            get {
                return "sound/flak_fire.wav";
            }
        }

        public static string ExplodeSFX {
            get {
                return "sound/flak_explosion.wav";
            }
        }

        public static DateTime lastFire;
        
        public DateTime explodeTime;
        public float explosionLife;

        new public float damage = 50f;

        public flak(DateTime explodeTime) {
            rsFlak = new RectangleShape(new Vector2f(10, 4));
            rsFlak.OutlineColor = Color.Black;
            rsFlak.OutlineThickness = 1f;
            rsFlak.FillColor = Colour.LightYellow;

            csExplosion = new CircleShape();
            csExplosion.Radius = explosionSizeDefault;
            csExplosion.OutlineColor = Colour.Orange;
            csExplosion.OutlineThickness = explosionSizeDefault / 3f;
            csExplosion.FillColor = Color.Red;
            csExplosion.Origin = new Vector2f(csExplosion.Radius, csExplosion.Radius);

            this.explodeTime = explodeTime;
        }

        public override void update(float delta)
        {
            if (isAlive && explosionLife == 0) {
                this.position = this.position + this.velocity * delta;
                this.velocity.Y = this.velocity.Y + 10f * delta; // gravity

                if (DateTime.Now >= explodeTime) {
                    explode();
                    return;
                }
                    
                if (this.position.X < -100 || this.position.X > Globals.ScreenSize.X + 100 ||
                    this.position.Y < -100 || this.position.Y > Globals.ScreenSize.Y + 100) {
                    this.isAlive = false;
                }
            } else if (isAlive && explosionLife > 0) {
                explosionLife = explosionLife - delta;
                
                if (explosionLife <= 0) {
                    isAlive = false;
                }
            }
        }

        public override void applyDamage(float delta, enemy e)
        {
            if (e.isAlive && isAlive && explosionLife > 0) {
                if (isAlive) {
                    e.health -= damage * delta;
                }
            }
        }

        public override void draw(RenderWindow window)
        {
            if (isAlive && explosionLife == 0) {
                rsFlak.Position = position;
                rsFlak.Rotation = (float)(Math.Atan2((double)velocity.Y, (double)velocity.X) * 180 / Math.PI);
                window.Draw(rsFlak);
            } else if (isAlive && explosionLife > 0) {
                csExplosion.FillColor = new Color(255, 0, 0, (byte)(255 * explosionLife));
                csExplosion.OutlineColor = new Color(255, 128, 0, (byte)(255 * explosionLife));
                window.Draw(csExplosion);
            }
        }

        public void fire(Vector2f position, Vector2f velocity, DateTime explodeTime) {
            base.fire(position, velocity);
            this.explodeTime = explodeTime;
            this.explosionLife = 0;
            Globals.playSound(flak.FireSFX, pitch: util.randfloat(0.95f, 1.05f));
        }

        public void explode() {
            explosionLife = explosionLifeDefault;
            csExplosion.Position = position;
            velocity = new Vector2f();
            Globals.playSound(ExplodeSFX, pitch: util.randfloat(0.95f, 1.05f));
        }        
    }

    public class bullet : shell
    {
        private RectangleShape rsBullet;

        public static float speed = 600f;
        public static float fireRate = 1000f / 10f;
        
        public static DateTime lastFire;

        new public float damage = 2f;

        public static string FireSFX {
            get {
                return "sound/machine_gun" + util.randint(1, 3).ToString() + ".wav";
            }
        }

        public bullet() {
            rsBullet = new RectangleShape(new Vector2f(4, 2));
            rsBullet.OutlineColor = Color.Black;
            rsBullet.OutlineThickness = 1f;
            rsBullet.FillColor = Color.Yellow;
        }

        public override void update(float delta)
        {
            if (isAlive) {
                this.position = this.position + this.velocity * delta;
                this.velocity.Y = this.velocity.Y + 10f * delta; // gravity
                    
                if (this.position.X < -100 || this.position.X > Globals.ScreenSize.X + 100 ||
                    this.position.Y < -100 || this.position.Y > Globals.ScreenSize.Y + 100) {
                    this.isAlive = false;
                }
            }
        }

        public override void applyDamage(float delta, enemy e)
        {
            if (isAlive && e.isAlive) {
                e.health -= damage;
                this.isAlive = false;
            }
        }

        public override void draw(RenderWindow window)
        {
            if (isAlive) {
                rsBullet.Position = position;
                rsBullet.Rotation = (float)(Math.Atan2((double)velocity.Y, (double)velocity.X) * 180 / Math.PI);
                window.Draw(rsBullet);
            }
        }

        public new void fire(Vector2f position, Vector2f velocity) {
            base.fire(position, velocity);
            Globals.playSound(bullet.FireSFX, vol: 40f, pitch: util.randfloat(0.95f, 1.05f));
        }
    }
}