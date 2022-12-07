using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class enemy {
        public enum eLifeState {
            alive,
            dying,
            dead,
            despawning
        }

        public eLifeState lifeState;
        public float lifeStateTime;

        public Vector2f position;
        public Vector2f velocity;
        public Sprite sprite;
        public RectangleShape rsHealthCurrent;
        public RectangleShape rsHealthBackground;

        public float initialHealth = 100f;
        public float health;

        internal List<Vector2f> hitbox;
        public List<Vector2f> Hitbox {
            get {
                List<Vector2f> output = new List<Vector2f>();

                foreach (Vector2f v in hitbox) {
                    output.Add(v + position);
                }

                return output;
            }
        }

        public static bool drawHitbox = false;

        public enemy(Sprite sprite, Vector2f position, Vector2f velocity) {
            this.sprite = sprite;
            this.position = position;
            this.velocity = velocity;
            lifeState = eLifeState.alive;
            health = initialHealth;

            rsHealthBackground = new RectangleShape(new Vector2f(104, 14));
            rsHealthBackground.FillColor = Colour.Grey;

            rsHealthCurrent = new RectangleShape(new Vector2f(100, 10));
            rsHealthCurrent.FillColor = new Color(75, 230, 35);

            hitbox = util.FloatRectToVectors(sprite.GetLocalBounds());
        }

        public abstract void update(float delta);
        public abstract void draw(RenderWindow window);
    }

    public class smallBlimp : enemy {
        public smallBlimp(Sprite sprite, Vector2f position, Vector2f velocity) : base(sprite, position, velocity) {
            FloatRect spriteSize = sprite.GetLocalBounds();
            hitbox = new List<Vector2f>();
            hitbox.Add(new Vector2f(spriteSize.Width / -2f, 0f));
            hitbox.Add(new Vector2f(spriteSize.Width / -3f, spriteSize.Height / -2.5f));
            hitbox.Add(new Vector2f(spriteSize.Width /  3f, spriteSize.Height / -2.5f));
            hitbox.Add(new Vector2f(spriteSize.Width /  2f, 0f));
            hitbox.Add(new Vector2f(spriteSize.Width /  6f, spriteSize.Height /  2.5f));
            hitbox.Add(new Vector2f(spriteSize.Width / -6f, spriteSize.Height /  2.5f));
            //hitbox.Add(new Vector2f(spriteSize.Width / -2f, 0f));
        }

        public override void update(float delta)
        {
            if (health <= 0 && lifeState == eLifeState.alive) {
                lifeState = eLifeState.dying;
            }

            if (lifeState != eLifeState.dead) {
                position = position + velocity * delta;

                if (lifeState == eLifeState.dying) {
                    // remove horizonal velocity
                    // remove 5% per second
                    velocity.X = velocity.X - (velocity.X * 0.1f * delta);
                    velocity.Y = velocity.Y + 10f * delta;

                    // make the sprite tilt up or down
                    sprite.Rotation += velocity.X / 10 * delta;
                }

                if (this.position.X < sprite.TextureRect.Width * -2 || this.position.X > Globals.ScreenSize.X + sprite.TextureRect.Width * 2 ||
                    this.position.Y < -100 || this.position.Y > Globals.ScreenSize.Y + 100) {
                    lifeState = eLifeState.dead; // go straight to death off screen
                }
            }
        }
        
        public override void draw(RenderWindow window)
        {
            if (lifeState != eLifeState.dead) {
                sprite.Position = position;
                window.Draw(sprite);

                if (drawHitbox) {
                    VertexArray vaHitbox = util.VectorsToVertexArray(Hitbox,
                                                                     Colour.DarkGreen,
                                                                     Color.Transparent);
                    vaHitbox.Append(vaHitbox[0]); // wrap around back to the first vertex

                    window.Draw(vaHitbox);
                }

                if (lifeState == eLifeState.alive) {
                    rsHealthBackground.Position = position + new Vector2f(-sprite.Origin.X, sprite.TextureRect.Height + 10);
                    window.Draw(rsHealthBackground);
                    
                    if (health > 0) {
                        rsHealthCurrent.Position = position + new Vector2f(-sprite.Origin.X + 2, sprite.TextureRect.Height + 12);
                        rsHealthCurrent.Size = new Vector2f(100 * health / initialHealth, 10);
                        window.Draw(rsHealthCurrent);
                    }
                }
            }
        }

    }
}