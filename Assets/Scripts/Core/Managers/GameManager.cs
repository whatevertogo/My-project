using CDTU.Utils;
using Conclutions;
using UnityEngine;

public class GameManager : SingletonDD<GameManager>
{

    public static ConclutionsType ConclutionType;

    public static int core = 0;

    public float time = 0.0f;

    public float timeLimit = 3000f; // 5分钟


    public void Update()
    {
        //开始游戏计时
        time += Time.deltaTime;
        if (time > timeLimit)
        {
            // 结束游戏
            //todo-游戏结束逻辑
            Debug.Log("游戏结束");
            
        }
    }



    private void SetConclutionType()
    {
        ConclutionType = ConclutionsType.None;
        //PlayerCG(ConclutionType);
    }

    // public void PlayerCG(ConclutionsType ConclutionType)
    // {
    // }

    public void WinCore()
    {
        core = core + 1;
    }







}