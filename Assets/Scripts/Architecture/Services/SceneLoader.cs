using System;
using System.Collections;
using Architecture.Boot;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Architecture.Services {
    public class SceneLoader : ISceneLoader {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISceneTransitionService _transitionService;

        public SceneLoader(ICoroutineRunner coroutineRunner, ISceneTransitionService transitionService) {
            _coroutineRunner = coroutineRunner;
            _transitionService = transitionService;
        }

        public void LoadScene(Scenes name, Action onLoaded = null) =>
            _coroutineRunner.StartCoroutine(Load((int)name, onLoaded));

        private IEnumerator Load(int nextScene, Action onLoaded) {
            bool isSameScene = SceneManager.GetActiveScene().buildIndex == nextScene;

            if (!isSameScene)
                yield return _transitionService.FadeOut();

            if (!isSameScene) {
                AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);
                while (!waitNextScene.isDone)
                    yield return null;
            }

            onLoaded?.Invoke();

            if (!isSameScene)
                yield return _transitionService.FadeIn();
        }
    }
}