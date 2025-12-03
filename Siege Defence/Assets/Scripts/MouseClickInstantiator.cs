using UnityEngine;
using UnityEngine.InputSystem;

public class MouseClickInstantiator : MonoBehaviour
{
    [Header("Configuration")]
    public PrefabListData prefabList;
    public LayerMask groundLayer;
    public Material ghostMaterial; // Material for transparent ghost

    [SerializeField]private Camera mainCamera;
    private GameObject prefabToInstantiate;
    private GameObject ghostInstance;

    private bool isMouseHeld = false;

    // Input actions
    private InputAction clickAction;
    private InputAction positionAction;

    private void Awake()
    {
        if(mainCamera == null)
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
            HandleMouseHover();
        }
    }

    private void OnMouseDown()
    {
        isMouseHeld = true;

        // Create the ghost prefab
        if (prefabToInstantiate != null && ghostInstance == null)
        {
            ghostInstance = Instantiate(prefabToInstantiate);
            SetLayerRecursively(ghostInstance, LayerMask.NameToLayer("Ignore Raycast")); // Prevent ghost from interfering with raycasts

            // Apply ghost material
            ApplyGhostMaterial(ghostInstance, ghostMaterial);
        }
    }

    private void OnMouseUp()
    {
        if (isMouseHeld)
        {
            HandleRelease();
            isMouseHeld = false;
        }
    }

    void HandleMouseHover()
    {
        Vector2 mouseScreenPosition = positionAction.ReadValue<Vector2>();
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            if (ghostInstance != null)
            {
                ghostInstance.transform.position = hit.point;
            }
        }
    }

    void HandleRelease()
    {
        if (ghostInstance != null)
        {
            // Instantiate the real prefab at ghost's position
            Instantiate(prefabToInstantiate, ghostInstance.transform.position, Quaternion.identity);
            Destroy(ghostInstance);
        }

        this.enabled = false;
    }

    void ApplyGhostMaterial(GameObject obj, Material ghostMat)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            Material[] mats = new Material[rend.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = ghostMat;
            }
            rend.materials = mats;
        }
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
