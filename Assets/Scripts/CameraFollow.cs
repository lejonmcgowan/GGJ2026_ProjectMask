using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow: MonoBehaviour
{
    public Transform Target;
    public Rigidbody TargetBody;
    public float SmoothSpeed = 500f;
    public Vector3 Offset;

    void Start()
    {
        Offset = transform.position- Target.position;
    }

    void Update ()
    {
        if (!Target || !TargetBody)
            return;
        
        Vector3 desiredPosition = Target.position + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);
        
        
        
        transform.position = smoothedPosition;
    }
}