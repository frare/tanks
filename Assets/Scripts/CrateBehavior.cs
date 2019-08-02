using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour {

    private int crateId;

    public void SetCrateId(int number) {

        crateId = number;
    }

    public int GetCrateId() {

        return crateId;
    }
}