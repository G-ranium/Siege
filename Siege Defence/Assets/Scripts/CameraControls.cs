using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraControls : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float baseMoveSpeed = 30f;
    [SerializeField] private float keyboardMoveSpeedOffset = 40f;
    [SerializeField] private float fastMoveMultiplier = 2.5f;

    [Header("Max Zoom")]
    [SerializeField] private float zoomSpeed = 40f;
    [SerializeField] private float minHeight = 10f;
    [SerializeField] private float maxHeight = 120f;

    [Header("Camera bounds X Z")]
    [SerializeField] private Vector2 worldMin = new Vector2(-150f, -150f);
    [SerializeField] private Vector2 worldMax = new Vector2(150f, 150f);

    [Header("Edge Scroll and Drag")]
    [SerializeField] private bool enableEdgeScroll = true;
    [SerializeField] private float edgeTolerance = 20f;
    [SerializeField] private float edgeScrollSpeed = 40f;
    [SerializeField] private float middleDragSpeed = 0.08f;

    [Header("Camera Acceleration")]
    [Range(0f, 0.5f)]
    [SerializeField] private float moveSmoothTime = 0.1f;
    [SerializeField] private float zoomSmoothTime = 0.2f;

    private Camera cam;
    private Vector2 moveInput;
    private float scrollInput;
    private Vector2 mousePos;

    private Vector3 moveVelocity = Vector3.zero;
    private float targetHeight;
    private float heightVelocity = 0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetHeight = transform.position.y;
    }

    private void Update()
    {
        GatherInput();
        HandleZoom();
    }

    private void LateUpdate()
    {
        HandleMovement();
        ClampPosition();
    }

    private void GatherInput()
    {
        if (Keyboard.current == null)
        {
            moveInput = Vector2.zero;
            scrollInput = 0f;
            return;
        }

        // Keyboard input
        float x = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)     x -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)   x += 1f;

        float z = 0f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)    z -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)      z += 1f;

        moveInput = (new Vector2(x, z).normalized) * keyboardMoveSpeedOffset; // normalized so diagonal isn't faster

        // Scroll & mouse position
        scrollInput = Mouse.current?.scroll.ReadValue().y ?? 0f;
        mousePos = Mouse.current?.position.ReadValue() ?? Vector2.zero;
    }

    private void HandleZoom()
    {
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetHeight -= scrollInput / 120f * zoomSpeed;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }

        // Smoothly move camera height
        float newY = Mathf.SmoothDamp(transform.position.y, targetHeight, ref heightVelocity, zoomSmoothTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void HandleMovement()
    {
        Vector3 desiredVelocity = Vector3.zero;

        // Keyboard input
        if (moveInput.sqrMagnitude > 0.01f)
        {
            desiredVelocity += new Vector3(moveInput.x, 0f, moveInput.y);
        }

        // Edge scrolling
        if (enableEdgeScroll)
        {
            Vector2 edge = Vector2.zero;
            if (mousePos.x < edgeTolerance) edge.x -= 1f;
            if (mousePos.x > Screen.width - edgeTolerance) edge.x += 1f;
            if (mousePos.y < edgeTolerance) edge.y -= 1f;
            if (mousePos.y > Screen.height - edgeTolerance) edge.y += 1f;

            desiredVelocity += new Vector3(edge.x, 0f, edge.y).normalized * edgeScrollSpeed;
        }

        // Middle mouse drag
        if (Mouse.current != null && Mouse.current.middleButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            desiredVelocity += new Vector3(-delta.x, 0f, -delta.y) * (middleDragSpeed * targetHeight);
        }

        // Apply speed + shift
        float speed = baseMoveSpeed *
                      (Keyboard.current?.leftShiftKey.isPressed == true || Keyboard.current?.rightShiftKey.isPressed == true
                          ? fastMoveMultiplier
                          : 1f) *
                      Time.deltaTime;

        Vector3 motion = (transform.right * desiredVelocity.x + transform.forward * desiredVelocity.z) * speed;
        motion.y = 0f;

        Vector3 targetPos = transform.position + motion;

        // Smooth movement on XZ plane
        targetPos.y = transform.position.y;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVelocity, moveSmoothTime);
    }

    private void ClampPosition()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, worldMin.x, worldMax.x);
        p.z = Mathf.Clamp(p.z, worldMin.y, worldMax.y);
        transform.position = p;
    }
}