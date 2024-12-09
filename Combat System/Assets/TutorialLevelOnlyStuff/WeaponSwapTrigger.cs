using UnityEngine;

public class WeaponSwapTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerManager>() != null)
        {
            TutorialUIManager.instance.learnAttack = true;

            PlayerManager player = other.GetComponent<PlayerManager>();
            player.switchRightWeapon = true;
            Destroy(gameObject);
        }
    }


}
