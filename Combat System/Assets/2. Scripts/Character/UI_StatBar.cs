using UnityEngine;
using UnityEngine.UI;

public class UI_StatBar : MonoBehaviour
{
    private Slider slider;
    //scale size of bar depending on stats(health/stamina/flux/etc)

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public virtual void SetStat(float newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }





}
