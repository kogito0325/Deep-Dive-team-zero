using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Sprite[] sprs;
    public AudioClip[] audioClips;
    public AudioClip[] fxClips;

    float soundBgm;
    float soundFx;

    public AudioSource mySource;
    public AudioSource fxSpeaker;

    public Slider sliderBGM;
    public Slider sliderFX;

    public Image handleBgm;
    public Image handleFx;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        soundBgm = PlayerPrefs.GetFloat("BGM", 1f); // 기본값 1f 추가
        soundFx = PlayerPrefs.GetFloat("FX", 1f); // 기본값 1f 추가

        mySource.volume = soundBgm;
        fxSpeaker.volume = soundFx;

        sliderBGM.value = soundBgm;
        sliderFX.value = soundFx;
    }

    // ... 나머지 함수들은 그대로 유지 ...
    public void SetVolumeBGM(float volume)
    {
        soundBgm = volume;
        mySource.volume = soundBgm;
        PlayerPrefs.SetFloat("BGM", soundBgm);

        if (soundBgm == 0)
        {
            handleBgm.sprite = sprs[0];
        }
        else { handleBgm.sprite = sprs[1]; }
    }

    public void SetVolumeFX(float volume)
    {
        soundFx = volume;
        fxSpeaker.volume = soundFx;
        PlayerPrefs.SetFloat("FX", soundFx);

        if (soundFx == 0)
        {
            handleFx.sprite = sprs[0];
        }
        else { handleFx.sprite = sprs[1]; }
    }

    public void SetAudioClipToBGM(int index)
    {
        mySource.clip = audioClips[index];
        mySource.Play();
    }

    public void SetAudioClipToFX(int index)
    {
        fxSpeaker.clip = fxClips[index];
        fxSpeaker.Play();
    }
}