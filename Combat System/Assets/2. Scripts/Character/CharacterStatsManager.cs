using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    



    protected virtual void Awake()
    {

    }

    public int CalculateStaminaBasedOnEndurance(int endurance)
    {
        float stamina = 0;

        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
        
    }



}
