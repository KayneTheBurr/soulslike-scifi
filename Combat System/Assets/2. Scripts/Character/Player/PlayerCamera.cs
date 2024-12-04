using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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

    [Header("Lock On")]
    [SerializeField] float lockOnRadius = 20;
    [SerializeField] float minViewableAngle = -50;
    [SerializeField] float maxViewableAngle = 50;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;

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
        //if locked on, force camera rotation towards target
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            //rotate this gameobject left/right
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //this rotates pivot object up/down
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //save our rotation to our look angles so when we unlock we doesnt snap too far away
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;

        }
        //if we dont have a lock on target, rotate normally 
        else
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

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity; //used to find the target closest to us 
        float shortestDistanceOfRightTarget = Mathf.Infinity; //find target on shortest distance on one axis to the right 
        float shortestDistanceOfLeftTarget = -Mathf.Infinity; //find target on shortest distance on one axis to the left (-)

        //get all colliders around player in a radius
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers()); 

        //check all targets in the area and add available lock on targets to list 
        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();
            if (lockOnTarget != null)
            {
                //check if they are within our field of view
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, cam.transform.forward);

                //skip over lock on to targets that are dead, check next
                if (lockOnTarget.isDead.Value)
                    continue;
                
                //dont let us lock onto ourself, check next
                if (lockOnTarget.transform.root == player.transform.root)
                    continue;
                
                //last, if the target is outside fov or blocked by enviro, check next target
                if(viewableAngle > minViewableAngle && viewableAngle < maxViewableAngle)
                {
                    RaycastHit hit;

                    //only check for environemnt layer only 
                    if(Physics.Linecast(player.playerCombatManager.lockOnTransform.position, 
                        lockOnTarget.characterCombatManager.lockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        //if true, we hit something and cannot lock on, try next target 
                        continue;
                    }
                    else
                    {
                        //add to the list of potential targets 
                        availableTargets.Add(lockOnTarget);
                    }
                }
            }
        }

        //sort through all potential targets to see which is the closest, that we lock on first
        for(int j = 0; j < availableTargets.Count; j++)
        {
            if(availableTargets[j] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[j].transform.position);
                Vector3 lockOnTargetDirection = availableTargets[j].transform.position - player.transform.position;

                if(distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[j];
                }
            }
            else
            {
                ClearLockOnTargets();
                player.playerNetworkManager.isLockedOn.Value = false;
            }
        }
    }

    public void ClearLockOnTargets()
    {
        nearestLockOnTarget = null;
        availableTargets.Clear();
    }

}
