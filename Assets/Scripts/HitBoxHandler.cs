using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HitboxHandler {
    void OnHitboxEnter(Collider2D hitboxCollider, Collider2D foreignCollider);
    void OnHitboxExit(Collider2D hitboxCollider, Collider2D foreignCollider);
}
