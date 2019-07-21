using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {

    private static T _instance;

    // Declarar no inspector
    private bool DoNotDestroy;

    // Ternário
    public static T instance { get { return _instance != null ? _instance : ( _instance = FindObjectOfType<T>()); } } 

    void Awake() {
        if (DoNotDestroy) {

            // Permanecer entre cenas
            DontDestroyOnLoad(this.gameObject);

            // Procura todos os singletons das cena
            T[] singletons = FindObjectsOfType<T>();

            // Se houver mais de um, destrói o novo, mantendo o que permanecia
            if (singletons.Length > 1) {
                Destroy(this.gameObject);
            }
        }
    }
}
