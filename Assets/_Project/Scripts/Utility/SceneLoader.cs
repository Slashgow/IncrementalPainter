using System.Collections;
using inkolorgames;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentMonoSingleton<SceneLoader>
{
    [SerializeField] private Animator transitionAnimator;
    [SerializeField, Range(0f,3f)] private float transitionDuration = 1f;

    private readonly int END = Animator.StringToHash("end");
    private readonly int START = Animator.StringToHash("start");

    public void LoadSceneAsync(int buildIndex) => StartCoroutine(LoadSceneAsyncCoroutine(buildIndex));
    public void LoadNextSceneAsync() => StartCoroutine(LoadSceneAsyncCoroutine((SceneManager.GetActiveScene().buildIndex + 1) % 
        SceneManager.sceneCountInBuildSettings));

    public void ReloadSceneAsync() => StartCoroutine(LoadSceneAsyncCoroutine(SceneManager.GetActiveScene().buildIndex));
    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex)
    {
        transitionAnimator.SetTrigger(END);
        yield return new WaitForSecondsRealtime(transitionDuration);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        transitionAnimator.SetTrigger(START);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

      
    }
}
