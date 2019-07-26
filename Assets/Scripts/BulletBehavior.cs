using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

    private Rigidbody2D rb2d;
    private SpriteRenderer sprRend;

    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Sprite> normalSprites, explosiveSprites, penetrationSprites;
    [SerializeField] private int owner;
    private int type;

    private void Awake() {

        rb2d = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
    }

    private void Update() {

        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }

    public void SetOwner(int value) {

        if (type == 0) {
            if (value == 1 || value == 2) {
                owner = value;
                sprRend.sprite = normalSprites[value];
            }
            else {
                owner = 0;
                sprRend.sprite = normalSprites[0];
            }
        }
        else if (type == 1) {
            if (value == 1 || value == 2) {
                owner = value;
                sprRend.sprite = explosiveSprites[value];
            }
            else {
                owner = 0;
                sprRend.sprite = explosiveSprites[0];
            }
        }
        else if (type == 2) {
            if (value == 1 || value == 2) {
                owner = value;
                sprRend.sprite = penetrationSprites[value];
            }
            else {
                owner = 0;
                sprRend.sprite = penetrationSprites[0];
            }
        }
    }

    public void SetType(int value) {

        type = value;

        if (type == 1) {
            moveSpeed /= 1.5f;
        }
        else if (type == 2) {
            moveSpeed *= 1.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {

        // Explosive
        if (type == 1) {
            if (col.tag == "Enemy" || col.tag == "Object") {
                if (PhotonNetwork.player.ID == owner) {
                    EnemyController.instance.SpawnExplosionSmoke(transform.position);
                    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << 9);
                    foreach (Collider2D hit in hits) {
                        EnemyController.instance.DamageEnemy(hit.GetComponent<EnemyBehavior>().GetEnemyId(), 1);
                    }
                }
                Destroy(this.gameObject);
            }
        }
        else {

            if (col.tag == "Enemy") {
                if (PhotonNetwork.player.ID == owner) {

                    EnemyController.instance.DamageEnemy(col.GetComponent<EnemyBehavior>().GetEnemyId(), 1);
                }

                if (owner != 0) {

                    if (type != 2) {
                        Destroy(this.gameObject);
                    }
                }
            }

            if (col.tag == "Object") {

                Destroy(this.gameObject);
            }

            if (col.tag == "Player") {
                if (owner == 0 && PhotonNetwork.player.ID == 1) {

                    col.GetComponent<PlayerController>().TakeDamage(1);
                    if (type != 2) {
                        Destroy(this.gameObject);
                    }
                }
            }
        }
    }
}