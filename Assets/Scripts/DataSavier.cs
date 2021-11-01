using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DataSavier : MonoBehaviour
{
    public static DataSavier Instance;
    public string currentPlayerName;

    [System.Serializable]
    class SaveData

    {
        public string currentPlayerName;
    }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGameInfo();
        
        

    }
    public void SaveGameInfo()
    {
        SaveData data = new SaveData();
        
        data.currentPlayerName = currentPlayerName;


        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Saved =" + data.currentPlayerName);
    }
    public void LoadGameInfo()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            currentPlayerName = data.currentPlayerName;

            GameObject inputPlayerName = GameObject.Find("InputPlayerName/Placeholder");
            inputPlayerName.GetComponent<Text>().text ="Player name: " + currentPlayerName;

            Debug.Log(currentPlayerName + currentPlayerName.ToString());
        }
    }
    public void InputPlayerName()
    {
        GameObject inputPlayerName = GameObject.Find("InputPlayerName/Text");
        currentPlayerName = inputPlayerName.GetComponent<Text>().text;
        Debug.Log("Player name inputed: " + currentPlayerName);
    }
    /* public void ClearData()
    {
        SaveData data = new SaveData();
        data.bestScore = 0;
        data.bestPlayerName = "";

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json); 

    }*/
}
