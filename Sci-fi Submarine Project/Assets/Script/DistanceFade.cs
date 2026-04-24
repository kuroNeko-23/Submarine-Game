using UnityEngine;

public class DistanceFade : MonoBehaviour
{
    public Transform cameraTransform;
    public float maxDistance = 100f;
    public float minDistance = 30f;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, cameraTransform.position);

        float t = Mathf.InverseLerp(minDistance, maxDistance, dist);
        float alpha = 1f - t;

        Color c = mat.color;
        c.a = alpha;
        mat.color = c;
    }
}