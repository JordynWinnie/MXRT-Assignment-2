using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Slider ConfidenceValues;

    private void Start()
    {
        
    }
    public void ForceLanguageRedownload()
    {
        //Deletes the Language Cache, then loads the preload screen,
        //which will download the file again since it no longer exists
        DataLoader.DeleteLanguageFile();
        SceneManager.LoadScene(0);
    }

    public void StartARExperience()
    {
        SceneManager.LoadScene(2);

    }
}

