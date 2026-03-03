using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float moveSpeed = 10f;

    void Update()
    {
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        if (transform.position.z < -20f)
        {
            Destroy(gameObject);
        }
    }
}