using UnityEngine;
using UnityEngine.UI;

namespace HexGame.Harvest
{
    /// <summary>
    /// 收获进度条组件，用于显示资源收获冷却时间
    /// </summary>
    public class HarvestProgressBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private GameObject container;
        
        // 设置进度条显示值（0-1）
        public void SetProgress(float value)
        {
            fillImage.fillAmount = 1 - Mathf.Clamp01(value); // 反向填充，0是满的，1是空的
        }
        
        // 设置进度条可见性
        public void SetVisible(bool visible)
        {
            container.SetActive(visible);
        }
        
        // 设置进度条的世界坐标位置
        public void SetWorldPosition(Vector3 position, float yOffset = 0.5f)
        {
            // 在目标位置上方显示进度条
            transform.position = position + Vector3.up * yOffset;
            
            // 让进度条始终面向摄像机
            if (Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}
