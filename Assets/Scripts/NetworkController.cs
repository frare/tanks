using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : Singleton<NetworkController> {

    private string versionNumber = "0.1.1";

    [SerializeField] private GameObject connectionMenu;
    [SerializeField] private List<GameObject> views;
    [SerializeField] private InputField nameInput, roomInput;
    [SerializeField] private Text infoText;
    private string playerName;


    // Photon Methods
    private void Awake() {

        DontDestroyOnLoad(this.gameObject);

        PhotonNetwork.ConnectUsingSettings(versionNumber);

        Debug.Log("Connecting to Server...");
    }

    private void Start() {

        if (views[0].activeInHierarchy == false) {
            views[0].SetActive(true);
        }
        for (int i = 1; i < views.Count - 1; i++) {
            if (views[i].activeInHierarchy == true) {
                views[i].SetActive(false);
            }
        }
    }

    private void OnConnectedToMaster() {

        PhotonNetwork.JoinLobby(TypedLobby.Default);

        Debug.Log("Connected to Master");
    }

    private void OnJoinedLobby() {

        views[0].SetActive(false);
        views[1].SetActive(true);

        Debug.Log("Joined the Lobby");
    }

    private void OnDisconnectedFromPhoton() {

        for (int i = views.Count - 1; i > 0; i--) {
            if (views[i].activeInHierarchy == true) {
                views[i].SetActive(false);
            }
        }
        views[views.Count - 1].SetActive(true);
        Debug.Log("Disconnected");
    }

    private void OnJoinedRoom() {

        PhotonNetwork.LoadLevel("Game");

        Debug.Log("Connected to " + PhotonNetwork.room.Name);
    }
    // ***** ******

    public void OnNameClicked() {

        PhotonNetwork.player.NickName = nameInput.text;

        views[1].SetActive(false);
        views[2].SetActive(true);
    }

    public void OnClickPlay() {

        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        if (rooms.Length == 0) {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(roomInput.text, roomOptions, TypedLobby.Default);

            infoText.text = "Room not found! Creating...";
        }
        else {
            for (int i = 0; i < rooms.Length; i++) {
                if (rooms[i].Name == roomInput.text) {
                    if (rooms[i].PlayerCount == rooms[i].MaxPlayers) {

                        infoText.text = "Room is full!";
                    }
                    else {

                        PhotonNetwork.JoinRoom(roomInput.text);

                        infoText.text = "Room found! Joining...";
                    }
                }
                else {
                    RoomOptions roomOptions = new RoomOptions();
                    roomOptions.MaxPlayers = 2;
                    PhotonNetwork.CreateRoom(roomInput.text, roomOptions, TypedLobby.Default);

                    infoText.text = "Room not found! Creating...";
                }
            }
        }
    }

    public string GetPlayerName() {

        return playerName;
    }
}