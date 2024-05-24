using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Vector3 baseOffset = new Vector3(0, 0, -10);
    [SerializeField] private float offsetScale = 0.0f;
    [SerializeField] private Transform player;
    private Rigidbody2D playerRigidbody;  // Добавляем ссылку на Rigidbody игрока

    void Awake()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();  // Получаем компонент Rigidbody
    }

    void Update()
    {
        Vector3 playerDirection = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y, 0).normalized;  // Направление движения игрока
          // Масштаб изменения смещения
        Vector3 dynamicOffset = baseOffset + playerDirection * offsetScale;  // Динамически изменяем смещение

        Vector3 targetPosition = player.position + dynamicOffset;  // Целевая позиция камеры с учетом динамического смещения
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);  // Плавное перемещение камеры к целевой позиции
    }
}
