using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Referansý")]
    public GameObject tutorialPanel;

    private const string PREF_KEY = "TutorialSeen_V2";

    void Awake()
    {
        int hasSeen = PlayerPrefs.GetInt(PREF_KEY, 0);

        if (hasSeen == 1)
        {
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        if (!tutorialPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.L) ||
            Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            CloseTutorial();
        }
    }

    void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        Time.timeScale = 1f;

        PlayerPrefs.SetInt(PREF_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log("Tutorial tamamlandý.");
    }

    [ContextMenu("Reset Tutorial Save")]
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(PREF_KEY);
        Debug.Log("Kayýt silindi.");
    }
}