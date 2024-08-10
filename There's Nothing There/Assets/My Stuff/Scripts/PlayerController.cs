using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Overhead Components")]
    CharacterController controller = null;
    [SerializeField] Transform playerCamera = null;
    public InsaneAngel insaneScript;


    [Header("Mouse Settings")]
    [SerializeField] float mouseSensitivity = 4.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;
    float cameraPitch = 0.0f;
    bool lockCursor = true;


    [Header("Movement Settings")]
    public float currentSpeed = 2;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.1f;
    float velocityY = 0;
    public float gravity = -9.81f;
    public float sprintSpeed = 4;
    public bool isSprinting = false;
    public float crouchSpeed = 1;
    public float toCrouchSpeed = .1f;
    public bool isCrouching = false;
    private bool canStand = true;
    public float walkSpeed = 2;
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
    bool standing = true;

    [Header("Head Bob Settings")]
    public float headBobAmmount = .001f;
    public float headBobSpeed = 14;
    public float headBobWalkSpeed = 14;
    public float headBobSprintSpeed = 20;
    public float lerpSpeed = 15;
    private float timer = 0;
    private Vector3 initialPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (lockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        initialPos = playerCamera.transform.localPosition;
    }
    void Update()
    {
        if(GameObject.Find("Insane Enemy") != null)
        {
            insaneScript = GameObject.Find("Insane Enemy").gameObject.transform.GetChild(0).gameObject.transform.GetComponent<InsaneAngel>();
        }

        UpdateMouseLook();
        UpdateMovement();
        if (!isCrouching)
        {
            HeadBobHandler();
        }
    }

    //Movement
    void UpdateMovement()
    {
        //Direction setting
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        //State Setting
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            isSprinting = true;
            headBobSpeed = headBobSprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            isSprinting = false;
            isCrouching = true;
        }
        else
        {
            isSprinting = false;
            headBobSpeed = headBobWalkSpeed;
            if (canStand)
            {
                isCrouching = false;
            }
        }

        //Speed Setting
        if (isSprinting)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, lerpSpeed * Time.deltaTime);
        }
        else if (isCrouching)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, crouchSpeed, lerpSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, lerpSpeed * Time.deltaTime);
        }
        
        //Camera work for crouching
        if (isCrouching)
        {
            standing = false;
            if (playerCamera.transform.localPosition.y > 0)
            {
                playerCamera.transform.position += (Vector3.down * toCrouchSpeed * Time.deltaTime);
            }

            controller.center = new Vector3(0, -0.3f, 0);
            controller.height = 1f;
        }
        else if(!standing)
        {
            if (playerCamera.transform.localPosition.y < 1)
            {
                playerCamera.transform.position += (Vector3.up * toCrouchSpeed * Time.deltaTime);
            }
            else
            {
                standing = true;
            }

            controller.center = new Vector3(0, 0, 0);
            controller.height = 2;
        }
        
        //Ray cast for crouch
        Ray upRay = new Ray(transform.position, Vector3.up);
        if (Physics.Raycast(upRay, out RaycastHit hitUp, 2f))
        {
            if (hitUp.collider.tag == "Untagged")
            {
                canStand = false;
                // Debug.Log("grrrrrr");
            }
        }
        else
        {
            canStand = true;
            //Debug.Log("hee hee haw");
        }

        //Debug.DrawRay(transform.position, Vector3.up * 2, Color.red);


        //Sets velocity and puts player in motion
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime * 2);


        //Gravity Controls
        if (!controller.isGrounded)
        {
            velocityY += gravity * Time.deltaTime;
        }
        else
        {
            velocityY = 0;
        }
    }

    /*
     void OnGUI()
     {
        GUI.Box(new Rect(angleScript.xStartVal, (Screen.height - 200) - angleScript.yStartVal, 200, 200), "");
     }
    */

    //Mouse movement
    void UpdateMouseLook()
    {
        //Gets mouse inputs
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //Calculates mouse positions
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        cameraPitch -= targetMouseDelta.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        /*float lookAngle = Vector3.SignedAngle(orbs[0].transform.position - playerCamera.transform.position, playerCamera.transform.forward, playerCamera.transform.right);
        if(lookAngle != 0)
        {
            cameraPitch = Mathf.Clamp(cameraPitch + 1f / lookAngle / 5, -90.0f, 90.0f);
        }*/

        //Rotates player and the camera
        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        if(insaneScript != null)
        {
            transform.Rotate(Vector3.up * targetMouseDelta.x * mouseSensitivity + new Vector3(0, insaneScript.pushSet, 0));
        }
        else
        {
            transform.Rotate(Vector3.up * targetMouseDelta.x * mouseSensitivity);
        }
    }

    //Headbob
    void HeadBobHandler()
    {
        //Moves your head when moving
        if (Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.S) || (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D)))))
        {
            timer += Time.deltaTime;

            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y + Mathf.Sin(timer * headBobSpeed) * headBobAmmount, playerCamera.transform.localPosition.z);
        }
        //Resets head position
        else if (playerCamera.localPosition != initialPos)
        {
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, initialPos, lerpSpeed * Time.deltaTime);
            timer = 0;
        }
    }
}
