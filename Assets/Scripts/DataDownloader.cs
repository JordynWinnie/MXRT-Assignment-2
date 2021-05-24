using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataDownloader : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI FilePathCheck;
    private readonly string baseURL = "https://microsoft-translator-text.p.rapidapi.com/languages?api-version=3.0&scope=translation%2Ctransliteration";
    // Start is called before the first frame update
    void Start()
    {
        CheckForJsonData();
    }
    
    void CheckForJsonData()
    {

        if (JsonDataLoader.CheckIfFileExists())
        {
            FilePathCheck.text = "Data Check: File Exists";
            FilePathCheck.text += "\n";
            FilePathCheck.text += "Languages Found: " + JsonDataLoader.GetLanguageData().Count;
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

        JsonDataLoader.WriteToJson(languageNames.ToString());
        
        CheckForJsonData();
    }

    public void LoadAR()
    {
        SceneManager.LoadScene(1);
    }
}
