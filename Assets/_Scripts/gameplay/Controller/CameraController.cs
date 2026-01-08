using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float edgeScrollThreshold = 20f;
    [SerializeField] private bool enableEdgeScrolling = true;

    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    [SerializeField] private bool enableBoundaries = true;
    [SerializeField] private Vector2 minBoundary = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxBoundary = new Vector2(60, 60);

    [SerializeField] private BuildingPaletteUI buildingPalette;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private bool isPointerOverUI = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
    }

    private void Update()
    {
        UpdatePointerOverUI();

        if (!isPointerOverUI)
        {
            HandleMovement();
            HandleZoom();
        }
    }

    private void UpdatePointerOverUI()
    {
        bool wasOverUI = isPointerOverUI;

        if (buildingPalette != null)
        {
            isPointerOverUI = buildingPalette.IsMouseOverPalette();
        }
        //else
        //{
        //    isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        //}

        if (wasOverUI != isPointerOverUI && isDragging)
        {
            isDragging = false;
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movement.y += 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movement.y -= 1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            movement.x -= 1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            movement.x += 1;

        if (enableEdgeScrolling && !isPointerOverUI)
        {
            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x < edgeScrollThreshold) movement.x -= 1;
            if (mousePos.x > Screen.width - edgeScrollThreshold) movement.x += 1;
            if (mousePos.y < edgeScrollThreshold) movement.y -= 1;
            if (mousePos.y > Screen.height - edgeScrollThreshold) movement.y += 1;
        }

        if (Input.GetMouseButtonDown(2) && !isPointerOverUI)
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        if (Input.GetMouseButton(2) && isDragging && !isPointerOverUI)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = dragOrigin - currentPos;
            transform.position += difference;
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (movement != Vector3.zero)
        {
            movement.Normalize();
            transform.position += movement * moveSpeed * Time.deltaTime;
        }

        if (enableBoundaries)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, minBoundary.x, maxBoundary.x);
            pos.y = Mathf.Clamp(pos.y, minBoundary.y, maxBoundary.y);
            transform.position = pos;
        }
    }

    private void HandleZoom()
    {
        if (isPointerOverUI) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newSize = cam.orthographicSize - scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    public bool IsCameraControlEnabled()
    {
        return !isPointerOverUI;
    }
}