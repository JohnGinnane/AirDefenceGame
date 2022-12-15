using Global;
using SFML.Graphics;
using SFML.System;

namespace ww1defence {
    public abstract class turret : entity {        
        internal float minSpread = 1f;
        internal float curSpread = 1f;
        internal float maxSpread = 45f;         // degrees

        internal float spreadDepreciation = 1f; // per second
        internal float spreadAppreciation = 4f; // per shot fired

        internal uint magazineSize = 1;
        internal float reloadTime = 3f;         // seconds
        internal float fireRate = 600;          // rounds per minute
        internal DateTime nextFire;

        public abstract shell? fire(float delta);
    }

    public class machinegun : turret
    {
        public machinegun() {
            
        }

        public override shell? fire(float delta)
        {
            if (DateTime.Now < nextFire) { return null; }
            
            nextFire = DateTime.Now.AddSeconds(60f / fireRate);

            shell? output = null;

            return output;
        }
    }
}