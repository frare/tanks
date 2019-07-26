using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairBehavior : MonoBehaviour {

    private void Start() {

        Cursor.visible = false;
    }

    private void Update() {

        Vector3 aux = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aux.z = 0;
        transform.position = aux;
    }
}