using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public float speed = 5f;

    private bool hasNotifiedSpawner = false;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (transform.position.z < -5f)
        {
            Debug.Log("missed");

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Spawner.Instance == null) return;

        if (!hasNotifiedSpawner)
        {
            hasNotifiedSpawner = true;
            Spawner.Instance.OnBlockDestroyed();
        }
    }
}