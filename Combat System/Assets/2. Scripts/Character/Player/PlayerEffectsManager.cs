using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug Delete Later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool processEffect = false;


    private void Update()
    {
        if(processEffect)
        {
            processEffect = false;
            Debug.Log("effect");
            InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffects(effect);
        }
        
    }
}
