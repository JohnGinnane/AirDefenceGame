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
        
        // returns the duration from a range of 0 to 1
        internal float explosionAlpha {
            get {
                if (initialDuration == 0) { return 0; }
                if (Duration == 0) { return 0; }
                if (Duration > initialDuration) { return 1; }
                return Duration / initialDuration;
            }
        }

        internal CircleShape csExplosion;

        public explosion(Vector2f position, float radius, float duration, float damage) : base() {
            Position = position;
            initialDuration = duration;
            this.duration = duration;
            initialRadius = radius;
            this.radius = radius;
            Damage = damage;

            isActive = true;

            csExplosion = new CircleShape();
            csExplosion.Radius = Radius;
            csExplosion.OutlineColor = Colour.Orange;
            csExplosion.OutlineThickness = initialRadius / 3f;
            csExplosion.FillColor = Color.Red;
            csExplosion.Origin = new Vector2f(csExplosion.Radius, csExplosion.Radius);
            
            Globals.playSound(ExplodeSFX, pitch: util.randfloat(0.45f, 0.55f));
        }

        public override void update(float delta)
        {
            if (isActive) {
                base.update(delta);

                duration = duration - delta;
                csExplosion.FillColor.setAlpha((byte)(255 * explosionAlpha));

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
                window.Draw(csExplosion);
                drawHitbox(window);
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

        public abstract void applyDamage(float delta, enemy e);

        public void draw() {

        }
    }

    public class smallBomb : shell
    {
        public new float damage = 25f;        
        public static float fireRate = 1000 * 5; // 1 every 5 seconds
        
        public smallBomb(Sprite sprite) : base() {
            setSprite(sprite);
            sprite.Scale = new Vector2f(0.3f, 0.3f);
            setBoundingCircleRadius((sprite.GetGlobalBounds().Height + sprite.GetGlobalBounds().Width) / 2f);
        }

        public new void fire(Vector2f position, Vector2f velocity) {
            base.fire(position, velocity);
        }

        public override void applyDamage(float delta, enemy e)
        {
            
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

        public static float fireRate = 1000f / 4f; // 4 shots per second
        public static float speed = 300f;
        public static float explosionLifeDefault = 1f; // 1 second long explosion
        public static float explosionSizeDefault = 20f;
        new public float damage = 50f;
        
        internal DateTime explodeTime;

        public flak(DateTime explodeTime) : base(new Vector2f(10, 4)) {
            rsShell.OutlineColor = Color.Black;
            rsShell.OutlineThickness = 1f;
            rsShell.FillColor = Colour.LightYellow;

            this.explodeTime = explodeTime;
        }

        public override void update(float delta)
        {
            base.update(delta);

            if (isActive) {
                // do this outside the class
                // if (DateTime.Now >= explodeTime) {
                //     explode();
                //     return;
                // }                
            }
        }

        public override void applyDamage(float delta, enemy e)
        {
            
        }

        public void fire(Vector2f position, Vector2f velocity, DateTime explodeTime) {
            base.fire(position, velocity);
            this.explodeTime = explodeTime;
            Globals.playSound(FireSFX, pitch: util.randfloat(0.95f, 1.05f));
        }  
    }

    public class bullet : shell
    {
        public static new string FireSFX {
            get {
                return $"sound/machine_gun{util.randint(1, 3)}.wav";
            }
        }

        public static float speed = 600f;
        public static float fireRate = 1000f / 10f;
        
        new public float damage = 2f;

        public bullet() : base(new Vector2f(4, 2)) {
            rsShell.OutlineColor = Color.Black;
            rsShell.OutlineThickness = 1f;
            rsShell.FillColor = Color.Yellow;
        }

        public override void applyDamage(float delta, enemy e)
        {
            if (isActive && e.lifeState == enemy.eLifeState.alive) {
                e.health -= damage;
                kill();
            }
        }

        public new void fire(Vector2f position, Vector2f velocity) {
            base.fire(position, velocity);
            Globals.playSound(bullet.FireSFX, vol: 40f, pitch: util.randfloat(0.95f, 1.05f));
        }
    }
}