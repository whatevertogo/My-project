using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationMixConfig", menuName = "Configs/AnimationMixConfig")]
public class AnimationMixConfig : ScriptableObject
{
    [Serializable]
    public class MixSetting
    {
        public string From; // 起始动画名称
        public string To;   // 目标动画名称
        public float Duration; // 混合时间
    }

    public List<MixSetting> MixSettings = new List<MixSetting>();
}
