using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;   
    public GameObject gameHUDPanel;    
    public GameObject settingsPanel;   
    public GameObject guidePanel;      

    [Header("Settings UI - Sliders")]
    public Slider widthSlider;
    public Slider heightSlider;
    public Slider colorSlider;
    public Slider condASlider;
    public Slider condBSlider;
    public Slider condCSlider;
    public Slider volumeSlider;

    [Header("Settings UI - Labels")]
    public TextMeshProUGUI widthText;
    public TextMeshProUGUI heightText;
    public TextMeshProUGUI colorText;
    public TextMeshProUGUI condAText;
    public TextMeshProUGUI condBText;
    public TextMeshProUGUI condCText;

    private const string MAIN_MENU_SCENE = "MainMenuScene";
    private const string GAME_SCENE = "GameScene";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (activeScene == MAIN_MENU_SCENE)
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
            if (gameHUDPanel) gameHUDPanel.SetActive(false); 
        }
        else if (activeScene == GAME_SCENE)
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false); 
            if (gameHUDPanel) gameHUDPanel.SetActive(true);
        }

        if (settingsPanel) settingsPanel.SetActive(false);
        if (guidePanel) guidePanel.SetActive(false);

        SetupSliderLimits();
        LoadSettingsToUI();
        ValidateSliders();
        UpdateLabels();

        if (widthSlider) widthSlider.onValueChanged.AddListener(delegate { UpdateLabels(); });
        if (heightSlider) heightSlider.onValueChanged.AddListener(delegate { UpdateLabels(); });
        if (colorSlider) colorSlider.onValueChanged.AddListener(delegate { UpdateLabels(); });

        if (condASlider) condASlider.onValueChanged.AddListener(delegate { ValidateSliders(); UpdateLabels(); });
        if (condBSlider) condBSlider.onValueChanged.AddListener(delegate { ValidateSliders(); UpdateLabels(); });
        if (condCSlider) condCSlider.onValueChanged.AddListener(delegate { ValidateSliders(); UpdateLabels(); });

        if (volumeSlider) volumeSlider.onValueChanged.AddListener(delegate { AudioListener.volume = volumeSlider.value; });
    }


    public void OnPlayClicked()
    {
        SaveSettingsFromUI();
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void OnPauseClicked()
    {
        LoadSettingsToUI(); 
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnResumeClicked()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnApplyAndRestartClicked()
    {
        Time.timeScale = 1f;
        SaveSettingsFromUI();
        SceneManager.LoadScene(GAME_SCENE); 
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    public void OnSettingsClicked()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnGuideClicked()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        guidePanel.SetActive(true);
    }

    public void OnBackClicked()
    {
        SaveSettingsFromUI(); 
        settingsPanel.SetActive(false);
        guidePanel.SetActive(false);

        if (SceneManager.GetActiveScene().name == MAIN_MENU_SCENE)
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
        }
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    private void SetupSliderLimits()
    {
        if (condASlider) { condASlider.minValue = 3; condASlider.maxValue = 8; condASlider.wholeNumbers = true; }
        if (condBSlider) { condBSlider.maxValue = 16; condBSlider.wholeNumbers = true; }
        if (condCSlider) { condCSlider.maxValue = 24; condCSlider.wholeNumbers = true; }
    }

    private void ValidateSliders()
    {
        if (condASlider && condBSlider)
        {
            condBSlider.minValue = condASlider.value + 1;
            if (condBSlider.value < condBSlider.minValue) condBSlider.value = condBSlider.minValue;
        }

        if (condBSlider && condCSlider)
        {
            condCSlider.minValue = condBSlider.value + 1;
            if (condCSlider.value < condCSlider.minValue) condCSlider.value = condCSlider.minValue;
        }
    }

    private void UpdateLabels()
    {
        if (widthText && widthSlider) widthText.text = widthSlider.value.ToString();
        if (heightText && heightSlider) heightText.text = heightSlider.value.ToString();
        if (colorText && colorSlider) colorText.text = colorSlider.value.ToString();
        if (condAText && condASlider) condAText.text = condASlider.value.ToString();
        if (condBText && condBSlider) condBText.text = condBSlider.value.ToString();
        if (condCText && condCSlider) condCText.text = condCSlider.value.ToString();
    }

    private void LoadSettingsToUI()
    {
        if (widthSlider) widthSlider.value = PlayerPrefs.GetInt("BoardWidth", 8);
        if (heightSlider) heightSlider.value = PlayerPrefs.GetInt("BoardHeight", 9);
        if (colorSlider) colorSlider.value = PlayerPrefs.GetInt("ColorCount", 6);
        if (condASlider) condASlider.value = PlayerPrefs.GetInt("CondA", 5);
        if (condBSlider) condBSlider.value = PlayerPrefs.GetInt("CondB", 7);
        if (condCSlider) condCSlider.value = PlayerPrefs.GetInt("CondC", 9);
        if (volumeSlider) volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    private void SaveSettingsFromUI()
    {
        if (widthSlider) PlayerPrefs.SetInt("BoardWidth", (int)widthSlider.value);
        if (heightSlider) PlayerPrefs.SetInt("BoardHeight", (int)heightSlider.value);
        if (colorSlider) PlayerPrefs.SetInt("ColorCount", (int)colorSlider.value);
        if (condASlider) PlayerPrefs.SetInt("CondA", (int)condASlider.value);
        if (condBSlider) PlayerPrefs.SetInt("CondB", (int)condBSlider.value);
        if (condCSlider) PlayerPrefs.SetInt("CondC", (int)condCSlider.value);
        if (volumeSlider) PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }
}