using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyTo : MonoBehaviour
{
    // Offset between tracker and trackee to avoid overlap.
    [SerializeField]
    private Vector3 offset = new Vector3(0, 1, 0);
    
    // How far from linear progress.
    [SerializeField]
    private float sharpness = 5f;
    
    // How long is the animation.
    [SerializeField]
    private float duration = 5f;
    
    // How long into the follow animation.
    private float elapsedTime = 0.0f;
    
    // Item that should follow.
    [SerializeField]
    private GameObject flyToItem;
    
    // How far away should be ignored.
    [SerializeField]
    private float maxDistance = 1;
    
    // Buffer period before pulling.
    [SerializeField]
    private float delaySnapTime = 1;
    
    // How long since the follow object was last out of range.
    private float delayElapsedTime = 0.0f;
    
    // Starting point of the annimation.
    private Vector3 start;

    // Starting rotation of the animation.
    private Quaternion startR;

    // Flag to only output missing item warning once.
    private bool first = true;


    // Start is called before the first frame update.
    void Start()
    {
        elapsedTime = duration;
    }

    // Update is called once per frame.
    void Update()
    {
        float distance = (flyToItem.transform.position - transform.position - offset).magnitude;
        
        // In range, and not currently animating, reset delay timer.
        if (distance < maxDistance && elapsedTime >= duration)
        {
            delayElapsedTime = 0;
        }
        // Out of range, and not currently animating, start thr delay timer.
        else if (distance > maxDistance && elapsedTime >= duration)
        {
            delayElapsedTime += Time.deltaTime;
            
            // Out of range, and delay timer elapsed, start snap.
            if (delayElapsedTime > delaySnapTime)
            {
                start = transform.position;
                startR = transform.rotation;
                elapsedTime = 0.0f;
            }
        }

        // Snap triggered, running animation.
        else if (elapsedTime < duration)
        {
            Vector3 target = flyToItem.transform.position + offset;
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / duration;
            float progress = Progress(percent);
            transform.position = Vector3.Lerp(start, target, progress);
            transform.rotation = Quaternion.Lerp(startR, flyToItem.transform.rotation, progress);
        }
    }

    private float Progress(float percentTime)
    {
        return Mathf.Pow(percentTime, 1 / sharpness);
    }
}
