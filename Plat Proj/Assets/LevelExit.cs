using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [Tooltip("Leave empty to use Build Settings order.")]
    [SerializeField] private string nextSceneName = "";

    [Tooltip("If we're at the last scene in Build Settings, load this (e.g., LevelSelect). Leave empty to do nothing.")]
    [SerializeField] private string fallbackSceneIfAtEnd = "LevelSelect";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int i = SceneManager.GetActiveScene().buildIndex;
        int count = SceneManager.sceneCountInBuildSettings;
        if (i + 1 < count)
        {
            SceneManager.LoadScene(i + 1);
        }
        else if (!string.IsNullOrEmpty(fallbackSceneIfAtEnd))
        {
            SceneManager.LoadScene(fallbackSceneIfAtEnd);
        }
        else
        {
            Debug.Log("Reached end of build order and no fallback set.");
        }
    }
}
