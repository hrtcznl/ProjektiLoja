using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NavKeypad { 
public class KeypadInteractionFPV : MonoBehaviour
{
    private Camera cam;

    [Header("Object to Enable When Looking at Keypad")]
    public GameObject objectToToggle;

    [Header("Max Distance to Detect Keypad")]
    public float interactDistance = 2f;

    private void Awake() => cam = Camera.main;

    private void Update()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        bool lookingAtKeypad = false;

        if (Physics.Raycast(ray, out var hit, interactDistance))
        {
            // Detect individual buttons
            if (hit.collider.TryGetComponent(out KeypadButton keypadButton))
            {
                lookingAtKeypad = true;

                if (Input.GetMouseButtonDown(0))
                {
                    keypadButton.PressButton();
                }
            }
            else
            {
                // Detect the whole keypad by checking for a parent Keypad component
                if (hit.collider.GetComponentInParent<Keypad>() != null)
                {
                    lookingAtKeypad = true;
                }
            }
        }

        // Enable or disable object depending on look
        if (objectToToggle != null)
            objectToToggle.SetActive(lookingAtKeypad);
    }
}
}