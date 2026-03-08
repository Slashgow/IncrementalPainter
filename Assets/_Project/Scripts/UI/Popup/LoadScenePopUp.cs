using System;
using UnityEngine;

public class LoadScenePopUp : PopUp
{
    [SerializeField, Range(0, 3)] private int sceneIndex;

    protected override void OnClickDoActionButton()
    {
        SceneLoader.Instance.LoadSceneAsync(sceneIndex);
        base.OnClickDoActionButton();
    }
}
