using UnityEngine;

public class EnableOnDisable : MonoBehaviour
{
    [Header("Object to enable when this object gets disabled")]
    public GameObject objectToEnable;

    private void OnDisable()
    {
        // Prevent errors when quitting the game
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }
    }
}