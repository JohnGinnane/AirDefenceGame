using System;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace Global {
    // Basic concept of a control which can be used to create
    // more complex controls such as buttons
    public abstract class control {
        public delegate void ClickedEventHandler(object sender, EventArgs e);
        public ClickedEventHandler? Click;

        protected FloatRect dimensions = new FloatRect(0, 0, 60, 30);
        public virtual FloatRect Dimensions {
            get { return dimensions; }
            set { dimensions = value; }
        }

        private Vector2f relPos;
        public Vector2f RelPos {
            get {
                return relPos;
            }
        }
        
        private Vector2f position;
        public virtual Vector2f Position {
            get { return position; }
            set {
                position = value;

                if (Globals.ScreenSize.X == 0) {
                    relPos.X = 0;
                } else {
                    relPos.X = position.X / Globals.ScreenSize.X;
                }

                if (Globals.ScreenSize.Y == 0) {
                    relPos.Y = 0;
                } else {
                    relPos.Y = position.Y / Globals.ScreenSize.Y;
                }

                dimensions.Left = value.X;
                dimensions.Top = value.Y;
            }
        }

        private Vector2f size;
        public virtual Vector2f Size {
            get { return size; }
            set {
                size = value;
                dimensions.Width = value.X;
                dimensions.Height = value.Y;
            }
        }

        protected float outlineThickness = 1f;
        public float OutlineThickness {
            get { return outlineThickness; }
            set { outlineThickness = value; }
        }

        // The colour of a standard button
        protected Color fillColour = Colour.None;
        public Color FillColour {
            get { return fillColour; }
            set { fillColour = value; }
        }

        protected bool mouseHovering = false;
        public bool MouseHovering => mouseHovering;

        protected bool mousePressing = false;
        public bool MousePressing => mousePressing;

        public virtual void draw(RenderWindow window) {
            RectangleShape rs = new RectangleShape();
            rs.Position = Position;
            rs.Size = Size;
            rs.OutlineColor = Color.Black;
            rs.OutlineThickness = OutlineThickness;
            rs.FillColor = FillColour;

            window.Draw(rs);
        }

        public virtual void Control_MouseMoved(object? sender, MouseMoveEventArgs? e) {
            if (sender == null || e == null) { return; }

            if (Dimensions.Contains(e.X, e.Y)) {
                mouseHovering = true;
            } else {
                mouseHovering = false;
            }
        }

        public virtual void Control_MouseButtonPressed(object? sender, MouseButtonEventArgs? e) {
            if (sender == null || e == null) { return; }
            
            if (MouseHovering) {
                if (!MousePressing) {
                    mousePressing = true;
                }
            }
        }

        public virtual void Control_MouseButtonReleased(object? sender, MouseButtonEventArgs? e) {
            if (sender == null || e == null) { return; }
            
            // only register a "click" if we started the click on this control
            if (MouseHovering && MousePressing) {
                this.Click?.Invoke(sender, e);
            }

            mousePressing = false;
        }

        public virtual void Control_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs? e) {

        }
    }
}