using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LaserSelection : MonoBehaviour
{
    private LineRenderer line;
    [SerializeField]
    private float width = 0.05f;
    [SerializeField]
    private float distance = 5f;
    private Vector3[] points = new Vector3[2];

    // Start is called before the first frame update.
    void Start()
    {
        // Make a line renderer.
        line = gameObject.AddComponent<LineRenderer>();

        // Give it a base material so the color can be changed.
        line.material = new Material((Material)Resources.Load("Materials/ColorChange"));
        line.startWidth = width;
        line.endWidth = width;
        points[0] = transform.position + transform.up * transform.localScale.y; // Point at hand.
        points[1] = transform.position + transform.up * distance; // Point 4 units forward from hand.
        line.SetPositions(points);
        SetColor(Color.magenta);
        line.enabled = false;

        // Make a new player input component if needed.
        PlayerInput filter = GameObject.FindObjectOfType<PlayerInput>();
        if (filter == null)
        {
            filter = gameObject.AddComponent<PlayerInput>();
            Preset inputSettings = Resources.Load<Preset>("PlayerInputGrab");
            inputSettings.ApplyTo(filter);

            // IMPORTANT since player input components are not enabled when made in code by default.
            filter.ActivateInput();
        }

        // Register listener.
        filter.actions["LaserOn"].started += OnLaserOn;
        filter.actions["LaserOn"].canceled += OnLaserOn;
    }

    public void OnLaserOn(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            line.enabled = true;
        }
        else if (context.canceled)
        {
            Button hit = PerformButtonRayCast();
            if (hit != null)
            {
                hit.onClick.Invoke();
            }
            line.enabled = false;
        }
    }

    private void SetColor(Color color)
    {
        line.startColor = color;
        line.endColor = color;

        line.material.SetColor("_EmissionColor", color * 255);
    }
    public void Update()
    {
        if (line.enabled == true)
        {
            points[0] = transform.position + transform.up * transform.localScale.y; // Point at hand.
            points[1] = transform.position + transform.up * distance; // Point 4 units forward from hand.
            line.SetPositions(points);
        }

        Button hit = PerformButtonRayCast();
        if (hit != null)
        {
            SetColor(Color.green);
        }
        else
        {
            SetColor(Color.magenta);
        }
    }

    private Button PerformButtonRayCast()
    {
        Ray r = new Ray(points[0], points[1] - points[0]);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, distance))
        {
            // Update line reguardless.
            points[1] = hit.point;
            line.SetPositions(points);
            return hit.collider.gameObject.GetComponent<Button>();
        }
        return null;
    }
}
