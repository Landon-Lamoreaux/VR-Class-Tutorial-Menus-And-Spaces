using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private float horizontalAngle = 100;
    [SerializeField]
    private float verticalAngle = 40;
    private float thetaY = 0;
    private float thetaX = 0;

    [SerializeField]
    private float speed = 0.2f;

    private void Start()
    {
        PlayerInput filter = GameObject.FindObjectOfType<PlayerInput>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<PlayerInput>();
            Preset inputSettings = Resources.Load<Preset>("PlayerInputGrab");
            inputSettings.ApplyTo(filter);
        }

        filter.actions["CameraMove"].performed += OnCameraMove;
    }
    public void OnCameraMove(InputAction.CallbackContext context)
    {

        //get mouse change and recale to speed
        Vector2 delta = context.ReadValue<Vector2>();
        thetaY += delta.x * speed;
        thetaX -= delta.y * speed;

        //limit rotation
        thetaY = Mathf.Clamp(thetaY, -horizontalAngle/2, horizontalAngle/2);
        thetaX = Mathf.Clamp(thetaX, -verticalAngle/2, verticalAngle/2);
        transform.localRotation = Quaternion.Euler(thetaX, thetaY, 0);

    }


}
