using System.Collections;
using inkolorgames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    public void LoadNextSceneAsync() => StartCoroutine(LoadSceneAsyncCoroutine((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings));

    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
