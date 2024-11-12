using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
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
    private List<GameObject> buttons = new(); // Interactible buttons
    private List<RectTransform> buttonsRects = new(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlight = new(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTexts = new(); // Texts of said buttons

    [SerializeField]
    private RectTransform mainHolder;

    [SerializeField]
    private CanvasGroup mainHolderCanvasGroup;

    [SerializeField]
    private RectTransform settingsHolder;

    [SerializeField]
    private CanvasGroup settingsHolderCanvasGroup;

    [SerializeField]
    private RectTransform videoHolder;

    [SerializeField]
    private CanvasGroup videoHolderCanvasGroup;

    [SerializeField]
    private RectTransform audioHolder;

    [SerializeField]
    private CanvasGroup audioHolderCanvasGroup;

    [SerializeField]
    private List<GameObject> buttonsSettings = new(); // Interactible buttons
    private List<RectTransform> buttonsRectsSettings = new(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightSettings = new(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsSettings = new(); // Texts of said buttons

    [SerializeField]
    private List<GameObject> buttonsVideo = new(); // Interactible buttons
    private List<RectTransform> buttonsRectsVideo = new(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightVideo = new(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsVideo = new(); // Texts of said buttons

    [SerializeField]
    private List<GameObject> buttonsAudio = new(); // Interactible buttons
    private List<RectTransform> buttonsRectsAudio = new(); // RectTransforms of said buttons
    private List<SlicedFilledImage> buttonsHighlightAudio = new(); // Highlights of said buttons
    private List<TextMeshProUGUI> buttonsTextsAudio = new(); // Texts of said buttons

    [SerializeField]
    private TextMeshProUGUI resolutionText;

    [SerializeField]
    private TextMeshProUGUI windowTypeText;

    [SerializeField]
    private TextMeshProUGUI framerateText;

    [SerializeField]
    private GameObject vsyncIndicator;

    [SerializeField]
    private GameObject closedCaptionIndicator;

    [SerializeField]
    private RectTransform inputGuideHolder;

    [SerializeField]
    private TextMeshProUGUI masterAudioPercentageText;

    [SerializeField]
    private TextMeshProUGUI musicAudioPercentageText;

    [SerializeField]
    private TextMeshProUGUI soundAudioPercentageText;

    [SerializeField]
    private Slider masterAudioSlider;

    [SerializeField]
    private Slider musicAudioSlider;

    [SerializeField]
    private Slider soundAudioSlider;

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

    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private FullScreenMode[] fullScreenModes;

    private int[,] resolutions = new int[9, 2] { { 854, 480 }, { 1024, 576 }, { 1280, 720 }, { 1366, 768 }, { 1600, 900 }, { 1920, 1080 }, { 2560, 1440 }, { 3840, 2160 }, { 7680, 4320 } };

    List<int[]> availableResolutions = new List<int[]>();

    int resolutionIndex;

    int fullscreenModeIndex;

    int[] maxFramerate = new int[] { 60, 120, 144, 165, 240, 300, 500 };
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

    private Coroutine mainHolderRectTransformCoroutine;
    private Coroutine mainHolderCanvasGroupCoroutine;

    private Coroutine settingsHolderRectTransformCoroutine;
    private Coroutine settingsHolderCanvasGroupCoroutine;

    private Coroutine videoHolderRectTransformCoroutine;
    private Coroutine videoHolderCanvasGroupCoroutine;

    private Coroutine audioHolderRectTransformCoroutine;
    private Coroutine audioHolderCanvasGroupCoroutine;
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
        InitializeSettings();
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

            switch (currentGroupIndex)
            {
                case 0:
                    
                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttons.Count - 1);
                    break;

                case 1:

                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttonsSettings.Count - 1);
                    break;

                case 2:

                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttonsVideo.Count - 1);
                    break;

                case 3:

                    currentButtonIndex = UIUtils.Wrap(currentButtonIndex, -1, buttonsAudio.Count - 1);
                    break;

            }
            
            SelectButton(currentButtonIndex);
        }

        if (inputActions.UI.Confirm.WasPressedThisFrame())
        {
            ConfirmButton();
        }

        if (inputActions.UI.Return.WasPressedThisFrame())
        {
            ReturnButton();
        }

        if (inputActions.UI.LeftRight.WasPressedThisFrame())
        {
            LeftRightButton(inputActions.UI.LeftRight.ReadValue<float>());
        }
    }

    #region Button Behaviors
    private void BTResumeGame()
    {
        SetButtonMouseFocus(9);
        PopOutAnimate();
    }

    public void BTSettings()
    {
        try { StopCoroutine(mainHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(mainHolderCanvasGroupCoroutine); } catch { }

        try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

        mainHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(mainHolder, new Vector3(-640, 0, 0), 0.35f));
        mainHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(mainHolderCanvasGroup, 0.35f, 0.35f));

        settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(-160, 0, 0), 0.35f));
        settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 1f, 0.35f));

        SetButtonMouseFocus(1);

        currentGroupIndex = 1;

        SelectButton(0);
    }

    public void BTMainMenu()
    {

    }

    public void BTVideoSettings()
    {
        try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

        try { StopCoroutine(videoHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(videoHolderCanvasGroupCoroutine); } catch { }

        try { StopCoroutine(audioHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(audioHolderCanvasGroupCoroutine); } catch { }

        settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(-160, 0, 0), 0.35f));
        settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 0.4f, 0.35f));

        videoHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(videoHolder, new Vector3(270, 0, 0), 0.35f));
        videoHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(videoHolderCanvasGroup, 1, 0.35f));

        audioHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(audioHolder, new Vector3(600, 0, 0), 0.35f));
        audioHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(audioHolderCanvasGroup, 0, 0.35f));

        SetButtonMouseFocus(2);

        currentGroupIndex = 2;

        SelectButton(0);
    }

    public void BTResolution(int index = 1)
    {
        resolutionIndex = UIUtils.Wrap(resolutionIndex + index, -1, availableResolutions.Count - 1);

        resolutionText.text = availableResolutions[resolutionIndex][0].ToString() + " x " + availableResolutions[resolutionIndex][1].ToString();

        PlayerPrefs.SetInt("RESOLUTION_INDEX", resolutionIndex);
    }

    public void BTWindowType(int index = 1)
    {
        fullscreenModeIndex = UIUtils.Wrap(fullscreenModeIndex + index, -1, 2);

        if (fullscreenModeIndex == 0)
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "WINDOWED");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        } else if (fullscreenModeIndex == 1)
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "BORDERLESS");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        }
        else
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "EXCLUSIVEFULLSCREEN");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        }

        PlayerPrefs.SetInt("WINDOWTYPE", fullscreenModeIndex);
    }

    public void BTVsync()
    {
        PlayerPrefs.SetInt("VSYNC", 1 - PlayerPrefs.GetInt("VSYNC", 0));

        vsyncIndicator.SetActive(PlayerPrefs.GetInt("VSYNC", 0) == 1);
    }

    public void BTFramerate(int index = 1)
    {
        PlayerPrefs.SetInt("MAX_FRAMERATE", UIUtils.Wrap(PlayerPrefs.GetInt("MAX_FRAMERATE", 0) + index, -1, maxFramerate.Length - 1));

        framerateText.text = maxFramerate[PlayerPrefs.GetInt("MAX_FRAMERATE", 0)].ToString();
    }

    public void BTAudioSettings()
    {
        try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

        try { StopCoroutine(videoHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(videoHolderCanvasGroupCoroutine); } catch { }

        try { StopCoroutine(audioHolderRectTransformCoroutine); } catch { }
        try { StopCoroutine(audioHolderCanvasGroupCoroutine); } catch { }

        settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(-160, 0, 0), 0.35f));
        settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 0.4f, 0.35f));

        videoHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(videoHolder, new Vector3(600, 0, 0), 0.35f));
        videoHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(videoHolderCanvasGroup, 0, 0.35f));

        audioHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(audioHolder, new Vector3(270, 0, 0), 0.35f));
        audioHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(audioHolderCanvasGroup, 1, 0.35f));

        SetButtonMouseFocus(3);

        currentGroupIndex = 3;
    }

    public void BTSetAudioMasterVolume(float volumeNormalized)
    {
        mixer.SetFloat("MASTERPARAM", Mathf.Log10(volumeNormalized) * 20);
        PlayerPrefs.SetFloat("MASTER_VOLUME", volumeNormalized);
    }

    public void BTSetAudioMusicVolume(float volumeNormalized)
    {
        mixer.SetFloat("MUSICPARAM", Mathf.Log10(volumeNormalized) * 20);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", volumeNormalized);
    }

    public void BTSetAudioSoundVolume(float volumeNormalized)
    {
        mixer.SetFloat("SOUNDPARAM", Mathf.Log10(volumeNormalized) * 20);
        PlayerPrefs.SetFloat("SOUND_VOLUME", volumeNormalized);
    }

    public void BTMasterVolume(int index = 1)
    {
        float currentVolume = PlayerPrefs.GetFloat("MASTER_VOLUME", 1);
        float newVolume = UIUtils.WrapFloat(currentVolume * 10 + index, 0f, 10f) / 10;
        mixer.SetFloat("MASTERPARAM", Mathf.Log10(newVolume) * 20);
        PlayerPrefs.SetFloat("MASTER_VOLUME", newVolume);

        masterAudioPercentageText.text = Mathf.FloorToInt(newVolume * 100).ToString() + "%";
        masterAudioSlider.value = PlayerPrefs.GetFloat("MASTER_VOLUME", 1);
    }
    public void BTMusicVolume(int index = 1)
    {
        float currentVolume = PlayerPrefs.GetFloat("MUSIC_VOLUME", 1);
        float newVolume = UIUtils.WrapFloat(currentVolume * 10 + index, 0f, 10f) / 10;
        mixer.SetFloat("MUSICPARAM", Mathf.Log10(newVolume) * 20);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", newVolume);

        musicAudioPercentageText.text = Mathf.FloorToInt(newVolume * 100).ToString() + "%";
        musicAudioSlider.value = PlayerPrefs.GetFloat("MUSIC_VOLUME", 1);
    }

    public void BTSoundVolume(int index = 1)
    {
        float currentVolume = PlayerPrefs.GetFloat("SOUND_VOLUME", 1);
        float newVolume = UIUtils.WrapFloat(currentVolume * 10 + index, 0f, 10f) / 10;
        mixer.SetFloat("SOUNDPARAM", Mathf.Log10(newVolume) * 20);
        PlayerPrefs.SetFloat("SOUND_VOLUME", newVolume);

        soundAudioPercentageText.text = Mathf.FloorToInt(newVolume * 100).ToString() + "%";
        soundAudioSlider.value = PlayerPrefs.GetFloat("SOUND_VOLUME", 1);
    }

    public void BTClosedCaptions()
    {
        PlayerPrefs.SetInt("CLOSED_CAPTIONS", 1 - PlayerPrefs.GetInt("CLOSED_CAPTIONS", 0));

        closedCaptionIndicator.SetActive(PlayerPrefs.GetInt("CLOSED_CAPTIONS", 0) == 1);
    }

    public void BTLanguageSettings()
    {
        GetComponent<LocaleSelector>().ChangeLocale();
    }
    
    /// <summary>
    /// Locks the interaction of the mouse with the buttons that are not in the current button group.
    /// </summary>
    /// <param name="index"></param>
    private void SetButtonMouseFocus(int index)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].GetComponent<Button>().interactable = (index == 0);
        }

        for (int i = 0; i < buttonsSettings.Count; i++)
        {
            buttonsSettings[i].GetComponent<Button>().interactable = (index == 1);
        }

        for (int i = 0; i < buttonsVideo.Count; i++)
        {
            buttonsVideo[i].GetComponent<Button>().interactable = (index == 2);
        }

        for (int i = 0; i < buttonsAudio.Count; i++)
        {
            buttonsAudio[i].GetComponent<Button>().interactable = (index == 3);
        }
    }

    #endregion

    /// <summary>
    /// Calculates the amount of buttons to order and positions them symmetrically in specified angle and offset.<br></br>
    /// Gets the highlight and text children as well.<br></br>
    /// - Sets the position and angle of each button<br></br>
    /// - Gets the highlight and text children and sets them into buttonsHighlights and buttonsTexts<br></br>
    /// - Sets the buttonsCoroutinesRunning array
    /// </summary>
    private void InitializeButtons()
    {
        SetButtonMouseFocus(0);

        mainHolder.anchoredPosition = new Vector2(0, 0);
        settingsHolder.anchoredPosition = new Vector2(200, 0);
        videoHolder.anchoredPosition = new Vector2(600, 0);
        audioHolder.anchoredPosition = new Vector2(600, 0);

        mainHolderCanvasGroup.alpha = 0f;
        settingsHolderCanvasGroup.alpha = 0f;
        videoHolderCanvasGroup.alpha = 0f;
        audioHolderCanvasGroup.alpha = 0f;

        mainHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(mainHolder, mainHolder.anchoredPosition, 0.1f));
        mainHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(mainHolderCanvasGroup, 1, 0.3f));

        buttonsAngle = new float[buttons.Count];
        buttonsCoroutines = new Coroutine[buttons.Count];
        buttonsCoroutinesRunning = new bool[buttons.Count];
        buttonsCoroutinesRunningHighlight = new bool[buttons.Count];

        buttonsSettingsCoroutines = new Coroutine[buttonsSettings.Count];
        buttonsSettingsCoroutinesRunning = new bool[buttonsSettings.Count];
        buttonsSettingsCoroutinesRunningHighlight = new bool[buttonsSettings.Count];

        buttonsVideoCoroutines = new Coroutine[buttonsVideo.Count];
        buttonsVideoCoroutinesRunning = new bool[buttonsVideo.Count];
        buttonsVideoCoroutinesRunningHighlight = new bool[buttonsVideo.Count];

        buttonsAudioCoroutines = new Coroutine[buttonsAudio.Count];
        buttonsAudioCoroutinesRunning = new bool[buttonsAudio.Count];
        buttonsAudioCoroutinesRunningHighlight = new bool[buttonsAudio.Count];

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
    /// Gets the current player settings if there are and appropriately shows the correct data in the settings menu.
    /// </summary>
    private void InitializeSettings()
    {
        // Resolution

        int[] maxResolution = { Screen.currentResolution.width, Screen.currentResolution.height };

        for (int i = 0; i < 9; i++)
        {
            if (resolutions[i, 0] <= maxResolution[0] && resolutions[i, 1] <= maxResolution[1])
            {
                availableResolutions.Add(new int[2] { resolutions[i, 0], resolutions[i, 1] });
            }
        }

        resolutionIndex = PlayerPrefs.GetInt("RESOLUTION_INDEX", 0);

        if (availableResolutions[resolutionIndex][0] < maxResolution[0] && availableResolutions[resolutionIndex][1] < maxResolution[1])
        {
            resolutionIndex = 0;
        }

        resolutionText.text = availableResolutions[resolutionIndex][0].ToString() + "x" + availableResolutions[resolutionIndex][1].ToString();

        // Fullscreen

        fullscreenModeIndex = PlayerPrefs.GetInt("WINDOWTYPE", 0);

        if (fullscreenModeIndex == 0)
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "WINDOWED");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        }
        else if (fullscreenModeIndex == 1)
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "BORDERLESS");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        }
        else
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Settings", "EXCLUSIVEFULLSCREEN");
            if (op.IsDone)
            {
                Debug.Log(op.Result);
                windowTypeText.text = op.Result;
            }
        }

        // Framerate

        framerateText.text = maxFramerate[PlayerPrefs.GetInt("MAX_FRAMERATE", 0)].ToString();

        // Vsync

        vsyncIndicator.SetActive(PlayerPrefs.GetInt("VSYNC", 0) == 1);

        // Audio

        masterAudioPercentageText.text = Mathf.FloorToInt(PlayerPrefs.GetFloat("MASTER_VOLUME", 1) * 100).ToString() + "%";
        musicAudioPercentageText.text = Mathf.FloorToInt(PlayerPrefs.GetFloat("MUSIC_VOLUME", 1) * 100).ToString() + "%";
        soundAudioPercentageText.text = Mathf.FloorToInt(PlayerPrefs.GetFloat("SOUND_VOLUME", 1) * 100).ToString() + "%";

        masterAudioSlider.value = PlayerPrefs.GetFloat("MASTER_VOLUME", 1);
        musicAudioSlider.value = PlayerPrefs.GetFloat("MUSIC_VOLUME", 1);
        soundAudioSlider.value = PlayerPrefs.GetFloat("SOUND_VOLUME", 1);

        closedCaptionIndicator.SetActive(PlayerPrefs.GetInt("CLOSED_CAPTIONS", 0) == 1);
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
        currentButtonIndex = index;

        if (currentGroupIndex == 0)
        {
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
                    if (!buttonsSettingsCoroutinesRunning[i]) // If no coroutine is running, start a new unhilight one
                    {
                        buttonsSettingsCoroutinesRunning[i] = true;
                        buttonsSettingsCoroutines[i] = StartCoroutine(HighlightButton(i, false, 1));
                    }
                }
            }
        } 
        else if (currentGroupIndex == 2)
        {
            for (int i = 0; i < buttonsVideo.Count; i++) // Do for all settings buttons
            {
                if (i == index) // If the button corresponds to the given index, start a highlighting coroutine
                {
                    StopHighlightAnimationAtIndex(i, 2);
                    buttonsVideoCoroutinesRunning[i] = true;
                    buttonsVideoCoroutinesRunningHighlight[i] = true;
                    buttonsVideoCoroutines[i] = StartCoroutine(HighlightButton(i, true, 2));
                }
                else // If not,
                {
                    if (buttonsVideoCoroutinesRunning[i]) // If any coroutine is already running for this button,
                    {
                        if (buttonsVideoCoroutinesRunningHighlight[i]) // Check if it is a highlight one. If it is, stop it and start a new unhilight one
                        {
                            StopHighlightAnimationAtIndex(i, 2);
                            buttonsVideoCoroutinesRunningHighlight[i] = false;
                            buttonsVideoCoroutines[i] = StartCoroutine(HighlightButton(i, false, 2));
                        }
                    }
                    if (!buttonsVideoCoroutinesRunning[i]) // If no coroutine is running, start a new unhilight one
                    {
                        buttonsVideoCoroutinesRunning[i] = true;
                        buttonsVideoCoroutines[i] = StartCoroutine(HighlightButton(i, false, 2));
                    }
                }
            }
        }
        else if (currentGroupIndex == 3)
        {
            for (int i = 0; i < buttonsAudio.Count; i++) // Do for all settings buttons
            {
                if (i == index) // If the button corresponds to the given index, start a highlighting coroutine
                {
                    StopHighlightAnimationAtIndex(i, 3);
                    buttonsAudioCoroutinesRunning[i] = true;
                    buttonsAudioCoroutinesRunningHighlight[i] = true;
                    buttonsAudioCoroutines[i] = StartCoroutine(HighlightButton(i, true, 3));
                }
                else // If not,
                {
                    if (buttonsAudioCoroutinesRunning[i]) // If any coroutine is already running for this button,
                    {
                        if (buttonsAudioCoroutinesRunningHighlight[i]) // Check if it is a highlight one. If it is, stop it and start a new unhilight one
                        {
                            StopHighlightAnimationAtIndex(i, 3);
                            buttonsAudioCoroutinesRunningHighlight[i] = false;
                            buttonsAudioCoroutines[i] = StartCoroutine(HighlightButton(i, false, 3));
                        }
                    }
                    if (!buttonsAudioCoroutinesRunning[i]) // If no coroutine is running, start a new unhilight one
                    {
                        buttonsAudioCoroutinesRunning[i] = true;
                        buttonsAudioCoroutines[i] = StartCoroutine(HighlightButton(i, false, 3));
                    }
                }
            }
        }
        
        
    }

    /// <summary>
    /// Executes the action corresponding to the current button.
    /// </summary>
    public void ConfirmButton()
    {
        SelectButton(currentButtonIndex);
        switch (currentGroupIndex)
        {
            case 0: // Main pause menu

                switch (currentButtonIndex)
                {
                    case 0:
                        BTResumeGame();
                        break;

                    case 1:
                        currentButtonIndex = 0;
                        BTSettings();
                        break;

                    case 2:
                        BTMainMenu();
                        break;
                }
                break;

            case 1: // Settings menu

                switch (currentButtonIndex)
                {
                    case 0:
                        currentButtonIndex = 0;
                        BTVideoSettings();
                        break;

                    case 1:
                        currentButtonIndex = 0;
                        BTAudioSettings();
                        break;

                    case 2:
                        BTLanguageSettings();
                        break;
                }
                break;

            case 2: // Video menu

                switch (currentButtonIndex)
                {
                    case 0:

                        BTResolution();
                        break;

                    case 1:
                        BTWindowType();
                        break;

                    case 2:
                        BTVsync();
                        break;

                    case 3:
                        BTFramerate();
                        break;
                }
                break;

            case 3: // Audio menu

                switch (currentButtonIndex)
                {
                    case 0:
                        BTMasterVolume();
                        break;

                    case 1:
                        BTMusicVolume();
                        break;

                    case 2:
                        BTSoundVolume();
                        break;

                    case 3:
                        BTClosedCaptions();
                        break;
                }
                break;
        }

        SelectButton(currentButtonIndex);
    }

    private void LeftRightButton(float index)
    {
        int value = (int)Mathf.Floor(index);
        SelectButton(currentButtonIndex);
        switch (currentGroupIndex)
        {
            case 0: // Main pause menu

                switch (currentButtonIndex)
                {
                    case 0:
                        //BTResumeGame();
                        break;

                    case 1:
                        //currentButtonIndex = 0;
                        //BTSettings();
                        break;

                    case 2:
                        //BTMainMenu();
                        break;
                }
                break;

            case 1: // Settings menu

                switch (currentButtonIndex)
                {
                    case 0:
                        //currentButtonIndex = 0;
                        //BTVideoSettings();
                        break;

                    case 1:
                        //currentButtonIndex = 0;
                        //BTAudioSettings();
                        break;

                    case 2:
                        //BTLanguageSettings();
                        break;
                }
                break;

            case 2: // Video menu

                switch (currentButtonIndex)
                {
                    case 0:

                        BTResolution(value);
                        break;

                    case 1:
                        BTWindowType(value);
                        break;

                    case 2:
                        //BTVsync();
                        break;

                    case 3:
                        BTFramerate(value);
                        break;
                }
                break;

            case 3: // Audio menu

                switch (currentButtonIndex)
                {
                    case 0:
                        BTMasterVolume(value);
                        break;

                    case 1:
                        BTMusicVolume(value);
                        break;

                    case 2:
                        BTSoundVolume(value);
                        break;

                    case 3:
                        //BTClosedCaptions();
                        break;
                }
                break;
        }

        SelectButton(currentButtonIndex);
    }

    /// <summary>
    /// Returns back to the previous menu based on the current menu
    /// </summary>
    private void ReturnButton()
    {
        switch (currentGroupIndex)
        {
            case 0: // Main pause menu

                SetButtonMouseFocus(9);
                PopOutAnimate();
                break;

            case 1: // Settings menu

                try { StopCoroutine(mainHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(mainHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(videoHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(videoHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(audioHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(audioHolderCanvasGroupCoroutine); } catch { }

                mainHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(mainHolder, new Vector3(0, 0, 0), 0.35f));
                mainHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(mainHolderCanvasGroup, 1, 0.35f));

                settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(200, 0, 0), 0.35f));
                settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 0, 0.35f));

                videoHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(videoHolder, new Vector3(600, 0, 0), 0.35f));
                videoHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(videoHolderCanvasGroup, 0, 0.35f));

                audioHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(audioHolder, new Vector3(600, 0, 0), 0.35f));
                audioHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(audioHolderCanvasGroup, 0, 0.35f));

                SetButtonMouseFocus(0);

                currentGroupIndex = 0;

                SelectButton(1);

                break;

            case 2: // Video menu

                try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(videoHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(videoHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(audioHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(audioHolderCanvasGroupCoroutine); } catch { }

                settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(-160, 0, 0), 0.35f));
                settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 1, 0.35f));

                videoHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(videoHolder, new Vector3(600, 0, 0), 0.35f));
                videoHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(videoHolderCanvasGroup, 0, 0.35f));

                audioHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(audioHolder, new Vector3(600, 0, 0), 0.35f));
                audioHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(audioHolderCanvasGroup, 0, 0.35f));

                SetButtonMouseFocus(1);

                currentGroupIndex = 1;

                SelectButton(0);

                Screen.SetResolution(availableResolutions[resolutionIndex][0], availableResolutions[resolutionIndex][1], fullScreenModes[fullscreenModeIndex]);

                Application.targetFrameRate = maxFramerate[PlayerPrefs.GetInt("MAX_FRAMERATE", 0)];

                QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSYNC", 0);

                break;


            case 3: // Audio menu

                try { StopCoroutine(settingsHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(settingsHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(videoHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(videoHolderCanvasGroupCoroutine); } catch { }

                try { StopCoroutine(audioHolderRectTransformCoroutine); } catch { }
                try { StopCoroutine(audioHolderCanvasGroupCoroutine); } catch { }

                settingsHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(settingsHolder, new Vector3(-160, 0, 0), 0.35f));
                settingsHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(settingsHolderCanvasGroup, 1, 0.35f));

                videoHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(videoHolder, new Vector3(600, 0, 0), 0.35f));
                videoHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(videoHolderCanvasGroup, 0, 0.35f));

                audioHolderRectTransformCoroutine = StartCoroutine(UIUtils.MoveOverSecondsRectTransform(audioHolder, new Vector3(600, 0, 0), 0.35f));
                audioHolderCanvasGroupCoroutine = StartCoroutine(UIUtils.FadeCanvasGroup(audioHolderCanvasGroup, 0, 0.35f));

                SetButtonMouseFocus(1);

                currentGroupIndex = 1;

                SelectButton(1);

                break;
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
