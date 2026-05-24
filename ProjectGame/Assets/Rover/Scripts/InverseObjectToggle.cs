using UnityEngine;

public class InverseObjectToggle : MonoBehaviour
{
    public GameObject[] targetObjects;
    public GameObject[] enableOnEnable;

    void OnEnable()
    {
        if (targetObjects != null)
        {
            for (int i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target != null)
                {
                    target.SetActive(false);
                }
            }
        }

        if (enableOnEnable != null)
        {
            for (int i = 0; i < enableOnEnable.Length; i++)
            {
                var target = enableOnEnable[i];
                if (target != null)
                {
                    target.SetActive(true);
                }
            }
        }
    }

    void OnDisable()
    {
        if (targetObjects != null)
        {
            for (int i = 0; i < targetObjects.Length; i++)
            {
                var target = targetObjects[i];
                if (target != null)
                {
                    target.SetActive(!target.activeSelf);
                }
            }
        }

        if (enableOnEnable != null)
        {
            for (int i = 0; i < enableOnEnable.Length; i++)
            {
                var target = enableOnEnable[i];
                if (target != null)
                {
                    target.SetActive(!target.activeSelf);
                }
            }
        }
    }
}