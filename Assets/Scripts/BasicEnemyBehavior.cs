using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour {

    private EnemyBehavior enemyScript;
    [SerializeField] private float minShotTime, maxShotTime;
    private float currentShotTime, shotTime;

    private void Awake() {

        enemyScript = GetComponent<EnemyBehavior>();
    }

    private void Start() {

        SetShotTime();
        StartCoroutine(RandomMovement());
    }

    private void Update() {

        if (PhotonNetwork.player.ID == 1) {
            if (enemyScript.GetIsAiming()) {

                currentShotTime += Time.deltaTime;

                if (currentShotTime >= shotTime) {
                    enemyScript.Shoot();
                    currentShotTime = 0.0f;
                    SetShotTime();
                }
            }
        }
    }

    private IEnumerator RandomMovement() {

        if (PhotonNetwork.player.ID == 1) {
            Vector3 randomDirection = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            EnemyController.instance.MoveEnemy(enemyScript.GetEnemyId(), randomDirection * 5f);
        }

        if (enemyScript.GetIsAiming()) {
            yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
        }
        else {
            yield return new WaitForSeconds(Random.Range(5.0f, 8.0f));
        }

        StartCoroutine(RandomMovement());
    }

    private void SetShotTime() {

        shotTime = Random.Range(minShotTime, maxShotTime);
    }
}
