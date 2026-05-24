using UnityEngine;

public class MoveUpOnKey : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 5f;

    [Header("Key Settings")]
    public KeyCode moveKey = KeyCode.U;

    void Update()
    {
        if (Input.GetKeyDown(moveKey))
        {
            transform.position += Vector3.up * moveDistance;
        }
    }
}