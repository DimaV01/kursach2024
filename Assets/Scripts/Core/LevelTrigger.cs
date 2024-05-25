using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTrigger : MonoBehaviour
{
    [SerializeField] private SceneAsset nextLevel; // Индекс сцены для перехода

    private string sceneName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, имеет ли другой объект тег "Player" (или другой необходимый тег)
        if (other.tag == "PlayerBoat")
        {
            // Загружаем сцену с указанным индексом
            SceneManager.LoadScene(sceneName);
        }
    }

    private void Awake()
    {
        sceneName = nextLevel.name;
    }
}
