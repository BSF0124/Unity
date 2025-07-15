using UnityEngine;
using UnityEngine.Video;

public class Epilogue : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool videoEnd = true;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.SetDirectAudioVolume(0, AudioManager.instance.bgmVolume);
        Invoke("SetVideoEndBool", 1f);
    }

    private void Update()
    {
        videoPlayer.SetDirectAudioVolume(0, AudioManager.instance.bgmVolume);
        
        if(GameManager.instance.isSkipedCutScene)
        {
            SkipCutScene();
        }

        else if(!videoPlayer.isPlaying && !videoEnd)
        {
            SkipCutScene();
        }
    }

    public void SkipCutScene()
    {
        videoEnd = true;
        GameManager.instance.isSkipedCutScene = false;
        StartCoroutine(SceneLoader.instance.LoadScene(2, 5));
    }

    private void SetVideoEndBool()
    {
        videoEnd = false;
    }
}
