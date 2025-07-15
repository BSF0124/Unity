using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip[] bgmClips_CutScene;
    public AudioClip[] bgmClips_Game;
    public AudioClip[] sfxClips_CutScene;
    public AudioClip[] sfxClips_Main;
    public AudioClip[] sfxClips_DualWin;
    public AudioClip[] sfxClips_DualLose;
    public AudioClip[] sfxClips_DualMode;
    public AudioClip[] sfxClips_CardPack;

    public bool isBgmMute = false;

    [Header("#BGM")]
    public float bgmVolume;
    public AudioSource bgmPlayer;

    [Header("#SFX")]
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        bgmVolume = PlayerPrefs.GetFloat("BGMVolumn", 50f) / 100;
        sfxVolume = PlayerPrefs.GetFloat("SFXVolumn", 100f) / 100;
        Init();
    }

    private void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.SetParent(transform);
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.SetParent(transform);
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlayBgm(AudioClip audioClip)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = audioClip;
        bgmPlayer.Play();
    }

    public void StopBgm()
    {
        bgmPlayer.Stop();
    }

    public void PlaySfx(AudioClip audioClip)
    {
        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if(sfxPlayers[loopIndex].isPlaying)
                continue;
            
            sfxPlayers[loopIndex].clip = audioClip;
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void StopAllSfx()
    {
        foreach(AudioSource audioSource in sfxPlayers)
        {
            audioSource.Stop();
        }
    }

    public void UpdateVoulumn()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolumn", 50f) / 100;
        bgmPlayer.volume = bgmVolume;
        if(bgmVolume == 0)
        {
            isBgmMute = true;
            bgmPlayer.Pause();
        }
        else
        {
            if(isBgmMute)
            {
                isBgmMute = false;
                bgmPlayer.Play();
            }
        }

        sfxVolume = PlayerPrefs.GetFloat("SFXVolumn", 100f) / 100;
        foreach(AudioSource item in sfxPlayers)
        {
            item.volume = sfxVolume;
        }
    }
}
