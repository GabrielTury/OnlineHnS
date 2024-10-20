using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    #region Object and Asset References
    [Header("--- Object References")]
    [SerializeField]
    private RawImage[] scrollers; // The scrolling checkers in the top and bottom

    [SerializeField]
    private Image backdrop; // The black background

    

    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRects = new List<RectTransform>(); // RectTransforms of said buttons
    #endregion

    #region Timelines
    [Header("--- Timelines")]
    [SerializeField]
    private int currentTimeline; // ------------------------- !!! Reminder to update this based on what the current timeline is

    [SerializeField]
    private Color[] mainColors; // The main color palette to be used in the images
                                // 0 = past, 1 = present, 2 = future
    #endregion

    #region Animation Settings
    [Header("--- Animation Settings")]
    [SerializeField]
    private float radius = 100f; // Offset of the buttons from the main handle

    [SerializeField]
    private float angleOffset = 45f; // Difference of angle between buttons
    #endregion

    #region Animations (Coroutines)
    private Coroutine[] scrollersCoroutines = new Coroutine[2];
    private Coroutine backdropCoroutine;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ResetPauseMenu();
        PositionButtons();
        PopInAnimate();

        //Debug.Log(buttons.Count.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        ScrollScrollers();
        UpdateControllerIcons();
    }

    /// <summary>
    /// Calculates the amount of buttons to order and positions them symmetrically in specified angle and offset
    /// </summary>
    private void PositionButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            //Debug.Log(i.ToString());
            buttonsRects.Add(buttons[i].GetComponent<RectTransform>());
        }

        int buttonCount = buttons.Count;
        int middleIndex = buttonCount / 2;

        for (int i = 0; i < buttonCount; i++)
        {
            float currentAngle = (middleIndex - i) * angleOffset;
            float radians = currentAngle * Mathf.Deg2Rad;

            Vector2 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;

            buttonsRects[i].anchoredPosition = position;
            buttonsRects[i].localEulerAngles = new Vector3(0, 0, currentAngle);
        }
    }

    /// <summary>
    /// Sets the controller input guide icons based on the current controller scheme.
    /// </summary>
    private void UpdateControllerIcons()
    {

    }

    /// <summary>
    /// Resets the pause menu back to its default state and refreshes the colors
    /// </summary>
    private void ResetPauseMenu()
    {
        currentTimeline = 1;

        backdrop.color = new Color(0, 0, 0, 0);

        scrollers[0].rectTransform.anchoredPosition = new Vector3(0, -25.5f, 0);
        scrollers[1].rectTransform.anchoredPosition = new Vector3(0, 25.5f, 0);
        scrollers[0].color = mainColors[currentTimeline];
        scrollers[1].color = mainColors[currentTimeline];
        scrollers[0].uvRect = new Rect(0, 0, 1, 1);
        scrollers[1].uvRect = new Rect(0, 0, 1, 1);
    }

    /// <summary>
    /// Scrolls the scrollers on the top and bottom of the screen.
    /// </summary>
    private void ScrollScrollers()
    {
        scrollers[0].uvRect = new Rect(scrollers[0].uvRect.x - (0.001f * Time.unscaledDeltaTime), 0, 1, 1);
        scrollers[1].uvRect = new Rect(scrollers[1].uvRect.x - (0.001f * Time.unscaledDeltaTime), 0, 1, 1);
    }

    /// <summary>
    /// Invoke the animation for all of the elements of the pause menu, including transformation and coloring + alpha.<br></br>
    /// The StopAllAnimations() method will be called before the animation starts.
    /// </summary>
    private void PopInAnimate()
    {
        StopAllAnimations();
        scrollersCoroutines[0] = StartCoroutine(UIUtils.MoveOverSecondsRaw(scrollers[0], new Vector3(0, 25.5f, 0), 0.35f));
        scrollersCoroutines[1] = StartCoroutine(UIUtils.MoveOverSecondsRaw(scrollers[1], new Vector3(0, -25.5f, 0), 0.35f));
        backdropCoroutine = StartCoroutine(UIUtils.FadeColor(backdrop, new Color(0, 0, 0, 125), 0.5f));
    }

    /// <summary>
    /// Attempts to stop all of the coroutines related to the pause menu animations.
    /// </summary>
    private void StopAllAnimations()
    {
        try { StopCoroutine(scrollersCoroutines[0]); } catch { }
        try { StopCoroutine(scrollersCoroutines[1]); } catch { }
        try { StopCoroutine(backdropCoroutine); } catch { }
    }
}
