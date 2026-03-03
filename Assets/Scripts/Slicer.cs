using UnityEngine;
using EzySlice;

public class Slicer : MonoBehaviour
{
    [Header("Settings")]
    public Material crossSectionMaterial;
    public LayerMask sliceableLayer;
    public string targetTag;

    [Header("Speed Settings (Anti-Cheat)")]
    public float minSliceVelocity = 2.0f;
    public float Velocity { get; private set; }
    private Vector3 _lastPos;

    [Header("Slice Powers")]
    public float normalSliceForce = 1000f;
    public float sustainSliceForce = 2000f;

    [Header("Effects")]
    public GameObject hitVFX;
    public AudioClip slashSound;

    private void Start()
    {
        _lastPos = transform.position;
    }

    private void Update()
    {
        Velocity = (transform.position - _lastPos).magnitude / Time.deltaTime;
        _lastPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Velocity < minSliceVelocity)
        {
            return;
        }

        if (((1 << other.gameObject.layer) & sliceableLayer) != 0)
        {
            SustainBlock sustain = other.GetComponent<SustainBlock>();
            if (sustain != null) return;

            if (other.gameObject.CompareTag(targetTag))
            {
                SliceTarget(other.gameObject, normalSliceForce, transform.position);

                if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(100);
                PlayEffects(other.transform.position);
            }
            else
            {
                if (ScoreManager.Instance != null) ScoreManager.Instance.TakeDamage(1);
                Destroy(other.gameObject);
            }
        }
    }

    public void SliceTarget(GameObject targetObject, float force, Vector3 sliceOrigin)
    {
        Vector3 cuttingDirection = transform.right;

        // EzySlice
        SlicedHull hull = targetObject.Slice(sliceOrigin, cuttingDirection, crossSectionMaterial);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(targetObject, crossSectionMaterial);
            GameObject lowerHull = hull.CreateLowerHull(targetObject, crossSectionMaterial);

            SetupHullPhysics(upperHull, force);
            SetupHullPhysics(lowerHull, force);

            Destroy(targetObject);
        }
        else // kesim baþarýsýz olsa bile yok et.
        {
            Destroy(targetObject);
        }
    }

    public void PlayEffects(Vector3 pos)
    {
        if (hitVFX != null)
        {
            GameObject vfx = Instantiate(hitVFX, pos, Quaternion.identity);
            Destroy(vfx, 2f);
        }
        if (slashSound != null) AudioSource.PlayClipAtPoint(slashSound, pos, 1.0f);
    }

    private void SetupHullPhysics(GameObject hullPart, float force)
    {
        Rigidbody rb = hullPart.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        MeshCollider collider = hullPart.AddComponent<MeshCollider>();
        collider.convex = true;

        // Parçayý patlat
        rb.AddExplosionForce(force, transform.position, 1f);

        // Katmaný deðiþtir (Tekrar kesilmesin)
        hullPart.layer = LayerMask.NameToLayer("Default");
        Destroy(hullPart, 3f);
    }
}