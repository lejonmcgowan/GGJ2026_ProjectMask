using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public Vector2 moveSpeed;
    public float jumpForce;
    public Transform GroundPoint;

    public LayerMask Grounded;
    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction InteractAction;
    
    public void Awake()
    {
        // assign a callback for the "jump" action.
        JumpAction.performed += ctx => { OnJump(ctx); };
        InteractAction.performed += ctx => { OnInteract(ctx); };
    }

    
    public void Start()
    {
        
    }

    public void Update()
    {
        Vector2 moveAmount = MoveAction.ReadValue<Vector2>();

        Rigidbody.linearVelocity =
            new Vector3(moveAmount.x * moveSpeed.x, Rigidbody.linearVelocity.y, moveAmount.y * moveSpeed.y);
    }
    
    public void OnEnable()
    {
        MoveAction.Enable();
        JumpAction.Enable();
        InteractAction.Enable();
    }

    public void OnDisable()
    {
        MoveAction.Disable();
        JumpAction.Disable();
        InteractAction.Enable();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundPoint.position, Vector3.down, out hit, .3f, Grounded));
        {
            Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, jumpForce, Rigidbody.linearVelocity.z);
        }
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        
    }
}
