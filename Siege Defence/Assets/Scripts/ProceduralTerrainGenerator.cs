// ProceduralTerrainGenerator.cs
// ---------------------------------------------------------------
// Procedurally generate a grid of random prefabs with weighted probabilities.
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProceduralTerrainGenerator : MonoBehaviour
{
    // ------------------------------------------------------------------
    // Inspector fields – fill these in the Unity Inspector or via code.
    // ------------------------------------------------------------------
    [Tooltip("Prefabs that can be chosen from. Must contain at least one entry.")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Tooltip("Weights for each prefab (must match prefabs list length). Higher weight means higher chance.")]
    public List<float> weights = new List<float>();

    [Tooltip("Number of tiles in the X direction.")]
    public int width = 5;

    [Tooltip("Number of tiles in the Z direction.")]
    public int height = 5;

    [Tooltip("Spacing offset between tiles in X and Z directions.")]
    public Vector2 spacing = new Vector2(1f, 1f);

    // ------------------------------------------------------------------
    // Public API – call these from other scripts or the custom editor.
    // ------------------------------------------------------------------
    /// <summary>
    /// Instantiates random prefabs in a grid pattern based on width, height, and spacing.
    /// If <paramref name="clearFirst"/> is true, any children of this transform are destroyed first.
    /// </summary>
    public void PlaceRandomPrefabs(bool clearFirst = false)
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("[ProceduralTerrainGenerator] No prefabs assigned – nothing to instantiate.");
            return;
        }

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("[ProceduralTerrainGenerator] Invalid grid dimensions – must be positive.");
            return;
        }

        // Optional: clean existing children
        if (clearFirst)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // Check if weights are valid; fallback to uniform if not
        bool useWeights = weights != null && weights.Count == prefabs.Count && weights.Any(w => w > 0);
        float totalWeight = useWeights ? weights.Sum() : 0f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = transform.position + new Vector3(x * spacing.x, 0f, z * spacing.y);
                Quaternion rotation = transform.rotation;

                // Pick a random prefab (weighted if possible)
                int index;
                if (useWeights)
                {
                    float rand = Random.Range(0f, totalWeight);
                    float cumulative = 0f;
                    index = 0;
                    for (int i = 0; i < weights.Count; i++)
                    {
                        cumulative += weights[i];
                        if (rand < cumulative)
                        {
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    index = Random.Range(0, prefabs.Count);
                }

                GameObject prefab = prefabs[index];

                // Instantiate at the calculated position/rotation
                GameObject instance = Instantiate(prefab, position, rotation);

                // Make the new object a child of this transform – keeps hierarchy tidy
                instance.transform.SetParent(transform, true);
            }
        }

        Debug.Log($"[ProceduralTerrainGenerator] Placed {width * height} random prefabs.");
    }

    // ------------------------------------------------------------------
    // Example usage at runtime (e.g., on Start or via a button)
    // ------------------------------------------------------------------
    private void Start()
    {
        // Uncomment the line below if you want the placement to happen automatically on Start.
        // PlaceRandomPrefabs();
    }
}

#if UNITY_EDITOR
// ------------------------------------------------------------------
// Custom editor – adds a button in the Inspector for quick testing.
// ------------------------------------------------------------------
[CustomEditor(typeof(ProceduralTerrainGenerator))]
public class ProceduralTerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ProceduralTerrainGenerator placer = (ProceduralTerrainGenerator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Place Random Prefabs (clear children first)"))
        {
            placer.PlaceRandomPrefabs(clearFirst: true);
        }

        if (GUILayout.Button("Place Random Prefabs (keep children)"))
        {
            placer.PlaceRandomPrefabs(clearFirst: false);
        }
    }
}
#endif