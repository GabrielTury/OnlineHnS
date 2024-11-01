using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private RectTransform pauseText;

    [SerializeField]
    private RectTransform clockMenuHolder;

    [SerializeField]
    private RectTransform arrowPointer;

    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRects = new List<RectTransform>(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlight = new List<SlicedFilledImage>(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTexts = new List<TextMeshProUGUI>(); // Texts of said buttons

    [SerializeField]
    private CanvasGroup mainHolder;

    [SerializeField]
    private CanvasGroup settingsHolder;

    [SerializeField]
    private CanvasGroup videoHolder;

    [SerializeField]
    private CanvasGroup audioHolder;

    [SerializeField]
    private List<GameObject> buttonsSettings = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRectsSettings = new List<RectTransform>(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightSettings = new List<SlicedFilledImage>(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsSettings = new List<TextMeshProUGUI>(); // Texts of said buttons

    [SerializeField]
    private List<GameObject> buttonsVideo = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRectsVideo = new List<RectTransform>(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightVideo = new List<SlicedFilledImage>(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsVideo = new List<TextMeshProUGUI>(); // Texts of said buttons

    [SerializeField]
    private List<GameObject> buttonsAudio = new List<GameObject>(); // Interactible buttons
    private List<RectTransform> buttonsRectsAudio = new List<RectTransform>(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightAudio = new List<SlicedFilledImage>(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsAudio = new List<TextMeshProUGUI>(); // Texts of said buttons

    [SerializeField]
    private RectTransform inputGuideHolder;

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

    [SerializeField]
    private int currentGroupIndex = 0;

    private bool isAngleAnimationRunning = true;
    #endregion

    #region Animations (Coroutines)
    private Coroutine[] scrollersCoroutines = new Coroutine[2];
    private Coroutine backdropCoroutine;
    private Coroutine clockMenuCoroutine;
    private Coroutine inputGuideCoroutine;
    private Coroutine pauseTextCoroutine;
    private Coroutine arrowPointerCoroutine;
    private Coroutine[] buttonsCoroutines;
    private Coroutine[] buttonsSettingsCoroutines;
    private Coroutine[] buttonsVideoCoroutines;
    private Coroutine[] buttonsAudioCoroutines;

    private bool[] buttonsCoroutinesRunning;
    private bool[] buttonsCoroutinesRunningHighlight;

    private bool[] buttonsSettingsCoroutinesRunning;
    private bool[] buttonsSettingsCoroutinesRunningHighlight;

    private bool[] buttonsVideoCoroutinesRunning;
    private bool[] buttonsVideoCoroutinesRunningHighlight;

    private bool[] buttonsAudioCoroutinesRunning;
    private bool[] buttonsAudioCoroutinesRunningHighlight;
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

            switch (currentButtonIndex)
            {
                case 0:
                    
                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttons.Count - 1);
                    break;

                case 1:

                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttonsSettings.Count - 1);
                    break;

            }
            
            SelectButton(currentButtonIndex);
        }
    }

    private void ConfirmButton()
    {

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

        buttonsSettingsCoroutines = new Coroutine[buttonsSettings.Count];
        buttonsSettingsCoroutinesRunning = new bool[buttonsSettings.Count];
        buttonsSettingsCoroutinesRunningHighlight = new bool[buttonsSettings.Count];

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

        for (int i = 0; i < buttonsSettings.Count; i++)
        {
            buttonsRectsSettings.Add(buttonsSettings[i].GetComponent<RectTransform>());
            buttonsHighlightSettings.Add(buttonsSettings[i].transform.GetChild(0).GetComponent<SlicedFilledImage>());
            buttonsTextsSettings.Add(buttonsSettings[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            buttonsSettingsCoroutinesRunning[i] = false;
            buttonsSettingsCoroutinesRunningHighlight[i] = false;
        }

        for (int i = 0; i < buttonsVideo.Count; i++)
        {
            buttonsRectsVideo.Add(buttonsVideo[i].GetComponent<RectTransform>());
            buttonsHighlightVideo.Add(buttonsVideo[i].transform.GetChild(0).GetComponent<SlicedFilledImage>());
            buttonsTextsVideo.Add(buttonsVideo[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            buttonsVideoCoroutinesRunning[i] = false;
            buttonsVideoCoroutinesRunningHighlight[i] = false;
        }

        for (int i = 0; i < buttonsAudio.Count; i++)
        {
            buttonsRectsAudio.Add(buttonsAudio[i].GetComponent<RectTransform>());
            buttonsHighlightAudio.Add(buttonsAudio[i].transform.GetChild(0).GetComponent<SlicedFilledImage>());
            buttonsTextsAudio.Add(buttonsAudio[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>());
            buttonsAudioCoroutinesRunning[i] = false;
            buttonsAudioCoroutinesRunningHighlight[i] = false;
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
                float t = Mathf.SmoothStep(0, 1, elapsedTime / angleAnimationSpeed);

                float angle = Mathf.Lerp(0, buttonsAngle[i], t);

                buttonsRects[i].localEulerAngles = new Vector3(0, 0, angle);

                float radians = angle * Mathf.Deg2Rad;

                Vector2 position = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * (radius + Mathf.Sin(Time.unscaledTime * waveStrength) * waveAmplitude);
            
                buttonsRects[i].anchoredPosition = position;
            }
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            buttonsRects[i].localEulerAngles = new Vector3(0, 0, buttonsAngle[i]);
        }
        isAngleAnimationRunning = false;
    }

    /// <summary>
    /// Selects/highlights the button at the specified index. Automatically gets the current group index.<br></br>
    /// Used by input devices such as keyboard and gamepads, as well as mouse hovering.
    /// </summary>
    /// <param name="index"></param>
    public void SelectButton(int index)
    {
        if (currentGroupIndex == 0)
        {
            currentButtonIndex = index;

            try { StopCoroutine(arrowPointerCoroutine); } catch { }

            arrowPointerCoroutine = StartCoroutine(UIUtils.RotateOverSecondsRectTransform(arrowPointer, new Vector3(0, 0, buttonsAngle[index]), 0.10f));

            for (int i = 0; i < buttons.Count; i++) // Do for all main buttons
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
        else if (currentGroupIndex == 1)
        {
            currentButtonIndex = index;

            for (int i = 0; i < buttonsSettings.Count; i++) // Do for all settings buttons
            {
                if (i == index) // If the button corresponds to the given index, start a highlighting coroutine
                {
                    StopHighlightAnimationAtIndex(i, 1);
                    buttonsSettingsCoroutinesRunning[i] = true;
                    buttonsSettingsCoroutinesRunningHighlight[i] = true;
                    buttonsSettingsCoroutines[i] = StartCoroutine(HighlightButton(i, true, 1));
                }
                else // If not,
                {
                    if (buttonsSettingsCoroutinesRunning[i]) // If any coroutine is already running for this button,
                    {
                        if (buttonsSettingsCoroutinesRunningHighlight[i]) // Check if it is a highlight one. If it is, stop it and start a new unhilight one
                        {
                            StopHighlightAnimationAtIndex(i, 1);
                            buttonsSettingsCoroutinesRunningHighlight[i] = false;
                            buttonsSettingsCoroutines[i] = StartCoroutine(HighlightButton(i, false, 1));
                        }
                    }
                    if (!buttonsCoroutinesRunning[i]) // If no coroutine is running, start a new unhilight one
                    {
                        buttonsSettingsCoroutinesRunning[i] = true;
                        buttonsSettingsCoroutines[i] = StartCoroutine(HighlightButton(i, false, 1));
                    }
                }
            }
        }
        
        
    }

    /// <summary>
    /// Highlights the button at the specified index. Can be used to unhighlight as well.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isHighlighted"></param>
    /// <param name="group">Selects the group of buttons. 0 is main buttons, 1 is settings, 2 is video, 3 is audio</param>
    /// <returns></returns>
    private IEnumerator HighlightButton(int index, bool isHighlighted, int group = 0)
    {
        float elapsedTime = 0;
        SlicedFilledImage bHigh;
        TextMeshProUGUI bText;
        float startingWidth;
        float startingAlpha;

        switch (group)
        {
            case 0:

                bHigh = buttonsHighlight[index];
                bText = buttonsTexts[index];
                break;

            case 1:

                bHigh = buttonsHighlightSettings[index];
                bText = buttonsTextsSettings[index];
                break;

            case 2:

                bHigh = buttonsHighlightVideo[index];
                bText = buttonsTextsVideo[index];
                break;

            case 3:

                bHigh = buttonsHighlightAudio[index];
                bText = buttonsTextsAudio[index];
                break;

            default:

                throw new Exception("Group does not exist.");
        }

        startingWidth = bHigh.fillAmount;
        startingAlpha = bText.alpha;

        if (isHighlighted)
        {
            while (elapsedTime < highlightTime)
            {
                bHigh.fillAmount = Mathf.Lerp(startingWidth, 1, (elapsedTime / highlightTime));
                bText.alpha = Mathf.Lerp(startingAlpha, 1, (elapsedTime / highlightTime));
                elapsedTime += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            bHigh.fillAmount = 1;
        } 
        else
        {
            while (elapsedTime < highlightTime)
            {
                bHigh.fillAmount = Mathf.Lerp(startingWidth, 0, (elapsedTime / highlightTime));
                bText.alpha = Mathf.Lerp(startingAlpha, 0.4f, (elapsedTime / highlightTime));
                elapsedTime += Time.unscaledDeltaTime;
                yield return new WaitForEndOfFrame();
            }

            bHigh.fillAmount = 0;
        }

        switch (group)
        {
            case 0:

                buttonsCoroutinesRunning[index] = false;
                break;

            case 1:

                buttonsSettingsCoroutinesRunning[index] = false;
                break;

            case 2:

                buttonsVideoCoroutinesRunning[index] = false;
                break;

            case 3:

                buttonsAudioCoroutinesRunning[index] = false;
                break;

        }
                
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

        clockMenuHolder.localScale = new Vector3(0, 0, 0);

        inputGuideHolder.anchoredPosition = new Vector3(0, -150, 0);

        pauseText.anchoredPosition = new Vector3(0, 600, 0);

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
        scrollers[0].uvRect = new Rect(scrollers[0].uvRect.x - (0.0015f * Time.unscaledDeltaTime), 0, 1, 1);
        scrollers[1].uvRect = new Rect(scrollers[1].uvRect.x - (0.0015f * Time.unscaledDeltaTime), 0, 1, 1);
    }

    /// <summary>
    /// Invoke the animation for all of the elements of the pause menu, including transformation and coloring + alpha.<br></br>
    /// The StopAllAnimations() method will be called before the animation starts.
    /// </summary>
    private void PopInAnimate()
    {
        StopAllMainAnimations();

        scrollersCoroutines[0] = StartCoroutine(UIUtils.MoveOverSecondsRawImage(scrollers[0], new Vector3(0, 25.5f, 0), 0.35f));
        scrollersCoroutines[1] = StartCoroutine(UIUtils.MoveOverSecondsRawImage(scrollers[1], new Vector3(0, -25.5f, 0), 0.35f));

        pauseTextCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(pauseText, new Vector3(0, 300, 0), 0.35f));

        backdropCoroutine = StartCoroutine(UIUtils.FadeColor(backdrop, new Color32(0, 0, 0, 125), 0.5f));

        inputGuideCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(inputGuideHolder, new Vector3(0, 0, 0), 0.35f));

        clockMenuCoroutine = StartCoroutine(UIUtils.ScaleOverSecondsRectTransform(clockMenuHolder, new Vector3(1, 1, 1), 0.35f));
    }

    /// <summary>
    /// Dials back the pause menu's animations.<br></br>
    /// The StopAllMainAnimations() method will be called before the animation starts.
    /// </summary>
    private void PopOutAnimate()
    {
        StopAllMainAnimations();

        scrollersCoroutines[0] = StartCoroutine(UIUtils.MoveOverSecondsRawImage(scrollers[0], new Vector3(0, -25.5f, 0), 0.35f));
        scrollersCoroutines[1] = StartCoroutine(UIUtils.MoveOverSecondsRawImage(scrollers[1], new Vector3(0, 25.5f, 0), 0.35f));

        pauseTextCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(pauseText, new Vector3(0, 600, 0), 0.35f));

        backdropCoroutine = StartCoroutine(UIUtils.FadeColor(backdrop, new Color32(0, 0, 0, 0), 0.35f));

        inputGuideCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(inputGuideHolder, new Vector3(0, -150, 0), 0.35f));

        clockMenuCoroutine = StartCoroutine(UIUtils.ScaleOverSecondsRectTransform(clockMenuHolder, new Vector3(0, 0, 0), 0.35f));
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
        try { StopCoroutine(inputGuideCoroutine); } catch { }
        try { StopCoroutine(clockMenuCoroutine); } catch { }
        try { StopCoroutine(pauseTextCoroutine); } catch { }
        try { StopCoroutine(arrowPointerCoroutine); } catch { }
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

    /// <summary>
    /// Attemps to stop a button's highlight animation at the specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="group">Selects the group of buttons. 0 is main buttons, 1 is settings, 2 is video, 3 is audio</param>
    private void StopHighlightAnimationAtIndex(int index, int group = 0)
    {
        switch (group)
        {
            case 0:
                try { StopCoroutine(buttonsCoroutines[index]); } catch { }
                break;
            case 1:
                try { StopCoroutine(buttonsSettingsCoroutines[index]); } catch { }
                break;
            case 2:
                try { StopCoroutine(buttonsVideoCoroutines[index]); } catch { }
                break;
            case 3:
                try { StopCoroutine(buttonsAudioCoroutines[index]); } catch { }
                break;
        }
    }
}
