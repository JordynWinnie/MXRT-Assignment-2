using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggle : MonoBehaviour
{
    [SerializeField] private Image FlashIcon;
    [SerializeField] private Sprite FlashOnIcon;
    [SerializeField] private Sprite FlashOffIcon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleFlash()
    {
        if (FlashIcon.sprite == FlashOnIcon)
        {
            
        }
        FlashIcon.sprite = FlashIcon.sprite == FlashOnIcon ? FlashOffIcon : FlashOnIcon;
    }
}
