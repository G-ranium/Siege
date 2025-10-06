using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickInstantiator : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject prefabToInstantiate;
    public LayerMask interactableLayer;
    public Material highlightMaterial;
    public PrefabListData prefabList;

    private Camera mainCamera;
    private GameObject currentlyHoveredObject = null;
    private Material originalMaterial = null;

    private bool isMouseHeld = false;

    // Input actions
    private InputAction clickAction;
    private InputAction positionAction;

    private void Awake()
    {
        mainCamera = Camera.main;

        // Create actions
        clickAction = new InputAction("Click", binding: "<Mouse>/leftButton");
        positionAction = new InputAction("MousePosition", binding: "<Mouse>/position");

        clickAction.started += ctx => OnMouseDown();
        clickAction.canceled += ctx => OnMouseUp();
    }

    private void OnEnable()
    {
        clickAction.Enable();
        positionAction.Enable();
        prefabToInstantiate = prefabList.GetSelectedPrefab();
    }

    private void OnDisable()
    {
        clickAction.Disable();
        positionAction.Disable();
    }

    private void Update()
    {
        if (isMouseHeld)
        {
            HandleHovering();
        }
    }

    private void OnMouseDown()
    {
        isMouseHeld = true;
    }

    private void OnMouseUp()
    {
        if (isMouseHeld)
        {
            HandleRelease();
            isMouseHeld = false;
        }
    }

    void HandleHovering()
    {
        Vector2 mouseScreenPosition = positionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;
        
        int groundLayerMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(ray, out hit, 100f, groundLayerMask))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject != currentlyHoveredObject)
            {
                ClearHighlight();

                currentlyHoveredObject = hitObject;

                Renderer renderer = currentlyHoveredObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalMaterial = renderer.material;
                    renderer.material = highlightMaterial;
                }
            }
        }
        else
        {
            ClearHighlight();
        }
    }

    void HandleRelease()
    {
        if (currentlyHoveredObject != null)
        {
            Vector3 spawnPosition = currentlyHoveredObject.transform.position + Vector3.up;
            Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
        }

        ClearHighlight();
        this.enabled = false;
    }

    void ClearHighlight()
    {
        if (currentlyHoveredObject != null)
        {
            Renderer renderer = currentlyHoveredObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.material = originalMaterial;
            }
        }

        currentlyHoveredObject = null;
        originalMaterial = null;
    }
}
