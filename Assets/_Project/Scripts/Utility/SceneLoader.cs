using System.Collections;
using inkolorgames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    public void LoadSceneAsync(int buildIndex) => StartCoroutine(LoadSceneAsyncCoroutine(buildIndex));
    public void LoadNextSceneAsync() => StartCoroutine(LoadSceneAsyncCoroutine((SceneManager.GetActiveScene().buildIndex + 1) % 
        SceneManager.sceneCountInBuildSettings));

    public void ReloadSceneAsync() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);// StartCoroutine(LoadSceneAsyncCoroutine(SceneManager.GetActiveScene().buildIndex));
    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
