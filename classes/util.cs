using Global;

using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections;

namespace Global {
    public static class util {
        private static Random getRandom() {
            return new Random((int)DateTime.Now.Ticks%Int32.MaxValue);
        }
        public static bool randbit() {
            return getRandom().Next(2) == 0;
        }
        
        public static Vector2f randvec2(float minx, float maxx, float miny, float maxy) {
            Vector2f v = new Vector2f();
            v.X = randfloat(minx, maxx);
            v.Y = randfloat(miny, maxy);
            return v;
        }

        public static Vector2f randvec2(float min, float max) {
            return randvec2(min, max, min, max);
        }

        public static int randint(int min, int max) {
            Random r = new Random((int)(DateTime.Now.Ticks%Int32.MaxValue));
            return min + (int)(Math.Round(r.NextDouble() * (max - min)));
        }

        public static float randfloat(float min, float max) {
            Random r = new Random((int)(DateTime.Now.Ticks&Int32.MaxValue));
            return min + (float)r.NextDouble() * (max - min);
        }
        
        

        public static byte randbyte()  {
            Random r = new Random((int)(DateTime.Now.Ticks%Int32.MaxValue));
            return (byte)(r.NextDouble() * 256);
        }

        public static int randsign() {
            return getRandom().Next(0, 2) * 2 - 1;
        }

        public static VertexArray rotate(VertexArray va, float angle) {
            VertexArray vaout = new VertexArray(va);
            
            for (uint i = 0; i < vaout.VertexCount; i++) {
                vaout[i] = new Vertex(rotate(vaout[i].Position, angle), vaout[i].Color);
            }
            
            return vaout;
        }

        public static Color hsvtocol(float hue, float sat, float val)
        {
            hue %= 360f;

            while(hue<0) hue += 360;

            if(sat<0f) sat = 0f;
            if(sat>1f) sat = 1f;

            if(val<0f) val = 0f;
            if(val>1f) val = 1f;

            int h = (int)(hue/60f);
            float f = hue/60-h;
            byte p = (byte)(val*(1f-sat) * 255);
            byte q = (byte)(val*(1f-sat*f) * 255);
            byte t = (byte)(val*(1f-sat*(1-f)) * 255);
            
            byte bVal = (byte)(val * 255);
            switch(h) {
                default:
                case 0:
                case 6: return new Color(bVal, t, p);
                case 1: return new Color(q, bVal, p);
                case 2: return new Color(p, bVal, t);
                case 3: return new Color(p, q, bVal);
                case 4: return new Color(t, p, bVal);
                case 5: return new Color(bVal, p, q);
            }
        }

