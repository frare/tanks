using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    private PhotonView photView;
    private Rigidbody2D rb2d;

    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Sprite> sprites;

    private Vector3 selfPosition;

    private void Awake() {

        photView = GetComponent<PhotonView>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start() {

        if (photView.owner.ID == 1) {
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
        else if (photView.owner.ID == 2) {
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        else {
            GetComponent<SpriteRenderer>().sprite = sprites[3];
        }
    }

    private void Update() {

        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col) {

        if (photView.isMine) {
            if (col.gameObject.tag == "Enemy") {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
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
