using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sfx")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void PlayGame()
    {
        PlayClickSound();

        StartCoroutine(LoadSceneWithDelay());
    }

    public void QuitGame()
    {
        PlayClickSound();
        Application.Quit();
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("Songs");
    }
}