using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EnterAccountForm : MonoBehaviour
{
    [Header("REGISTER")] [SerializeField] private TMP_InputField register_nickname_inputField;
    [SerializeField] private TMP_InputField register_password_inputField;
    [SerializeField] private Button register_registerButton;
    [SerializeField] private TextMeshProUGUI register_errorLog_inputField;

    [Header("LOGIN")] [SerializeField] private TMP_InputField login_nickname_inputField;
    [SerializeField] private TMP_InputField login_password_inputField;
    [SerializeField] private Button login_registerButton;
    [SerializeField] private TextMeshProUGUI login_errorLog_inputField;

    
    private void Start()
    {
        register_registerButton.onClick.AddListener(async delegate
        {
            try
            {
                await Register();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        });
    }
    
    private async UniTask Register()
    {
        print("REGISTER");
        string nickname = register_nickname_inputField.text;
        string password = register_password_inputField.text;

        APICalls.Instance._player.name = nickname;
        APICalls.Instance._player.password = password;
        
        //<test 
        Resource t = new Resource();
        t.name = "JAR";
        t.count = 19;
        
        Resource c = new Resource();
        c.name = "ghhnb";
        c.count = 444;
        
        Resource d = new Resource();
        d.name = "asd";
        d.count = 1129;
        
        print($"{t}: {t.name}, {t.count}");
        APICalls.Instance._player.resources.Add(t);
        APICalls.Instance._player.resources.Add(c);
        APICalls.Instance._player.resources.Add(d);
        //>
        
        APICalls.Instance.SaveToJson();

        string jsonData = File.ReadAllText(APICalls.filePath);

        UnityWebRequest request = new UnityWebRequest(APICalls.url_players, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.SetRequestHeader("Authorization", "7317031d-266a-41b0-a311-c16c6e3b974a");
        // Устанавливаем тело запроса
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Отправляем запрос
        try
        {
            await request.SendWebRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                register_errorLog_inputField.text = "";
                Debug.Log("Запрос успешно отправлен, аккаунт создан: " + request.downloadHandler.text);
                await APICalls.Instance.GetAccount(APICalls.Instance._player.name);

                LogInfo registerLog = new LogInfo("Account registration: ", APICalls.Instance._player.name,
                    new List<Resource>());
                await APICalls.Instance.SendLogs(registerLog);

                await APICalls.Instance.GetLogs(APICalls.Instance._player.name);
            }
            else
            {
                print("A");
                Debug.Log("Ошибка: " + request.error);
                if (request.error.Contains("400"))
                {
                    register_errorLog_inputField.text = "Пользователь с таким именем уже существует.";
                }
            }
        }
    }
}



[Serializable]
public class Account
{
    public string name;
    public string password;
    public List<Resource> resources;

}

[Serializable]
public class Resource
{
    public string name;
    public int count;
}
