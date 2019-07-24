using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
    }

    public bool GetDevTesting() {

        return devTesting;
    }
}