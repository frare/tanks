using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour, IPunObservable {

    private PhotonView photView;
    private Camera playerCamera;
    private GameObject playerHud;
    private Text playerPing;
    private TankBehavior tankScript;
    private Animator emoteAnimator;

    [SerializeField] private List<Sprite> bodySprites;
    [SerializeField] private List<Sprite> cannonSprites;
    [SerializeField] private int playerNumber;
    [SerializeField] private float health;

    // Net smoothing
    private List<GameObject> cannons;
    private List<float> cannonRotations;
    private Vector3 selfPosition;
    private float selfRotation;



    private void Awake() {

        photView = GetComponent<PhotonView>();
        playerCamera = transform.GetChild(2).GetComponent<Camera>();
        playerHud = transform.GetChild(3).gameObject;
        playerPing = playerHud.transform.GetChild(1).GetComponent<Text>();
        tankScript = GetComponent<TankBehavior>();
        emoteAnimator = transform.GetChild(4).GetComponent<Animator>();

        cannons = new List<GameObject>();
        cannons = tankScript.GetCannons();
        cannonRotations = new List<float>();
        cannonRotations.Add(transform.GetChild(0).transform.GetChild(1).transform.eulerAngles.z);

        if (!GameController.instance.GetDevTesting()) {
            if (photView.isMine) {
                playerCamera.gameObject.SetActive(true);
                playerHud.SetActive(true);
                playerHud.transform.GetChild(0).GetComponent<Text>().text = "Room: " + PhotonNetwork.room.Name + " (BR)";
                photView.RPC("SetName", PhotonTargets.AllBuffered, PhotonNetwork.player.NickName);
                playerNumber = PhotonNetwork.player.ID;
                photView.RPC("SetColor", PhotonTargets.AllBuffered, playerNumber);
            }
        }
    }

    private void FixedUpdate() {

        if (!GameController.instance.GetDevTesting()) {
            if (photView.isMine) {
                CheckInput();
                UpdateHUD();
            }
        }
        else {
            CheckInput();
        }
    }

    private void Update() {

        if (!GameController.instance.GetDevTesting()) {
            if (photView.isMine) {
                if (Input.GetButtonDown("Shoot")) {
                    photView.RPC("Shoot", PhotonTargets.All);
                }

                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    photView.RPC("ShowEmote", PhotonTargets.AllViaServer, 0);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    photView.RPC("ShowEmote", PhotonTargets.AllViaServer, 1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                    photView.RPC("ShowEmote", PhotonTargets.AllViaServer, 2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                    photView.RPC("ShowEmote", PhotonTargets.AllViaServer, 3);
                }
            }
            else {
                NetSmooth();
            }
        }
    }

    private void UpdateHUD() {

        playerPing.text = "Ping: " + PhotonNetwork.GetPing();
    }

    private void CheckInput() {

        tankScript.Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        tankScript.RotateCannons(Input.mousePosition, playerCamera);
    }

    private void NetSmooth() {

        // Position
        transform.position = transform.position = Vector3.Lerp(transform.position, selfPosition, Time.deltaTime * 10);
        // Body rotation
        tankScript.GetTankTransform().eulerAngles = new Vector3(0, 0,
            Mathf.LerpAngle(transform.eulerAngles.z, selfRotation, Time.deltaTime * 10));
        // Cannons
        cannons = tankScript.GetCannons();
        for (int i = 0; i < cannons.Count; i++) {
            cannons[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0,
                Mathf.LerpAngle(cannons[i].transform.eulerAngles.z, cannonRotations[i], Time.deltaTime * 10)));
        }
    }

    public int GetPlayerNumber() {

        return playerNumber;
    }

    public void TakeDamage(int amount) {

        photView.RPC("TakeDamageRPC", PhotonTargets.All, amount);
    }

    [PunRPC]
    private void TakeDamageRPC(int amount) {

        health -= amount;
    }

    [PunRPC]
    private void SetName(string name) {

        transform.GetChild(1).GetChild(0).GetComponent<Text>().text = name;
    }

    [PunRPC]
    private void SetColor(int playerID) {

        playerNumber = playerID;
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = bodySprites[playerID - 1];
        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = cannonSprites[playerID - 1];
        if (playerID == 1) {
            transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.HSVToRGB(0.6f, 0.5f, 1.0f);
        }
        else if (playerID == 2) {
            transform.GetChild(1).GetChild(0).GetComponent<Text>().color = Color.HSVToRGB(0.0f, 0.5f, 1.0f);
        }
    }

    [PunRPC]
    private void ShowEmote(int emote) {

        switch (emote) {
            case 0:
                emoteAnimator.SetTrigger("happy");
                break;

            case 1:
                emoteAnimator.SetTrigger("sad");
                break;

            case 2:
                emoteAnimator.SetTrigger("angry");
                break;

            case 3:
                emoteAnimator.SetTrigger("exclamation");
                break;
        }
    }

    // Photon Methods
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (stream.isWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(tankScript.GetTankTransform().eulerAngles.z);
            List<GameObject> cans = new List<GameObject>();
            cans = tankScript.GetCannons();
            for (int i = 0; i < cans.Count; i++) {
                stream.SendNext(cans[i].transform.eulerAngles.z);
            }
        }
        else {
            selfPosition = (Vector3)stream.ReceiveNext();
            selfRotation = (float)stream.ReceiveNext();
            for (int i = 0; i < cannonRotations.Count; i++) {
                cannonRotations[i] = (float)stream.ReceiveNext();
            }
        }
    }
    // ***** *****
}