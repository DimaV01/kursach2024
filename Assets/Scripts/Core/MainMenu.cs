using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.DestroySound();
        }
    }

    public void StartGame()
    {
        PlayerData.ResetHealth();
        SceneManager.LoadScene(1); // Замените '1' на имя или индекс вашей первой сцены
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

