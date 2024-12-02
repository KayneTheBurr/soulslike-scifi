using UnityEngine;
using Unity.Netcode;
using UnityEngine.TextCore.Text;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Position")]
    
    public NetworkVariable<Vector3> networkPosition = 
        new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public NetworkVariable<Quaternion> networkRotation = 
        new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float networkRotationSmoothTime = 0.1f;

    [Header("Flags")]
    public NetworkVariable<bool> isSprinting =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isLockedOn =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Animator")]
    public NetworkVariable<bool> isMoving =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> horizontalMovement =
        new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement =
        new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount =
        new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Attributes")]
    public NetworkVariable<int> vitality =
        new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> endurance =
        new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<float> currentStamina = 
        new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = 
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentHealth =
        new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth =
        new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    public void CheckHP(float oldHealth, float newHealth)
    {
        if(currentHealth.Value <= 0)
        {
            StartCoroutine(character.HandleDeathEvents());
        }

        if(character.IsOwner)
        {
            if(currentHealth.Value > maxHealth.Value) //if you heal for more health than you were missing, heal to full health
            { 
                currentHealth.Value = maxHealth.Value;
            }
        }

    }

    public void OnIsMovingChanged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool("IsMoving", isMoving.Value);
    }

    //Play Normal Action Animations Across Server
    //a server RPC is a function called from a client to the zerver (in our case the host)
    [ServerRpc]
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        //if this character is the host/server, then activate the client rpc
        if(IsServer)
        {
            PlayActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
        }
    }
    //a client rpc is sent to all clients present from the server/host
    [ClientRpc]
    public void PlayActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        //make sure to not run the function on the character who sent it(so we dont play the animation twice)
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(animationID, applyRootMotion);
        }
    }
    private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(animationID, 0.2f);
    }

    //Attack Animations
    [ServerRpc]
    public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        //if this character is the host/server, then activate the client rpc
        if (IsServer)
        {
            PlayAttackActionAnimationForAllClientsClientRpc(clientID, animationID, applyRootMotion);
        }
    }
    
    [ClientRpc]
    public void PlayAttackActionAnimationForAllClientsClientRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        //make sure to not run the function on the character who sent it(so we dont play the animation twice)
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
        }
    }
    private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(animationID, 0.2f);
    }

    //DAMAGE
    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerOfCharacterDamageServerRpc(ulong damagedCharacterID, ulong characterCausingDamageID, float physicalDamage, 
        float plasmaDamage, float electricalDamage, float cryoDamage, float chemicalDamage, float radiationDamage, float geneticDamage, 
        float poiseDamage, float angleHitFrom, float contactPointX, float contactPointY, float contactPointZ)
    {
        if(IsServer)
        {
            NotifyServerOfCharacterDamageClientRpc(damagedCharacterID, characterCausingDamageID, physicalDamage, 
                plasmaDamage, electricalDamage, cryoDamage, chemicalDamage, radiationDamage, geneticDamage, 
                poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
    }

    [ClientRpc]
    public void NotifyServerOfCharacterDamageClientRpc(ulong damagedCharacterID, ulong characterCausingDamageID, float physicalDamage,
        float plasmaDamage, float electricalDamage, float cryoDamage, float chemicalDamage, float radiationDamage, float geneticDamage,
        float poiseDamage, float angleHitFrom, float contactPointX, float contactPointY, float contactPointZ)
    {
        ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage,
                plasmaDamage, electricalDamage, cryoDamage, chemicalDamage, radiationDamage, geneticDamage,
                poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
    }

    public void ProcessCharacterDamageFromServer(ulong damagedCharacterID, ulong characterCausingDamageID, float physicalDamage,
        float plasmaDamage, float electricalDamage, float cryoDamage, float chemicalDamage, float radiationDamage, float geneticDamage,
        float poiseDamage, float angleHitFrom, float contactPointX, float contactPointY, float contactPointZ)
    {

        CharacterManager damagedCharacter = 
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();

        CharacterManager characterCausingDamage = 
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.plasmaDamage = plasmaDamage;
        damageEffect.electricalDamage = electricalDamage;
        damageEffect.cryoDamage = cryoDamage;
        damageEffect.chemicalDamage = chemicalDamage;
        damageEffect.radiationDamage = radiationDamage;
        damageEffect.geneticDamage = geneticDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);

        damagedCharacter.characterEffectsManager.ProcessInstantEffects(damageEffect);

    }

    




}
