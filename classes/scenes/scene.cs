using SFML.Graphics;
using SFML.Window;
using Global;

namespace ww1defence {

    public abstract class scene {
        public delegate void SceneRequestEventHandler(object? sender, SceneRequestEventArgs? e);
        public event SceneRequestEventHandler? sceneRequestEvent;

        public abstract void update(float delta);

        public abstract void draw(RenderWindow window);

#region "Events"
        public abstract void MouseMoved(object? sender, MouseMoveEventArgs? e);
        public abstract void MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs? e);
        public abstract void MouseButtonPressed(object? sender, MouseButtonEventArgs? e);
        public abstract void MouseButtonReleased(object? sender, MouseButtonEventArgs? e);

        protected virtual void onSceneRequested(object? sender, SceneRequestEventArgs? e) {
            if (sceneRequestEvent != null) {
                sceneRequestEvent?.Invoke(sender, e);
            }
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