using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class enemy : entity {
        public enum eLifeState {
            alive,
            dying,
            dead,
            despawning
        }

        public eLifeState lifeState;
        public float lifeStateTime;

        public DateTime lastFire;

        public enemy(Sprite sprite, Vector2f position, Vector2f velocity) : base() {
            this.sprite = sprite;
            Position = position;
            Velocity = velocity;
            lifeState = eLifeState.alive;
            health = initialHealth;
        }
    }

    public class smallBlimp : enemy {
        public smallBlimp(Sprite sprite, Vector2f position, Vector2f velocity) : base(sprite, position, velocity) {
            FloatRect spriteSize = sprite.GetLocalBounds();
            List<Vector2f> newHitbox = new List<Vector2f>();
            newHitbox.Add(new Vector2f(spriteSize.Width / -2f, 0f));
            newHitbox.Add(new Vector2f(spriteSize.Width / -3f, spriteSize.Height / -2.5f));
            newHitbox.Add(new Vector2f(spriteSize.Width /  3f, spriteSize.Height / -2.5f));
            newHitbox.Add(new Vector2f(spriteSize.Width /  2f, 0f));
            newHitbox.Add(new Vector2f(spriteSize.Width /  6f, spriteSize.Height /  2.5f));
            newHitbox.Add(new Vector2f(spriteSize.Width / -6f, spriteSize.Height /  2.5f));
            setHitbox(newHitbox);
        }
        
        public override void update(float delta)
        {
            if (health <= 0 && lifeState == eLifeState.alive) {
                lifeState = eLifeState.dying;
            }

            if (lifeState != eLifeState.dead) {
                Position = Position + Velocity * delta;

                if (lifeState == eLifeState.dying) {
                    // remove horizonal velocity
                    // remove 5% per second
                    Velocity.X = Velocity.X - (Velocity.X * 0.1f * delta);
                    Velocity.Y = Velocity.Y + Globals.grabity * delta;

                    // make the sprite tilt up or down
                    if (Sprite != null) { Sprite.Rotation += Velocity.X / 10 * delta; }
                }

                if (this.Position.X < -1000 || this.Position.X > Globals.ScreenSize.X + 1000 ||
                    this.Position.Y < -1000 || this.Position.Y > Globals.ScreenSize.Y + 1000) {
                    lifeState = eLifeState.dead; // go straight to death off screen
                }
            }
        }
        
        public override void draw(RenderWindow window)
        {
            if (lifeState != eLifeState.dead) {
                if (Sprite != null) {
                    Sprite.Position = Position;
                    window.Draw(sprite);

                    if (lifeState == eLifeState.alive) {
                        rsHealthBackground.Position = Position + new Vector2f(-Sprite.Origin.X, Sprite.TextureRect.Height + 10);
                        window.Draw(rsHealthBackground);
                        
                        if (health > 0) {
                            rsHealthCurrent.Position = Position + new Vector2f(-Sprite.Origin.X + 2, Sprite.TextureRect.Height + 12);
                            rsHealthCurrent.Size = new Vector2f(100 * health / initialHealth, 10);
                            window.Draw(rsHealthCurrent);
                        }
                    }
                }

                drawHitbox(window);
            }
        }

    }
}