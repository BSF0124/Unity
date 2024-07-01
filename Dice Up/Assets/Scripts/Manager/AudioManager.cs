using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolumn;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    public bool isBgmOn = true;


    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolumn;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;
    public bool isSfxOn = true;



    public enum Sfx {SelectButton, PressButton, Jump, Bump, Gameover, IncreaseScore, GameResult, BestScore, RandomJump}

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

        Init();
    }

    private void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolumn;
        bgmPlayer.clip = bgmClip;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassEffects = true;
            sfxPlayers[i].volume = sfxVolumn;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if(isPlay)
        {
            if(!bgmPlayer.isPlaying)
                bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;

            if(sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void SetBgmEffect(AudioHighPassFilter _AudioHighPassFilter)
    {
        bgmEffect = _AudioHighPassFilter;
    }

    public void EffectBgm(bool isPlay)
    {
        if(bgmEffect != null)
        {
            bgmEffect.enabled = isPlay;
        }
    }

    public void BgmOnOff()
    {
        if(isBgmOn)
        {
            isBgmOn = false;
            bgmPlayer.volume = 0;
        }
        else
        {
            isBgmOn = true;
            bgmPlayer.volume = bgmVolumn;
        }
    }

    public void SfxOnOff()
    {
        if(isSfxOn)
        {
            isSfxOn = false;
            for(int i = 0; i < sfxPlayers.Length; i++)
            {
                sfxPlayers[i].volume = 0;
            }
        }
        else
        {
            isSfxOn = true;
            for(int i = 0; i < sfxPlayers.Length; i++)
            {
                sfxPlayers[i].volume = sfxVolumn;
            }
        }
    }

}
