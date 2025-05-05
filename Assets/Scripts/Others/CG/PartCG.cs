using System.Collections.Generic;
using Conclutions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PartCG : MonoBehaviour
{
    public ConclutionsPart1Type part1Type = ConclutionsPart1Type.None;
    public ConclutionsPart2Type part2Type = ConclutionsPart2Type.None;
    public ConclutionsPart3Type part3Type = ConclutionsPart3Type.None;

    [Header("CG图片列表")]
    public List<Sprite> CGs;  // 在Inspector中按顺序放入CG图片
    public Image currentCGImage;  // 当前显示CG的Image组件
    public Button button;
    public GameObject nextCG;
    private int currentCGIndex = 0;  // 当前显示的CG索引

    public void Awake()
    {
        // 从GameManager获取结局类型
        part1Type = GameManager.ConclutionPart1Type;
        part2Type = GameManager.ConclutionPart2Type;
        part3Type = GameManager.ConclutionPart3Type;

        // 检查必要组件
        if (currentCGImage == null)
        {
            Debug.LogError("请在Inspector中设置currentCGImage！");
            enabled = false;
            return;
        }

        if (CGs == null || CGs.Count == 0)
        {
            Debug.LogError("请在Inspector中设置CG图片列表！");
            enabled = false;
            return;
        }

        if (button == null)
        {
            Debug.LogError("请在Inspector中设置button！");
            enabled = false;
            return;
        }
    }

    public void Start()
    {
        // 检查结局类型
        if (part1Type == ConclutionsPart1Type.None ||
            part2Type == ConclutionsPart2Type.None ||
            part3Type == ConclutionsPart3Type.None)
        {
            Debug.LogError("某个Part类型为None，请检查GameManager的结局设置！");
            return;
        }

        // 设置按钮点击事件
        button.onClick.AddListener(ShowNextCG);

        // 显示第一张CG
        ShowCurrentCG();
    }

    private void ShowCurrentCG()
    {
        if (currentCGIndex >= 3)
        {
            Debug.Log("所有CG都已经显示完毕");
            if (nextCG != null)
            {
                nextCG.SetActive(false);
            }
            SceneManager.LoadSceneAsync("开始界面");
            return;
        }

        // 根据当前索引决定显示哪个Part的CG
        switch (currentCGIndex)
        {
            case 0: // Part1的CG
                ShowPart1CG();
                break;
            case 1: // Part2的CG
                ShowPart2CG();
                break;
            case 2: // Part3的CG
                ShowPart3CG();
                break;
        }
    }

    private void ShowNextCG()
    {
        currentCGIndex++;
        ShowCurrentCG();
    }

    private void ShowPart1CG()
    {
        int cgIndex = 0;
        switch (part1Type)
        {
            case ConclutionsPart1Type.Part1_Home:
                cgIndex = 0;
                break;
            case ConclutionsPart1Type.Part1_Wasted:
                cgIndex = 1;
                break;
        }

        if (cgIndex < CGs.Count && CGs[cgIndex] != null)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
        else
        {
            Debug.LogError($"CG图片索引 {cgIndex} 无效或图片为空！");
        }
    }

    private void ShowPart2CG()
    {
        int cgIndex = 2;
        switch (part2Type)
        {
            case ConclutionsPart2Type.Part2_Full:
                cgIndex = 2;
                break;
            case ConclutionsPart2Type.Part2_Hungry:
                cgIndex = 3;
                break;
        }

        if (cgIndex < CGs.Count && CGs[cgIndex] != null)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
        else
        {
            Debug.LogError($"CG图片索引 {cgIndex} 无效或图片为空！");
        }
    }

    private void ShowPart3CG()
    {
        int cgIndex = 4;
        switch (part3Type)
        {
            case ConclutionsPart3Type.Part3_GoodFriend:
                cgIndex = 4;
                break;
            case ConclutionsPart3Type.Part3_Lonely:
                cgIndex = 5;
                break;
        }

        if (cgIndex < CGs.Count && CGs[cgIndex] != null)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
        else
        {
            Debug.LogError($"CG图片索引 {cgIndex} 无效或图片为空！");
        }
    }
}