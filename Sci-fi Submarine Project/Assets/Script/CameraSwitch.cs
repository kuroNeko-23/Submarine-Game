using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Transform[] cameraPoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private NodeManager nodeManager;

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
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % cameraPoints.Length;
            targetPoint = cameraPoints[currentIndex];
            nodeManager.SetNode((NodeType)currentIndex);
        }

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = cameraPoints.Length - 1;

            targetPoint = cameraPoints[currentIndex];
            nodeManager.SetNode((NodeType)currentIndex);
        }
    }

    void SmoothMove()
    {
        Vector3 basePos = Vector3.Lerp(transform.position, targetPoint.position, Time.deltaTime * moveSpeed);
        Quaternion baseRot = Quaternion.Slerp(transform.rotation, targetPoint.rotation, Time.deltaTime * rotateSpeed);

        Vector3 shake = Vector3.zero;

        if (TryGetComponent<CameraShake>(out var shakeComp))
        {
            shake = shakeComp.GetShakeOffset();
        }

        transform.position = basePos + shake;
        transform.rotation = baseRot;
    }
}