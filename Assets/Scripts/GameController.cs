using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController> {

    [SerializeField] private bool devTesting;

    private void Start() {

        if (!devTesting) {
            GameObject sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);

            SpawnPlayer();
        }
    }

    private void SpawnPlayer() {

        switch (PhotonNetwork.player.ID) {
            case 1:
                PhotonNetwork.Instantiate("Player1", Vector3.zero, Quaternion.identity, 0);
                break;

            case 2:
                PhotonNetwork.Instantiate("Player2", Vector3.zero, Quaternion.identity, 0);
                break;
        }
    }

    public bool IsDevTesting() {

        return devTesting;
    }
}