using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Singleton<EnemyController> {

    private PhotonView photView;

    [SerializeField] private float spawnTimer;

    [SerializeField] private GameObject explosionPfb, smokeExplosionPfb;

    private List<GameObject> enemies = new List<GameObject>();
    private int lastEnemyId;
    [SerializeField] private GameObject enemyPfb;

    private List<GameObject> crates = new List<GameObject>();
    private int lastCrateId;
    [SerializeField] private GameObject cratePfb;

    private void Awake() {

        photView = GetComponent<PhotonView>();
    }

    public void SpawnEnemy() {

        int random = Random.Range(0, 8);
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

    public void ShootEnemy(int enemyId) {

        photView.RPC("ShootEnemyRPC", PhotonTargets.AllBufferedViaServer, enemyId);
    }

    public void SpawnExplosionSmoke(Vector3 position) {

        photView.RPC("SpawnExplosionSmokeRPC", PhotonTargets.AllBufferedViaServer, position);
    }

    public void SpawnCrate() {

        int random = Random.Range(0, 8);
        photView.RPC("SpawnCrateRPC", PhotonTargets.AllBufferedViaServer, random);
    }

    public void DestroyCrate(int crateId) {

        photView.RPC("DestroyCrateRPC", PhotonTargets.AllBuffered, crateId);
    }

    private void Start() {
        
        if (PhotonNetwork.player.ID == 1) {
            StartCoroutine(SpawnEnemyTimer());
            StartCoroutine(SpawnCrateTimer());
        }
    }

    [PunRPC]
    private void ShootEnemyRPC(int enemyId) {

        foreach (GameObject enemy in enemies) {
            if (enemyId == enemy.GetComponent<EnemyBehavior>().GetEnemyId()) {
                enemy.GetComponent<TankBehavior>().Shoot();
                break;
            }
        }
    }

    [PunRPC]
    private void SpawnEnemyRPC(int position) {

        Vector3 spawnPosition = Vector3.zero;

        switch (position) {

            case 0:
                spawnPosition = new Vector3(-13.5f, 5.25f, 0);
                break;

            case 1:
                spawnPosition = new Vector3(-9.0f, 1.75f, 0);
                break;

            case 2:
                spawnPosition = new Vector3(-16.0f, -8.0f, 0);
                break;

            case 3:
                spawnPosition = new Vector3(-0.5f, -5.0f, 0);
                break;

            case 4:
                spawnPosition = new Vector3(12.5f, -6.5f, 0);
                break;

            case 5:
                spawnPosition = new Vector3(6.0f, -3.15f, 0);
                break;

            case 6:
                spawnPosition = new Vector3(13.5f, 7.5f, 0);
                break;

            case 7:
                spawnPosition = new Vector3(5.0f, 5.0f, 0);
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

    [PunRPC]
    private void SpawnCrateRPC(int position) {

        Vector3 spawnPosition = Vector3.zero;

        switch (position) {

            case 0:
                spawnPosition = new Vector3(-13.5f, 5.25f, 0);
                break;

            case 1:
                spawnPosition = new Vector3(-9.0f, 1.75f, 0);
                break;

            case 2:
                spawnPosition = new Vector3(-16.0f, -8.0f, 0);
                break;

            case 3:
                spawnPosition = new Vector3(-0.5f, -5.0f, 0);
                break;

            case 4:
                spawnPosition = new Vector3(12.5f, -6.5f, 0);
                break;

            case 5:
                spawnPosition = new Vector3(6.0f, -3.15f, 0);
                break;

            case 6:
                spawnPosition = new Vector3(13.5f, 7.5f, 0);
                break;

            case 7:
                spawnPosition = new Vector3(5.0f, 5.0f, 0);
                break;
        }

        GameObject crate = Instantiate(cratePfb, spawnPosition, Quaternion.identity);
        crate.GetComponent<CrateBehavior>().SetCrateId(lastCrateId);
        lastCrateId++;
        crates.Add(crate);
    }

    [PunRPC]
    private void DestroyCrateRPC(int crateId) {

        foreach (GameObject crate in crates) {
            if (crateId == crate.GetComponent<CrateBehavior>().GetCrateId()) {
                crates.Remove(crate);
                Destroy(crate);
                break;
            }
        }
    }

    private IEnumerator SpawnEnemyTimer() {

        yield return new WaitForSeconds(spawnTimer);

        for (int i = 0; i < 20 / spawnTimer; i++) {
            yield return new WaitForSeconds(1.0f);
            SpawnEnemy();
        }

        if (spawnTimer > 5) {
            spawnTimer -= 0.5f;
        }
        StartCoroutine(SpawnEnemyTimer());
    }

    private IEnumerator SpawnCrateTimer() {

        yield return new WaitForSeconds(30.0f);

        SpawnCrate();

        StartCoroutine(SpawnCrateTimer());
    }
}