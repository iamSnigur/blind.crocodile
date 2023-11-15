using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;

namespace BlindCrocodile.Core
{
    public class SceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;

        public SceneLoader(ICoroutineRunner coroutineRunner) =>
            _coroutineRunner = coroutineRunner;

        public void Load(string name, Action onLoaded = null) =>
            _coroutineRunner.StartCoroutine(LoadSceneCoroutine(name, onLoaded));

        private IEnumerator LoadSceneCoroutine(string name, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name.Equals(name))
            {
                onLoaded?.Invoke();
                yield break;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(name);

            while (!waitNextScene.isDone)
                yield return null;

            onLoaded?.Invoke();
        }
    }
}
