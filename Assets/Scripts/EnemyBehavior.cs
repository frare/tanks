using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : Photon.MonoBehaviour {

    private TankBehavior tankScript;

    [SerializeField] private float health, range;
    private float cannonRotation;
    private int enemyId;
    private Vector3 targetPosition;
    private List<GameObject> cannons;
    private bool isAiming;

    private void Awake() {

        tankScript = GetComponent<TankBehavior>();

        cannons = new List<GameObject>();
        cannons = tankScript.GetCannons();
    }

    private void Start() {

        targetPosition = transform.position;
        isAiming = false;
    }

    private void Update() {

        cannons = tankScript.GetCannons();
        for (int i = 0; i < cannons.Count; i++) {
            cannons[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0,
                Mathf.LerpAngle(cannons[i].transform.eulerAngles.z, cannonRotation, Time.deltaTime * 5)));
        }
    }

    private void FixedUpdate() {

        if (Vector2.Distance(transform.position, targetPosition) > 0.1f) {

            tankScript.Move(targetPosition - transform.position);
        }
        else {
            tankScript.Stop();
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, range, 1 << 8);
        if (hit) { 
            isAiming = true;
            tankScript.RotateCannons(hit.transform.position, Camera.main);
        }
        else {
            isAiming = false;
        }
    }

    public void TakeDamage(int amount) {

        health -= amount;
        if (health <= 0) {

            EnemyController.instance.DestroyEnemy(enemyId);
        }
    }

    public void Shoot() {

        EnemyController.instance.ShootEnemy(enemyId);
    }

    public void SetTargetPosition(Vector3 target) {

        targetPosition = target;
    }

    public void SetEnemyId(int number) {

        enemyId = number;
    }

    public void SetCannonRotation(float value) {

        cannonRotation = value;
    }

    public int GetEnemyId() {

        return enemyId;
    }

    public bool GetIsAiming() {

        return isAiming;
    }
}
