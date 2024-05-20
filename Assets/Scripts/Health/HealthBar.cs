using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Health playerHealth; // Ссылка на скрипт с текущим здоровьем игрока
    [SerializeField] private GameObject healthBarLVL; // Объект healthbar1LVL

    private List<GameObject> healthImages; // Список изображений здоровья

    private void Awake()
    {
        // Инициализация списка изображений здоровья
        healthImages = new List<GameObject>();

        // Получение всех дочерних объектов healthBar1LVL и добавление их в список
        foreach (Transform child in healthBarLVL.transform)
        {
            healthImages.Add(child.gameObject);
        }

        // Сортировка списка изображений по имени в порядке убывания (от большего к меньшему)
        healthImages.Sort((x, y) => string.Compare(x.name, y.name));
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // Получение текущего здоровья игрока
        float currentHealth = playerHealth.currentHealth;

        // Вычисление процента оставшегося здоровья
        float healthPercentage = currentHealth / playerHealth.startingHealth;

        // Определение индекса текущего изображения здоровья
        int healthIndex = Mathf.FloorToInt(healthPercentage * (healthImages.Count - 1));

        // Обновление видимости изображений в зависимости от текущего здоровья
        for (int i = 0; i < healthImages.Count; i++)
        {
            healthImages[i].SetActive(i == healthIndex);
        }
    }
}

