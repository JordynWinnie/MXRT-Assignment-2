using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranslationDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown DropdownMenu;

    private List<LanguageModel> LanguageListInfo;

    private List<string> LanguageListDisplay = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        LanguageListInfo = JsonDataLoader.LoadLanguageList();
        
        //This sets up the Languages that the user can SEE:
        foreach (var language in LanguageListInfo)
        {
            LanguageListDisplay.Add(language.LanguageDisplayName);
        }
        
        DropdownMenu.AddOptions(LanguageListDisplay);
        var optionData = DropdownMenu.options.FindIndex(x => x.text == "Japanese");
        DropdownMenu.SetValueWithoutNotify(optionData);
    }
    public LanguageModel ReturnSelectedLanguageInfo()
    {
        return LanguageListInfo[DropdownMenu.value];
    }
}
