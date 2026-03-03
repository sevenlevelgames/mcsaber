using UnityEngine;

public class RankSystem : MonoBehaviour
{
    public string saveKey = "PlayerLevel";

    [Header("Star Backgrounds")]
    public GameObject star1bg;
    public GameObject star2bg;
    public GameObject star3bg;

    [Header("Star Items")]
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    [Header("Crown Backgrounds")]
    public GameObject crown1bg;
    public GameObject crown2bg;
    public GameObject crown3bg;

    [Header("Crown Items")]
    public GameObject crown1;
    public GameObject crown2;
    public GameObject crown3;

    [Header("Netherite Items")]
    public GameObject netherite1;
    public GameObject netherite2;
    public GameObject netherite3;

    void Start()
    {
        int currentScore = PlayerPrefs.GetInt(saveKey, 0);

        UpdateVisuals(currentScore);
    }

    void UpdateVisuals(int score) // yýldýz veya taç gösterme
    {
        DisableAll();

        if (score <= 3)
        {
            star1bg.SetActive(true);
            star2bg.SetActive(true);
            star3bg.SetActive(true);
        }
        else
        {
            crown1bg.SetActive(true);
            crown2bg.SetActive(true);
            crown3bg.SetActive(true);
        }

        if (score >= 1 && score <= 3)
        {
            if (score >= 1) star1.SetActive(true);
            if (score >= 2) star2.SetActive(true);
            if (score >= 3) star3.SetActive(true);
        }
        else if (score >= 4 && score <= 6)
        {
            if (score >= 4) crown1.SetActive(true);
            if (score >= 5) crown2.SetActive(true);
            if (score >= 6) crown3.SetActive(true);
        }
        else if (score >= 7)
        {
            if (score >= 7) netherite1.SetActive(true);
            if (score >= 8) netherite2.SetActive(true);
            if (score >= 9) netherite3.SetActive(true);
        }
    }

    void DisableAll() // reset
    {
        star1bg.SetActive(false); star2bg.SetActive(false); star3bg.SetActive(false);
        star1.SetActive(false); star2.SetActive(false); star3.SetActive(false);

        crown1bg.SetActive(false); crown2bg.SetActive(false); crown3bg.SetActive(false);
        crown1.SetActive(false); crown2.SetActive(false); crown3.SetActive(false);

        netherite1.SetActive(false); netherite2.SetActive(false); netherite3.SetActive(false);
    }
}