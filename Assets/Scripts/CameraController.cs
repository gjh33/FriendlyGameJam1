using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;
    public Vector2 targetScreenOffset = new Vector2(0, 100);

    private Camera hostCamera;

    private void Awake()
    {
        hostCamera = GetComponent<Camera>();
        if (hostCamera == null) throw new System.Exception("This controller must be attached to a camera");
    }

    private void Update()
    {
        Vector2 cameraPosition = new Vector2(transform.position.x, target.position.y);
        cameraPosition += targetScreenOffset;
        transform.position = cameraPosition;
    }
}
