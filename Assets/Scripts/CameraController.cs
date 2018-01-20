using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;

    private void Awake()
    {
        if (target == null) throw new System.Exception("Camera controller requires a target to follow");
    }

    private void Update()
    {
        transform.position = new Vector2(transform.position.x, target.position.y);
    }
}
