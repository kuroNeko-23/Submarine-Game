using UnityEngine;

public class RulerLoop : MonoBehaviour
{
    public float scrollSpeed = 100f;     // how fast it moves
    public float segmentHeight = 500f;   // MUST match your image height

    private RectTransform rect;
    private float offset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        offset += scrollSpeed * Time.deltaTime;

        float y = offset % segmentHeight;

        rect.anchoredPosition = new Vector2(0, -y);
    }
}