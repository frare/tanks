using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    [SerializeField] private float moveSpeed;

    private Vector3 selfPosition;

    private void Update() {

        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    // Photon Methods
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.isWriting) {
            stream.SendNext(transform.position);
        }
        else {
            selfPosition = (Vector3)stream.ReceiveNext();
        }
    }
    // ***** *****
}