        public static float distance(Vector2f a, Vector2f b) {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static float magnitude(Vector2f vec) {
            return (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        }

        public static Vector2f normalise(Vector2f vec) {
            return vec / magnitude(vec);
        }

        public static float dot(Vector2f a, Vector2f b) {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public static Vector2f reflect(Vector2f dir, Vector2f normal) {
            return -2f * dot(dir, normal) * normal + dir;
        }

        public static Vector2f vector2f(double angle) {
            return new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// Rotates the vector around the angle (radians)
        /// </summary>
        public static Vector2f rotate(Vector2f vector, double angle) {
            if (angle == 0) { return vector; }
            if (angle == (float)Math.PI /  2f) { return new Vector2f(-vector.Y,  vector.X); }
            if (angle == (float)Math.PI / -2f) { return new Vector2f( vector.Y, -vector.X); }
            if (angle == (float)Math.PI)       { return new Vector2f(-vector.X, -vector.Y); }

            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            return new Vector2f(vector.X * c - vector.Y * s,
                                vector.X * s + vector.Y * c);
        }

        public static double rad2deg(double angle) {
            return angle * 180 / Math.PI;
        }

        public static double deg2rad(double angle) {
            return angle * (Math.PI / 180f);
        }

        public static VertexArray VectorsToVertexArray(List<Vector2f> vectors, Color outlineColour, Color fillColour) {
            VertexArray va = new VertexArray(PrimitiveType.LineStrip, (uint)vectors.Count);

            for (uint i = 0; i < vectors.Count; i++) {
                Vertex v = new Vertex(vectors[(int)i], outlineColour);
                va[i] = v;
            }

            return va;
        }

        public static VertexArray VectorsToVertexArray(List<Vector2f> vectors, Color outlineColour) {
            return VectorsToVertexArray(vectors, outlineColour, Color.White);
        }

        public static VertexArray VectorsToVertexArray(List<Vector2f> vectors) {
            return VectorsToVertexArray(vectors, Color.White);
        }

        public static List<Vector2f> VertexArrayToVectors(VertexArray va) {
            List<Vector2f> output = new List<Vector2f>();
            
            for (uint k = 0; k < va.VertexCount; k++) {
                Vertex v = va[k];
                output.Add(v.Position);
            }

            return output;
        }

        public static FloatRect RectShapeToFloatRect(RectangleShape rs) {
            return new FloatRect(rs.Position, rs.Size);
        }

        public static List<Vector2f> FloatRectToVectors(FloatRect fr, bool wrapAroundToFirstPoint = false) {
            List<Vector2f> output = new List<Vector2f>();

            // Clockwise starting at top left corner
            output.Add(new Vector2f(fr.Left, fr.Top));
            output.Add(new Vector2f(fr.Left + fr.Width, fr.Top));
            output.Add(new Vector2f(fr.Left + fr.Width, fr.Top + fr.Height));
            output.Add(new Vector2f(fr.Left, fr.Top + fr.Height));
            
            return output;
        }

        public static Vector2f Position(FloatRect fr) {
            return new Vector2f(fr.Left, fr.Top);
        }

        public static Vector2f Size(FloatRect fr) {
            return new Vector2f(fr.Width, fr.Height);
        }

        public static FloatRect PointsToFloatRect(Vector2f a, Vector2f b) {
            FloatRect output = new FloatRect();

            if (a.X > b.X) {
                output.Left = b.X;
                output.Width = a.X - b.X;
            } else {
                output.Left = a.X;
                output.Width = b.X - a.X;
            }

            if (a.Y > b.Y) {
                output.Top = b.Y;
                output.Height = a.Y - b.Y;
            } else {
                output.Top = a.Y;
                output.Height = b.Y - a.Y;
            }

            return output;
        }

        public static VertexArray translate(VertexArray va, Vector2f offset) {
            if (offset == new Vector2f()) { return va; }
            VertexArray newVa = new VertexArray(va.PrimitiveType, va.VertexCount);

            for (uint i = 0; i < va.VertexCount; i++) {
                newVa[i] = new Vertex(va[i].Position + offset, va[i].Color);
            }

            return newVa;
        }
        
        public static Vector2f multi(Vector2f a, Vector2f b) {
            return new Vector2f(a.X * b.X, a.Y * b.Y);
        }

        public static VertexArray scale(VertexArray va, float scale) {
            if (scale == 1f) { return va; }

            VertexArray newVa = new VertexArray(va);

            for (uint i = 0; i < newVa.VertexCount; i++) {
                newVa[i] = new Vertex(va[i].Position * scale, va[i].Color, va[i].TexCoords);
            }

            return newVa;
        }

        public static Vector2f randomScreenPos() {
            return randvec2(0, Globals.ScreenSize.X, 0, Globals.ScreenSize.Y);
        }

        public static Vector2f toWorld(this Sprite spr, Vector2f localPos) {
            Vector2f output = new Vector2f();

            // rotate the localPos around spr.Rotation
            Vector2f rotatedLocalPos = rotate(localPos, deg2rad(spr.Rotation));
            output = spr.Position + rotatedLocalPos;

            return output;
        }

        public static Color setAlpha(this Color C, byte A) {
            return new Color(C.R, C.G, C.B, A);
        }

        public static Color setRed(this Color C, byte R) {
            return new Color(R, C.G, C.B, C.A);
        }

        public static Color setGreen(this Color C, byte G) {
            return new Color(C.R, G, C.B, C.A);
        }

        public static Color setBlue(this Color C, byte B) {
            return new Color(C.R, C.G, B, C.A);
        }

        public static void Use<T>(this T item, Action<T> work) {
            work(item);
        }

        public static List<T> AllOf<T>(this IEnumerable list) {
            List<T> output = new List<T>();

            foreach (object o in list) {
                if (o is T) { output.Add((T)o); }
            }

            return output;
        }

        // https://stackoverflow.com/a/17190236
        public static bool IsList(object o) {
            if (o == null) { return false; }
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static Type? GetListGeneric(object o) {
            if (!IsList(o)) { return null; }
            Type[] args = o.GetType().GetGenericTypeDefinition().GetGenericArguments();
            if (args.Count() > 0) { return args[0]; }

            return null;
        }

        // https://stackoverflow.com/a/2742288
        public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                || potentialDescendant == potentialBase;
        }
    } // end class
} // end namespace

