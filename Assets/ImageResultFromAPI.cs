using UnityEngine.Networking;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine;

public class ImageResultFromAPI : MonoBehaviour
{
    private readonly string baseUrl = "https://api.imagga.com/v2/tags";

    public byte[] currentByteArray = null;

    private List<string> Top3Confidence = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        var base54String = Convert.ToBase64String(currentByteArray);
        StartCoroutine(GetJsonData(base54String));
    }
    

    IEnumerator GetJsonData(string data)
    {
        var formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("image_base64", data));
        var www = UnityWebRequest.Post(baseUrl, formData);
        www.SetRequestHeader("Authorization", "Basic YWNjXzYyZjczOGUyN2JjNGVhMDoxMzUyMTE2ODA0MDFkNjlmZDljNDkwZmQ2MGVmNWU2Nw==");
        yield return www.SendWebRequest();

        var imageInformation = JSON.Parse(www.downloadHandler.text);
        var lol = imageInformation["result"]["tags"];
        
        
        foreach (var child in lol.Children.Take(3))
        {
            var confidence = child["confidence"];
            var tag = child["tag"]["en"];
            
            Top3Confidence.Add($"Name: {tag.ToString().Replace("\"","")} Confidence: {confidence}");
        }

        GetComponentInChildren<TextMesh>().text = string.Join(", ", Top3Confidence);
    }
    
}
