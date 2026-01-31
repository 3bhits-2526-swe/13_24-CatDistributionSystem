using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float edgeScrollThreshold = 20f;
    [SerializeField] private bool enableEdgeScrolling = true;

    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    [SerializeField] private bool enableBoundaries = true;
    [SerializeField] private Vector2 minBoundary;
    [SerializeField] private Vector2 maxBoundary;

    [SerializeField] private bool blockWhenOverUI = true;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool dragging;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (blockWhenOverUI && EventSystem.current.IsPointerOverGameObject())
            return;

        HandleMovement();
        HandleZoom();
        ClampPosition();
    }

    private void HandleMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) move.y += 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) move.y -= 1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) move.x -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) move.x += 1;

        if (enableEdgeScrolling)
        {
            Vector3 mouse = Input.mousePosition;
            if (mouse.x < edgeScrollThreshold) move.x -= 1;
            if (mouse.x > Screen.width - edgeScrollThreshold) move.x += 1;
            if (mouse.y < edgeScrollThreshold) move.y -= 1;
            if (mouse.y > Screen.height - edgeScrollThreshold) move.y += 1;
        }

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            dragging = true;
        }

        if (Input.GetMouseButton(2) && dragging)
        {
            Vector3 current = cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += dragOrigin - current;
        }

        if (Input.GetMouseButtonUp(2))
            dragging = false;

        if (move != Vector3.zero)
            transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.01f)
            return;

        float size = cam.orthographicSize - scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(size, minZoom, maxZoom);
    }

    private void ClampPosition()
    {
        if (!enableBoundaries)
            return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBoundary.x, maxBoundary.x);
        pos.y = Mathf.Clamp(pos.y, minBoundary.y, maxBoundary.y);
        transform.position = pos;
    }
}
