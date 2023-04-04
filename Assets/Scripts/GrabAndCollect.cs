using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabAndCollect : MonoBehaviour
{


    public void OnTriggerEnter(Collider other)
    {
        Grab g = other.GetComponent<Grab>();
        if (g != null)
        {
            g.registerGrab(this.gameObject, OnGrab);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Grab g = other.GetComponent<Grab>();
        if (g != null)
        {
            g.unregisterGrab(this.gameObject);
        }
    }

    private void OnGrab(GameObject controller)
    {
        gameObject.SetActive(false);
    }
}
