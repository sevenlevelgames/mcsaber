using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SongSelection
{
    Sweden,
    SubwooferLullaby,
    DryHands,
    Thumbnail
}

public class Spawner : MonoBehaviour
{
    // Singleton
    public static Spawner Instance;

    [Header("Sustain Setting")]
    [Range(0.1f, 2.0f)]
    public float sustainLengthMultiplier = 0.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Save Settings")]
    public string levelID = "SongName_1";

    [Header("Normal Cubes")]
    public GameObject normalRedPrefab;
    public GameObject normalBluePrefab;

    [Header("Sustain Cubes")]
    public GameObject sustainRedPrefab;
    public GameObject sustainBluePrefab;

    [Header("Settings")]
    public float beatTempo = 10f; // Temel hız
    public List<BlockData> levelMap = new List<BlockData>();

    [Header("Transition Settings")]
    public float phaseWaitDuration = 0f;

    [Header("Phaser Settings")]
    private float[] phaseMultipliers = { 1.0f, 1.5f, 2.0f }; // Faz 1, Faz 2, Faz 3 hızları
    private int currentPhase = 0;

    [Header("Phase 1 UI")]
    public GameObject[] phase1Slots;
    public GameObject[] phase1Stars;

    [Header("Phase 2 UI")]
    public GameObject[] phase2Slots;
    public GameObject[] phase2Crowns;

    [Header("Phase 3 UI")]
    public GameObject[] phase3Crowns;

    private int currentBlockIndex = 0;
    private int handledBlockCount = 0;
    private float timer;
    private bool isLevelFinished = false;
    private bool isTransitioning = false;
    private int totalBlocksInLevel = 0;

    [Header("Song Selection")]
    public SongSelection selectedSong;

    void Start()
    {
        levelMap.Clear();

        switch (selectedSong)
        {
            case SongSelection.Sweden:
                LoadC418Level();
                break;
            case SongSelection.SubwooferLullaby:
                LoadSubwooferLevel();
                break;
            case SongSelection.DryHands:
                LoadDryHandsLevel();
                break;
            case SongSelection.Thumbnail: // <--- BURAYI EKLE
                LoadThumbnailLevel();
                break;
        }

        totalBlocksInLevel = levelMap.Count;
        if (totalBlocksInLevel == 0) totalBlocksInLevel = 1;

        currentPhase = 0;
        SetupPhase(0);
    }

    void Update()
    {
        if (isLevelFinished || isTransitioning) return;

        float currentMultiplier = phaseMultipliers[currentPhase];
        timer += Time.deltaTime * currentMultiplier;

        while (currentBlockIndex < levelMap.Count && timer >= levelMap[currentBlockIndex].spawnTime)
        {
            SpawnBlock(levelMap[currentBlockIndex], currentMultiplier);
            currentBlockIndex++;
        }
    }

    public void OnBlockDestroyed()
    {
        if (isTransitioning || isLevelFinished) return;

        handledBlockCount++;

        float progress = (float)handledBlockCount / (float)totalBlocksInLevel;
        UpdatePhaseUI(progress);

        if (handledBlockCount >= totalBlocksInLevel)
        {
            StartCoroutine(PhaseTransitionRoutine());
        }
    }

    public void PlayerFailed()
    {
        if (isLevelFinished) return;

        Debug.Log("Oyuncu Yandı! Skor kaydediliyor...");
        CalculateAndSaveProgress();

        isLevelFinished = true;
        if (ScoreManager.Instance != null) ScoreManager.Instance.EndGame(false);
    }

    void FinishLevel()
    {
        Debug.Log("Level Başarıyla Bitti! Skor kaydediliyor...");
        CalculateAndSaveProgress();

        isLevelFinished = true;
        isTransitioning = false;

        if (ScoreManager.Instance != null) ScoreManager.Instance.EndGame(true);
    }

    void CalculateAndSaveProgress()
    {
        int baseScore = currentPhase * 3;
        int currentPhaseScore = 0;
        float progress = 0;

        if (totalBlocksInLevel > 0)
            progress = (float)handledBlockCount / (float)totalBlocksInLevel;

        if (progress >= 0.99f) currentPhaseScore = 3;
        else if (progress >= 0.66f) currentPhaseScore = 2;
        else if (progress >= 0.33f) currentPhaseScore = 1;

        int finalScore = baseScore + currentPhaseScore;
        string saveKey = levelID;
        int oldScore = PlayerPrefs.GetInt(saveKey, 0);

        if (finalScore > oldScore)
        {
            PlayerPrefs.SetInt(saveKey, finalScore);
            PlayerPrefs.Save();
            Debug.Log($"Yeni Rekor! {levelID} Skor: {finalScore}");
        }
    }

    IEnumerator PhaseTransitionRoutine()
    {
        isTransitioning = true;
        UpdatePhaseUI(1.0f);

        Debug.Log("Faz geçişi bekleniyor...");
        yield return new WaitForSeconds(phaseWaitDuration);

        currentPhase++;

        if (currentPhase > 2)
        {
            FinishLevel();
        }
        else
        {
            currentBlockIndex = 0;
            handledBlockCount = 0;
            timer = 0;
            SetupPhase(currentPhase);
            isTransitioning = false;
        }
    }

