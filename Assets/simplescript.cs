using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplescript : MonoBehaviour
{
    public Transform camerapos;
    // Update is called once per frame
    void Update()
    {
        transform.position = camerapos.position;
    }
}
