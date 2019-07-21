using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour {

    private Rigidbody2D rb2d;

    [SerializeField] private float moveSpeed;

    private float targetRotation;
    private Transform tankBody;
    private List<GameObject> cannons;

    private void Awake() {

        rb2d = GetComponent<Rigidbody2D>();

        tankBody = transform.GetChild(0);

        cannons = new List<GameObject>();
        cannons.Add(transform.GetChild(0).GetChild(1).gameObject);
    }

    public void Move(Vector2 moveDirection) {

        rb2d.velocity = moveDirection.normalized * moveSpeed;
        if (moveDirection.magnitude > 0.5f) {
            RotateBody(moveDirection);
        }
    }

    public void Shoot() {

        foreach (GameObject obj in cannons) {
            PhotonNetwork.Instantiate("Bullet", obj.transform.GetChild(0).transform.position, obj.transform.rotation, 0);
        }
    }

    private void RotateBody(Vector2 moveDirection) {

        targetRotation = (int)Mathf.Round(Vector2.SignedAngle(Vector2.right, moveDirection) / 45) * 45;
        tankBody.rotation = Quaternion.Euler(new Vector3(0, 0,
            Mathf.LerpAngle(tankBody.eulerAngles.z, targetRotation, Time.deltaTime * 10)));
    }

    public void RotateCannons(Vector3 targetPos, Camera cam) {

        foreach (GameObject obj in cannons) {
            targetPos = Input.mousePosition;
            Vector3 objectPos = cam.WorldToScreenPoint(transform.position);

            targetPos.x = targetPos.x - objectPos.x;
            targetPos.y = targetPos.y - objectPos.y;
            targetPos.z = 10.0f;

            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    public Transform GetTankTransform() {

        return tankBody.transform;
    }

    public List<GameObject> GetCannons() {

        return cannons;
    }
}
