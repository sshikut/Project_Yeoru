using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    public GameObject settingUI;

    private void Start()
    {
        SoundManager.Instance.PlayBGM("Port city of Balora");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void OpenSettingUI()
    {
        settingUI.SetActive(true);
    }

    public void CloseSettingUI()
    {
        settingUI.SetActive(false);
    }
}
