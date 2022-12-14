using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using Global;

namespace ww1defence {
    public class explosion : entity {
        internal static string explodeSFX = "sound/flak_explosion.wav";
        public static string ExplodeSFX {
            get {
                return explodeSFX;
            }
        }
        
        internal float initialDuration;
        internal float duration;
        public float Duration {
            get { return duration; }
        }

        internal float initialRadius;
        internal float radius;
        public float Radius {
            get { return radius; }
        }

        internal float damage;
        public float Damage {
            get { return  damage; }
            set { damage = value; }
        }
        
        // returns the duration from a range of 0 to 255
        internal byte ExplosionAlpha {
            get {
                if (initialDuration == 0)       { return 0; }
                if (Duration == 0)              { return 0; }
                if (Duration > initialDuration) { return 1; }
                return (byte)(255f * Duration / initialDuration);
            }
        }

        internal CircleShape csExplosion;

        public explosion() : base() {
            csExplosion = new CircleShape();
            csExplosion.OutlineColor = Colour.Orange;
            csExplosion.FillColor = Color.Red;
        }

        public void start(Vector2f position, float radius, float duration, float damage, float pitch = 1f) {
            Position = position;
            initialDuration = duration;
            this.duration = initialDuration;
            initialRadius = radius;
            this.radius = initialRadius;
            Damage = damage;
            isActive = true;

            csExplosion.OutlineColor = Colour.Orange;
            csExplosion.FillColor = Color.Red;
            csExplosion.Radius = Radius;
            csExplosion.OutlineThickness = initialRadius / 3f;
            csExplosion.Origin = new Vector2f(csExplosion.Radius, csExplosion.Radius);
            Globals.playSound(ExplodeSFX, pitch: pitch);

            test = DateTime.Now;
        }

        internal DateTime test;

        public override void update(float delta)
        {
            if (isActive) {
                base.update(delta);
                //Console.WriteLine($"{test.ToString()} - {DateTime.Now.ToString("hh:mm:ss.ffff")} - init: {initialDuration}, dur: {duration}, a: {ExplosionAlpha}, d: {delta}");

                duration -= delta;
                
                if (Duration <= 0) {
                    kill();
                }
            }
        }

        public override void kill() {
            base.kill();
            duration = 0;
        }

        public override void draw(RenderWindow window)
        {
            if (isActive) {
                csExplosion.Position = Position;
                csExplosion.FillColor = csExplosion.FillColor.setAlpha(ExplosionAlpha);
                csExplosion.OutlineColor = csExplosion.OutlineColor.setAlpha(ExplosionAlpha);
                window.Draw(csExplosion);

                drawHitbox(window);
            }
        }
        
        public void applyDamage(float delta, entity e) {
            if (isActive && e.isActive && e.health > 0) {
                e.health = Math.Max(e.health - (damage * delta), 0);
            }
        }
    }

    public abstract class shell : entity {
        public float damage = 10f;  

        internal static string fireSFX = "sound/flak_fire.wav";
        public static string FireSFX {
            get {
                return fireSFX;
            }
        }

        internal RectangleShape rsShell;
        
        public shell(Vector2f size = new Vector2f()) : base() {
            isActive = true;
            rsShell = new RectangleShape(size);
            setBoundingCircleRadius((rsShell.GetGlobalBounds().Width + rsShell.GetGlobalBounds().Height) / 2f);
        }

        public override void update(float delta) {
            base.update(delta);

            if (isActive) {
                Velocity.Y = Velocity.Y + Globals.grabity * delta;
                
                if (Position.X < -100 || Position.X > Globals.ScreenSize.X + 100 ||
                    Position.Y < -100 || Position.Y > Globals.ScreenSize.Y + 100) {
                    kill();
                }
            }
        }

        public void fire(Vector2f position, Vector2f velocity) {            
            isActive = true;
            Position = position;
            Velocity = velocity;
        }

        public virtual void applyDamage(float delta, entity e) { }

        public override void draw(RenderWindow window) {
            if (isActive) {
                if (Sprite == null) {
                    rsShell.Position = Position;
                    rsShell.Rotation = Rotation;
                    window.Draw(rsShell);
                    drawHitbox(window);
                } else {
                    base.draw(window);
                }
            }
        }
    }

    public class smallBomb : shell
    {
        public new float damage = 25f;
        public static float explosionRadius = 25f;
        public static float explosionDuration = 2f;
        public static float fireRate = 5; // 1 every 5 seconds
        
        public smallBomb(Sprite sprite) : base() {
            setSprite(sprite);
            SpriteOffsetRotation = 90f;
            sprite.Scale = new Vector2f(0.3f, 0.3f);
            setBoundingCircleRadius((sprite.GetGlobalBounds().Height + sprite.GetGlobalBounds().Width) / 3f);
        }

        public new void fire(Vector2f position, Vector2f velocity) {
            base.fire(position, velocity);
        }

        public static float explosionPitch() {
            return util.randfloat(0.45f, 0.55f);
        }
    }

    public class flak : shell
    {
        internal static new string fireSFX = "sound/flak_fire.wav";
        public static new string FireSFX {
            get {
                return fireSFX;
            }
        }

        public static float fireRate = 1f / 4f; // 4 shots per second
        public static float speed = 300f;
        
        public static float explosionRadius = 15f;
        public static float explosionDuration = 1f;
        new public float damage = 35f;
        
        internal float explodeTime;

        public flak(float explodeTime) : base(new Vector2f(10, 4)) {
            rsShell.OutlineColor = Color.Black;
            rsShell.OutlineThickness = 1f;
            rsShell.FillColor = Colour.LightYellow;

            this.explodeTime = explodeTime;
        }

        public void fire(Vector2f position, Vector2f velocity, float explodeTime) {
            base.fire(position, velocity);
            this.explodeTime = explodeTime;
            Globals.playSound(FireSFX, pitch: util.randfloat(0.95f, 1.05f));
        }
        
        public static float explosionPitch() {
            return util.randfloat(0.95f, 1.05f);
        }
    }

    public class bullet : shell
    {
        public static new string FireSFX {
            get {
                return $"sound/machine_gun{util.randint(1, 3)}.wav";
            }
        }

        public static float speed = 600f; // 600 pixels per second
        public static float fireRate = 1f / 10f;
        
        new public float damage = 2f;

        public bullet() : base(new Vector2f(4, 2)) {
            rsShell.OutlineColor = Color.Black;
            rsShell.OutlineThickness = 1f;
            rsShell.FillColor = Color.Yellow;
        }

        public override void applyDamage(float delta, entity e)
        {
            if (isActive && e.isActive && e.health > 0) {
                if (e.GetType() == typeof(enemy)) {
                    enemy en = (enemy)e;
                    if (en.lifeState == enemy.eLifeState.alive) {
                        en.health = Math.Max(en.health - damage, 0);
                    }
                } else {
                    e.health = Math.Max(e.health - damage, 0);
                }

                kill();
            }
        }

        public new void fire(Vector2f position, Vector2f velocity) {
            base.fire(position, velocity);
            Globals.playSound(bullet.FireSFX, vol: 40f, pitch: util.randfloat(0.95f, 1.05f));
        }
    }
}