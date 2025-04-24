using System;
using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class AudioManager : SingletonDD<AudioManager>
{
    [Range(0, 1)]
    public float musicVolume = 0.6f;
    [Range(0, 1)]
    public float allVolume = 0.6f;
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public float AllVolume { get => allVolume; set => allVolume = value; }

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

        // 初始化背景音乐AudioSource
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
    }

    #region 背景音乐控制
    public void PlayBGM(string audioName, bool isLoop = true)
    {
        if (!Name_AudioClipDic.TryGetValue(audioName, out AudioClip clip)) return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = isLoop;
        bgmAudioSource.volume = musicVolume * allVolume;
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
        audioSource.volume = allVolume;
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
            bgmAudioSource.volume = musicVolume * allVolume;
        }

        foreach (var audioSource in soundAudioSources.Values)
        {
            audioSource.volume = allVolume;
        }
    }
    #endregion
}