using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class          PlayerController : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public Vector2 moveSpeed;
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

    public void Awake()
    {
        // assign a callback for the "jump" action.
        JumpAction.performed += ctx => { OnJump(ctx); };
        InteractAction.performed += ctx => { OnInteract(ctx); };
    }

    void ToggleControllable(bool controlEnabled)
    {
        if(controlEnabled)
        {
            MoveAction.Enable();
            JumpAction.Enable();
            InteractAction.Enable();
        }
        else
        {
            MoveAction.Disable();
            JumpAction.Disable();
            InteractAction.Disable();
        }
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        Vector2 moveAmount = MoveAction.ReadValue<Vector2>();
        if(moveAmount.x > 0)
            facing.x = 1;
        else if(moveAmount.x < 0)
            facing.x = -1;

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
                    if(interactable.Interact(()=> ToggleControllable(true)))
                    {
                        currentInteractCooldown = kInteractCooldown;
                        ToggleControllable(false);
                    }
                }
                else
                    Debug.LogError(closest.name + " has no Interactable component on its parent");
            }
        }
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
