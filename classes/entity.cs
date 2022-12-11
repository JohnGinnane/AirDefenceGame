using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class entity {
        public Vector2f Position;
        public Vector2f Velocity;
        public float Rotation;
        public float SpriteOffsetRotation;

        public enum enumFaction {
            friend,
            enemy,
            ally,
            neutral
        }
        
        internal Sprite? sprite;
        public Sprite? Sprite {
            get { return sprite; }
        }

        public bool isActive;

        public float initialHealth = 100f;
        public float health;

        public RectangleShape rsHealthCurrent;
        public RectangleShape rsHealthBackground;

        // Used for fast collision checks
        internal CircleShape csBoundingCircle;
        public CircleShape BoundingCircle {
            get {
                csBoundingCircle.Position = Position;
                return csBoundingCircle;
            }
        }

        // Used for more complex shapes
        // Effectively overrides BoundingCircle
        private List<Vector2f> hitbox;
        public List<Vector2f> Hitbox {
            get {
                List<Vector2f> output = new List<Vector2f>();

                foreach (Vector2f v in hitbox) {
                    output.Add(Position + util.rotate(v, Rotation));
                }

                return output;
            }
        }

        public entity() {
            hitbox = new List<Vector2f>();            
            csBoundingCircle = new CircleShape();
            sprite = null;

            Position = new Vector2f();
            Velocity = new Vector2f();

            // if we ever draw hitbox then draw this circle
            csBoundingCircle.FillColor = Color.Transparent;
            csBoundingCircle.OutlineColor = Color.Green;
            csBoundingCircle.OutlineThickness = 1f;
            
            rsHealthBackground = new RectangleShape(new Vector2f(104, 14));
            rsHealthBackground.FillColor = Colour.Grey;

            rsHealthCurrent = new RectangleShape(new Vector2f(100, 10));
            rsHealthCurrent.FillColor = new Color(75, 230, 35);
        }

        public virtual void update(float delta) {
            Position = Position + (Velocity * delta);
            Rotation = SpriteOffsetRotation + (float)(Math.Atan2((double)Velocity.Y, (double)Velocity.X) * 180 / Math.PI);
        }

        internal virtual void drawHitbox(RenderWindow window) {
            if (isActive && Globals.DrawHitbox) {
                window.Draw(BoundingCircle);

                if (Hitbox.Count > 0) {
                    VertexArray va = util.VectorsToVertexArray(Hitbox, Colour.DarkGreen, Color.Transparent);
                    va.Append(va[0]); // wrap around back to first point
                    window.Draw(va);                    
                }
            }
        }

        public virtual void draw(RenderWindow window) {
            if (isActive) {
                if (Sprite != null) { 
                    Sprite.Position = Position;
                    Sprite.Rotation = Rotation;
                    window.Draw(Sprite);
                }

                drawHitbox(window);
            }
        }

        public virtual void kill() { isActive = false; }

        internal void setBoundingCircleRadius(float radius) {
            csBoundingCircle.Radius = radius;
            csBoundingCircle.Origin = new Vector2f(radius, radius);
        }

        public void setHitbox(List<Vector2f> points) {
            hitbox = points;

            // find furthest point and then use that as bounding circle
            float farthest = 0;
            foreach (Vector2f v in hitbox) {
                if (util.distance(new Vector2f(), v) > farthest) {
                    farthest = util.distance(new Vector2f(), v);
                }
            }
            setBoundingCircleRadius(farthest);
        }

        public void setSprite(Sprite newSprite, bool rebuildHitbox = false) {
            sprite = newSprite;

            if (rebuildHitbox && Sprite != null) {
                hitbox = util.FloatRectToVectors(Sprite.GetLocalBounds());
            }
        }

        public static bool fastIntersection(entity A, entity B) {
            if (A == null) { return false; }
            if (B == null) { return false; }
            if (!A.isActive) { return false; }
            if (!B.isActive) { return false; }

            if (intersection.circleInsideCircle(A.BoundingCircle, B.BoundingCircle)) {
                return true;
            }

            return false;
        }

        public static bool collision(entity A, entity B) {
            if (A == null) { return false; }
            if (B == null) { return false; }

            // The bounding circle should always cover all hitbox points
            bool fi = fastIntersection(A, B);
            if (A.Hitbox.Count == 0 && B.Hitbox.Count == 0) {
                return fi;
            }

            if (!fi) { return false; }

            // if one entity DOES have a hitbox then do circle to polygon
            // Make sure A always has the hitbox and B doesn't
            if (A.Hitbox.Count == 0 && B.Hitbox.Count > 0) {
                entity C = A;
                A = B;
                B = C;
            }

            if (A.Hitbox.Count > 0 && B.Hitbox.Count == 0) {
                return intersection.circleInsidePolygon(B.Position, B.BoundingCircle.Radius, A.Hitbox);
            }

            // Finally if BOTH entities DO have a hitbox then do polygon to polygon
            if (A.Hitbox.Count > 0 && B.Hitbox.Count > 0) {
                return intersection.polygonInsidePolygon(A.Hitbox, B.Hitbox);
            }

            return false;
        }
    }
}