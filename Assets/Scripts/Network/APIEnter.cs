using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class APIEnter : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button applyButton;
    public GameObject menuWin;

    private void Awake()
    {
        applyButton.onClick.AddListener( async delegate
        {
            await new RequestsManager().GetResourcesForPlayer(inputField.text);
            print("Button clicked");
            menuWin.SetActive(true);
            gameObject.SetActive(false);
            return;
        });

        // foreach (var player in new RequestsManager().GetAllPlayers().Result)
        // {
        //     print(player.Name);
        // }
    }

    public async void ApplyButtonOnClick()
    {
        
    }
    //
    // public void SaveCurrentResources(Container container)
    // {
    //     string saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
    //         
    //     Container con = new Container();
    //     Container conCurrent = JsonConvert.DeserializeObject<Container>(File.ReadAllText(saveFilePath));
    //         
    //     con = JsonConvert.DeserializeObject<Container>(File.ReadAllText(saveFilePath));
    //         
    //     con.Name = conCurrent.Name;
    //     con.Resources = new List<object>();
    //     if(conCurrent.Resources != null) con.Resources = conCurrent.Resources;
    //     con.Resources.Add(new ResourceToSave() { Count = 10, Name = "Wood" });
    //     con.Resources.Add(new TableToSave() {Position = new SerializableVector3(new Vector3(3, 5, 0))});
    //         
    //     string jsonOut = JsonConvert.SerializeObject(con, Formatting.Indented);
    //     File.WriteAllText(saveFilePath,jsonOut);
    //         
    //     print("P clicked");
    //         
    //     await new RequestsManager().UpdateResources(con);
    // }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {

        }
    }
}
