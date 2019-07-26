using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionBehavior : MonoBehaviour {

    private void Start() {

        Destroy(this.gameObject, 0.4f);
    }
}