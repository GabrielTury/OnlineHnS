using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class UIUtils : MonoBehaviour
{
    /// <summary>
    /// Wraps a given number variable between a min and max. Only includes the max.
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
    /// Wraps a given float variable between a min and max. Only includes the max.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="x_max"></param>
    /// <param name="x_min"></param>
    /// <returns></returns>
    public static float Wrap(float x, float x_max, float x_min)
    {
        return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
    }
    public static float WrapFloat(float x, float x_max, float x_min)
    {
        return ((x - x_min) % (x_max - x_min) + (x_max - x_min)) % (x_max - x_min) + x_min;
    }

    /// <summary>
    /// Moves a given rect transform from one position to another over a certain amount of time.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="end"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator MoveOverSecondsRectTransform(RectTransform rect, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = rect.anchoredPosition;
        while (elapsedTime < seconds)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / seconds);
            rect.anchoredPosition = Vector3.Lerp(startingPos, end, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        rect.anchoredPosition = end;
    }

    /// <summary>
    /// Rotates a given rect transform from one rotation to another over a certain amount of time.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="end"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator RotateOverSecondsRectTransform(RectTransform rect, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingAngle = rect.localEulerAngles;
        while (elapsedTime < seconds)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / seconds);
            rect.localEulerAngles = UIUtils.AngleLerp(startingAngle, end, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        rect.localEulerAngles = end;
    }

    /// <summary>
    /// Lerps a given Vector3 from one angle to another over a certain amount of time.<br></br>
    /// This method is used in place of basic Vector3.Lerp() when lerping angles to avoid 360 degree jumps.
    /// </summary>
    /// <param name="startAngle"></param>
    /// <param name="finishAngle"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 AngleLerp(Vector3 startAngle, Vector3 finishAngle, float t)
    {
        float xLerp = Mathf.LerpAngle(startAngle.x, finishAngle.x, t);
        float yLerp = Mathf.LerpAngle(startAngle.y, finishAngle.y, t);
        float zLerp = Mathf.LerpAngle(startAngle.z, finishAngle.z, t);
        Vector3 Lerped = new Vector3(xLerp, yLerp, zLerp);
        return Lerped;
    }

    /// <summary>
    /// Scales a given rect transform from one scale to another over a certain amount of time.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="end"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator ScaleOverSecondsRectTransform(RectTransform rect, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = rect.localScale;
        while (elapsedTime < seconds)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / seconds);
            rect.localScale = Vector3.Lerp(startingPos, end, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        rect.localScale = end;
    }

    /// <summary>
    /// Moves a given raw image from one position to another over a certain amount of time.
    /// </summary>
    /// <param name="imageToMove"></param>
    /// <param name="end"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static IEnumerator MoveOverSecondsRawImage(RawImage imageToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = imageToMove.rectTransform.anchoredPosition;
        while (elapsedTime < seconds)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / seconds);
            imageToMove.rectTransform.anchoredPosition = Vector3.Lerp(startingPos, end, t);
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        imageToMove.rectTransform.anchoredPosition = end;
    }

    /// <summary>
    /// Fades a given image from one color to another over a certain amount of time.
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
            float tl = Mathf.SmoothStep(0, 1, t / duration);
            imageToFade.color = Color32.Lerp(startColor, end, tl);
            yield return new WaitForEndOfFrame();
        }
        imageToFade.color = end;
    }

    /// <summary>
    /// Fades a given float from one value to another over a certain amount of time.
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="end"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static IEnumerator FadeFloat(float variable, float end, float duration)
    {
        float time = 0;
        float current = variable;
        while (time < duration)
        {
            current = Mathf.Lerp(current, end, time / duration);
            variable = current;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        variable = end;
    }

    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float end, float duration)
    {
        float time = 0;
        float currentAlpha = canvasGroup.alpha;
        while (time < duration)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, end, time / duration);
            time += Time.deltaTime;
            canvasGroup.alpha = currentAlpha;
            yield return new WaitForEndOfFrame();
        }
        currentAlpha = end;
        canvasGroup.alpha = currentAlpha;
    }
}
