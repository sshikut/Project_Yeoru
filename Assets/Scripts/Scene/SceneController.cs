using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("UI 설정")]
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadRoutine(sceneName));
    }

    private IEnumerator FadeAndLoadRoutine(string sceneName)
    {
        fadePanel.blocksRaycasts = true;
        GameManager.Instance.isInputActive = false;
        yield return StartCoroutine(Fade(1f)); 

        // 2. 씬 로딩
        // (비동기 로딩을 쓰면 로딩 게이지도 만들 수 있음)
        yield return SceneManager.LoadSceneAsync(sceneName);
        GameManager.Instance.isInputActive = true;

        // 3. 페이드 인 (검은색 -> 투명)
        yield return StartCoroutine(Fade(0f)); // Alpha를 0으로
        fadePanel.blocksRaycasts = false; // 조작 허용
        
    }

    private IEnumerator Fade(float finalAlpha)
    {
        float startAlpha = fadePanel.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            // Lerp를 이용해 부드럽게 투명도 조절
            fadePanel.alpha = Mathf.Lerp(startAlpha, finalAlpha, time / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = finalAlpha;
    }
}