using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance { get; private set; }
    public float currentHealth;
    public float startingHealth; // Добавляем startingHealth

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Метод для сохранения здоровья
    public void SaveHealth(float health)
    {
        currentHealth = health;
    }

    // Метод для получения сохраненного здоровья
    public float LoadHealth()
    {
        return currentHealth;
    }

    // Статический метод для сброса здоровья к начальному значению
    public static void ResetHealth()
    {
        if (instance != null)
        {
            instance.currentHealth = instance.startingHealth;
        }
    }
}
