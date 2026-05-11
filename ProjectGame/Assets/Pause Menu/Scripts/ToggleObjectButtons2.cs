using UnityEngine;

public class ToggleObjectButtons2 : MonoBehaviour
{
    [Header("Group 1")]
    public GameObject[] objectsGroup1;

    [Header("Group 2")]
    public GameObject[] objectsGroup2;

    public void EnableGroup1()
    {
        SetObjectsState(objectsGroup1, true);
    }

    public void DisableGroup1()
    {
        SetObjectsState(objectsGroup1, false);
    }

    public void EnableGroup2()
    {
        SetObjectsState(objectsGroup2, true);
    }

    public void DisableGroup2()
    {
        SetObjectsState(objectsGroup2, false);
    }

    void SetObjectsState(GameObject[] objects, bool state)
    {
        if (objects == null) return;

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
                objects[i].SetActive(state);
        }
    }
}