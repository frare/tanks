using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehavior : MonoBehaviour {

    private Rigidbody2D rb2d;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject bulletPfb, shotEffectPfb;

    private float targetRotation;
    private Transform tankBody;
    private List<GameObject> cannons;
    private int playerNumber;
    private int bulletType;

    private EnemyBehavior enemyScript;

    private void Awake() {

        rb2d = GetComponent<Rigidbody2D>();

        tankBody = transform.GetChild(0);

        cannons = new List<GameObject>();
        cannons.Add(transform.GetChild(0).GetChild(1).gameObject);
    }

    private void Start() {

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
            GameObject bullet = Instantiate(bulletPfb, obj.transform.GetChild(0).position, obj.transform.rotation);
            BulletBehavior bulletScript = bullet.GetComponent<BulletBehavior>();
            bulletScript.SetType(bulletType);
            bulletScript.SetOwner(playerNumber);
            //bulletScript.SetOwner(GetComponent<PlayerController>().GetPlayerNumber());

            GameObject effect = Instantiate(shotEffectPfb, obj.transform.GetChild(0).position, obj.transform.rotation);
            effect.transform.parent = obj.transform.GetChild(0);
        }
    }

    private void RotateBody(Vector2 moveDirection) {

        targetRotation = (int)Mathf.Round(Vector2.SignedAngle(Vector2.right, moveDirection) / 45) * 45;
        tankBody.eulerAngles = new Vector3(0, 0,
            Mathf.LerpAngle(tankBody.eulerAngles.z, targetRotation, Time.deltaTime * 10));
    }

    public void RotateCannons(Vector3 targetPos, Camera cam) {

        if (!enemyScript) {
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

    [PunRPC]
    public void ChangeCannonRPC(int cannonId) {

        Sprite spr = null;
        GameObject newCannon = Instantiate(GameController.instance.GetNewCannon(), transform.position, Quaternion.identity);
        newCannon.transform.parent = tankBody;

        switch (cannonId) {

            // Normal 
            case 0:
                if (playerNumber == 1) {
                    spr = Resources.Load<Sprite>("Images/tankBlue_barrel2_outline");
                }
                else if (playerNumber == 2) {
                    spr = Resources.Load<Sprite>("Images/tankRed_barrel2_outline");
                }
                else {
                    spr = Resources.Load<Sprite>("Images/tankDark_barrel2_outline");
                }

                if (bulletType != 0) {
                    foreach (GameObject cannon in cannons.ToArray()) {
                        cannons.Remove(cannon);
                        Destroy(cannon);
                    }
                }

                if (cannons.Count == 0) {

                    cannons.Add(newCannon);
                }
                else if (cannons.Count == 1) {
                        
                    cannons[0].transform.localPosition = new Vector3(0.0f, -0.1f, 0.0f);
                    newCannon.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);

                    cannons.Add(newCannon);
                }
                else {

                    Destroy(newCannon);
                }

                break;    

            // Explosive
            case 1:
                spr = Resources.Load<Sprite>("Images/specialBarrel2_outline");

                foreach (GameObject cannon in cannons.ToArray()) {
                    cannons.Remove(cannon);
                    Destroy(cannon);
                }

                cannons.Add(newCannon);
                break;

            // Penetration
            case 2:
                spr = Resources.Load<Sprite>("Images/specialBarrel7_outline");

                foreach (GameObject cannon in cannons.ToArray()) {
                    cannons.Remove(cannon);
                    Destroy(cannon);
                }

                cannons.Add(newCannon);
                break;
        }

        bulletType = cannonId;
        newCannon.GetComponent<SpriteRenderer>().sprite = spr;
    }

    public Transform GetTankTransform() {

        return tankBody.transform;
    }

    public List<GameObject> GetCannons() {

        return cannons;
    }
}
