using System;
using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonDD<AudioManager>
{
    [Range(0, 1)]
    [Tooltip("音效音量")]
    public float SVolume = 0.6f;

    [Range(0, 1)]
    [Tooltip("背景音乐音量")]
    public float BVolume = 0.6f;
    public float SoundVolume { get => SVolume; set => SVolume = value; }
    public float BGMVolume { get => BVolume; set => BVolume = value; }

    [Serializable]
    public class Name_AudioClip
    {
        public string AudioName;
        public AudioClip AudioClip;
    }

    public List<Name_AudioClip> name_AudioClipsList = new();
    public Dictionary<string, AudioClip> Name_AudioClipDic = new();

    private AudioSource bgmAudioSource;
    private Dictionary<string, AudioSource> soundAudioSources = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var name_AudioClip in name_AudioClipsList)
        {
            Name_AudioClipDic.Add(name_AudioClip.AudioName, name_AudioClip.AudioClip);
        }
        bgmAudioSource = GetComponent<AudioSource>();
        // 初始化背景音乐AudioSource
        bgmAudioSource.loop = true;
    }

    private void Start()
    {
        PlayBGM("BGM", true);
    }

    private void Update()
    {
        // 更新音量
        UpdateVolumes();

    }

    #region 背景音乐控制
    public void PlayBGM(string audioName, bool isLoop = true)
    {
        if (!Name_AudioClipDic.TryGetValue(audioName, out AudioClip clip)) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = isLoop;
        bgmAudioSource.volume = BGMVolume;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }

    public void PauseBGM()
    {
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        bgmAudioSource.UnPause();
    }
    #endregion

    #region 音效控制
    public void PlaySound(string audioName, bool isLoop = false)
    {
        if (!Name_AudioClipDic.TryGetValue(audioName, out AudioClip clip)) return;

        if (soundAudioSources.TryGetValue(audioName, out AudioSource existingSource))
        {
            existingSource.Play();
            return;
        }

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = isLoop;
        audioSource.volume = SoundVolume;
        audioSource.Play();

        if (!isLoop)
        {
            Destroy(audioSource, clip.length);
        }
        else
        {
            soundAudioSources[audioName] = audioSource;
        }
    }

    public void StopSound(string audioName)
    {
        if (soundAudioSources.TryGetValue(audioName, out AudioSource audioSource))
        {
            audioSource.Stop();
            Destroy(audioSource);
            soundAudioSources.Remove(audioName);
        }
    }

    public void UpdateVolumes()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = BGMVolume;
        }

        foreach (var audioSource in soundAudioSources.Values)
        {
            audioSource.volume = SoundVolume;
        }
    }
    #endregion
}