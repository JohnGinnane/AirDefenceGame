using SFML.Graphics;
using SFML.System;
using SFML.Audio;

namespace Global {
    public static class Globals {
        private static Texture? textureSpritesheet;
        
        public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
        public static Dictionary<string, SoundBuffer> Buffers = new Dictionary<string, SoundBuffer>();
        public static List<Sound> Sounds = new List<Sound>();
        private static Vector2f screenSize;
        public static Vector2f ScreenSize {
            get { return screenSize; }
            set { screenSize = value; }
        }

        public static bool DrawHitbox = false;
        public static float grabity = 10f;
        public static float masterVolumeMulti = 0.5f;

        public static void loadTextureSheet(string filepath) {
            textureSpritesheet = new Texture(filepath);
        }

        public static Sprite copySprite(string name) {
            Sprite spr = getSprite(name);
            Sprite sprReturn = new Sprite(spr.Texture, spr.TextureRect);

            sprReturn.Origin = spr.Origin;
            sprReturn.Position = spr.Position;
            sprReturn.Rotation = spr.Rotation;
            sprReturn.Scale = spr.Scale;
            sprReturn.Color = spr.Color;

            return sprReturn;
        }

        public static Sprite copySprite(Sprite spr) {
            Sprite sprReturn = new Sprite(spr.Texture, spr.TextureRect);

            sprReturn.Origin = spr.Origin;
            sprReturn.Position = spr.Position;
            sprReturn.Rotation = spr.Rotation;
            sprReturn.Scale = spr.Scale;
            sprReturn.Color = spr.Color;

            return sprReturn;
        }

        public static Sprite createSprite(string name,
                                        IntRect rectangle,
                                        Vector2f origin = new Vector2f()) {
            if (textureSpritesheet == null) {
                Exception ex = new Exception("Texture sprite sheet has not been initialised!");
                throw ex;
            }

            Sprite newSprite = new Sprite(textureSpritesheet, rectangle);
            if (origin == new Vector2f()) {
                origin = new Vector2f(rectangle.Width / 2f, rectangle.Height / 2f);
            }

            newSprite.Origin = origin;

            Sprites.Add(name, newSprite);

            return newSprite;
        }

        public static Sprite getSprite(string name, bool createCopy = false) {
            Exception ex = new Exception($"Unspecified error");

            if (Sprites.ContainsKey(name)) {
                if (Sprites[name] != null) {
                    Sprite spr = Sprites[name];

                    if (createCopy) {
                        return copySprite(spr);
                    } else {
                        return spr;
                    }
                } else {
                    ex = new Exception($"Sprite {name} is not defined!");
                }
            } else {
                ex = new Exception($"Sprite '{name}' not found in list!");
            }
            
            throw ex;
        }

        public static void playSound(string path, float vol = 100f, float pitch = 1f) {
            SoundBuffer? useBuffer = null;

            // Try to find the buffer
            if (!Buffers.ContainsKey(path) ) {
                useBuffer = new SoundBuffer(path);
                Buffers.Add(path, useBuffer);
            } else {
                useBuffer = Buffers[path];
            }

            // for look spare sounds
            Sound? useSound = Sounds.Find((x) => x.Status == SoundStatus.Stopped);

            if (useSound == null) {
                useSound = new Sound(useBuffer);
                Sounds.Add(useSound);
            } else {
                useSound.SoundBuffer = useBuffer;
            }

            useSound.Pitch = pitch;
            useSound.Volume = vol * masterVolumeMulti;
            useSound.Play();
        }
    }
}