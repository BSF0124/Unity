using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Option : MonoBehaviour
{
    public Toggle fullScreenToggle;
    public Toggle dualVoiceToggle;
    public TMP_Dropdown resolutionDropdown;
    public GameObject DataResetPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;
    public Button DataResetButton;
    public Button confirmButton;

    private List<ResolutionData> _options = new();

    void Awake()
    {
        fullScreenToggle.isOn = Screen.fullScreen;

        Resolution[] resolutions = Screen.resolutions;

        foreach(Resolution resolution in resolutions)
        {
            if(ResolutionUtility.CheckMinimumResolution(resolution.width) && 
            ResolutionUtility.CheckRefreshRateRatio((float)resolution.refreshRateRatio.value) &&
            ResolutionUtility.Check16To9Ratio(resolution.width, resolution.height))
                _options.Add(new ResolutionData(resolution.width, resolution.height, resolution.refreshRateRatio));
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(_options.ConvertAll(option => option.ToString()));

        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
        dualVoiceToggle.onValueChanged.AddListener(DualVoiceOnOff);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        
        bgmSlider.onValueChanged.AddListener(BgmVolumnChanged);
        sfxSlider.onValueChanged.AddListener(SfxVolumnChanged);


        if(DataResetButton != null)
            DataResetButton.onClick.AddListener(DataReset);
            
        confirmButton.onClick.AddListener(Confirm);
    }

    void OnEnable()
    {
        // 해상도 드롭다운 value 업데이트
        int currentResolutionIndex = -1;
        for(int i = 0; i < _options.Count; i++)
        {
            if(_options[i].Width == Screen.width && _options[i].Height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
    
        if (currentResolutionIndex != -1)
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue(); // 드롭다운 UI 갱신
        }

        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolumn", 50f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolumn", 100f);
        bgmText.text = PlayerPrefs.GetFloat("BGMVolumn", 50f).ToString();
        sfxText.text = PlayerPrefs.GetFloat("SFXVolumn", 100f).ToString();

        dualVoiceToggle.isOn = PlayerPrefs.GetInt("DualVoice", 1) == 1;
    }

    void DataReset()
    {
        DataResetPanel.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        GameManager.instance.current_Stage = 0;
    }

    void Confirm()
    {
        gameObject.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        resolutionDropdown.RefreshShownValue();
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }

    void SetResolution(int resolutionIndex)
    {
        ResolutionData resolutionData = _options[resolutionIndex];
        Screen.SetResolution(resolutionData.Width, resolutionData.Height, Screen.fullScreen);
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        // Screen.SetResolution(resolutionData.Width, resolutionData.Height, FullScreenMode.ExclusiveFullScreen, resolutionData.RefreshRateRatio);
    }

    void DualVoiceOnOff(bool voiceOn)
    {
        if(voiceOn)
            PlayerPrefs.SetInt("DualVoice", 1);
        else
            PlayerPrefs.SetInt("DualVoice", 0);
    }

    void BgmVolumnChanged(float value)
    {
        bgmSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = value == 0?
            new Color(1f,1f,1f,0f) : new Color(1f,1f,1f,1f);

        PlayerPrefs.SetFloat("BGMVolumn", value);
        PlayerPrefs.Save();
        bgmText.text = PlayerPrefs.GetFloat("BGMVolumn", 50f).ToString();
        AudioManager.instance.UpdateVoulumn();
    }

    void SfxVolumnChanged(float value)
    {
        sfxSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = value == 0?
            new Color(1f,1f,1f,0f) : new Color(1f,1f,1f,1f);

        PlayerPrefs.SetFloat("SFXVolumn", value);
        PlayerPrefs.Save();
        sfxText.text = PlayerPrefs.GetFloat("SFXVolumn", 100f).ToString();
        AudioManager.instance.UpdateVoulumn();
    }
}
