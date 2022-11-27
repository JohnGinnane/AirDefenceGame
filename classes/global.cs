using SFML.Graphics;
using SFML.System;

namespace Global {
    public static class Globals {            
        private static Vector2f screenSize;
        public static Vector2f ScreenSize {
            get { return screenSize; }
            set { screenSize = value; }
        }
    }
}