using SFML.Graphics;
using SFML.Window;
using Global;

namespace ww1defence {

    public abstract class scene {
        public delegate scene? SceneRequestEventHandler(object? sender, SceneRequestEventArgs? e);

        public scene? requestedBy;
        public event SceneRequestEventHandler? sceneRequestEvent;

        internal View sceneView = new View(Globals.ScreenSize/2f, Globals.ScreenSize);

        internal List<control> controls = new List<control>();

        public abstract void update(float delta);

        public abstract void draw(RenderWindow window);

#region "Events"
        public virtual void WindowResized(object? sender, SizeEventArgs? e) {
            sceneView.Center = Globals.ScreenSize/2f;
            sceneView.Size = Globals.ScreenSize;
            Console.WriteLine($"Resized window: {Globals.ScreenSize}");
        }
        
        public virtual void MouseMoved(object? sender, MouseMoveEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseMoved(sender, e);
            }
        }

        public virtual void MouseButtonPressed(object? sender, MouseButtonEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseButtonPressed(sender, e);
            }
        }

        public virtual void MouseButtonReleased(object? sender, MouseButtonEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseButtonReleased(sender, e);
            }
        }

        public virtual void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs? e) {
            foreach (control c in controls) {
                c.Control_MouseWheelScrolled(sender, e);
            }
        }

        protected virtual scene? onSceneRequested(object? sender, SceneRequestEventArgs? e) {
            if (sceneRequestEvent != null) {
                return sceneRequestEvent?.Invoke(sender, e);
            }

            return null;
        }
#endregion
    }

    public class SceneRequestEventArgs : EventArgs {
        public Type? targetScene;
        public bool unloadMe;
        
        public SceneRequestEventArgs(Type? targetScene, bool unloadMe = true) {
            if (targetScene != null) {
                if (!util.IsSameOrSubclass(typeof(scene), targetScene)) {
                    Exception ex = new Exception("Invalid scene type specified!");
                    throw ex;
                }
            }

            this.targetScene = targetScene;
            this.unloadMe = unloadMe;
        }
    }
}