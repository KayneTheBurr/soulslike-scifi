using System.Linq.Expressions;
using UnityEngine;

public class PlayerUIHUDManager : MonoBehaviour
{
    [SerializeField] UI_StatBar healthBar;
    [SerializeField] UI_StatBar staminaBar;

    public void RefreshHUD()
    {
        healthBar.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(true);
        
    }

    public void SetNewHealthValue(float oldHealth, float newHealth)
    {

        healthBar.SetStat(newHealth);
    }
    public void SetMaxHealthValue(float maxHealth)
    {
        healthBar.SetMaxStat(maxHealth);
    }
    public void SetNewStaminaValue(float oldStamina, float newStamina)
    {
        //Debug.Log("set stamina");
        staminaBar.SetStat(newStamina);
    }
    public void SetMaxStaminaValue(float maxStamina)
    {
        staminaBar.SetMaxStat(maxStamina);
    }

    

}
