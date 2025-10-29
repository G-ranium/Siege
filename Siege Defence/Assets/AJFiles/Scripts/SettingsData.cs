using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject
{
    public float musicVolume = 1f;
    public float sfxVolume;
}
