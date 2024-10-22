using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UIUtils : MonoBehaviour
{
    /// <summary>
    /// Wraps a given number variables between a min and max. Only includes the max.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="x_max"></param>
    /// <param name="x_min"></param>
    /// <returns></returns>
    public static int Wrap(int x, int x_max, int x_min)
    {
        return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
    }

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
    public static IEnumerator FadeColor(Image imageToFade, Color32 end, float duration)
    {
        Color32 startColor = imageToFade.color;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            float normalizedTime = t / duration;
            imageToFade.color = Color32.Lerp(startColor, end, normalizedTime);
            yield return null;
        }
        imageToFade.color = end;
    }

    public static IEnumerator FadeFloat(float variable, float end, float duration)
    {
        float time = 0;
        float current = variable;
        while (time < duration)
        {
            current = Mathf.Lerp(current, end, time / duration);
            time += Time.deltaTime;
            variable = current;
            yield return null;
        }
        current = end;
        variable = current;
    }
}
