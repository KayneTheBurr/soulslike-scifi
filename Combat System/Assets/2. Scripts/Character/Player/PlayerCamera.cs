using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public Camera cam;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")] //affect camera operation
    [SerializeField] float cameraSmoothSpeed = 1; //bigger num the longer it takes cam to reach target
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minPivotAngle = -30; //lowest to look down
    [SerializeField] float maxPivotAngle = 60; //highest to look up 

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    [SerializeField] float leftAndRightLookAngle, upAndDownLookAngle;

    private void Awake()
    {
        //one at a time 
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void HandleAllCameraAction()
    {
        if(player != null)
        {
            FollowTarget();
            HandleCameraRotation();
            //collide with environment and not move through walls
        }

    }
    private void FollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
            ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);

        transform.position = targetCameraPosition;

    }
    private void HandleCameraRotation()
    {
        //rotate cam left and right 
        leftAndRightLookAngle += (PlayerInputManager.instance.camHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        //rotate up and down and clamp vertical look angles 
        upAndDownLookAngle -= (PlayerInputManager.instance.camVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minPivotAngle, maxPivotAngle);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        //rotate main camera object left and right 
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        //rotate camera pivot point to look up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }



}
