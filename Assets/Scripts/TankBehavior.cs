using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour {

    private Rigidbody2D rb2d;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject bulletPfb;

    private float targetRotation;
    private Transform tankBody;
    private List<GameObject> cannons;
    private int playerNumber;

    private EnemyBehavior enemyScript;

    private void Awake() {

        rb2d = GetComponent<Rigidbody2D>();

        tankBody = transform.GetChild(0);

        cannons = new List<GameObject>();
        cannons.Add(transform.GetChild(0).GetChild(1).gameObject);

        if (GetComponent<PlayerController>()) {
            playerNumber = GetComponent<PlayerController>().GetPlayerNumber();
        }
        else {
            playerNumber = 0;
            enemyScript = GetComponent<EnemyBehavior>();
        }
    }

    public void Move(Vector2 moveDirection) {

        rb2d.velocity = moveDirection.normalized * moveSpeed;
        if (moveDirection.magnitude > 0.5f) {
            RotateBody(moveDirection);
        }
    }

    public void Stop() {

        rb2d.velocity = Vector2.zero;
    }

    [PunRPC]
    public void Shoot() {

        foreach (GameObject obj in cannons) {
            GameObject bullet = Instantiate(bulletPfb, obj.transform.GetChild(0).transform.position, obj.transform.rotation);
            bullet.GetComponent<BulletBehavior>().SetOwner(playerNumber);
        }
    }

    private void RotateBody(Vector2 moveDirection) {

        targetRotation = (int)Mathf.Round(Vector2.SignedAngle(Vector2.right, moveDirection) / 45) * 45;
        tankBody.eulerAngles = new Vector3(0, 0,
            Mathf.LerpAngle(tankBody.eulerAngles.z, targetRotation, Time.deltaTime * 10));
    }

    public void RotateCannons(Vector3 targetPos, Camera cam) {

        if (playerNumber != 0) {
            foreach (GameObject obj in cannons) {
                targetPos = Input.mousePosition;
                Vector3 objectPos = cam.WorldToScreenPoint(transform.position);

                targetPos.x = targetPos.x - objectPos.x;
                targetPos.y = targetPos.y - objectPos.y;
                targetPos.z = 10.0f;

                float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
                obj.transform.eulerAngles = new Vector3(0, 0, angle);
            }
        }
        else {
            foreach (GameObject obj in cannons) {

                targetPos = cam.WorldToScreenPoint(targetPos);
                Vector3 objectPos = cam.WorldToScreenPoint(transform.position);

                targetPos.x = targetPos.x - objectPos.x;
                targetPos.y = targetPos.y - objectPos.y;
                targetPos.z = 10.0f;

                enemyScript.SetCannonRotation(Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg);
            }
        }
    }

    public Transform GetTankTransform() {

        return tankBody.transform;
    }

    public List<GameObject> GetCannons() {

        return cannons;
    }
}
