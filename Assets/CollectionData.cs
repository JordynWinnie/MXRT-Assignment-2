using UnityEngine;
using UnityEngine.UI;

public class CollectionData : MonoBehaviour
{
    //Represents a Template item for the List:
    [SerializeField] private Collection CollectedWordPrefab;
    [SerializeField] private Transform ListviewParent;
    //This represents a guide to the user to use AR when they currently have no collected anything
    [SerializeField] private GameObject HintToUser;
    private void Start()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        //Check if the CollectionFile exists:
        if (!DataLoader.CheckIfFileExists(DataLoader.GetCollectionPath())) return;
        //Disable the hint if there are phrases
        HintToUser.SetActive(false);
        
        //Destroy the current list
        foreach (Transform child in ListviewParent)
        {
            Destroy(child.gameObject);
        }
        
        //re-populate list with all the user's collected words:
        foreach (var word in DataLoader.LoadCollectionList())
        {
            //The prefab instantiated is a template of an element in a Listview:
            var collectedWordInstance = Instantiate(CollectedWordPrefab, ListviewParent);
            collectedWordInstance.CollectionItemName = word;
        }
    }

}
