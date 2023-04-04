using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public class Grab : MonoBehaviour
{
    [SerializeField]
    private string canGrabTag = "Collectible";

    //handles for the input events we care about
    private PlayerInput filter;
    private InputAction grab;
    private InputAction release;

    //current object in range of this object
    protected List<GameObject> inRange = new List<GameObject>();

    //what is currently held in the hand
    protected GameObject inHand = null;
    public GameObject InHand { get => inHand; set => inHand = value; }

    public delegate void GrabEvent(GameObject controller);
    protected Dictionary<GameObject, GrabEvent> grabObjects = new Dictionary<GameObject, GrabEvent>();
    protected Dictionary<GameObject, GrabEvent> releaseObjects = new Dictionary<GameObject, GrabEvent>();


    public void registerGrab(GameObject item, GrabEvent callback)
    {
        grabObjects[item] = callback;
    }

    public void registerRelease(GameObject item, GrabEvent callback)
    {
        releaseObjects[item] = callback;
    }

    public void unregisterGrab(GameObject item)
    {
        grabObjects.Remove(item);
    }

    public void unregisterRelease(GameObject item)
    {
        releaseObjects.Remove(item);;
    }


    void Awake()
    {
        
        filter = GameObject.FindObjectOfType<PlayerInput>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<PlayerInput>();
            Preset inputSettings = Resources.Load<Preset>("PlayerInputGrab");
            inputSettings.ApplyTo(filter);
            filter.ActivateInput();
        }

        //have to separate controller events to hand if BOTH need to be fully monitored
        if (name.Contains("Left"))
        {
            grab = filter.actions["GrabLeft"];
            release = filter.actions["ReleaseLeft"];
        }
        else
        {
            grab = filter.actions["GrabRight"];
            release = filter.actions["ReleaseRight"];
        }

        grab.started += OnGrab;
        release.performed += OnRelease;
    }

    // Start is called before the first frame update
    void Start()
    {
        //filter.ActivateInput();
    }

    //register and unregiser within range
    public void OnTriggerEnter(Collider other)
    {

        if (other.tag == canGrabTag)
        {
            inRange.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == canGrabTag)
        {
            inRange.Remove(other.gameObject);
        }
    }

    //grab action
    public void OnGrab(InputAction.CallbackContext context)
    {
        CleanUp();

        //sanity check, do not grab if hands are full
        if (inHand == null && inRange.Count > 0)
        {
            GameObject closest = inRange[0];
            for (int i = 1; i < inRange.Count; i++)
            {
                if ((this.transform.position - closest.transform.position).magnitude >
                    (this.transform.position - inRange[i].transform.position).magnitude)
                {
                    closest = inRange[i];
                }
            }

            inHand = closest;

            //check for all the objects that are to be notified, but default, this is just one
            List<GameObject> list = getGrabCallbackSet();
            foreach (GameObject go in list)
            {
                if (go != null && grabObjects.ContainsKey(go))
                {
                    grabObjects[go].Invoke(this.gameObject);
                }
            }

        }
    }

    /// <summary>
    /// Gets a list of objects to be notified of a grab callbacks. 
    /// Override this is more than just the object in the hand is to be notified.
    /// </summary>
    /// <returns></returns>
    protected virtual List<GameObject> getGrabCallbackSet()
    {
        List<GameObject> list = new List<GameObject>
        {
            inHand
        };
        return list;
    }


    /// <summary>
    /// Gets a list of objects to be notified of a grab callbacks. 
    /// Override this is more than just the object in the hand is to be notified.
    /// </summary>
    /// <returns></returns>
    protected virtual List<GameObject> getReleaseCallbackSet()
    {
        List<GameObject> list = new List<GameObject>
        {
            inHand
        };
        return list;
    }

    /// <summary>
    /// Helper function to remove elements from the list if they have someone 
    /// been disabled or removed since the last grab/release event
    /// </summary>
    private void CleanUp()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject obj in inRange)
        {
            if (obj == null || !obj.activeSelf)
            {
                toRemove.Add(obj);
            }
        }

        foreach (GameObject obj in toRemove)
        {
            inRange.Remove(obj);
            grabObjects.Remove(obj);
            releaseObjects.Remove(obj);
            if (obj == inHand)
                inHand = null;
        }
    }



    //release action
    public void OnRelease(InputAction.CallbackContext context)
    {
        //check for all the objects that are to be notified, but default, this is just one
        List<GameObject> list = getReleaseCallbackSet();
        foreach (GameObject go in list)
        {
            if (go != null && releaseObjects.ContainsKey(go))
            {
                releaseObjects[go].Invoke(this.gameObject);
            }
        }

        inHand = null;


        CleanUp();

    }
}
