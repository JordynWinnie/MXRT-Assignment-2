using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class DataChecker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FilePathCheck;
    private string FilePath = string.Empty;
    // Start is called before the first frame update
    void Start()
    {
        FilePath = Application.persistentDataPath + "/LanguageCache.json";
        print(FilePath);
        CheckForJsonData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckForJsonData()
    {

        if (File.Exists(FilePath))
        {
            FilePathCheck.text = "Persistent Data Check: File Exists";
            FilePathCheck.text += "\n";
            var data = File.ReadAllText(FilePath);
            var jsonData = JSON.Parse(data);
            print(jsonData.ToString());
            FilePathCheck.text += jsonData.ToString();
            return;
        }

        JSONObject test = new JSONObject();

        test.Add("hello", "world");

        File.WriteAllText(FilePath, test.ToString());
    }
    
}
