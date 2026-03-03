using UnityEngine;
using System.Collections.Generic;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Settings")]
    public float beatTempo = 10f;

    [Header("Decoration List")]
    public List<DecorationEntry> decorationMap = new List<DecorationEntry>();

    private int currentIndex = 0;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (currentIndex < decorationMap.Count)
        {
            if (timer >= decorationMap[currentIndex].spawnTime)
            {
                SpawnDecoration(decorationMap[currentIndex]);
                currentIndex++;
            }
        }
    }

    void SpawnDecoration(DecorationEntry data)
    {
        if (data.prefab == null) return;

        Vector3 spawnPos = transform.position + data.spawnOffset;

        GameObject newDeco = Instantiate(data.prefab, spawnPos, Quaternion.Euler(data.rotation));

        newDeco.transform.localScale = Vector3.one * data.scale;

        var mover = newDeco.GetComponent<LinearMover>();
        if (mover != null)
        {
            mover.moveSpeed = beatTempo;
        }
    }
}