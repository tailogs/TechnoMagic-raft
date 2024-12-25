using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость движения
    public float jumpForce = 5f; // Сила прыжка
    public Camera playerCamera; // Ссылка на камеру
    private Rigidbody rb; // Компонент Rigidbody
    private bool isGrounded; // Проверка на земле

    public float mouseSensitivity = 2f; // Чувствительность мыши
    private float verticalRotation = 0f; // Вертикальный угол наклона камеры

    public GameObject cubePrefab; // Префаб кубика для установки
    public float reachDistance = 5f; // Дальность до блока для взаимодействия

    private Color currentBlockColor; // Цвет текущего блока для установки

    // Вода
    public float waterDrag = 3f;  // Сопротивление воды
    public float normalDrag = 0.5f; // Обычное сопротивление
    private bool isInWater = false; // Проверка, в воде ли игрок

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Получаем компонент Rigidbody
        Cursor.lockState = CursorLockMode.Locked; // Скрываем курсор и блокируем его
        currentBlockColor = Color.white; // Устанавливаем начальный цвет блока по умолчанию
    }

    void Update()
    {
        LookAround();
        Move();
        Jump();

        // Проверка на падение ниже определенной высоты
        if (transform.position.y < -10) // Например, если игрок падает ниже -10 по оси Y
        {
            Debug.Log("Игрок упал за пределы!");
            return; // Выходим из метода Update
        }

        if (Input.GetMouseButtonDown(1))
        {
            DestroyBlock();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlaceBlock();
        }

        // Обработка взаимодействия с водой
        HandleWaterInteraction();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // Получаем ввод по оси X (A/D)
        float moveVertical = Input.GetAxis("Vertical"); // Получаем ввод по оси Z (W/S)

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical; // Создаем вектор движения

        if (movement.magnitude > 0.1f) // Проверяем, есть ли движение
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.deltaTime); // Двигаем игрока с помощью MovePosition
        }
    }

    void Jump()
    {
        CheckGrounded(); // Проверяем состояние "на земле"

        if (isGrounded && Input.GetButtonDown("Jump")) // Проверяем, на земле ли игрок и нажата ли кнопка прыжка
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Прыжок
            isGrounded = false; // Устанавливаем флаг, что игрок не на земле после прыжка
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;
        
        // Выполняем Raycast вниз от позиции игрока
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f)) 
        {
            if (hit.collider.CompareTag("Block")) 
            {
                isGrounded = true; // Игрок на земле
            }
            else 
            {
                isGrounded = false; // Игрок в воздухе
            }
        }
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // Получаем ввод мыши по оси X
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity; // Получаем ввод мыши по оси Y

        verticalRotation -= mouseY; // Уменьшаем вертикальный угол наклона камеры
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f); // Ограничиваем наклон камеры

        playerCamera.transform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f); // Применяем наклон к камере

        transform.Rotate(Vector3.up * mouseX); // Поворачиваем игрока по оси Y (вокруг вертикальной оси)
    }

    void DestroyBlock()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Block")) // Убедитесь, что блок можно уничтожить
            {
                currentBlockColor = hit.collider.GetComponent<Renderer>().material.color; // Сохраняем цвет разрушенного блока
                Destroy(hit.collider.gameObject); // Уничтожаем блок при попадании луча
            }
        }
    }

    void PlaceBlock()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, reachDistance))
        {
            Vector3 positionToPlace = hit.point + hit.normal * 0.5f; // Позиция для установки блока

            positionToPlace.x = Mathf.Round(positionToPlace.x);
            positionToPlace.y = Mathf.Round(positionToPlace.y);
            positionToPlace.z = Mathf.Round(positionToPlace.z);

            if (!Physics.CheckSphere(positionToPlace, 0.1f)) 
            {
                GameObject newBlock = Instantiate(cubePrefab, positionToPlace, Quaternion.identity);
                Renderer blockRenderer = newBlock.GetComponent<Renderer>();
                blockRenderer.material.color = currentBlockColor; // Устанавливаем цвет нового блока равным цвету разрушенного блока
            }
        }
    }

    // Обработка взаимодействия с водой
    void HandleWaterInteraction()
    {
        RaycastHit hit;
        
        // Проверка, попадает ли игрок в воду
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f)) 
        {
            if (hit.collider.CompareTag("Water") && !isInWater)
            {
                // Входим в воду
                isInWater = true;
                rb.drag = waterDrag; // Применяем сопротивление воды
            }
            else if (!hit.collider.CompareTag("Water") && isInWater)
            {
                // Выходим из воды
                isInWater = false;
                rb.drag = normalDrag; // Восстанавливаем нормальное сопротивление
            }
        }
    }
}
