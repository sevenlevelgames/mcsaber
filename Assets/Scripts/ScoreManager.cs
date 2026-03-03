using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreTextUI;
    public GameObject[] hearts;

    [Header("Outro")]
    public GameObject outroPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalLivesText;

    [Header("Sfx")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private int currentScore = 0;
    private int currentLives = 5;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        if (gameEnded) return;

        currentScore += amount;
        scoreTextUI.text = "Score: " + currentScore.ToString();
    }

    public void TakeDamage(int amount)
    {
        if (gameEnded) return;

        currentLives -= amount;

        if (currentLives >= 0 && currentLives < hearts.Length)
        {
            hearts[currentLives].SetActive(false);
        }

        if (currentLives <= 0)
        {
            EndGame(false);

            if (Spawner.Instance != null)
            {
                Spawner.Instance.PlayerFailed();
            }
        }
    }

    public void EndGame(bool isWin)
    {
        if (gameEnded) return;
        gameEnded = true;

        if (outroPanel != null)
            outroPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Total Score: " + currentScore;

        if (finalLivesText != null)
            finalLivesText.text = "Lives Left: " + Mathf.Max(0, currentLives);

        if (titleText != null)
        {
            if (isWin)
            {
                titleText.text = "LEVEL COMPLETED!";
                titleText.color = Color.green;
            }
            else
            {
                titleText.text = "GAME OVER";
                titleText.color = Color.red;
            }
        }

        Time.timeScale = 0;
    }

    public void OnRestartClicked()
    {
        StartCoroutine(PlaySoundAndLoadScene(true));
    }

    public void OnMainMenuClicked()
    {
        StartCoroutine(PlaySoundAndLoadScene(false));
    }

    private IEnumerator PlaySoundAndLoadScene(bool isRestart)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        Time.timeScale = 1;

        if (isRestart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}