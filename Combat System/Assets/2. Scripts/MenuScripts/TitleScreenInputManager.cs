using System.Runtime.CompilerServices;
using UnityEngine;

public class TitleScreenInputManager : MonoBehaviour
{
    PlayerControls playerControls;


    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlot = false;

    private void Update()
    {
        if (deleteCharacterSlot)
        {
            //Debug.Log("delete char");
            deleteCharacterSlot = false;
            TitleScreenManager.instance.AttemptToDeleteCharacterSlot();
        }
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.UI.DeleteSaveFile.performed += i => deleteCharacterSlot = true;


        }
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }




}
