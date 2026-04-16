using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Transform[] cameraPoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    private int currentIndex = 0;
    private Transform targetPoint;

    void Start()
    {
        if (cameraPoints.Length == 0) return;

        targetPoint = cameraPoints[currentIndex];
        transform.position = targetPoint.position;
        transform.rotation = targetPoint.rotation;
    }

    void Update()
    {
        HandleInput();
        SmoothMove();
    }

    void HandleInput()
    {
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % cameraPoints.Length;
            targetPoint = cameraPoints[currentIndex];
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = cameraPoints.Length - 1;

            targetPoint = cameraPoints[currentIndex];
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetPoint.rotation, Time.deltaTime * rotateSpeed);
    }
}