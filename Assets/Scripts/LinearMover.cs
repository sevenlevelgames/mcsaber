using UnityEngine;

public class LinearMover : MonoBehaviour
{
    [HideInInspector]
    public float moveSpeed;

    private float lifeTime = 15f;

    void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public class DecorationEntry
{
    public string name;
    public float spawnTime;
    public GameObject prefab;
    public Vector3 spawnOffset;
    public Vector3 rotation;
    public float scale = 1f;
}