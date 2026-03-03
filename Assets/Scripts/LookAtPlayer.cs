using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform target;

    [Header("Offset")]
    public Vector3 rotationOffset;

    void Start()
    {
        if (Camera.main != null)
        {
            target = Camera.main.transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);

            transform.Rotate(rotationOffset);
        }
    }
}