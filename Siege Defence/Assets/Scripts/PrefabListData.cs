using UnityEngine;

[CreateAssetMenu(fileName = "NewPrefabListData", menuName = "Custom/Prefab List Data")]
public class PrefabListData : ScriptableObject
{
    [Header("Prefab List")] public GameObject[] prefabOptions;

    [Tooltip("Index of the prefab to instantiate from the list above.")]
    public int selectedPrefabIndex = 0;

    public GameObject GetSelectedPrefab()
    {
        if (prefabOptions == null || prefabOptions.Length == 0)
        {
            Debug.LogWarning("Prefab list is empty.");
            return null;
        }

        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= prefabOptions.Length)
        {
            Debug.LogWarning("Selected prefab index is out of range.");
            return null;
        }

        return prefabOptions[selectedPrefabIndex];
    }

    public void setSelectedPrefabIndex(int selectedIndex)
    {
        selectedPrefabIndex = selectedIndex;
    }
}