using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Photon.MonoBehaviour, IPunObservable {

    private PhotonView photView;
    private Camera playerCamera;

    [SerializeField] private float moveSpeed;
    private List<GameObject> cannons;
    private List<float> cannonRotations;
    private Vector3 selfPosition;


    private void Awake() {

        photView = GetComponent<PhotonView>();
        playerCamera = transform.GetChild(2).GetComponent<Camera>();

        cannons = new List<GameObject>();
        cannons.Add(transform.GetChild(0).GetChild(1).gameObject);

        cannonRotations = new List<float>();
        cannonRotations.Add(cannons[0].transform.eulerAngles.z);
    }

    private void Start() {
        
        if (!GameController.instance.IsDevTesting() && photView.isMine) {
            playerCamera.gameObject.SetActive(true);
        }
    }

    private void Update() {

        if (!GameController.instance.IsDevTesting()) {
            if (photView.isMine) {
                CheckInput();
            }
            else { 
                SmoothNetMovement();
            }
        }
        else {
            CheckInput();
        }
    }

    private void CheckInput() {

        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized * moveSpeed * Time.deltaTime);
        RotateCannons();
    }

    private void RotateCannons() {

        if (!GameController.instance.IsDevTesting()) {
            if (photView.isMine) {
                foreach (GameObject obj in cannons) {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 objectPos = playerCamera.WorldToScreenPoint(transform.position);

                    mousePos.x = mousePos.x - objectPos.x;
                    mousePos.y = mousePos.y - objectPos.y;
                    mousePos.z = 10.0f;

                    float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                    obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }
            }
        }
        else {
            foreach (GameObject obj in cannons) {
                Vector3 mousePos = Input.mousePosition;
                Vector3 objectPos = playerCamera.WorldToScreenPoint(transform.position);

                mousePos.x = mousePos.x - objectPos.x;
                mousePos.y = mousePos.y - objectPos.y;
                mousePos.z = 10.0f;

                float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
        }
    }

    private void SmoothNetMovement() {
        transform.position = Vector3.Lerp(transform.position, selfPosition, Time.deltaTime * 10);
        for (int i = 0; i < cannons.Count; i++) {
            //cannons[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, cannonRotations[i]));
            cannons[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 
                Mathf.LerpAngle(cannons[i].transform.eulerAngles.z, cannonRotations[i], Time.deltaTime * 10)));
        }
    }

    // Photon Methods
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.isWriting) {
            stream.SendNext(transform.position);
            for (int i = 0; i < cannons.Count; i++) {
                stream.SendNext(cannons[i].transform.eulerAngles.z);
            }
        }
        else {
            selfPosition = (Vector3)stream.ReceiveNext();
            for (int i = 0; i < cannonRotations.Count; i++) {
                cannonRotations[i] = (float)stream.ReceiveNext();
            }
        }
    }
    // ***** *****
}