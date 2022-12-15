using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class player : entity {
        
        public override void kill() {
            base.kill();
            // lose the game
        }

        public abstract void handleInputs();
    }
}