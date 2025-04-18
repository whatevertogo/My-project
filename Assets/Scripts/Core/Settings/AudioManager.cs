
using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class AudioManager : SingletonDD<AudioManager>
{
    [Range(0, 1)]
    public float MusicVolume;
    [Range(0, 1)]
    public float AllVolume;

    public class Name_AudioClip
    {
        public string AudioName;
        public AudioClip AudioClip;
    }

    public List<Name_AudioClip> name_AudioClipsList = new();
    public Dictionary<string, AudioClip> Name_AudioClipDic = new();

    protected override void Awake()
    {
        base.Awake();
        foreach (var name_AudioClip in name_AudioClipsList)
        {
            Name_AudioClipDic.Add(name_AudioClip.AudioName, name_AudioClip.AudioClip);
        }


    }

    private void Start()
    {
    }

    private void Update()
    {
    }









}