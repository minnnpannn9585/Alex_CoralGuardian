using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameBtn()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGameBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL
        // Web builds can't be programmatically "quit" like desktop apps.
        // Common approach: navigate away or close the tab (often blocked by browsers).
        Application.OpenURL("about:blank");
#else
        // Windows / macOS / Linux standalone player
        Application.Quit();
#endif
    }
}
