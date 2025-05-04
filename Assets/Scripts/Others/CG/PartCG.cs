using System.Collections.Generic;
using Conclutions;
using UnityEngine;
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
        part1Type = GameManager.ConclutionPart1Type;
        part2Type = GameManager.ConclutionPart2Type;
        part3Type = GameManager.ConclutionPart3Type;
    }

    public void Start()
    {
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
            nextCG.SetActive(false);
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
        int cgIndex = 0; // CG在列表中的索引
        switch (part1Type)
        {
            case ConclutionsPart1Type.Part1_Home:
                cgIndex = 0; // 对应筑巢成功的CG
                break;
            case ConclutionsPart1Type.Part1_Wasted:
                cgIndex = 1; // 对应筑巢失败的CG
                break;
        }
        if (cgIndex < CGs.Count)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
    }

    private void ShowPart2CG()
    {
        int cgIndex = 2; // Part2的CG起始索引
        switch (part2Type)
        {
            case ConclutionsPart2Type.Part2_Full:
                cgIndex = 2; // 对应吃饱的CG
                break;
            case ConclutionsPart2Type.Part2_Hungry:
                cgIndex = 3; // 对应饥饿的CG
                break;
        }
        if (cgIndex < CGs.Count)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
    }

    private void ShowPart3CG()
    {
        int cgIndex = 4; // Part3的CG起始索引
        switch (part3Type)
        {
            case ConclutionsPart3Type.Part3_GoodFriend:
                cgIndex = 4; // 对应好朋友的CG
                break;
            case ConclutionsPart3Type.Part3_Lonely:
                cgIndex = 5; // 对应孤独的CG
                break;
        }
        if (cgIndex < CGs.Count)
        {
            currentCGImage.sprite = CGs[cgIndex];
        }
    }
}