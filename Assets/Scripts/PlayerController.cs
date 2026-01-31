using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum ControlSchemeType
{
    FIELD,
    DIALOGUE,
    MASK_SELECT
}

public class          PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public Rigidbody Rigidbody;
    public Vector2 moveSpeed;
    public Animator SpriteAnimator;
    
    public float jumpForce;
    public Transform GroundPoint;

    public LayerMask Grounded;
    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction InteractAction;

    public Vector3 BoxExtents = Vector3.one;
    public float interactDistance = 2f;

    Vector2 facing = new Vector2(1,-1);

    Vector3 boxCastCenter;
    
    //thresholds that the character can move towards and away from the character
    public Vector2 zLimits; 

    public bool inDialogue;

    const float kInteractCooldown = .5f;
    float currentInteractCooldown;

    public DialogueFace[] faces;
    public MaskType currentMask = MaskType.NONE;

    public SpriteRenderer mask;

    public void Awake()
    {
        // assign a callback for the "jump" action.
        JumpAction.performed += ctx => { OnJump(ctx); };
        InteractAction.performed += ctx => { OnInteract(ctx); };
        Instance = this;
    }

    public void ChangeControlScheme(ControlSchemeType scheme)
    {
        switch(scheme) 
        {
            case ControlSchemeType.FIELD:
                MoveAction.Enable();
                JumpAction.Enable();
                InteractAction.Enable();
                break;
            case ControlSchemeType.DIALOGUE:
                MoveAction.Disable();
                JumpAction.Disable();
                InteractAction.Disable();
                break;
            case ControlSchemeType.MASK_SELECT:
                MoveAction.Disable();
                JumpAction.Disable();
                InteractAction.Disable();
                break;
        }
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        Vector2 moveAmount = MoveAction.ReadValue<Vector2>();
        if (moveAmount.x > 0)
        {
            if(facing.x < 0)
            {
                SpriteAnimator.Play("Player_Flip");
            }
            facing.x = 1;
        }
        else if (moveAmount.x < 0)
        {
            if(facing.x > 0)
            {
                SpriteAnimator.Play("Player_Flip");
            }
            facing.x = -1;
        }

        if(moveAmount.y > 0)
            facing.y = 1;
        else if (moveAmount.y < 0)
            facing.y = -1;

        boxCastCenter = transform.position + new Vector3(BoxExtents.x / 2 * facing.x, BoxExtents.y / 2, BoxExtents.z / 2 * facing.y);

        var rbVel = Rigidbody.linearVelocity;
        
        if (transform.position.z < zLimits.x && moveAmount.y < 0 || transform.position.z > zLimits.y && moveAmount.y > 0)
        {
            moveAmount.y = 0;
        }
        
        
        Rigidbody.linearVelocity =
                new Vector3(moveAmount.x * moveSpeed.x, Rigidbody.linearVelocity.y, moveAmount.y * moveSpeed.y);

        if(InteractAction.enabled && currentInteractCooldown > 0)
            currentInteractCooldown -= Time.deltaTime;
            
    }
    
    public void OnEnable()
    {
        MoveAction.Enable();
        JumpAction.Enable();
        InteractAction.Enable();
        SetExpression(currentMask);
    }

    public void OnDisable()
    {
        MoveAction.Disable();
        JumpAction.Disable();
        InteractAction.Disable();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundPoint.position, Vector3.down, out hit, .3f, Grounded))
        {
            Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, jumpForce, Rigidbody.linearVelocity.z);
        }
    }
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(currentInteractCooldown > 0)
            return;

        Collider[] collisions = Physics.OverlapBox(boxCastCenter, BoxExtents / 2, transform.rotation);
        if(collisions != null)
        {
            Collider closest = null;
            float closestDistance = 9999;
            foreach(var item in collisions)
            {
                if(item.tag != "Interactable")
                    continue;
                float distanceToPlayer = Vector3.Distance(transform.position, item.ClosestPoint(transform.position));
                if(distanceToPlayer < closestDistance)
                {
                    closest = item;
                    closestDistance = distanceToPlayer;
                }
            }
            if(closest != null)
            {
                Interactable interactable = closest.gameObject.GetComponentInParent<Interactable>();
                if(interactable != null)
                {
                    if(interactable.Interact(()=> ChangeControlScheme(ControlSchemeType.FIELD)))
                    {
                        currentInteractCooldown = kInteractCooldown;
                        ChangeControlScheme(ControlSchemeType.DIALOGUE);
                    }
                }
                else
                    Debug.LogError(closest.name + " has no Interactable component on its parent");
            }
        }
    }

    void ToggleMaskMenu(InputAction.CallbackContext context)
    {
        if(InteractAction.enabled)
        {
            Debug.LogError("Toggling mask menu ON");
            ChangeControlScheme(ControlSchemeType.MASK_SELECT);
            MaskSelectMenu.Instance.ToggleActive(true);
        }
        else
        {
            Debug.LogError("Toggling mask menu OFF");
            ChangeControlScheme(ControlSchemeType.FIELD);
            MaskSelectMenu.Instance.ToggleActive(false);
        }
    }

    public Sprite GetExpression(MaskType expression)
    {
        foreach(DialogueFace face in faces)
        {
            if(face.expression == expression)
            {
                return face.faceSprite;
            }
        }
        return null;
    }

    public void SetExpression(MaskType expression)
    {
        currentMask = expression;
        mask.sprite = GetExpression(expression);
    }

    void OnDrawGizmos()
    {
        DrawBoxLines(boxCastCenter, boxCastCenter, BoxExtents, true);
    }

    protected void DrawBoxLines(Vector3 p1, Vector3 p2, Vector3 extents, bool boxes = false)
    {
        var length = (p2 - p1).magnitude;

        var halfExtents = extents / 2;

        var halfExtentsZ = transform.forward * halfExtents.z;

        var halfExtentsY = transform.up * halfExtents.y;

        var halfExtentsX = transform.right * halfExtents.x;

        if(boxes)
        {
            var matrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(p1, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = Matrix4x4.TRS(p2, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = matrix;
        }

        // draw connect lines 1

        Gizmos.DrawLine(p1 - halfExtentsX - halfExtentsY - halfExtentsZ, p2 - halfExtentsX - halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX - halfExtentsY - halfExtentsZ, p2 + halfExtentsX - halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 - halfExtentsX + halfExtentsY - halfExtentsZ, p2 - halfExtentsX + halfExtentsY - halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX + halfExtentsY - halfExtentsZ, p2 + halfExtentsX + halfExtentsY - halfExtentsZ);

        // draw connect lines 2

        Gizmos.DrawLine(p1 - halfExtentsX - halfExtentsY + halfExtentsZ, p2 - halfExtentsX - halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX - halfExtentsY + halfExtentsZ, p2 + halfExtentsX - halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 - halfExtentsX + halfExtentsY + halfExtentsZ, p2 - halfExtentsX + halfExtentsY + halfExtentsZ);

        Gizmos.DrawLine(p1 + halfExtentsX + halfExtentsY + halfExtentsZ, p2 + halfExtentsX + halfExtentsY + halfExtentsZ);
        
    }
}
