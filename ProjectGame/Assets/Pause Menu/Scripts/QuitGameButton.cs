using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();

        // This only runs in the Unity Editor (so you can test it)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}