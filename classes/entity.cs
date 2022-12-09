using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class entity {
        public Vector2f Position;
        public Vector2f Velocity;
        public Double Rotation;
        public Double SpriteOffsetRotation;
        
        internal Sprite? sprite;
        public Sprite? Sprite {
            get { return sprite; }
        }

        public bool isActive;

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
            sprite = new Sprite();

            Position = new Vector2f();
            Velocity = new Vector2f();

            // if we ever draw hitbox then draw this cirlc
            csBoundingCircle.FillColor = Color.Transparent;
            csBoundingCircle.OutlineColor = Color.Green;
            csBoundingCircle.OutlineThickness = 1f;
        }

        public virtual void update(float delta) {
            Position = Position + (Velocity * delta);
        }

        internal virtual void drawHitbox(RenderWindow window) {
            if (isActive && Globals.DrawHitbox) {
                if (Hitbox.Count == 0) {
                    window.Draw(BoundingCircle);
                } else {
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
                    Sprite.Rotation = (float)SpriteOffsetRotation + (float)(Math.Atan2((double)Velocity.Y, (double)Velocity.X) * 180 / Math.PI);
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
            if (!fastIntersection(A, B)) { return false; }

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