using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCam : MonoBehaviour
{

    public float lerpAmount = 0.5f;
    public Vector3 offset;
    public float returnDuration = 2f;

    private bool isFollowing = false;
    private Transform target;
    private float savedZ;
    private bool isReturning = false;
    private Vector3 origin;
    private float t = 0;


    private void Start()
    {
        savedZ = transform.position.z;
        origin = transform.position;
    }

    private void FixedUpdate()
    {
        if (isFollowing)
        {
            var position = Vector3.Lerp(transform.position, target.position + offset, lerpAmount);
            position.z = savedZ;
            transform.position = position;
        }
        if (isReturning)
        {
            t += Time.deltaTime;
            var position = Vector3.Lerp(transform.position, origin, t / returnDuration);
            transform.position = position;
            if (t > returnDuration)
            {
                transform.position = origin;
                isReturning = false;
            }
        }
    }

    public void Follow(Transform target)
    {
        this.target = target;
        isFollowing = true;
    }

    public void StopFollowing()
    {
        target = null;
        isFollowing = false;
    }

    public void ReturnToOrigin()
    {
        isReturning = true;
        t = 0;
    }
}
