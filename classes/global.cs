using SFML.Graphics;
using SFML.System;
using SFML.Audio;

namespace Global {
    public static class Globals {
        public static Dictionary<string, SoundBuffer> Buffers = new Dictionary<string, SoundBuffer>();
        public static List<Sound> Sounds = new List<Sound>();
        private static Vector2f screenSize;
        public static Vector2f ScreenSize {
            get { return screenSize; }
            set { screenSize = value; }
        }

        public static float masterVolumeMulti = 0.5f;

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