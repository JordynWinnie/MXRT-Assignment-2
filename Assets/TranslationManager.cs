using UnityEngine;
using UnityEngine.SceneManagement;

public class TranslationManager : MonoBehaviour
{
    //This will be the only reference to access any of the other managers,
    //this decreases the amount of Singletons created:
    public static TranslationManager Instance;
    private TranslationDropdown m_TranslationDropdownManager;
    private TranslationAnchorCreator m_TranslationAnchorManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    //Helper function to access the Language Currently selected:
    public LanguageModel ReturnSelectedLanguageInfo()
    {
        return m_TranslationDropdownManager.ReturnSelectedLanguageInfo();
    }

    private void Start()
    {
        m_TranslationAnchorManager = GetComponent<TranslationAnchorCreator>();
        m_TranslationDropdownManager = GetComponent<TranslationDropdown>();
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
        SceneManager.LoadScene(0);
    }
}
