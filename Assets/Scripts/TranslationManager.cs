using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TranslationManager : MonoBehaviour
{
    //This will be the only reference to access any of the other managers,
    //this decreases the amount of Singletons created:
    //It also provides a convinient location for Button OnClick() functions instead
    //of references many different items:
    public static TranslationManager Instance;
    public ARRaycastManager RaycastManager;
    public ARAnchorManager AnchorManager;
    public ARPlaneManager PlaneManager;
    private TranslationAnchorCreator m_TranslationAnchorManager;
    private TranslationDropdown m_TranslationDropdownManager;

    //Singleton initaliser:
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        m_TranslationAnchorManager = GetComponent<TranslationAnchorCreator>();
        m_TranslationDropdownManager = GetComponent<TranslationDropdown>();
    }

    //Helper function to access the Language Currently selected:
    public LanguageModel ReturnSelectedLanguageInfo()
    {
        return m_TranslationDropdownManager.ReturnSelectedLanguageInfo();
    }

    //Called by the TrashCan Button:
    public void ClearAllTranslation()
    {
        m_TranslationAnchorManager.RemoveAllAnchors();
    }

    //Called when the user selects another language in the drop down:
    public void RefreshAllTranslation()
    {
        m_TranslationAnchorManager.RefreshAllTranslations();
    }

    //Called by the Close button
    public void ExitAR()
    {
        SceneManager.LoadScene(1);
    }
}