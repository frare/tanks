using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController> {

    private PhotonView photView;

    [SerializeField] private GameObject explosionPfb, smokeExplosionPfb;

    private List<GameObject> enemies = new List<GameObject>();
    private int lastEnemyId;

    [SerializeField] private GameObject enemyPfb;

    private void Awake() {

        photView = GetComponent<PhotonView>();
    }

    public void SpawnEnemy() {

        int random = Random.Range(0, 4);
        photView.RPC("SpawnEnemyRPC", PhotonTargets.AllBufferedViaServer, random);
    }

    public void DamageEnemy(int enemyId, int amount) {

        photView.RPC("DamageEnemyRPC", PhotonTargets.AllBuffered, enemyId, amount);
    }

    public void DestroyEnemy(int enemyId) {

        photView.RPC("DestroyEnemyRPC", PhotonTargets.AllBuffered, enemyId);
    }

    public void MoveEnemy(int enemyId, Vector3 target) {

        photView.RPC("MoveEnemyRPC", PhotonTargets.AllBufferedViaServer, enemyId, target);
    }

    public void SpawnExplosionSmoke(Vector3 position) {

        photView.RPC("SpawnExplosionSmokeRPC", PhotonTargets.AllBufferedViaServer, position);
    }

    private void Update() {
        
        if (PhotonNetwork.player.ID == 1) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SpawnEnemy();
            }
        }
    }

    [PunRPC]
    private void SpawnEnemyRPC(int position) {

        Vector3 spawnPosition = Vector3.zero;

        switch (position) {

            case 0:
                spawnPosition = new Vector3(-8.0f, 3.25f, 0);
                break;

            case 1:
                spawnPosition = new Vector3(-8.0f, -3.25f, 0);
                break;

            case 2:
                spawnPosition = new Vector3(5.0f, 3.25f, 0);
                break;

            case 3:
                spawnPosition = new Vector3(7.25f, -4.25f, 0);
                break;

        }

        GameObject enemy = Instantiate(enemyPfb, spawnPosition, Quaternion.identity);
        enemy.GetComponent<EnemyBehavior>().SetEnemyId(lastEnemyId);
        lastEnemyId++;
        enemies.Add(enemy);
    }

    [PunRPC]
    private void DamageEnemyRPC(int enemyId, int amount) {

        foreach (GameObject enemy in enemies) {
            if (enemyId == enemy.GetComponent<EnemyBehavior>().GetEnemyId()) {
                enemy.GetComponent<EnemyBehavior>().TakeDamage(amount);
                break;
            }
        }
    }

    [PunRPC]
    private void DestroyEnemyRPC(int enemyId) {

        foreach (GameObject enemy in enemies) {
            if (enemyId == enemy.GetComponent<EnemyBehavior>().GetEnemyId()) {
                enemies.Remove(enemy);
                Instantiate(explosionPfb, enemy.transform.position, Quaternion.identity);
                Destroy(enemy);
                break;
            }
        }
    }

    [PunRPC]
    private void MoveEnemyRPC(int enemyId, Vector3 target) {

        foreach (GameObject enemy in enemies) {
            if (enemyId == enemy.GetComponent<EnemyBehavior>().GetEnemyId()) {
                enemy.GetComponent<EnemyBehavior>().SetTargetPosition(target);
            }
        }
    }

    [PunRPC]
    private void SpawnExplosionSmokeRPC(Vector3 position) {

        Instantiate(smokeExplosionPfb, position, Quaternion.identity);
    }
}