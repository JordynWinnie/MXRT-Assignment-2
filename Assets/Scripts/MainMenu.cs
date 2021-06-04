using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown DefaultLanguageDropdown;
    [SerializeField] private TextMeshProUGUI Title;
    public void ForceLanguageRedownload()
    {
        //Deletes the Language Cache, then loads the preload screen,
        //which will download the file again since it no longer exists
        DataLoader.DeleteLanguageFile();
        SceneManager.LoadScene(0);
    }
    //Loads the AR Scene:
    public void StartARExperience()
    {
        SceneManager.LoadScene(2);
    }
    //Modifies the current language selection to the one the user wants to change to
    //this is called by the DefaultLangugeDropdown OnValueChanged method:
    public void ChangeDefaultLanguage()
    {
        DataLoader.SetUpUserDefaultLang(DefaultLanguageDropdown.value);
    }
    //Used by the Settings and View collection buttons to change the header of the app:
    public void ChangeTitle(string title)
    {
        Title.text = title;
    }
    //Helper method for the Settings Button to clear all of the Collection data from the user's device
    public void ClearAllCollectedItems()
    {
        DataLoader.DeleteCollectionFile();
    }
    //Helper method to exit the application;
    public void QuitApplication()
    {
        Application.Quit();
    }
}

