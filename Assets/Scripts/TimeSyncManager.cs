using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

public class TimeSyncManager : MonoBehaviour
{
    public ClockManager clockManager;

    private void Start()
    {
        // Запуск и синхронизируем время с серверами и повторяем её каждый час
        StartCoroutine(SyncTimeWithMultipleServers());
        InvokeRepeating(nameof(UpdateTimeFromServers), 3600f, 3600f);
    }

    private IEnumerator SyncTimeWithMultipleServers()
    {
        DateTime time1 = DateTime.MinValue;
        DateTime time2 = DateTime.MinValue;

        // Отправка запроса к первому серверу для получения времени
        UnityWebRequest request1 = UnityWebRequest.Get("http://worldtimeapi.org/api/timezone/Europe/Moscow");
        yield return request1.SendWebRequest();

        if (request1.result == UnityWebRequest.Result.Success)
        {
            var json = request1.downloadHandler.text;
            WorldTimeResponse response1 = JsonUtility.FromJson<WorldTimeResponse>(json);
            time1 = DateTime.Parse(response1.datetime);
        }
        else
        {
            Debug.LogError("Ошибка при получении времени с первого сервера: " + request1.error);
        }
        
        // Отправка запроса ко второму серверу для получения времени
        UnityWebRequest request2 = UnityWebRequest.Get("http://timeapi.io/api/Time/current/zone?timeZone=Europe/Moscow");
        yield return request2.SendWebRequest();

        if (request2.result == UnityWebRequest.Result.Success)
        {
            var json = request2.downloadHandler.text;
            TimeApiResponse response2 = JsonUtility.FromJson<TimeApiResponse>(json);
            time2 = DateTime.Parse(response2.dateTime);
        }
        else
        {
            Debug.LogError("Ошибка при получении времени со второго сервера: " + request2.error);
        }
        
        // Вычисление среднего времени
        DateTime synchronizedTime;
        if (time1 != DateTime.MinValue && time2 != DateTime.MinValue)
        {
            synchronizedTime = new DateTime((time1.Ticks + time2.Ticks) / 2);
        }
        else if (time1 != DateTime.MinValue)
        {
            synchronizedTime = time1;
        }
        else
        {
            synchronizedTime = time2;
        }
        // Обновление времени на основных часах
        clockManager.UpdateCurrentTime(synchronizedTime);
        Debug.Log("Синхронизированное время: " + synchronizedTime.ToString("HH:mm:ss"));
    }

    private void UpdateTimeFromServers()
    {
        // Запуск синхронизации времени с серверами
        StartCoroutine(SyncTimeWithMultipleServers());
    }

    [Serializable]
    public class WorldTimeResponse
    {
        public string datetime; // Время в формате строки из первого API
    }

    [Serializable]
    public class TimeApiResponse
    {
        public string dateTime; // Время в формате строки из второго API
    }
}
