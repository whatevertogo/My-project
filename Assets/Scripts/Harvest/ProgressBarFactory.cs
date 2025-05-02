using CDTU.Utils;
using UnityEngine;

namespace HexGame.Harvest
{
    /// <summary>
    /// 收获进度条工厂，负责创建和管理收获进度条
    /// </summary>
    public class ProgressBarFactory : Singleton<ProgressBarFactory>
    {
        [SerializeField] private HarvestProgressBar progressBarPrefab;
        
        protected override void Awake()
        {
            base.Awake();
            // 如果没有设置预制体，尝试从Resources加载
            if (progressBarPrefab == null)
            {
                progressBarPrefab = Resources.Load<HarvestProgressBar>("UI/HarvestProgressBar");

                if (progressBarPrefab == null)
                {
                    Debug.LogWarning("未找到收获进度条预制体，请在检视面板中设置或在Resources/UI文件夹中添加HarvestProgressBar预制体");
                }
            }
        }
        
        /// <summary>
        /// 创建一个收获进度条
        /// </summary>
        /// <returns>进度条组件</returns>
        public HarvestProgressBar CreateProgressBar()
        {
            if (progressBarPrefab == null)
            {
                Debug.LogError("未设置进度条预制体，无法创建进度条");
                return null;
            }
            
            return Instantiate(progressBarPrefab);
        }
        
        /// <summary>
        /// 创建一个收获进度条并设置位置
        /// </summary>
        /// <param name="position">世界坐标位置</param>
        /// <param name="yOffset">Y轴偏移量</param>
        /// <returns>进度条组件</returns>
        public HarvestProgressBar CreateProgressBar(Vector3 position, float yOffset = 0.5f)
        {
            var progressBar = CreateProgressBar();
            if (progressBar != null)
            {
                progressBar.SetWorldPosition(position, yOffset);
            }
            return progressBar;
        }
    }
}
