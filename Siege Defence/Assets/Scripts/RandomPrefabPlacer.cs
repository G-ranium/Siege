// RandomPrefabPlacer.cs
// ---------------------------------------------------------------
// Place a random prefab from a list onto every Transform in another list.
// ---------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomPrefabPlacer : MonoBehaviour
{
    // ------------------------------------------------------------------
    // Inspector fields – fill these in the Unity Inspector or via code.
    // ------------------------------------------------------------------
    [Tooltip("Prefabs that can be chosen from. Must contain at least one entry.")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Tooltip("Transforms where a prefab will be instantiated. One prefab per transform.")]
    public List<Transform> spawnPoints = new List<Transform>();

    // ------------------------------------------------------------------
    // Public API – call these from other scripts or the custom editor.
    // ------------------------------------------------------------------
    /// <summary>
    /// Instantiates a random prefab at each transform in <see cref="spawnPoints"/>.
    /// If <paramref name="clearFirst"/> is true, any children of the spawn points are destroyed first.
    /// </summary>
    public void PlaceRandomPrefabs(bool clearFirst = false)
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogWarning("[RandomPrefabPlacer] No prefabs assigned – nothing to instantiate.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[RandomPrefabPlacer] No spawn points assigned – nothing to place.");
            return;
        }

        // Optional: clean existing children
        if (clearFirst)
        {
            foreach (var t in spawnPoints)
            {
                if (t != null)
                {
                    for (int i = t.childCount - 1; i >= 0; i--)
                    {
                        DestroyImmediate(t.GetChild(i).gameObject);
                    }
                }
            }
        }

        foreach (var point in spawnPoints)
        {
            if (point == null) continue; // skip missing references

            // Pick a random prefab
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

            // Instantiate at the transform's position/rotation, as a child (optional)
            GameObject instance = Instantiate(prefab, point.position, point.rotation);

            // Make the new object a child of the spawn point – keeps hierarchy tidy
            instance.transform.SetParent(point, true);
        }

        Debug.Log($"[RandomPrefabPlacer] Placed {spawnPoints.Count} random prefabs.");
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
[CustomEditor(typeof(RandomPrefabPlacer))]
public class RandomPrefabPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomPrefabPlacer placer = (RandomPrefabPlacer)target;

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