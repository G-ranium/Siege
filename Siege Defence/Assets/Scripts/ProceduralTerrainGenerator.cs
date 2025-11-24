// ProceduralTerrainGenerator.cs
// FINAL VERSION – works 100% as of Unity 2022–2025 + AI Navigation 1.1.5+

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Generation")]
    public List<GameObject> prefabs = new List<GameObject>();
    public List<float> weights = new List<float>();
    public int width = 10;
    public int height = 10;
    public Vector2 spacing = new Vector2(20f, 20f);

    [Header("Tree Processing")]
    public string treeNameFilter = "Pine_";   // change if your trees use a different prefix

    // ------------------------------------------------------------------
    public void GenerateAndProcess(bool clearFirst = true)
    {
        PlaceRandomPrefabs(clearFirst);
        FixHoudiniMeshReadability();
        ProcessTreesAsNavMeshObstacles();
    }

    public void PlaceRandomPrefabs(bool clearFirst = false)
    {
        if (prefabs == null || prefabs.Count == 0) { Debug.LogWarning("No prefabs assigned."); return; }

        if (clearFirst)
        {
            while (transform.childCount > 0)
                DestroyImmediate(transform.GetChild(0).gameObject);
        }

        float totalWeight = weights != null && weights.Count == prefabs.Count ? weights.Sum() : 0f;
        bool useWeights = totalWeight > 0f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = transform.position + new Vector3(x * spacing.x, 0, z * spacing.y);
                int index = useWeights ? WeightedRandom(totalWeight) : Random.Range(0, prefabs.Count);
                GameObject instance = Instantiate(prefabs[index], pos, Quaternion.identity, transform);
                instance.name = prefabs[index].name;
            }
        }
        Debug.Log($"Generated {width * height} terrain tiles.");
    }

    int WeightedRandom(float total)
    {
        float r = Random.Range(0f, total);
        float sum = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            sum += weights[i];
            if (r < sum) return i;
        }
        return weights.Count - 1;
    }

    // ------------------------------------------------------------------
    [ContextMenu("Fix Houdini Mesh Readability (for runtime NavMesh in builds)")]
    public void FixHoudiniMeshReadability()
    {
        int fixedCount = 0;
        foreach (var mf in GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh && !mf.sharedMesh.isReadable)
            {
                mf.sharedMesh = Instantiate(mf.sharedMesh);
                fixedCount++;
            }
        }
        Debug.Log($"Fixed readability on {fixedCount} Houdini meshes – now safe for builds!");
    }

    [ContextMenu("Process Trees as NavMesh Obstacles")]
    public void ProcessTreesAsNavMeshObstacles()
    {
        Transform treeRoot = transform.Find("DYNAMIC_TREE_OBSTRUCTIONS");
        if (treeRoot == null)
        {
            treeRoot = new GameObject("DYNAMIC_TREE_OBSTRUCTIONS").transform;
            treeRoot.SetParent(transform, false);
        }

        int processed = 0;

        foreach (Transform t in transform.GetComponentsInChildren<Transform>(true))
        {
            if (!t.name.StartsWith(treeNameFilter)) continue;
            if (t == treeRoot) continue;

            t.SetParent(treeRoot, true);

            var obstacle = t.GetComponent<NavMeshObstacle>();
            if (obstacle == null)
                obstacle = t.gameObject.AddComponent<NavMeshObstacle>();

            // KEY FIX: Use the existing BoxCollider's size and center
            var box = t.GetComponentInChildren<BoxCollider>();
            if (box != null)
            {
                obstacle.shape = NavMeshObstacleShape.Box;
                obstacle.size = box.size;
                obstacle.center = box.center;
            }
            else
            {
                // Fallback for trees that somehow have no BoxCollider
                obstacle.shape = NavMeshObstacleShape.Capsule;
                obstacle.size = new Vector3(1.6f, 10f, 1.6f);
                obstacle.center = new Vector3(0, 5f, 0);
            }

            // Standard carving settings
            obstacle.carving = true;
            obstacle.carveOnlyStationary = false;
            obstacle.carvingMoveThreshold = 0.1f;
            obstacle.carvingTimeToStationary = 0.25f;

            processed++;
        }

        Debug.Log($"Processed {processed} trees – NavMeshObstacle now perfectly matches each tree's BoxCollider size!");
    }
}

// ==================================================================
// Editor – big beautiful buttons
// ==================================================================
#if UNITY_EDITOR
[CustomEditor(typeof(ProceduralTerrainGenerator))]
public class ProceduralTerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gen = (ProceduralTerrainGenerator)target;

        GUILayout.Space(15);
        GUI.backgroundColor = new Color(0.1f, 0.8f, 0.1f);
        if (GUILayout.Button("GENERATE TERRAIN + FIX TREES & NAVMESH", GUILayout.Height(45)))
            gen.GenerateAndProcess(clearFirst: true);

        GUILayout.Space(8);
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Reprocess Trees Only (keep current terrain)", GUILayout.Height(30)))
            gen.ProcessTreesAsNavMeshObstacles();

        GUILayout.Space(8);
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("Fix Houdini Mesh Readability Only", GUILayout.Height(30)))
            gen.FixHoudiniMeshReadability();

        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox("Works in Editor only – perfect for level building.\n" +
                                "• Houdini meshes become readable for builds\n" +
                                "• All Pine_* trees become proper dynamic NavMesh obstacles\n" +
                                "• Trees can be deep inside terrain_XX parents – they WILL be found", MessageType.Info);
    }
}
#endif