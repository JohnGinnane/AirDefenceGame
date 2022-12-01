using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class enemy {
        public Vector2f position;
        public Vector2f velocity;
        public Sprite sprite;
        public RectangleShape rsHealthCurrent;
        public RectangleShape rsHealthBackground;

        public float initialHealth = 100f;
        public float health;

        public bool isAlive;

        public enemy(Sprite sprite, Vector2f position, Vector2f velocity) {
            this.sprite = sprite;
            this.position = position;
            this.velocity = velocity;
            isAlive = true;
            health = initialHealth;

            rsHealthBackground = new RectangleShape(new Vector2f(104, 14));
            rsHealthBackground.FillColor = Colour.Grey;

            rsHealthCurrent = new RectangleShape(new Vector2f(100, 10));
            rsHealthCurrent.FillColor = new Color(75, 230, 35);
        }

        public abstract void update(float delta);
        public abstract void draw(RenderWindow window);
    }

    public class smallBlimp : enemy {
        public smallBlimp(Sprite sprite, Vector2f position, Vector2f velocity) : base(sprite, position, velocity) {
            
        }

        public override void update(float delta)
        {
            if (health <= 0 && isAlive) {
                isAlive = false;
            }

            if (isAlive) {
                this.position = this.position + this.velocity * delta;
                    
                if (this.position.X < sprite.TextureRect.Width * -2 || this.position.X > Globals.ScreenSize.X + sprite.TextureRect.Width * 2 ||
                    this.position.Y < -100 || this.position.Y > Globals.ScreenSize.Y + 100) {
                    this.isAlive = false;
                }
            }
        }
        
        public override void draw(RenderWindow window)
        {
            if (isAlive) {
                sprite.Position = position;
                window.Draw(sprite);

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