using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndHold : MonoBehaviour
{
    [SerializeField]
    protected Vector3 rotationOffset;
    [SerializeField]
    protected Vector3 positionOffset;




    public void OnTriggerEnter(Collider other)
    {
        Grab g = other.GetComponent<Grab>();
        if (g != null)
        {
            g.registerGrab(this.gameObject,OnGrab);
            g.registerRelease(this.gameObject, OnRelease);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Grab g = other.GetComponent<Grab>();
        if (g != null)
        {
            g.unregisterGrab(this.gameObject);
            g.unregisterRelease(this.gameObject);
        }
    }

    private void OnGrab(GameObject controller)
    {

        //parent to and correct alignment
        transform.parent = controller.transform;
        transform.localPosition = positionOffset;
        transform.localRotation = controller.transform.rotation * Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);

        //add a unmovable joint
        FixedJoint joint = AddFixedJoint(controller);
        joint.connectedBody = GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint(GameObject obj)
    {
        FixedJoint fx = obj.gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }


    private void OnRelease(GameObject controller)
    {

        transform.parent = null;


        if (controller.GetComponent<FixedJoint>())
        {
            controller.GetComponent<FixedJoint>().connectedBody = null;
            Destroy(controller.GetComponent<FixedJoint>());

        }

    }
}
