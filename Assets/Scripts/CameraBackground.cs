using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackground : MonoBehaviour
{

    public Transform backgroundGO;


    private Vector3 offset;

    void Start()
    {
        offset = transform.position - backgroundGO.position;
    }

    void Update()
    {
        backgroundGO.position = transform.position - offset;
    }
}
