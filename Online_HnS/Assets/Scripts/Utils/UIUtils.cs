using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    /// <summary>
    /// Moves a given raw image from one position to another over a certain amount of time
    /// </summary>
    /// <param name="imageToMove"></param>
    /// <param name="end"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator MoveOverSecondsRaw(RawImage imageToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = imageToMove.rectTransform.anchoredPosition;
        while (elapsedTime < seconds)
        {
            imageToMove.rectTransform.anchoredPosition = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        imageToMove.rectTransform.anchoredPosition = end;
    }

    /// <summary>
    /// Fades a given image from one color to another over a certain amount of time
    /// </summary>
    /// <param name="imageToFade"></param>
    /// <param name="end"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator FadeColor(Image imageToFade, Color end, float duration)
    {
        Color startColor = imageToFade.color;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float normalizedTime = t / duration;
            imageToFade.color = Color.Lerp(startColor, end, normalizedTime);
            yield return null;
        }
        imageToFade.color = end;
    }
}
