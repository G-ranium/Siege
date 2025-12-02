using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private SettingsData settingsData;
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        if (settingsData != null)
        {
            audioSource.volume = settingsData.musicVolume;
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = settingsData.musicVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        if (toggle != null)
        {
            Screen.fullScreen = toggle.isOn;
            toggle.onValueChanged.AddListener(SetFullscreen);
        }
    }
    
    void SetVolume(float value)
    {
        if (audioSource != null)
        {
            audioSource.volume = value;
            settingsData.musicVolume = value;
        }
    }
        
    void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
    

    public void SetResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
