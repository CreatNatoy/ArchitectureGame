using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure
{
    public class SceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) => _coroutineRunner = coroutineRunner;

        public void Load(string name, Action onLoad = null) => _coroutineRunner.StartCoroutine(LoadScene(name, onLoad));

        private IEnumerator LoadScene(string nextScene, Action onLoaded = null) {
            if (SceneManager.GetActiveScene().name == nextScene) {
                onLoaded?.Invoke();
                yield break;
            }
            
            var waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
                yield return null;
            
            onLoaded?.Invoke();
        }
    }
}