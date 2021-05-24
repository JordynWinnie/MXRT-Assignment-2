using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataChecker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FilePathCheck;
    private string FilePath = string.Empty;
    private readonly string baseURL = "https://microsoft-translator-text.p.rapidapi.com/languages?api-version=3.0&scope=translation%2Ctransliteration";
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
            FilePathCheck.text = "Data Check: File Exists";
            FilePathCheck.text += "\n";
            var data = File.ReadAllText(FilePath);
            print(data);
            var jsonData = JSON.Parse(data);
            FilePathCheck.text += "Languages Found: " + jsonData.Count;
            return;
        }

        StartCoroutine(GetLanguages());
    }
    
    IEnumerator GetLanguages()
    {
        using var webRequest = UnityWebRequest.Get(baseURL);
        FilePathCheck.text = "Data Check: Downloading File...";
        webRequest.SetRequestHeader("x-rapidapi-key", "5249fbda84msh8c5a35ed399d28ep1637cdjsn0ba36b495f97");
        webRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        webRequest.SetRequestHeader("content-type", "application/json");
        webRequest.SetRequestHeader("Accept-Encoding", string.Empty);
        yield return webRequest.SendWebRequest();
        var data = webRequest.downloadHandler.data;
        var result = System.Text.Encoding.UTF8.GetString(data);
        print("Result: " + result);
        
        var languages = JSON.Parse(result);
        var transliterations = languages["transliteration"];
        var translations = languages["translation"];

        var languageNames = new JSONObject();
        foreach (var child in translations)
        {
            var langCode = child.Key;
            var langName = child.Value["name"].Value;
            languageNames.Add(langCode, langName);
        }
        print(languageNames);
       
        foreach (var child in transliterations)
        {
            var langCode = child.Key;
            if (languageNames[langCode] != null)
            {
                print(languageNames[langCode].Value);
                var langScript = child.Value["scripts"][0]["code"].Value;
                languageNames[langCode] += $";{langScript}";
            }
        }

        File.WriteAllText(FilePath, languageNames.ToString());
        
        CheckForJsonData();
    }

    public void LoadAR()
    {
        SceneManager.LoadScene(1);
    }
}
