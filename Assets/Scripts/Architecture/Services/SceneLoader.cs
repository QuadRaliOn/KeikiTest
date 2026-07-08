using System;
using System.Collections;
using Architecture.Boot;
using UnityEngine.SceneManagement;
using AsyncOperation = UnityEngine.AsyncOperation;

namespace Architecture.Services
{
    public class SceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void LoadScene(Scenes name, Action onLoaded = null) =>
            _coroutineRunner.StartCoroutine(Load((int)name, onLoaded));

        private IEnumerator Load(int nextScene, Action onLoaded)
        {
            if (SceneManager.GetActiveScene().buildIndex == nextScene)
            {
                onLoaded?.Invoke();
                yield break;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}