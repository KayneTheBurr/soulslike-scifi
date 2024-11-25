using UnityEngine;

public class CharacterSFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDodgeSFX()
    {
        Debug.Log("dodge sfx");
        audioSource.PlayOneShot(WorldSFXManager.instance.rollSFX);
    }

}
