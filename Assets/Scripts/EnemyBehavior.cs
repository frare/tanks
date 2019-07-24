using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : Photon.MonoBehaviour {

    [SerializeField] private float health;
    private int enemyId;

    public void TakeDamage(int amount) {

        health -= amount;
        if (health <= 0) {

            EnemyController.instance.DestroyEnemy(enemyId);
        }
    }

    public void SetEnemyId(int number) {

        enemyId = number;
    }

    public int GetEnemyId() {

        return enemyId;
    }
}
