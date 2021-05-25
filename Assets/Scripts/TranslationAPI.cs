using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public class TranslationAPI : MonoBehaviour
{
    [SerializeField] TextMesh TranslationDisplay;
    private readonly string baseTranslateURL = "https://microsoft-translator-text.p.rapidapi.com/translate?api-version=3.0&textType=plain&profanityAction=NoAction";
    private readonly string baseTransliterateURL = "https://microsoft-translator-text.p.rapidapi.com/transliterate?toScript=Latn&api-version=3.0";
    IEnumerator TranslateAndTransliterate(string textToTranslate, string targetLang, string fromScript)
    {
        //Json data serialisation:
        var translateData = new JSONArray();
        var translation = new JSONObject();
        translation.Add("Text", textToTranslate);
        translateData.Add(translation);
        //Url Interpretation:
        var translateFullURL = baseTranslateURL + "&to=" + targetLang;
        //Since Post Method does not accept byte[] by default, a new WebRequest must be made
        //from scratch
        var translateBodyRaw = Encoding.UTF8.GetBytes(translateData.ToString());
        using var translatePostRequest = new UnityWebRequest(translateFullURL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(translateBodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        translatePostRequest.SetRequestHeader("Content-Type", "application/json");
        translatePostRequest.SetRequestHeader("x-rapidapi-key", "5249fbda84msh8c5a35ed399d28ep1637cdjsn0ba36b495f97");
        translatePostRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        yield return translatePostRequest.SendWebRequest();

        if (translatePostRequest.result != UnityWebRequest.Result.Success)
        {
            TranslationDisplay.text = "Failed to connect to Translation Servers.";
            yield break;
        }
        
        var translationJsonResult = JSON.Parse(translatePostRequest.downloadHandler.text);
        var translationResult = translationJsonResult[0]["translations"][0]["text"].Value;
        
        //URL interpretation:
        var transliterateFullURL = $"{baseTransliterateURL}&fromScript={fromScript}&language={targetLang}";
        var transliterateData = new JSONArray();
        var transliteration = new JSONObject();
        transliteration.Add("Text", translationResult);
        transliterateData.Add(transliteration);
        
        using var transliteratePostRequest = new UnityWebRequest(transliterateFullURL, "POST")
        {
            uploadHandler = new UploadHandlerRaw(translateBodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        transliteratePostRequest.SetRequestHeader("Content-Type", "application/json");
        transliteratePostRequest.SetRequestHeader("x-rapidapi-key", "5249fbda84msh8c5a35ed399d28ep1637cdjsn0ba36b495f97");
        transliteratePostRequest.SetRequestHeader("x-rapidapi-host", "microsoft-translator-text.p.rapidapi.com");
        yield return transliteratePostRequest.SendWebRequest();
        
        if (transliteratePostRequest.result != UnityWebRequest.Result.Success)
        {
            TranslationDisplay.text = "Failed to connect to Translation Servers.";
            yield break;
        }
        var transliterationJsonResult = JSON.Parse(transliteratePostRequest.downloadHandler.text);
        var transliterationResult = transliterationJsonResult[0]["text"].Value;
        
        
    }
}
