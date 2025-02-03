using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName;  // Name of the scene to load

    public void LoadSpecificScene()
    {
        Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Optional: Load scene by build index
    public void LoadSceneByIndex(int buildIndex)
    {
        Debug.Log($"[SceneLoader] Loading scene index: {buildIndex}");
        SceneManager.LoadScene(buildIndex);
    }

    // Optional: Reload current scene
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneLoader] Reloading current scene: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }
}
