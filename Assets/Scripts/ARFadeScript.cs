using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ARFadeScript : MonoBehaviour
{
    [SerializeField] private Image ARIcon;

    [SerializeField] private Image ARBackground;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
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
}