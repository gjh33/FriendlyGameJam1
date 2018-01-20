using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Hitbox : MonoBehaviour {
    // Internals
    private Collider2D col;
    private Rigidbody2D rb;
    private HitboxHandler handler;

    // Use this for initialization
    private void Awake()
    {
        // Init collider
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Init rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        // Search for HitBoxHandler
        Transform curTransform = transform;
        while (curTransform != null)
        {
            HitboxHandler possibleHandler = curTransform.gameObject.GetComponent<HitboxHandler>();
            if (possibleHandler != null)
            {
                handler = possibleHandler;
                break;
            }
            curTransform = curTransform.parent;
        }

        if (handler == null) throw new System.Exception("Hitboxes must be a child of a HitboxHandler");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        handler.OnHitboxEnter(col, collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        handler.OnHitboxExit(col, collider);
    }
}
