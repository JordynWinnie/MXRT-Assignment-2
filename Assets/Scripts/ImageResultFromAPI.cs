using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public class ImageResultFromAPI : MonoBehaviour
{
    [SerializeField] private TextMesh TextDisplay;
    public byte[] currentByteArray;
    public string CurrentObjectName = string.Empty;

    private readonly string baseImageURL = "https://api.imagga.com/v2/tags";
    private readonly List<KeyValuePair<string, decimal>> ConfidenceValues = new List<KeyValuePair<string, decimal>>();
    private TranslationAPI m_TranslationAPI;

    // Start is called before the first frame update
    private void Start()
    {
        m_TranslationAPI = GetComponent<TranslationAPI>();
        var base64String = Convert.ToBase64String(currentByteArray);
        StartCoroutine(GetImageData(base64String));

        TextDisplay.text = "Identifying Object...";
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }

    private IEnumerator GetImageData(string data)
    {
        var formData = new List<IMultipartFormSection> {new MultipartFormDataSection("image_base64", data)};
        using (var webRequest = UnityWebRequest.Post(baseImageURL, formData))
        {
            webRequest.SetRequestHeader("Authorization",
                "Basic YWNjXzYyZjczOGUyN2JjNGVhMDoxMzUyMTE2ODA0MDFkNjlmZDljNDkwZmQ2MGVmNWU2Nw==");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                TextDisplay.text = "Connection Error. Please try again.";

                Destroy(gameObject, 5f);
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

        TextDisplay.text = $"English: {ConfidenceValues[0].Key} (Confidence: {ConfidenceValues[0].Value:0.0}%)";
        CurrentObjectName = ConfidenceValues[0].Key;

        //When the object has been recognised, call the Translation API to translate the text:
        m_TranslationAPI.StartTranslation(CurrentObjectName);
    }

    public void RefreshTranslations()
    {
        //This is for error protection, if an object fails recognition, any Language Refresh should not include this
        //object
        if (CurrentObjectName.Equals(string.Empty)) return;
        m_TranslationAPI.StartTranslation(CurrentObjectName);
    }
}