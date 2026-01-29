using Unity.Cinemachine;
using Unity.Mathematics.Geometry;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class CameraFollow: MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 500f;
    public Vector3 Offset;

    public CinemachineMixingCamera MixingCamera;
    public float Threshold = 1.0f;
    private readonly float BlendCameraSpeed = 0.02f;

    private Vector3 TargetLastPosition;
    
    void Start()
    {
        Offset = transform.position- Target.position;
        //for debug text
        Canvas.ForceUpdateCanvases();
        TargetLastPosition = Target.position;
    }

    void Update ()
    {
        if (!Target)
            return;
        
        Vector3 desiredPosition = transform.position;
        desiredPosition.x = Target.position.x + Offset.x;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);
        
        transform.position = smoothedPosition;

        Vector3 velocity = Target.position - TargetLastPosition;
        TargetLastPosition = Target.position;
        
        if (!Mathf.Approximately(velocity.x, 0))
        {
            MixingCamera.Weight0 =
                Mathf.Clamp(MixingCamera.Weight0 + -Mathf.Sign(velocity.x) * BlendCameraSpeed, 0, Threshold);

            MixingCamera.Weight1 = 1 - MixingCamera.Weight0;    
        }
        
    }
    
   
   
}