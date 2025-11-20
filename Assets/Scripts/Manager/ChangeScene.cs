using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneName;

    public void ChangeSceneLoad()
    {
        if (sceneName != null)
        {
            SceneController.Instance.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("[Error] SceneName Null");
        }
    }
}
