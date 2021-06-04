using System.Collections;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This script file slowly fades out the AR transition screen, then
/// disables the Game Object so the user can go into the experience.
/// </summary>
public class ARFadeScript : MonoBehaviour
{
    [SerializeField] private float SecondsBeforeFadeBegins;

    [SerializeField] private float FadeSpeed;
    //Global Image fields are put here so they can be accessed throughout the file,
    //and can be changed in the inspector
    [SerializeField] private Image ARIcon;

    [SerializeField] private Image ARBackground;
    private void Start()
    {
        //When AR screen starts, immediately begin to fade out the screen:
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        //Holds execution for the number of seconds specified
        yield return new WaitForSeconds(SecondsBeforeFadeBegins);
        //Checks the Alpha value of the icon, if it is not 0 (invisible), 
        //The loop should continue fading
        while (ARIcon.color.a > 0)
        {
            //Get the Colour of both the Icon and Background,
            //then decrement the a alpha by 0.01, to decrease the opacity:
            var arIconColor = ARIcon.color;
            var arBackground = ARBackground.color;

            arIconColor.a -= 0.01f;
            arBackground.a -= 0.01f;
            //Set the colour to the newly faded colour:
            ARIcon.color = arIconColor;
            ARBackground.color = arBackground;
            //Wait for a set number of seconds, which is determined by 1/fadespeed, 
            //meaning if fade speed is higher, wait for a shorter amount of time:
            yield return new WaitForSeconds(1/FadeSpeed);
        }
        //When the Icon is completely faded, turn of the entire Game Object so that 
        //user can continue interacting with the AR environment.
        ARBackground.gameObject.SetActive(false);
    }
}