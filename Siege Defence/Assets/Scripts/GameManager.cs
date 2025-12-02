using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private SettingsData settingsData;

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

        void SetVolume(float value)
        {
            if (audioSource != null)
            {
                audioSource.volume = value;
                settingsData.musicVolume = value;
            }
        }
    }

    public void pauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
