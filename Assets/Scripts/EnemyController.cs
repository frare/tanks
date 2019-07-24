using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController> {

    private PhotonView photView;

    [SerializeField] private GameObject enemyPfb;
    private List<GameObject> enemies = new List<GameObject>();
    private int lastEnemyId;

    private void Awake() {

        photView = GetComponent<PhotonView>();
    }

    public void SpawnEnemy() {

        photView.RPC("SpawnEnemyRPC", PhotonTargets.AllBufferedViaServer);
    }

    public void DamageEnemy(int enemyId, int amount) {

        photView.RPC("DamageEnemyRPC", PhotonTargets.AllBuffered, enemyId, amount);
    }

    public void DestroyEnemy(int enemyId) {

        photView.RPC("DestroyEnemyRPC", PhotonTargets.AllBuffered, enemyId);
    }

    private void Update() {
        
        if (PhotonNetwork.player.ID == 1) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SpawnEnemy();
            }
        }
    }

    [PunRPC]
    private void SpawnEnemyRPC() {

        GameObject enemy = Instantiate(enemyPfb, Vector3.zero, Quaternion.identity);
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
                Destroy(enemy);
                break;
            }
        }
    }
}