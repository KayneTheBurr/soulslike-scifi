using System.Linq.Expressions;
using UnityEngine;

public class PlayerUIHUDManager : MonoBehaviour
{

    [SerializeField] UI_StatBar staminaBar;




    public void SetNewStaminaValue(float oldStamina, float newStamina)
    {
        //Debug.Log("set stamina");
        staminaBar.SetStat(newStamina);
    }
    public void SetMaxStaminaValue(int maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

}
