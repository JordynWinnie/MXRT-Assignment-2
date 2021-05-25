using System.Collections;
using System.Text;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataDownloader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FilePathCheck;
    [SerializeField] private Button StartARButton;

    private readonly string baseURL =
        "https://microsoft-translator-text.p.rapidapi.com/languages?api-version=3.0&scope=translation%2Ctransliteration";

    // Start is called before the first frame update
    private void Start()
    {
        CheckForJsonData();
    }

    private void CheckForJsonData()
    {
        if (JsonDataLoader.CheckIfFileExists())
        {
            StartARButton.interactable = true;
            FilePathCheck.text = "Data Check: File Exists";
            FilePathCheck.text += "\n";
            FilePathCheck.text += "Languages Found: " + JsonDataLoader.GetLanguageData().Count;
            return;
        }
        StartARButton.interactable = false;
        StartCoroutine(GetLanguages());
    }

    private IEnumerator GetLanguages()
    {
        using var webRequest = UnityWebRequest.Get(baseURL);
        FilePathCheck.text = "Data Check: Downloading File...";
        webRequest.SetRequestHeader("x-rapidapi-key", "5249fbda84msh8c5a35ed399d28ep1637cdjsn0ba36b495f97");
        webRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        webRequest.SetRequestHeader("content-type", "application/json");
        webRequest.SetRequestHeader("Accept-Encoding", string.Empty);
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success) FilePathCheck.text = "Error Downloading File.";

        var data = webRequest.downloadHandler.data;
        var result = Encoding.UTF8.GetString(data);
        var languages = JSON.Parse(result);
        var transliterations = languages["transliteration"];
        var translations = languages["translation"];

        var languageNames = new JSONObject();
        foreach (var child in translations)
        {
            var langCode = child.Key;

            if (langCode == "en")
                continue; //Skip English Translation as translations will always be from English > Another Language

            var langName = child.Value["name"].Value;
            languageNames.Add(langCode, langName);
        }
        
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

    public void DeleteDataAndCheckAgain()
    {
        JsonDataLoader.DeleteLanguageFile();
        CheckForJsonData();
    }
}