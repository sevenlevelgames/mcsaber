using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SongSelectManager : MonoBehaviour
{
    [Header("Sfx")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void PlaySong(string name)
    {
        PlayClickSound();
        StartCoroutine(LoadSceneWithDelay(name));
    }

    public void BackToMenu()
    {
        PlayClickSound();
        StartCoroutine(LoadSceneWithDelay("MainMenu"));
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene(sceneName);
    }
}