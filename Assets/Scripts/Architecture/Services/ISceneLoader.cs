using System;

namespace Architecture.Services {
    public interface ISceneLoader {
        void LoadScene(Scenes name, Action onLoaded = null);
    }
}
