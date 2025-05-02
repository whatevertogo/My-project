using CDTU.Utils;
using Conclutions;

public class GameManager : Singleton<GameManager>
{
    
    public static ConclutionsType ConclutionType;
    
    
    protected override void Awake()
    {
        base.Awake();
        
    }

    private void SetConclutionType()
    {
        ConclutionType = ConclutionsType.None;
        //PlayerCG(ConclutionType);
    }

    // public void PlayerCG(ConclutionsType ConclutionType)
    // {
    // }







}