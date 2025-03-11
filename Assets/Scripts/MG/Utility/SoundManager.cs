using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
    Move,
    UI_Button,
    Trade,
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("BGM")]
    public AudioClip bgmClip;
    AudioSource bgmPlayer;
    [SerializeField] private Slider bgmVolumeSlider;

    [Header("SFX")]
    public AudioClip[] sfxClips;
    public int channels;
    AudioSource[] sfxPlayers;
    [SerializeField] private Slider sfxVolumeSlider;
    int channelIndex;

    private void Awake()
    {
        bgmVolumeSlider.minValue = 0f;
        bgmVolumeSlider.maxValue = 1f;
        bgmVolumeSlider.onValueChanged.AddListener(delegate { OnbgmVolumeSliderChanged(); });

        sfxVolumeSlider.minValue = 0f;
        sfxVolumeSlider.maxValue = 1f;
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnsfxVolumeSliderChanged(); });

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
        bgmPlayer.volume = bgmVolumeSlider.value;
        bgmPlayer.clip = bgmClip;

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolumeSlider.value;
        }
    }

    public void SFXplay(SoundType sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void BGMplay(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    private void OnbgmVolumeSliderChanged()
    {
        bgmPlayer.volume = bgmVolumeSlider.value;
    }

    private void OnsfxVolumeSliderChanged()
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index].volume = sfxVolumeSlider.value;
        }
    }
}