    void SpawnBlock(BlockData data, float multiplier)
    {
        float xPos = (data.lane == CubeLane.Left) ? -1f : 1f;

        float currentSpeed = beatTempo * multiplier;

        float timeLate = timer - data.spawnTime;

        float zOffset = timeLate * currentSpeed;

        Vector3 spawnPos = new Vector3(xPos, transform.position.y, transform.position.z + zOffset);

        GameObject prefabToUse = null;

        if (data.type == BlockType.Normal)
            prefabToUse = (data.color == CubeColor.Red) ? normalRedPrefab : normalBluePrefab;
        else if (data.type == BlockType.Sustain)
            prefabToUse = (data.color == CubeColor.Red) ? sustainRedPrefab : sustainBluePrefab;

        if (prefabToUse != null)
        {
            GameObject newBlock = Instantiate(prefabToUse, spawnPos, Quaternion.Euler(data.rotation));

            if (data.type == BlockType.Sustain)
            {
                float rawLength = beatTempo * data.sustainDuration;
                float finalLength = rawLength * sustainLengthMultiplier;

                Vector3 currentScale = newBlock.transform.localScale;
                newBlock.transform.localScale = new Vector3(currentScale.x, currentScale.y, finalLength);

                newBlock.transform.position += Vector3.forward * (finalLength / 2f);
            }

            var mover = newBlock.GetComponent<CubeMovement>();
            if (mover != null) mover.moveSpeed = currentSpeed;
        }
    }

    void SetupPhase(int phaseIndex)
    {
        ResetFilledObjects();
        if (phaseIndex == 0)
        {
            SetGroupActive(phase1Slots, true);
            SetGroupActive(phase2Slots, false);
        }
        else if (phaseIndex == 1)
        {
            SetGroupActive(phase1Slots, false);
            SetGroupActive(phase2Slots, true);
        }
    }

    void UpdatePhaseUI(float progress)
    {
        GameObject[] targetList = null;
        if (currentPhase == 0) targetList = phase1Stars;
        else if (currentPhase == 1) targetList = phase2Crowns;
        else if (currentPhase == 2) targetList = phase3Crowns;

        if (targetList != null)
        {
            if (progress >= 0.33f && !targetList[0].activeSelf) targetList[0].SetActive(true);
            if (progress >= 0.66f && !targetList[1].activeSelf) targetList[1].SetActive(true);
            if (progress >= 0.99f && !targetList[2].activeSelf) targetList[2].SetActive(true);
        }
    }

    void SetGroupActive(GameObject[] group, bool isActive)
    {
        foreach (var obj in group) if (obj != null) obj.SetActive(isActive);
    }

    void ResetFilledObjects()
    {
        SetGroupActive(phase1Stars, false);
        SetGroupActive(phase2Crowns, false);
        SetGroupActive(phase3Crowns, false);
    }

    void LoadC418Level()
    {
        float loopDuration = 9.5f;
        for (int i = 0; i < 3; i++)
        {
            float offset = i * loopDuration;
            AddNote(offset + 1.0f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 1.5f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 2.0f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 2.5f, CubeLane.Left, CubeColor.Red, BlockType.Sustain, 0.5f);
            AddNote(offset + 3.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 4.0f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 4.5f, CubeLane.Right, CubeColor.Blue, BlockType.Sustain, 0.4f);
            AddNote(offset + 6.0f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 6.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 7.2f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 7.8f, CubeLane.Right, CubeColor.Blue, BlockType.Sustain, 0.6f);
            AddNote(offset + 9.0f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 9.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 10.0f, CubeLane.Left, CubeColor.Red, BlockType.Sustain, 0.3f);
        }
    }

    void LoadSubwooferLevel()
    {
        float loopDuration = 6.5f;
        for (int i = 0; i < 3; i++)
        {
            float offset = i * loopDuration;
            AddNote(offset + 0.5f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 1.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 2.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 2.8f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 3.1f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 4.0f, CubeLane.Left, CubeColor.Red, BlockType.Sustain, 1.2f);
            AddNote(offset + 4.0f, CubeLane.Right, CubeColor.Blue, BlockType.Sustain, 1.2f);
            AddNote(offset + 6.0f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 6.5f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
        }
    }

    void LoadDryHandsLevel()
    {
        float loopDuration = 4.0f;
        for (int i = 0; i < 3; i++)
        {
            float offset = i * loopDuration;
            AddNote(offset + 0.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 0.8f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 1.1f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 1.5f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 2.5f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
            AddNote(offset + 2.8f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
            AddNote(offset + 3.5f, CubeLane.Right, CubeColor.Blue, BlockType.Sustain, 0.4f);
            AddNote(offset + 4.2f, CubeLane.Left, CubeColor.Red, BlockType.Normal, 0f);
        }
    }

    void LoadThumbnailLevel()
    {
        AddNote(0.9f, CubeLane.Left, CubeColor.Red, BlockType.Sustain, 3.0f);

        AddNote(1.0f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
        AddNote(1.5f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
        AddNote(2.0f, CubeLane.Right, CubeColor.Blue, BlockType.Normal, 0f);
    }

    void AddNote(float time, CubeLane lane, CubeColor color, BlockType type, float duration)
    {
        BlockData note = new BlockData();
        note.spawnTime = time;
        note.lane = lane;
        note.color = color;
        note.type = type;
        note.sustainDuration = duration;
        note.rotation = Vector3.zero;
        levelMap.Add(note);
    }
}