using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PrefabListSO", menuName = "ScriptableObjects/PrefabList", order = 1)]
public class PrefabListSO : ScriptableObject
{
    [Tooltip("Add the prefabs you want to reference here.")]
    public List<GameObject> prefabs = new List<GameObject>();
}