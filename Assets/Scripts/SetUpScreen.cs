using UnityEngine;
using UnityEngine.SceneManagement;

public class SetUpScreen : MonoBehaviour
{
    [SerializeField] TranslationDropdown LanguageSelection;
    public void ShowMainMenu()
    {
        //Set the Default Language to the one the user selected:
        DataLoader.SetUpUserDefaultLang(LanguageSelection.ReturnDropdownIdx());
        //Load the MainMenu:
        SceneManager.LoadScene(1);
    }
}
