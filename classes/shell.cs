using SFML.Graphics;
using SFML.System;
using Global;

namespace ww1defence {
    public abstract class shell {
        public Vector2f position;
        public Vector2f velocity;
        public bool isAlive = false;

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
    }

    public class flak : shell
    {
        public static float fireRate = 1000f / 4f; // 4 shots per second
        private RectangleShape rsFlak;
        public static float speed = 150f;

        public DateTime explodeTime;

        public flak(DateTime explodeTime) {
            rsFlak = new RectangleShape(new Vector2f(10, 4));
            rsFlak.OutlineColor = Color.Black;
            rsFlak.OutlineThickness = 1f;
            rsFlak.FillColor = Colour.LightYellow;

            this.explodeTime = explodeTime;
        }

        public override void draw(RenderWindow window)
        {
            if (isAlive) {
                rsFlak.Position = position;
                rsFlak.Rotation = (float)(Math.Atan2((double)velocity.Y, (double)velocity.X) * 180 / Math.PI);
                window.Draw(rsFlak);
            }
        }

        public override void update(float delta)
        {
            if (isAlive) {
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
            }
        }

        public void fire(Vector2f position, Vector2f velocity, DateTime explodeTime) {
            base.fire(position, velocity);
            this.explodeTime = explodeTime;
        }

        public void explode() {
            isAlive = false;
        }
    }

    public class bullet : shell
    {
        private RectangleShape rsBullet;

        public static float speed = 300f;
        public static float fireRate = 1000f / 10f;

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

        public override void draw(RenderWindow window)
        {
            if (isAlive) {
                rsBullet.Position = position;
                rsBullet.Rotation = (float)(Math.Atan2((double)velocity.Y, (double)velocity.X) * 180 / Math.PI);
                window.Draw(rsBullet);
            }
        }
    }
}