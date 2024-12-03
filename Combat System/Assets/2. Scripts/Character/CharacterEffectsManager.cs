using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    //instnat effects (heal, dmage) one and done effects
    //over time effect (poison, buff) effect processed over a period of time
    //static on/off effects (armor/jewelery effects)

    public virtual void ProcessInstantEffects(InstantCharacterEffect effect)
    {
        
        //take in an effect
        //do what the effect causes
        effect.ProcessEffect(character);


    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        //if we manually set a blood vfx on the model play that one, 
        if(bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        //
        else
        {
            
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity) ;
            Debug.Log(bloodSplatter.name);
        }
    }



}
