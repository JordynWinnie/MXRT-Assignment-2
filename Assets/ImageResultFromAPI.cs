using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using TMPro;

public class ImageResultFromAPI : MonoBehaviour
{
    [SerializeField] TextMesh TextDisplay;
    [SerializeField] TextMesh TranslationDisplay;

    private readonly string baseImageURL = "https://api.imagga.com/v2/tags";
    private readonly string baseTranslationURL = "https://systran-systran-platform-for-language-processing-v1.p.rapidapi.com/translation/text/translate";

    public byte[] currentByteArray = null;

    private List<KeyValuePair<string, decimal>> ConfidenceValues = new List<KeyValuePair<string, decimal>>();
    
    // Start is called before the first frame update
    void Start()
    {
        var base54String = Convert.ToBase64String(currentByteArray);
        StartCoroutine(GetImageData(base54String));

        TextDisplay.text = "Identifying Object...";
        TranslationDisplay.text = string.Empty;
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }

    IEnumerator GetImageData(string data)
    {
        var formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("image_base64", data));
        using (var webRequest = UnityWebRequest.Post(baseImageURL, formData))
        {
            webRequest.SetRequestHeader("Authorization", "Basic YWNjXzYyZjczOGUyN2JjNGVhMDoxMzUyMTE2ODA0MDFkNjlmZDljNDkwZmQ2MGVmNWU2Nw==");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                TextDisplay.text = "Connection Error. Please try again.";
                yield break;
            }

            var imageInformation = JSON.Parse(webRequest.downloadHandler.text);
            var results = imageInformation["result"]["tags"];


            foreach (var child in results.Children.Take(3))
            {
                var confidence = decimal.Parse(child["confidence"]);
                var identification = child["tag"]["en"].ToString();

                ConfidenceValues.Add(new KeyValuePair<string, decimal>(identification, confidence));
            }
        }
        TextDisplay.text = $"English: {ConfidenceValues[0].Key} (Confidence: {ConfidenceValues[1].Value:0.0}%)";
        StartCoroutine(GetTranslationData(ConfidenceValues[0].Key, "ja"));
    }

    IEnumerator GetTranslationData(string translation, string targetLang)
    {
        TranslationDisplay.text = "Getting Translation...";
        var source = "en";
        var finalUrl = $"{baseTranslationURL}?source={source}&target={targetLang}&input={translation}";
        using (var webRequest = UnityWebRequest.Get(finalUrl))
        {
            webRequest.SetRequestHeader("x-rapidapi-key", "5249fbda84msh8c5a35ed399d28ep1637cdjsn0ba36b495f97");
            webRequest.SetRequestHeader("x-rapidapi-host", "systran-systran-platform-for-language-processing-v1.p.rapidapi.com");


            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                TextDisplay.text = "Error Connecting to Translation Servers.";
                yield break;
            }

            var translationJson = JSON.Parse(webRequest.downloadHandler.text);
            var finalTranslation = translationJson[0][0]["output"].ToString().Replace("\"", string.Empty);

            TranslationDisplay.text = $"Japanese: {finalTranslation}";
        }
    }
}
