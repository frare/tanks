using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviour {

    private string versionNumber = "0.1";

    [SerializeField] private GameObject connectionMenu;
    [SerializeField] private List<GameObject> views;
    [SerializeField] private InputField roomInput;
    


    // Photon Methods
    private void Awake() {

        DontDestroyOnLoad(this.transform);

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

        if (views[0].activeInHierarchy == true) {
            views[0].SetActive(false);
        }
        if (views[1].activeInHierarchy == true) {
            views[1].SetActive(false);
        }
        views[2].SetActive(true);
        Debug.Log("Disconnected");
    }

    private void OnJoinedRoom() {

        PhotonNetwork.LoadLevel("Game");

        Debug.Log("Connected to " + PhotonNetwork.room.Name);
    }
    // ***** ******

    public void OnClickPlay() {

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(roomInput.text, roomOptions, TypedLobby.Default);
    }
}
