using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public string endingSceneName = "EndingScene";
    public float endingTransitionDelay = 1.0f;

    [Header("UI References")]
    public GameObject gameOverUI;
    public GameObject restartButton;    // Reference to restart button

    private bool isEndingGame = false;    // Flag to prevent multiple end game calls

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameManager] Initialized");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("[GameManager] Duplicate destroyed");
        }

        // Check build settings on startup
        CheckBuildSettings();
    }

    private void CheckBuildSettings()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"[GameManager] Total scenes in build: {sceneCount}");

        // List all scenes in build settings
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"[GameManager] Scene {i}: {sceneName}");
        }
    }

    public void EndGame()
    {
        if (isEndingGame)
        {
            Debug.Log("[GameManager] EndGame already in progress");
            return;
        }

        Debug.Log("[GameManager] EndGame called");
        isEndingGame = true;

        // Show UI if available
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Debug.Log("[GameManager] Game Over UI displayed");
        }

        // Schedule scene transition
        Debug.Log($"[GameManager] Scheduling scene transition with {endingTransitionDelay}s delay");
        Invoke("LoadEndingScene", endingTransitionDelay);
    }

    private void LoadEndingScene()
    {
        Debug.Log($"[GameManager] LoadEndingScene called for scene: {endingSceneName}");

        try
        {
            // Verify scene exists in build settings
            bool canLoad = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneName == endingSceneName)
                {
                    canLoad = true;
                    Debug.Log($"[GameManager] Found scene at build index {i}");
                    break;
                }
            }

            if (canLoad)
            {
                Time.timeScale = 1;
                Debug.Log("[GameManager] Loading scene...");
                SceneManager.LoadScene(endingSceneName);
            }
            else
            {
                Debug.LogError($"[GameManager] Scene '{endingSceneName}' not found in build settings!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] Error loading scene: {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            isEndingGame = false;
        }
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting game");
        Time.timeScale = 1;

        // Get the current scene name for debugging
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"[GameManager] Loading scene: {currentScene}");

        // Reload the current scene
        SceneManager.LoadScene(currentScene);
    }
}