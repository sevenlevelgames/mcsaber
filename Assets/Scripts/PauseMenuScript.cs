using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuScript : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject pauseMenuPanel;
    public string mainMenuSceneName = "MainMenu";

    [Header("Sfx")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGameLogic();
            }
            else
            {
                PauseGameLogic();
            }
        }
    }

    public void ResumeButton()
    {
        PlayClickSound();
        ResumeGameLogic();
    }
    public void MenuButton()
    {
        PlayClickSound();
        StartCoroutine(LoadMenuWithRealtimeDelay());
    }

    void PauseGameLogic()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void ResumeGameLogic()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    IEnumerator LoadMenuWithRealtimeDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}