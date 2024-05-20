using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Moving,
    Rushing,
    Shooting
}


public class BoatMovement : MonoBehaviour
{
    private BoatShoot boatShoot;
    private Rigidbody2D body;
    private Animator animator;
    private PlayerState currentState = PlayerState.Idle;
    private float lastInputTime; // Время последнего ввода
    private Vector2 lastDirection = Vector3.zero;
    private Quaternion lastTargetRotation = Quaternion.identity;
    [SerializeField] public float rotationSpeed; // Скорость вращения спрайта
    [SerializeField] public float rotationSpeedRushing;
    [SerializeField] private float movingSpeed;
    [SerializeField] private float rushingSpeed;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boatShoot = GetComponent<BoatShoot>();
    }

    private void Update()
    {
        HandleInput();
        if (Time.time - lastInputTime > 1.0) {ChangeState(PlayerState.Idle);} // Проверяем, прошло ли более 1 секунд с последнего ввода

        RotatePlayer(); //вращает игрока в соответствии с нажатыми клавишами

        switch (currentState) // В зависимости от текущего состояния вызываем соответствующий метод
        {
            case PlayerState.Idle: Idle(); break;
            case PlayerState.Moving: Moving(); break;
            case PlayerState.Rushing: Rushing(); break;
            case PlayerState.Shooting: boatShoot.Shooting(); break;
        }
        animator.SetInteger("currentState", (int)currentState);
    }

    private void RotatePlayer()
    {

        Quaternion targetRotation;

        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D)) {targetRotation = Quaternion.Euler(0, 0, -45);} // Северо-восток 
            else if (Input.GetKey(KeyCode.A)) {targetRotation = Quaternion.Euler(0, 0, 45); } // Северо-запад
            else {targetRotation = Quaternion.Euler(0, 0, 0);} // Север
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.D)) {targetRotation = Quaternion.Euler(0, 0, -135); } // Юго-восток
            else if (Input.GetKey(KeyCode.A)) {targetRotation = Quaternion.Euler(0, 0, 135); } // Юго-запад
            else{targetRotation = Quaternion.Euler(0, 0, 180);}  // Юг
        }

        else if (Input.GetKey(KeyCode.D)) { targetRotation = Quaternion.Euler(0, 0, -90);} // Восток
        else if (Input.GetKey(KeyCode.A)) {targetRotation = Quaternion.Euler(0, 0, 90);} // Запад
        else { targetRotation = lastTargetRotation; } // Используем текущий угол поворота, если ни одна клавиша не нажата
        lastTargetRotation = targetRotation;
        float speed = currentState == PlayerState.Rushing ? rotationSpeedRushing : rotationSpeed;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lastTargetRotation, speed * Time.deltaTime); // Плавно поворачиваем спрайт к целевому углу
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && boatShoot.CanShoot())
        {
            boatShoot.Shoot();             // Выполняем выстрел
            ChangeState(PlayerState.Shooting);
            boatShoot.lastShootTime = Time.time; // Обновляем время последнего выстрела
        }

        if (Input.anyKey) {lastInputTime = Time.time;} // Обновляем время последнего ввода при любом вводе от игрока
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space)) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) {ChangeState(PlayerState.Rushing);} // Проверяем нажатие клавиши Shift для ускорения
        else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space)) {ChangeState(PlayerState.Moving);} // Возвращаемся к обычному движению, когда Shift отпускается
    }
    private void ChangeState(PlayerState newState) {currentState = newState;}

    private void Idle() {return;}
    private void Rushing(){
        float speed = rushingSpeed;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            lastDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        }
        body.velocity = lastDirection * speed; // Если нет ввода, используем последнее направление
    }
    
    private void Moving()
    {
        float speed = movingSpeed; // Изначально устанавливаем скорость движения на обычную
        body.velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed; // Перемещаем лодку в соответствии с текущей скоростью и вводом игрока
    }

}
