using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotEffectBehavior : MonoBehaviour {

    private SpriteRenderer sprRend;

    private void Awake() {

        sprRend = GetComponent<SpriteRenderer>();
    }

    private void Start() {

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {

        while (sprRend.color.a > 0.0f) {

            Color color = sprRend.color;
            color.a -= Time.deltaTime * 2;
            sprRend.color = color;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
