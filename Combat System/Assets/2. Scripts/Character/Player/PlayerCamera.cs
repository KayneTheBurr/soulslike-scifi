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
    [SerializeField] float cameraCollisionRadius;
    [SerializeField] LayerMask collideWithLayers;



    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPos;
    [SerializeField] float leftAndRightLookAngle, upAndDownLookAngle;
    private float cameraZPos, targetCamZPos;

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
        cameraZPos = cam.transform.localPosition.z;
    }
    public void HandleAllCameraAction()
    {
        if(player != null)
        {
            FollowTarget();
            HandleCameraRotation();
            HandleCameraCollision();
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

    private void HandleCameraCollision()
    {
        targetCamZPos = cameraZPos;
        RaycastHit hit;
        //direction to check for collision
        Vector3 direction = cam.transform.position - cameraPivotTransform.position;
        direction.Normalize();
        
        //check if object in front of camera in desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCamZPos), collideWithLayers))
        {
            //figure out how close the object is to the camera
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //we make our z position to this then
            targetCamZPos = -(distanceFromHitObject - cameraCollisionRadius);
        }
        //if out target position is ;less than out radius, we subtract our radius 
        if(Mathf.Abs(targetCamZPos) < cameraCollisionRadius)
        {
            targetCamZPos = -cameraCollisionRadius;
        }
        //apply final position using a lerp over 0.2f
        cameraObjectPos.z = Mathf.Lerp(cam.transform.localPosition.z, targetCamZPos, 0.2f);
        cam.transform.localPosition = cameraObjectPos;

    }

}
