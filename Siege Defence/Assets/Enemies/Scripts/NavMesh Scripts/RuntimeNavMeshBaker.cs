using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class RuntimeNavMeshBaker : MonoBehaviour
{
    public GameObject targetGameObject; // Assign the GameObject in the Inspector

    void Start()
    {
        StartCoroutine(BuildMesh(1f));
    }

    private IEnumerator BuildMesh(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (targetGameObject != null)
        {
            // Get the NavMeshSurface component, or add it if it doesn't exist
            NavMeshSurface navMeshSurface = targetGameObject.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = targetGameObject.AddComponent<NavMeshSurface>();
            }

            // Configure NavMeshSurface settings (optional, but recommended)
            // For example, you can set the AgentTypeID, CollectObjects, etc.
            int test = navMeshSurface.agentTypeID;
            Debug.Log("The Mesh is set to " + test);

            navMeshSurface.agentTypeID = -1372625422;
            // navMeshSurface.collectObjects = CollectObjects.Children; 

            // Bake the NavMesh for this surface
            navMeshSurface.BuildNavMesh();

            Debug.Log($"NavMesh Surface applied and baked on {targetGameObject.name}");

            StartCoroutine(MeshNumber(4f, navMeshSurface));
        }
        else
        {
            Debug.LogError("Target GameObject is not assigned!");
        }
    }

    private IEnumerator MeshNumber(float delay, NavMeshSurface surface)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("The ID number is " + surface.agentTypeID);
    }
}
