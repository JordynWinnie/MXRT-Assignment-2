using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARFadeScript : MonoBehaviour
{
    [SerializeField] Image ARIcon;
    [SerializeField] Image ARBackground;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(0.75f);
        while (ARIcon.color.a > 0)
        {
            var arIconColor = ARIcon.color;
            var arBackground = ARBackground.color;

            arIconColor.a -= 0.01f;
            arBackground.a -= 0.01f;

            ARIcon.color = arIconColor;
            ARBackground.color = arBackground;

            yield return new WaitForSeconds(0.01f);
        }

        ARBackground.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
