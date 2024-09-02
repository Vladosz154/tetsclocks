using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour
{
    //Основные часы
    public Transform clockHourHandTransform; // часовая стрелка
    public Transform clockMinuteHandTransform; // минутная стрелка
    public Transform clockSecondHandTransform; // секундная стрелка
    public TMP_Text clockText;
    
    //Часы для будильника
    public ClockHandController hourHandController; // часовая стрелка
    public ClockHandController minuteHandController; // минутная стрелка
    public TMP_Text secondaryClockText; // секундная стрелка

    public TMP_InputField alarmInputField; //Ввод времени для будильника
    public Toggle is24HourToggle; //смена часового формата
    public TMP_Text timeFormatText; // текст часового формата
    public Button setAlarmButton; // кнопка применения будильника

    //Время 
    private DateTime currentTime; //текущее время
    private DateTime alarmTime; // время будильника
    private bool isAlarmSet = false;     
    
    //Статус будильника
    public TMP_Text alarmStatusText;
    public TMP_Text errorText;
    

    private void Start()
    {
        setAlarmButton.onClick.AddListener(SetAlarm);
    }

    private void Update()
    {
        UpdateClockHands();

        if (isAlarmSet && currentTime.Hour == alarmTime.Hour && currentTime.Minute == alarmTime.Minute)
        {
            TriggerAlarm();
        }

        currentTime = currentTime.AddSeconds(Time.deltaTime);
    }

   public void SetAlarm()
    {
        if (!string.IsNullOrWhiteSpace(alarmInputField.text))
        {
            if (DateTime.TryParse(alarmInputField.text, out DateTime parsedTime))
            {
                int hour = parsedTime.Hour;
                int minute = parsedTime.Minute;

                alarmTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, minute, 0);
                isAlarmSet = true;

                // Обновление текстового поля статуса будильника
                alarmStatusText.text = "Будильник установлен на: " + alarmTime.ToString(is24HourToggle.isOn ? "HH:mm" : "hh:mm");
                errorText.text = ""; // Очищаем текст ошибок
            }
            else
            {
                // Отображение ошибки в текстовом поле
                errorText.text = "Неверный формат времени в текстовом поле";
                alarmStatusText.text = "";
            }
        }
        else
        {
            float hourAngle = hourHandController.GetCurrentAngle();
            float minuteAngle = minuteHandController.GetCurrentAngle();

            int alarmHour = Mathf.RoundToInt((360 - hourAngle) / 30) % 12;
            int alarmMinute = Mathf.RoundToInt((360 - minuteAngle) / 6);

            if (alarmHour == 0) alarmHour = 12;

            if (is24HourToggle.isOn)
            {
                alarmHour = (alarmHour == 12) ? 12 : alarmHour + 12;
            }

            alarmTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, alarmHour, alarmMinute, 0);
            isAlarmSet = true;

            // Обновление текстового поля статуса будильника
            alarmStatusText.text = "Будильник установлен на: " + alarmTime.ToString(is24HourToggle.isOn ? "HH:mm" : "hh:mm");
        }
    }

    private void TriggerAlarm()
    {
        isAlarmSet = false;
        // Обновление текстового поля статуса будильника
        alarmStatusText.text = "Будильник сработал!";
        errorText.text = ""; // Очистка текста ошибок
    }

    private void UpdateClockHands()
    {
        float hours = currentTime.Hour % 12;
        float minutes = currentTime.Minute;
        float seconds = currentTime.Second;

        float hourAngle = (hours + minutes / 60f) * 30f;
        float minuteAngle = (minutes + seconds / 60f) * 6f;
        float secondAngle = seconds * 6f;

        clockHourHandTransform.eulerAngles = new Vector3(0, 0, -hourAngle);
        clockMinuteHandTransform.eulerAngles = new Vector3(0, 0, -minuteAngle);
        clockSecondHandTransform.eulerAngles = new Vector3(0, 0, -secondAngle);

        clockText.text = currentTime.ToString("HH:mm:ss");

        int displayedHour = Mathf.RoundToInt((360 - hourHandController.GetCurrentAngle()) / 30) % 12;
        if (is24HourToggle.isOn && displayedHour != 0) displayedHour += 12;
        secondaryClockText.text = $"{displayedHour:D2}:{Mathf.RoundToInt((360 - minuteHandController.GetCurrentAngle()) / 6):D2}";

        timeFormatText.text = is24HourToggle.isOn ? "24-часовой формат" : "12-часовой формат";
    }

    public void UpdateCurrentTime(DateTime time)
    {
        currentTime = time;
    }
}
