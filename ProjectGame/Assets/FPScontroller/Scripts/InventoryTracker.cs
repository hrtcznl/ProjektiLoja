using UnityEngine;
using TMPro;

public class InventoryTracker : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text displayText;
    public GameObject crosshair;

    [Header("Camera")]
    public Camera cam;

    [Header("Raycast")]
    public float interactDistance = 3f;

    [Header("Collectibles")]
    public GameObject transformer;
    public GameObject[] diodes = new GameObject[4];
    public GameObject[] capacitors = new GameObject[2];
    public GameObject voltageRegulator;

    [Header("Completion")]
    public GameObject completionObject;

    private int transformerCount;
    private int diodeCount;
    private int capacitorCount;
    private int regulatorCount;

    private GameObject currentTarget;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (crosshair != null)
            crosshair.SetActive(false);

        if (completionObject != null)
            completionObject.SetActive(false);

        UpdateUI();
    }

    void Update()
    {
        HandleLook();
        HandleInteract();
    }

    void HandleLook()
    {
        currentTarget = null;

        bool looking = false;

        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (IsCollectible(hitObj))
            {
                looking = true;
                currentTarget = hitObj;
            }
        }

        if (crosshair != null)
            crosshair.SetActive(looking);
    }

    void HandleInteract()
    {
        if (currentTarget == null)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            CollectObject(currentTarget);
        }
    }

    bool IsCollectible(GameObject obj)
    {
        if (obj == transformer) return true;
        if (IsInArray(obj, diodes)) return true;
        if (IsInArray(obj, capacitors)) return true;
        if (obj == voltageRegulator) return true;

        return false;
    }

    void CollectObject(GameObject obj)
    {
        if (obj == null) return;

        if (obj == transformer && transformerCount < 1)
        {
            transformerCount = 1;
            obj.SetActive(false);
        }
        else if (IsInArray(obj, diodes) && diodeCount < 4)
        {
            diodeCount++;
            obj.SetActive(false);
        }
        else if (IsInArray(obj, capacitors) && capacitorCount < 2)
        {
            capacitorCount++;
            obj.SetActive(false);
        }
        else if (obj == voltageRegulator && regulatorCount < 1)
        {
            regulatorCount = 1;
            obj.SetActive(false);
        }

        currentTarget = null;

        if (crosshair != null)
            crosshair.SetActive(false);

        UpdateUI();
        CheckCompletion();
    }

    bool IsInArray(GameObject obj, GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == obj)
                return true;
        }
        return false;
    }

    void UpdateUI()
    {
        if (displayText == null) return;

        displayText.text =
            $"Transformer {transformerCount}/1\n" +
            $"Diodes         {diodeCount}/4\n" +
            $"Capacitors   {capacitorCount}/2\n" +
            $"V. regulator  {regulatorCount}/1";
    }

    void CheckCompletion()
    {
        if (transformerCount == 1 && diodeCount == 4 && capacitorCount == 2 && regulatorCount == 1)
        {
            if (completionObject != null && !completionObject.activeSelf)
                completionObject.SetActive(true);
        }
    }
}