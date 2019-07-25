using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    private Rigidbody2D rb2d;
    private SpriteRenderer sprRend;

    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private int owner;

    private void Awake() {

        rb2d = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
    }

    private void Update() {

        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    public void SetOwner(int value) {

        if (value == 1 || value == 2) {
            owner = value;
            sprRend.sprite = sprites[value];
        }
        else {
            owner = 0;
            sprRend.sprite = sprites[0];
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {

        if (col.tag == "Enemy") {
            if (PhotonNetwork.player.ID == owner) {
                EnemyController.instance.DamageEnemy(col.GetComponent<EnemyBehavior>().GetEnemyId(), 1);
            }
            Destroy(this.gameObject);
        }

        if (col.tag == "Object") {

            Destroy(this.gameObject);
        }

        if (col.tag == "Player") {
            if (owner == 0 && PhotonNetwork.player.ID == 1) {

                col.GetComponent<PlayerController>().TakeDamage(1);
                Destroy(this.gameObject);
            }
        }
    }
}