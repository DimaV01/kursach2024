
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTrigger : MonoBehaviour
{
    [SerializeField] private string nextLevel; // Индекс сцены для перехода

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, имеет ли другой объект тег "Player" (или другой необходимый тег)
        if (other.tag == "PlayerBoat")
        {
            // Сохраняем текущее здоровье игрока перед переходом на новый уровень
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                PlayerData.instance.SaveHealth(playerHealth.currentHealth);
            }
            // Загружаем сцену с указанным индексом
            SceneManager.LoadScene(nextLevel);
        }
    }

}
