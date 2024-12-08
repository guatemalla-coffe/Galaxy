using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APICalls : MonoBehaviour
{
    public static APICalls Instance { get; private set; }
    
    public const string url_players = "https://2025.nti-gamedev.ru/api/games/7317031d-266a-41b0-a311-c16c6e3b974a/players/";
    public const string url_logs = "https://2025.nti-gamedev.ru/api/games/7317031d-266a-41b0-a311-c16c6e3b974a/logs/";
    public const string filePath = "Assets/save.json";
    
    public Account _player = new Account();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one APICalls Instance!");
            Destroy(gameObject);
        }
    }

    public void SaveToJson()
    {
        string json = JsonUtility.ToJson(_player, true); // true для форматирования
        File.WriteAllText(filePath, json);
        Debug.Log($"Данные записаны в {filePath}");
    }

    public void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            _player = JsonUtility.FromJson<Account>(json);
            Debug.Log($"Данные загружены из {filePath}");
        }
        else
        {
            Debug.LogWarning("Файл не найден.");
        }
    }

    
    public async UniTask SendLogs(LogInfo log)
    {
        string data = JsonUtility.ToJson(log);
        
        UnityWebRequest request = new UnityWebRequest(url_logs, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(data);
        
        request.SetRequestHeader("Authorization", "7317031d-266a-41b0-a311-c16c6e3b974a");
        
        // Устанавливаем тело запроса
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Отправляем запрос
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Запрос успешно отправлен, логи созданы: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Ошибка: " + request.error);
        }
    }

    public async UniTask GetLogs(string name)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{url_players}{name}/logs/"))
        {
            // Отправляем запрос
            await request.SendWebRequest();

            // Обрабатываем результат
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Данные логов игрока получены: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Ошибка получения данных: " + request.error);
            }
        }
    }
    
    
    /// <summary>
    /// Account data 'll be changed at _player
    /// </summary>
    /// <param name="name"></param>
    public async UniTask GetAccount(string name)
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{url_players}{name}/"))
        {
            // Отправляем запрос
            await request.SendWebRequest();

            // Обрабатываем результат
            if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.text != "")
            {
                Debug.Log($"Данные игрока __{name}__ получены: " + request.downloadHandler.text);
                _player = JsonUtility.FromJson<Account>(request.downloadHandler.text);
                print($"{_player.name}+{_player.password}+{_player.resources}");
            }
            else
            {
                Debug.LogError("Ошибка получения данных: " + request.error);
            }
        }
    }
}


[Serializable]
public class LogInfo
{
    public string comment;
    public string player_name;
    public List<Resource> resources_changed;

    public LogInfo(string comment, string playerName, List<Resource> resourcesChanged)
    {
        this.comment = comment;
        player_name = playerName;
        resources_changed = resourcesChanged;
    }
}