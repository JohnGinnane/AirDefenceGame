using SFML.Graphics;
using SFML.Window;
using SFML.System;
using Global;

namespace ww1defence {

    public abstract class scene {
        public delegate scene? SceneRequestEventHandler(object? sender, SceneRequestEventArgs? e);

        public scene? requestedBy;
        public event SceneRequestEventHandler? sceneRequestEvent;

        internal View sceneView = new View(Globals.ScreenSize/2f, Globals.ScreenSize);
        internal Vector2f lastViewPos;
        //internal Vector2f cameraPos;

        public float SceneZoom {
            get {
                return sceneView.Size.X / Globals.ScreenSize.X;
            }
        }


        internal List<control> controls = new List<control>();

        internal View? mouseLockedToView;

        public virtual void update(float delta) {


            if (Input.Mouse["middle"].justPressed && mouseLockedToView == null) {
                Console.WriteLine("moving camera");
                mouseLockedToView = sceneView;
                lastViewPos = sceneView.Center + (Vector2f)Input.Mouse.Position * SceneZoom;
            }

            if (Input.Mouse["middle"].isPressed && mouseLockedToView != null) {
                sceneView.Center = lastViewPos - (Vector2f)Input.Mouse.Position * SceneZoom;
            }            

            if (Input.Mouse["middle"].justReleased && mouseLockedToView != null) {
                Console.WriteLine("stopped moving camera");
                mouseLockedToView = null;
            }
        }

        public abstract void draw(RenderWindow window);

#region "Events"
        public virtual void WindowResized(object? sender, SizeEventArgs? e) {
            // sceneView.Center = Globals.ScreenSize/2f;
            // sceneView.Size = Globals.ScreenSize;
            // Console.WriteLine($"Resized window: {Globals.ScreenSize}");            
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