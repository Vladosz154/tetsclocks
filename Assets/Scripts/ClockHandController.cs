using UnityEngine;
using UnityEngine.EventSystems;

public class ClockHandController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public Transform clockHandTransform; //Привязка стрелки часов
    private Vector3 initialMousePosition; //Начальная позиция мыши
    private float initialAngle; // Начальный угол поворота стрелки

    // Обратный вызов для обработки начала перетаскивания
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        initialMousePosition = Input.mousePosition;
        initialAngle = clockHandTransform.eulerAngles.z;
    }

    // Обратный вызов для обработки перетаскивания
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        // Получение текущей позиции мыши
        Vector3 mousePosition = Input.mousePosition;

        // Центр стрелки
        Vector3 center = Camera.main.WorldToScreenPoint(clockHandTransform.position);

        // Вектор направления от центра до мыши
        Vector3 direction = mousePosition - center;

        // Рассчет угла в радианах между начальным текущим 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Обновление угла стрелки с учетом начального угла
        float newAngle = Mathf.Repeat(angle, 360f);

        // Новый угол для стрелки
        clockHandTransform.eulerAngles = new Vector3(0, 0, newAngle);
    }

    // Текущий угол стрелки
    public float GetCurrentAngle()
    {
        return clockHandTransform.eulerAngles.z;
    }
}