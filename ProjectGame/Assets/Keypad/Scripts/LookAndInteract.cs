using UnityEngine;

public class LookAndInteract : MonoBehaviour
{
    [Header("Object to Show When Looking")]
    public GameObject lookIndicator;      // Object enabled while looking at this
    [Header("Object Whose Collider to Enable On E")]
    public GameObject targetObject;       // Object whose BoxCollider gets enabled
    public float interactDistance = 3f;   // Maximum distance to detect look

    private Camera cam;
    private BoxCollider targetCollider;

    private void Awake()
    {
        cam = Camera.main;
        if (targetObject != null)
            targetCollider = targetObject.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        bool lookingAtThis = false;

        // Raycast from center of screen
        var ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out var hit, interactDistance))
        {
            if (hit.collider.gameObject == gameObject)
            {
                lookingAtThis = true;

                // Press E to enable BoxCollider
                if (Input.GetKeyDown(KeyCode.E) && targetCollider != null)
                {
                    targetCollider.enabled = true;
                }
            }
        }

        // Enable/disable the indicator object based on whether we're looking
        if (lookIndicator != null)
            lookIndicator.SetActive(lookingAtThis);
    }
}