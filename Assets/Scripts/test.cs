using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Скорость смещения кнопки влево
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Получаем компонент RectTransform объекта "Button"
        RectTransform buttonRectTransform = GetComponent<RectTransform>();

        // Смещаем кнопку влево на определенное расстояние
        Vector3 newPosition = buttonRectTransform.position;
        newPosition.x -= speed;
        buttonRectTransform.position = newPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
