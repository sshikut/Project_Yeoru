using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameUIState
{
    None,
    Inventory,
    Dialogue,
    Settings,
    Pause
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int pauseCounter = 0; // 여러 UI가 겹치는 경우 처리

    public bool isPause = false;

    public GameUIState CurrentUIState { get; private set; } = GameUIState.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PauseGame()
    {
        isPause = true;
        pauseCounter++;
        // Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPause = false;
        pauseCounter = Mathf.Max(0, pauseCounter - 1);
        // if (pauseCounter == 0)
            // Time.timeScale = 1f;
    }

    public bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }

    public void OpenUI(GameUIState state)
    {
        CurrentUIState = state;
    }

    public void CloseUI()
    {
        CurrentUIState = GameUIState.None;
    }

    public bool IsUIOpen()
    {
        return CurrentUIState != GameUIState.None;
    }
}
