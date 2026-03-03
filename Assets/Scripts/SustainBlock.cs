using UnityEngine;

public class SustainBlock : MonoBehaviour
{
    public float speed = 5f;

    private bool isSaberInside = false;
    private Slicer currentSlicer;
    private bool hasNotifiedSpawner = false;
    private Collider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Slicer incomingSlicer = other.GetComponent<Slicer>();

        if (incomingSlicer != null)
        {
            if (incomingSlicer.targetTag == gameObject.tag)
            {
                isSaberInside = true;
                currentSlicer = incomingSlicer;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Slicer exitingSlicer = other.GetComponent<Slicer>();

        if (exitingSlicer != null && exitingSlicer == currentSlicer)
        {
            isSaberInside = false;
            float blokArkaZ = myCollider.bounds.max.z;
            float finishThreshold = 1.0f;

            bool isFinished = blokArkaZ < finishThreshold;

            if (isFinished)
            {
                if (currentSlicer != null)
                {
                    currentSlicer.PlayEffects(other.transform.position);

                    if (ScoreManager.Instance != null)
                        ScoreManager.Instance.AddScore(500);

                    currentSlicer.SliceTarget(this.gameObject, currentSlicer.sustainSliceForce, transform.position);
                }
            }
            else
            {
                Debug.Log("Sustain erken býrakýldý (Blok Arka Z: " + blokArkaZ + ")");
            }

            currentSlicer = null;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

        if (isSaberInside)
        {
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(1);
        }

        if (transform.position.z < -10f)
        {
            if (ScoreManager.Instance != null) ScoreManager.Instance.TakeDamage(1);
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