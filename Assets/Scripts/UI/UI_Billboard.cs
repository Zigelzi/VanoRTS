using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Billboard : MonoBehaviour
{
    Transform camera;
    void Start()
    {
        camera = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + camera.forward);
    }
}
