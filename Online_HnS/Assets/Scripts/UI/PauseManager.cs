using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PauseManager : MonoBehaviour
{
    private UIControls inputActions;

    #region Object and Asset References
    [Header("--- Object and Asset References")]
    [SerializeField]
    private RawImage[] scrollers; // The scrolling checkers in the top and bottom

    [SerializeField]
    private Image backdrop; // The black background

    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRects = new List<RectTransform>(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlight = new List<SlicedFilledImage>(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTexts = new List<TextMeshProUGUI>(); // Texts of said buttons

    [SerializeField]
    private UIControllerIcons[] inputIcons; // The icons for each input type

    [System.Serializable]
    private struct InputGuideIcons
    {
        public Image navigateUpDown;
        public Image confirm;
        public Image cancel;
    }

    [SerializeField]
    private InputGuideIcons inputGuideIcons;
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

    [SerializeField]
    private float angleAnimationSpeed = 0.8f;

    [SerializeField]
    private float waveStrength = 0.5f; // Strength of the wave animation

    [SerializeField]
    private float waveAmplitude = 0.5f; // Amplitude of the wave animation

    private float[] buttonsAngle;

    [SerializeField]
    private float highlightTime; // Time it takes for the button to highlight

    [SerializeField]
    private int currentButtonIndex = 0;

    private bool isAngleAnimationRunning = true;
    #endregion

    #region Animations (Coroutines)
    private Coroutine[] scrollersCoroutines = new Coroutine[2];
    private Coroutine backdropCoroutine;
    private Coroutine[] buttonsCoroutines;
    private bool[] buttonsCoroutinesRunning;
    private bool[] buttonsCoroutinesRunningHighlight;
    #endregion

    private void OnEnable()
    {
        GameEvents.UIInputMade += UpdateControllerIcons;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        GameEvents.UIInputMade -= UpdateControllerIcons;
        inputActions.Disable();
    }

    private void Awake()
    {
        inputActions = new UIControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPauseMenu();
        InitializeButtons();
        PopInAnimate();
        SelectButton(0);
    }

    // Update is called once per frame
    void Update()
    {
        ScrollScrollers();
        WaveButtons();
        CheckInput();
    }

    private void CheckInput()
    {
        if (inputActions.UI.UpDown.WasPressedThisFrame())
        {
            currentButtonIndex -= (int)inputActions.UI.UpDown.ReadValue<float>();
            currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttons.Count - 1);
            SelectButton(currentButtonIndex);
        }
    }

    /// <summary>
    /// Calculates the amount of buttons to order and positions them symmetrically in specified angle and offset.<br></br>
    /// Gets the highlight and text children as well.
    /// </summary>
    /// <returns>
    /// - Sets the position and angle of each button<br></br>
    /// - Gets the highlight and text children and sets them into buttonsHighlights and buttonsTexts<br></br>
    /// - Sets the buttonsCoroutinesRunning array
    /// </returns>
    private void InitializeButtons()
    {
        buttonsAngle = new float[buttons.Count];
        buttonsCoroutines = new Coroutine[buttons.Count];
        buttonsCoroutinesRunning = new bool[buttons.Count];
        buttonsCoroutinesRunningHighlight = new bool[buttons.Count];

        int buttonCount = buttons.Count;
        float middleIndex = buttonCount / 2;

        for (int i = 0; i < buttonCount; i++)
        {
            float currentAngle = (middleIndex - (i + (buttonCount % 2 == 0 ? 0.5f : 0))) * angleOffset;
            float radians = currentAngle * Mathf.Deg2Rad;

            Vector2 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;

            buttonsRects.Add(buttons[i].GetComponent<RectTransform>());
            buttonsRects[i].anchoredPosition = position;
            buttonsRects[i].localEulerAngles = new Vector3(0, 0, currentAngle);
            buttonsAngle[i] = currentAngle;

            buttonsHighlight.Add(buttons[i].transform.GetChild(0).GetComponent<SlicedFilledImage>());
            buttonsTexts.Add(buttons[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            buttonsCoroutinesRunning[i] = false;
            buttonsCoroutinesRunningHighlight[i] = false;
        }

        StartCoroutine(ButtonsAngleOffsetAnimation());

    }

    /// <summary>
    /// Expands the angle of the buttons from 0 to their specified angle at a set speed.
    /// </summary>
    private IEnumerator ButtonsAngleOffsetAnimation()
    {
        float elapsedTime = 0;
        while (elapsedTime < angleAnimationSpeed)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float angle = Mathf.Lerp(0, buttonsAngle[i], (elapsedTime / angleAnimationSpeed));

                buttonsRects[i].localEulerAngles = new Vector3(0, 0, angle);

                float radians = angle * Mathf.Deg2Rad;

                Vector2 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * (radius + Mathf.Sin(Time.unscaledTime * waveStrength) * waveAmplitude);
            
                buttonsRects[i].anchoredPosition = position;
            }
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        isAngleAnimationRunning = false;
    }

    /// <summary>
    /// Selects/highlights the button at the specified index.<br></br>
    /// Used by input devices such as keyboard and gamepads, as well as mouse hovering.
    /// </summary>
    /// <param name="index"></param>
    public void SelectButton(int index)
    {
        currentButtonIndex = index;

        //StopHighlightAnimations();

        for (int i = 0; i < buttons.Count; i++) // Do for all buttons
        {
            if (i == index) // If the button corresponds to the given index, start a highlighting coroutine
            {
                StopHighlightAnimationAtIndex(i);
                buttonsCoroutinesRunning[i] = true;
                buttonsCoroutinesRunningHighlight[i] = true;
                buttonsCoroutines[i] = StartCoroutine(HighlightButton(i, true));
            } 
            else // If not,
            {
                if (buttonsCoroutinesRunning[i]) // If any coroutine is already running for this button,
                {
                    if (buttonsCoroutinesRunningHighlight[i]) // Check if it is a highlight one. If it is, stop it and start a new unhilight one
                    {
                        StopHighlightAnimationAtIndex(i);
                        buttonsCoroutinesRunningHighlight[i] = false;
                        buttonsCoroutines[i] = StartCoroutine(HighlightButton(i, false));
                    }
                }
                if (!buttonsCoroutinesRunning[i]) // If no coroutine is running, start a new unhilight one
                {
                    buttonsCoroutinesRunning[i] = true;
                    buttonsCoroutines[i] = StartCoroutine(HighlightButton(i, false));
                }
            }
        }
        
    }

    /// <summary>
    /// Highlights the button at the specified index. Can be used to unhighlight as well.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isHighlighted"></param>
    /// <returns></returns>
    private IEnumerator HighlightButton(int index, bool isHighlighted)
    {
        float elapsedTime = 0;
        float startingWidth = buttonsHighlight[index].fillAmount;
        float startingAlpha = buttonsTexts[index].alpha;
        if (isHighlighted)
        {
            while (elapsedTime < highlightTime)
            {
                buttonsHighlight[index].fillAmount = Mathf.Lerp(startingWidth, 1, (elapsedTime / highlightTime));
                buttonsTexts[index].alpha = Mathf.Lerp(startingAlpha, 1, (elapsedTime / highlightTime));
                elapsedTime += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            buttonsHighlight[index].fillAmount = 1;
        } 
        else
        {
            while (elapsedTime < highlightTime)
            {
                buttonsHighlight[index].fillAmount = Mathf.Lerp(startingWidth, 0, (elapsedTime / highlightTime));
                buttonsTexts[index].alpha = Mathf.Lerp(startingAlpha, 0.4f, (elapsedTime / highlightTime));
                elapsedTime += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            buttonsHighlight[index].fillAmount = 0;
        }

        buttonsCoroutinesRunning[index] = false;
        
        yield return null;
    }

    /// <summary>
    /// Animates the waving of the selectable buttons.
    /// </summary>
    private void WaveButtons()
    {
        if (!isAngleAnimationRunning)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                float radians = buttonsAngle[i] * Mathf.Deg2Rad;

                float offset = Mathf.Sin(Time.unscaledTime * waveStrength) * waveAmplitude;

                Vector3 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * (radius + offset);

                buttonsRects[i].anchoredPosition = position;
            }
        }
    }

    /// <summary>
    /// Sets the controller input guide icons based on the current controller scheme.<br></br>
    /// Is subscribed to and gets input info from the OnUIInputMade event.
    /// </summary>
    private void UpdateControllerIcons(InputDevice inputDevice)
    {
        if (inputDevice.name.Contains("Keyboard"))
        {
            inputGuideIcons.navigateUpDown.sprite = inputIcons[0].updown;
            inputGuideIcons.confirm.sprite = inputIcons[0].a;
            inputGuideIcons.cancel.sprite = inputIcons[0].b;
        }
        else if (inputDevice.name.Contains("XInput"))
        {
            inputGuideIcons.navigateUpDown.sprite = inputIcons[1].updown;
            inputGuideIcons.confirm.sprite = inputIcons[1].a;
            inputGuideIcons.cancel.sprite = inputIcons[1].b;
        }
        else if (inputDevice.name.Contains("DualShock"))
        {
            inputGuideIcons.navigateUpDown.sprite = inputIcons[2].updown;
            inputGuideIcons.confirm.sprite = inputIcons[2].a;
            inputGuideIcons.cancel.sprite = inputIcons[2].b;
        }
        Debug.Log(inputDevice.name);
    }

    /// <summary>
    /// Resets the pause menu back to its default state and refreshes the colors.
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
        StopAllMainAnimations();
        scrollersCoroutines[0] = StartCoroutine(UIUtils.MoveOverSecondsRaw(scrollers[0], new Vector3(0, 25.5f, 0), 0.35f));
        scrollersCoroutines[1] = StartCoroutine(UIUtils.MoveOverSecondsRaw(scrollers[1], new Vector3(0, -25.5f, 0), 0.35f));
        backdropCoroutine = StartCoroutine(UIUtils.FadeColor(backdrop, new Color32(0, 0, 0, 125), 0.5f));
    }

    /// <summary>
    /// Attempts to stop all of the coroutines related to the pause menu animations.<br></br>
    /// Does NOT include button highlighting animations.
    /// </summary>
    private void StopAllMainAnimations()
    {
        try { StopCoroutine(scrollersCoroutines[0]); } catch { }
        try { StopCoroutine(scrollersCoroutines[1]); } catch { }
        try { StopCoroutine(backdropCoroutine); } catch { }
    }

    /// <summary>
    /// Attemps to stop all of the coroutines related to the buttons highlight animations.
    /// </summary>
    private void StopHighlightAnimations()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            try { StopCoroutine(buttonsCoroutines[i]); } catch { }
        }
        
    }

    private void StopHighlightAnimationAtIndex(int index)
    {
        try { StopCoroutine(buttonsCoroutines[index]); } catch { }
    }
}
