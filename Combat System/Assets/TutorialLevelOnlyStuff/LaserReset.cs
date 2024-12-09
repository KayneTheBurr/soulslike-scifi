using UnityEngine;

public class LaserReset : MonoBehaviour
{
    public DamageCollider damageCollider;
    public PlayerManager player;

    [SerializeField] bool colliderUsed = false;

    private void Awake()
    {
        damageCollider = GetComponent<DamageCollider>();
        player = FindFirstObjectByType<PlayerManager>();
    }

    private void Update()
    {
        if(player.isDead.Value && colliderUsed == false)
        {
            colliderUsed = true;
        }
        else if(player.isDead.Value && colliderUsed == true)
        {
            damageCollider.DisableDamageCollider();
            damageCollider.EnableDamageCollider();
        }
        else
        {
            colliderUsed = false;
        }
    }
}
