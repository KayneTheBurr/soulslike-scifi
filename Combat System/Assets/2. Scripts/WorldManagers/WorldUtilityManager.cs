using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layer Masks")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask enviroLayers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }
    public LayerMask GetEnviroLayers()
    {
        return enviroLayers;
    }

    public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if (attackingCharacter == CharacterGroup.Friendly)
        {
            switch(targetCharacter)
            {
                case CharacterGroup.Friendly:
                    return false;
                    
                case CharacterGroup.Enemy:
                    return true;
                    
                default:
                    break;
            }
        }
        else if (attackingCharacter == CharacterGroup.Enemy)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Friendly:
                    return true;
                    
                case CharacterGroup.Enemy:
                    return false;
                    
                default:
                    break;
            }
        }
        return false;
    }
}
