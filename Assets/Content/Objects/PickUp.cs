﻿using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {
    public int amount = 0;

    protected virtual void Start() {
        
    }

    public void Remove() {
        Network.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col) {
    }
}
