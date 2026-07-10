using System.Collections;

namespace Architecture.Services {
    public interface ISceneTransitionService {
        IEnumerator FadeOut();
        IEnumerator FadeIn();
    }
}
